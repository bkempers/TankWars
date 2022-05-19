using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using GameModel;
using NetworkUtil;
using Newtonsoft.Json;

/// <summary>
/// Authors: Anish Narayanaswamy, Ben Kempers
/// Version: April 23, 2021
/// CS3500 - Prof. Kopta
/// 
/// This is a class to start a server, accept clients, and run the internal logic on the TankWars game.
/// </summary>
namespace ServerController
{
    public class Server
    {
        //Private instance variables that are determined either by the server settings file or hardcoded if not supplied in the settings.
        private int universeSize;
        private double frameTime;
        private double fireDelay;
        private double respawnRate;
        private int startHP;
        private double projSpeed;
        private double engineSpeed;
        private double tankSize;
        private double wallSize;
        private double numPowUps;
        private double powUpDelay;
        private string gameMode;

        //Private helper variables to hold information for creating walls from the server settings files.
        private Vector2D p1;
        private Vector2D p2;
        private double x;
        private double y;
        private int wallCount = 0;

        //More helper variables that are responsible for incrementing the client, powerup, proj, and beam number.
        private int clientNum = 0;
        private int powerUpNum = 0;
        private int projNum = 0;
        private int beamNum = 0;

        private bool powUpStart = false;
        private int powUpDelayCount = 0;

        //Private instance variables that hold the world, all client sockets, client names, and the ServerController object.
        private World world = new World();
        private HashSet<SocketState> Sockets;
        private Dictionary<int, String> clientName = new Dictionary<int, String>();
        private ServerController controller;
        private StringBuilder stringBuilderInfo;

        /// <summary>
        /// Main console application to start the frame timer as well as keep accepting new clients.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Server gameServer = new Server();

            Thread thread = new Thread(new ThreadStart(gameServer.FrameTimer));
            thread.Start();

            gameServer.startConnectingClients();

            Console.Read();
        }

        /// <summary>
        /// Default constructor to create the server with reading the server settings file.
        /// </summary>
        public Server()
        {
            controller = new ServerController();
            Sockets = new HashSet<SocketState>();
            ReadServerSettings(@"..\..\..\..\Resources\TankWars\settings.xml");

        }

        /// <summary>
        /// Reads a given server settings file and record the data from said file.
        /// </summary>
        /// <param name="filename"></param> Server settings file name
        public void ReadServerSettings(string filename)
        {
            try
            {
                ReadXmlFile(filename);
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// Method that the server calls to start the TCP server.
        /// </summary>
        public void startConnectingClients()
        {
            Networking.StartServer(AcceptClients, 11000);
        }

        /// <summary>
        /// Callback method from starting the server to get the initial info (client name) sent by the clients.
        /// </summary>
        /// <param name="state"></param> SocketState of the client
        private void AcceptClients(SocketState state)
        {
            state.OnNetworkAction = SendInitialInfo;
            Networking.GetData(state);
        }

        /// <summary>
        /// Callback method to send the initial server info to the client (client number, universe size, and wall locations)
        /// </summary>
        /// <param name="state"></param> SocketState of the client
        private void SendInitialInfo(SocketState state)
        {
            state.socketNum = clientNum;
            string message = state.GetData();
            string[] StartingData = Regex.Split(message, "\n");

            clientName.Add(clientNum, StartingData[0]);
            Console.WriteLine(StartingData[0]);

            lock (controller)
            {
                //Sends initial info and walls
                Networking.Send(state.TheSocket, clientNum + "\n");
                Networking.Send(state.TheSocket, universeSize + "\n");
                foreach (Walls wall in world.GetWalls())
                {
                    string wallElement = JsonConvert.SerializeObject(wall);
                    Networking.Send(state.TheSocket, wallElement + "\n");
                }

                Tank tank = createNewTank(state.socketNum, universeSize);
                world.SetTank(tank, state.socketNum);
                string socketTank = JsonConvert.SerializeObject(tank);

                Networking.Send(state.TheSocket, socketTank + "\n");
                Console.WriteLine("Sent initial info to client number: " + clientNum + " named " + tank.getName());
            }

            //add the new client to the set of sockets
            Sockets.Add(state);
            clientNum++;

            state.RemoveData(0, message.Length);
            state.OnNetworkAction = ServerEventLoop;
            Networking.GetData(state);
        }

        /// <summary>
        /// Callback method that contains the main portion of the clients event loop with the server. Processes client data, moves tanks accordingly, fires attacks, and updates projectiles.
        /// </summary>
        /// <param name="state"></param> SocketState of the client
        private void ServerEventLoop(SocketState state)
        {
            string message = state.GetData();
            string[] StartingData = Regex.Split(message, "\n");

            Tank stateTank = world.GetSpecificTank(state.socketNum);

            lock (controller)
            {
                ControlCommands userInput = JsonConvert.DeserializeObject<ControlCommands>(StartingData[0]);
                if (!(userInput is null) && stateTank.getHealth() != 0)
                {
                    Tank changedTank = applyUserMovement(userInput, stateTank);

                    //wrap around to the other side of the board if necessary
                    this.WrapAroundTank(changedTank, state.socketNum);

                    //set the tank based on how the command executed
                    world.SetTank(changedTank, state.socketNum);
                    Tank tank = world.GetSpecificTank(state.socketNum);

                    //updates the user's input attack string.
                    tank.setTankUserInput(userInput.getFire());
                }
            }

            state.RemoveData(0, message.Length);
            if (state.TheSocket.Connected)
            {
                Networking.GetData(state);
            }
        }

        /// <summary>
        /// helper to have the tank "wrap around" to the other side of the board if it gets to the end
        /// </summary>
        /// <param name="t"></param>
        public void WrapAroundTank(Tank t, int sNum)
        {
            //too left/right of the board
            if (Math.Abs(t.getLoc().GetX()) + tankSize / 2 >= (universeSize / 2))
            {
                //set location to the opposite side
                t.setLoc(new Vector2D((-universeSize / 2 + wallSize + tankSize / 2) * Math.Sign(t.getLoc().GetX()), t.getLoc().GetY()));
            }
            //too low/high
            else if (Math.Abs(t.getLoc().GetY()) + tankSize / 2 >= (universeSize / 2))
            {
                t.setLoc(new Vector2D(t.getLoc().GetX(), (-universeSize / 2 + wallSize + tankSize / 2) * Math.Sign(t.getLoc().GetY())));
            }

            world.SetTank(t, sNum);
        }

        /// <summary>
        /// Processes and sends out data to the sockets managed by the server
        /// </summary>
        public void sendOutData()
        {
            lock (world)
            {
                updateAllSocketAttacks();
            }

            stringBuilderInfo = new StringBuilder();
            lock (world)
            {
                buildServerInfo();
            }

            //iterate through all the sockets
            IEnumerable<SocketState> allSockets = (IEnumerable<SocketState>)Sockets;
            foreach (SocketState s in new List<SocketState>(allSockets))
            {
                Tank socketTank = world.GetSpecificTank(s.socketNum);
                //If the client's socket is still connected, loops over all the world's objects to send to all sockets
                if (s.TheSocket.Connected)
                {
                    //after new info is updated, send to the socket
                    Networking.Send(s.TheSocket, stringBuilderInfo.ToString());
                }

                //If the clients socket isn't connected updates the client's tank accordingly and closes the socket.
                else
                {
                    Console.WriteLine("Client number " + s.socketNum + " has disconnected from this server.");

                    socketTank.setDisconnected();
                    Sockets.Remove(s);
                    s.TheSocket.Close();
                }
            }

            lock (world)
            {
                IEnumerable<Tank> allTanks = world.GetTanks();
                foreach (Tank t in new List<Tank>(allTanks))
                {
                    if (t.IsDead() == true)
                        t.SetDead(false);
                }
            }
        }

        /// <summary>
        /// Contains infinite loop timed with stopwatch 
        /// to determine how often and when to update the world based on msperframe
        /// </summary>
        public void FrameTimer()
        {
            Stopwatch s = new Stopwatch();
            s.Start();

            while (true)
            {
                while (s.ElapsedMilliseconds < frameTime)
                {
                    //do nothing
                }
                s.Restart();

                //Removes dead powerups, beams, projectiles and dead tanks that were fired so all clients connected will get the same information.
                lock (world)
                {
                    RemoveDeadObjects();
                }

                //Private method to create and respawn the provided number of powerups.
                lock (world)
                {
                    PowerUpPlacement();
                }

                //Updates the world and then sends updated world to all the connected clients
                lock (world)
                {
                    sendOutData();
                }

            }
        }

        /// <summary>
        /// Update attacks for all the sockets
        /// </summary>
        private void updateAllSocketAttacks()
        {
            IEnumerable<SocketState> copySockets = (IEnumerable<SocketState>)Sockets;
            foreach (SocketState s in new List<SocketState>(copySockets))
            {
                //Checks the given tank to see if its respawn and attack count has to increment
                Tank socketTank = world.GetSpecificTank(s.socketNum);

                lock (world)
                {
                    if (socketTank.getWorldDied() == false)
                    {
                        tankInputAttack(socketTank);
                    }

                    socketTank.incrementRespawnDelay();

                    //Checks to see if the given tank's respawn count has reached respawn rate.
                    if (socketTank.checkRespawnDelay(respawnRate))
                    {
                        world.SetTank(respawnTank(socketTank, universeSize), s.socketNum);
                    }
                }
            }
        }

        /// <summary>
        /// Change the attack for the given tank based on the game mode
        /// </summary>
        /// <param name="socketTank"></param>
        private void tankInputAttack(Tank socketTank)
        {
            socketTank.checkAttackDelay(fireDelay);

            if (socketTank.checkTankUserInput() == "main" && socketTank.checkMainAttack())
            {
                Projectile proj = new Projectile(projNum, socketTank.getLoc(), socketTank.getAiming(), false, socketTank.getID());
                world.SetProjectile(proj, projNum);
                socketTank.setMainAttack(false);
                projNum++;
            }
            else if (socketTank.checkTankUserInput() == "alt" && socketTank.checkAltAttack())
            {
                if (gameMode == "basic")
                {
                    Beams beam = new Beams(beamNum, socketTank.getLoc(), socketTank.getAiming(), socketTank.getID());
                    world.SetBeam(beam, beamNum);
                    beamNum++;
                }
                else if (gameMode == "extra")
                    randomizePowerUpAbility(socketTank);
            }
        }

        /// <summary>
        /// Set the powerup to one of the random abilities
        /// </summary>
        private void randomizePowerUpAbility(Tank t)
        {
            Random r = new Random();
            int rand = r.Next(3);
            //health boost
            if(rand == 0)
            {
                t.SetHP(startHP);
            }
            //speed boost
            else if(rand == 1)
            {
                t.setSpeedBoost(true);
            }
            //does a beam
            else if(rand == 2)
            {
                Beams beam = new Beams(beamNum, t.getLoc(), t.getAiming(), t.getID());
                world.SetBeam(beam, beamNum);
                beamNum++;
            }
           
        }

        /// <summary>
        /// Build the Current world based on the current game objects
        /// </summary>
        private void buildServerInfo()
        {
            IEnumerable<Tank> copyTanks = world.GetTanks();
            foreach (Tank t in new List<Tank>(copyTanks))
            {
                removeCollidingProjTank(t);
                removeCollidingBeamTank(t);
                controller.removeCollidingPowUpTank(t, world.GetPowerUps(), tankSize, gameMode);

                stringBuilderInfo.Append(JsonConvert.SerializeObject(t) + "\n");
            }

            IEnumerable<Projectile> copyProj = world.GetProjectiles();
            foreach (Projectile proj in new List<Projectile>(copyProj))
            {
                if (proj.IsDead() == false)
                    proj.setLoc(projSpeed);

                controller.removeCollidingProjWall(proj, world.GetWalls(), wallSize);
                controller.cleanupProjCheck(proj, universeSize);

                stringBuilderInfo.Append(JsonConvert.SerializeObject(proj) + "\n");
            }

            IEnumerable<Beams> copyBeams = world.GetBeams();
            foreach (Beams beam in new List<Beams>(copyBeams))
            {
                stringBuilderInfo.Append(JsonConvert.SerializeObject(beam) + "\n");
            }

            IEnumerable<PowerUps> copyPowerups = world.GetPowerUps();
            foreach (PowerUps pow in new List<PowerUps>(copyPowerups))
            {
                stringBuilderInfo.Append(JsonConvert.SerializeObject(pow) + "\n");
            }
        }

        /// <summary>
        /// Remove dead/timed out objects from the world
        /// </summary>

        private void RemoveDeadObjects()
        {
            IEnumerable<Beams> allBeams = world.GetBeams();
            foreach (Beams beam in new List<Beams>(allBeams))
            {
                world.RemoveBeam(beam.getID());
            }

            IEnumerable<Projectile> allProj = world.GetProjectiles();
            foreach (Projectile proj in new List<Projectile>(allProj))
            {
                if (proj.IsDead())
                {
                    world.RemoveProjectile(proj);
                }
            }

            IEnumerable<PowerUps> deadPowerups = world.GetPowerUps();
            foreach (PowerUps pow in new List<PowerUps>(deadPowerups))
            {
                if (pow.IsDead())
                {
                    world.RemovePowerup(pow);
                }
            }
        }

        /// <summary>
        /// Private helper method that updates every frame to make sure the number of powerups in the world are correct & they respawn accordingly.
        /// </summary>
        /// <param name="s"></param>
        private void PowerUpPlacement()
        {
            if (powUpStart == false)
            {
                for (int i = 0; i < numPowUps; i++)
                {
                    world.SetPowerup(createNewPowerUp(), powerUpNum);
                    powerUpNum++;
                }
                powUpStart = true;
            }

            if (world.GetPowerUps().Count < numPowUps && powUpDelayCount == powUpDelay)
            {
                world.SetPowerup(createNewPowerUp(), powerUpNum);
                powerUpNum++;

                powUpDelayCount = 0;
            }

            powUpDelayCount++;
        }

        /// <summary>
        /// Apply movement 
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="stateTank"></param>
        /// <returns></returns>
        private Tank applyUserMovement(ControlCommands userInput, Tank stateTank)
        {
            double tankSpeed = engineSpeed;
            if (stateTank.hasSpeedBoost() && gameMode == "extra")
                tankSpeed = engineSpeed * 2;

            //change the tank to the desired location on the board
            Tank changedTank = controller.changeTankMovement(userInput.getMovement(), userInput.getAim(), stateTank, tankSpeed);

            //if there is a conflict with a wall, "reverse" the movement
            if ((controller.StopCollidingTankWall(changedTank, world.GetWalls(), tankSize, wallSize)))
            {
                changedTank = controller.OppositeChangeTankMovement(userInput.getMovement(), userInput.getAim(), stateTank, tankSpeed);
            }

            return changedTank;
        }

        /// <summary>
        /// Generate a new tank at a random location in the panel
        /// </summary>
        /// <param name="clientNum"></param>
        /// <returns></returns>
        private Tank createNewTank(int clientNum, int universeSize)
        {
            Random r = new Random();
            int tankX = r.Next(-(universeSize - 25) / 2, (universeSize - 25) / 2);
            int tankY = r.Next(-(universeSize - 25) / 2, (universeSize - 25) / 2);
            Vector2D loc = new Vector2D(tankX, tankY);
            Vector2D orientation = new Vector2D(0, 0);
            Vector2D aiming = new Vector2D(-1, 0);
            clientName.TryGetValue(clientNum, out string name);
            int score = 0;
            bool died = false;
            bool disconnected = false;
            bool joined = false;

            Tank newPlayerTank = new Tank(clientNum, loc, orientation, aiming, name, startHP, score, died, disconnected, joined);

            if (controller.StopCollidingTankWall(newPlayerTank, world.GetWalls(), tankSize, wallSize))
            {
                return createNewTank(clientNum, universeSize);
            }
            else
                return newPlayerTank;
        }

        /// <summary>
        /// Generate a new tank at a random location in the panel
        /// </summary>
        /// <param name="clientNum"></param>
        /// <returns></returns>
        public Tank respawnTank(Tank tank, int universeSize)
        {
            Random r = new Random();
            int tankX = r.Next(-(universeSize - 25) / 2, (universeSize - 25) / 2);
            int tankY = r.Next(-(universeSize - 25) / 2, (universeSize - 25) / 2);
            Vector2D loc = new Vector2D(tankX, tankY);
            Vector2D orientation = new Vector2D(0, 0);
            Vector2D aiming = new Vector2D(-1, 0);
            bool died = false;
            bool disconnected = false;
            bool joined = false;

            Tank respawedTank = new Tank(tank.getID(), loc, orientation, aiming, tank.getName(), startHP, tank.getScore(), died, disconnected, joined);
            if (controller.StopCollidingTankWall(respawedTank, world.GetWalls(), tankSize, wallSize))
            {
                return respawnTank(tank, universeSize);
            }
            else
                return respawedTank;
        }

        /// <summary>
        /// Generate a new tank at a random location in the panel
        /// </summary>
        /// <param name="clientNum"></param>
        /// <returns></returns>
        private PowerUps createNewPowerUp()
        {
            Random r = new Random();
            int powX = r.Next(-(universeSize - 25) / 2, (universeSize - 25) / 2);
            int powY = r.Next(-(universeSize - 25) / 2, (universeSize - 25) / 2);
            Vector2D loc = new Vector2D(powX, powY);

            PowerUps pow = new PowerUps(powerUpNum, loc, false);
            if (controller.StopCollidingPowWall(pow, world.GetWalls(), wallSize))
            {
                pow.setDead();
                return createNewPowerUp();
            }
            else
                return pow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tank"></param>
        private void removeCollidingProjTank(Tank tank)
        {
            IEnumerable<Projectile> copyProjectile = world.GetProjectiles();
            foreach (Projectile proj in new List<Projectile>(copyProjectile))
            {
                if (controller.IsCollidingProjTank(proj, tank, tankSize) && proj.getOwner() != tank.getID() && proj.IsDead() == false)
                {
                    Tank hitTank = tank.tankHit(tank);
                    world.SetTank(hitTank, tank.getID());

                    if (hitTank.IsDead() == true)
                        controller.incrementTankScore(world.GetSpecificTank(proj.getOwner()));

                    proj.setDead();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tank"></param>
        private void removeCollidingBeamTank(Tank tank)
        {
            IEnumerable<Beams> copyBeams = world.GetBeams();
            foreach (Beams beam in new List<Beams>(copyBeams))
            {
                if (controller.IsCollidingBeamTank(beam, tank, tankSize) && beam.getOwner() != tank.getID())
                {
                    Tank hitTank = tank.tankBeamHit(tank);
                    world.SetTank(hitTank, tank.getID());

                    controller.incrementTankScore(world.GetSpecificTank(beam.getOwner()));
                }
            }
        }

        /// <summary>
        /// Private method to open, read, record, and close the settings file.
        /// </summary>
        /// <param name="filename"></param> The given filename to be read
        private void ReadXmlFile(string filename)
        {
            using (XmlReader reader = XmlReader.Create(filename))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "GameSettings":
                                break;

                            case "Wall":
                                reader.Read();
                                break;

                            case "x":
                                reader.Read();
                                x = Double.Parse(reader.Value);
                                break;

                            case "y":
                                reader.Read();
                                y = Double.Parse(reader.Value);
                                break;

                            case "UniverseSize":
                                reader.Read();
                                universeSize = Int32.Parse(reader.Value);
                                break;

                            case "MSPerFrame":
                                reader.Read();
                                frameTime = Double.Parse(reader.Value);
                                break;

                            case "FramesPerShot":
                                reader.Read();
                                fireDelay = Double.Parse(reader.Value);
                                break;

                            case "RespawnRate":
                                reader.Read();
                                respawnRate = Double.Parse(reader.Value);
                                break;

                            case "StartingHitpoints":
                                reader.Read();
                                startHP = Int32.Parse(reader.Value);
                                break;

                            case "ProjectileSpeed":
                                reader.Read();
                                projSpeed = Double.Parse(reader.Value);
                                break;

                            case "EngineStrength":
                                reader.Read();
                                engineSpeed = Double.Parse(reader.Value);
                                break;

                            case "TankSize":
                                reader.Read();
                                tankSize = Double.Parse(reader.Value);
                                break;

                            case "WallSize":
                                reader.Read();
                                wallSize = Double.Parse(reader.Value);
                                break;

                            case "MaxPowerups":
                                reader.Read();
                                numPowUps = Double.Parse(reader.Value);
                                break;

                            case "MaxPowerupDelay":
                                reader.Read();
                                powUpDelay = Double.Parse(reader.Value);
                                break;

                            case "Mode":
                                reader.Read();
                                gameMode = reader.Value;
                                break;
                        }
                    }
                    else // If it's not a start element, it's probably an end element
                    {
                        if (reader.Name == "Wall")
                        {
                            Walls wall = new Walls(wallCount, p1, p2);
                            world.SetWall(wall, wallCount);
                            wallCount++;
                        }

                        if (reader.Name == "p1")
                            p1 = new Vector2D(x, y);

                        if (reader.Name == "p2")
                            p2 = new Vector2D(x, y);

                        if (reader.Name == "GameSettings")
                            Console.WriteLine("Server settings file has been recorded.");
                    }
                }
            }
        }
    }
}
