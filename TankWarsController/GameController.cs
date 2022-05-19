using System;
using NetworkUtil;
using GameModel;
using System.Windows.Forms;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using GameView;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;

namespace GameController
{
    /// <summary>
    /// Authors: Anish Narayanswamy and Ben Kempers
    /// 
    /// This class contains the Control of the MVC, responsible for processing and sending data back and forth to the server.
    /// </summary>
    public class gameController
    {
        private string IPAddress;
        private string Player;
        private World world;
        private int PlayerID;
        private int worldsize;
        private ControlCommands command = new ControlCommands("none", "none", new Vector2D(0, -1));
        private Socket playerSocket;

        public delegate void ServerUpdateHandler();
        public event ServerUpdateHandler UpdateArrived;

        /// <summary>
        /// No argument constructor
        /// </summary>
        public gameController()
        {
            world = new World(worldsize);
        }

        /// <summary>
        /// Set the id of the player tank
        /// </summary>
        /// <param name="dp"></param>
        public void SetPlayerID(DrawingPanel dp)
        {
            dp.PlayerID = PlayerID;
        }

        public World GetWorld()
        {
            return world;
        }

        /// <summary>
        /// Method to handle connecting to the server when the user hits the 'connect' button.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="playerName"></param>
        public void ConnectButton(string ipAddress, string playerName)
        {
            IPAddress = ipAddress;
            Player = playerName;

            Networking.ConnectToServer(OnConnect, IPAddress, 11000);
        }

        /// <summary>
        /// ASync callback from ConnectToServer from the Networking class which will send the server the players name and start recieving data.
        /// </summary>
        /// <param name="state"></param>
        private void OnConnect(SocketState state)
        {
            Networking.Send(state.TheSocket, Player + "\n");

            playerSocket = state.TheSocket;

            state.OnNetworkAction = ProcessOnConnectData;
            Networking.GetData(state);
        }

        /// <summary>
        /// Processes initial data, including the player id and the world size
        /// </summary>
        /// <param name="state"></param>
        private void ProcessOnConnectData(SocketState state)
        {
            Console.WriteLine(state.GetData());

            string message = state.GetData();
            string[] StartingData = Regex.Split(message, "\n");

            int count = 0;
            foreach (string line in StartingData)
            {
                if (count == 0 && !line.Equals(""))
                {
                    PlayerID = Int32.Parse(line);
                }
                else if (count == 1 && !line.Equals(""))
                    worldsize = Int32.Parse(line);
                else if (!line.Equals(""))
                {
                    lock (world)
                    {
                        ProcessObjects(line);
                    }
                }
                count++;
            }
            state.RemoveData(0, message.Length);

            state.OnNetworkAction = ProcessData;
            Networking.GetData(state);
        }

        /// <summary>
        /// Process data received from the server
        /// </summary>
        /// <param name="state"></param>
        private void ProcessData(SocketState state)
        {
            Console.WriteLine(state.GetData());

            string message = state.GetData();
            String[] StartingData = Regex.Split(message, "\n");

            lock (world)
            {
                try
                {
                    for (int i = 0; i < StartingData.Length; i++)
                    {
                        if (StartingData[i] != "")
                        {
                            ProcessObjects(StartingData[i]);
                        }
                    }
                }
                catch (Exception e)
                { }
            }

            //the rest of tank, powerup, projectile, beam data is processes here in an event loop
            lock (world)
            {
                IEnumerable<Tank> copyTanks = world.GetTanks();
                foreach (Tank tank in new List<Tank>(copyTanks))
                {
                    if (tank.IsDead())
                    {
                        world.RemoveTank(tank);
                    }
                    else
                        world.SetTank(tank, tank.getID());
                }

                IEnumerable<PowerUps> copyPowUps = world.GetPowerUps();
                foreach (PowerUps pow in new List<PowerUps>(copyPowUps))
                {
                    if (pow.IsDead())
                    {
                        world.RemovePowerup(pow);
                    }
                    else
                        world.SetPowerup(pow, pow.getID());
                }

                IEnumerable<Projectile> copyProjs = world.GetProjectiles();
                foreach (Projectile proj in new List<Projectile>(copyProjs))
                {
                    if (proj.IsDead())
                    {
                        world.RemoveProjectile(proj);
                    }
                    else
                        world.SetProjectile(proj, proj.getID());
                }
            }

            // Notify any listeners (the view) that a new game world has arrived from the server
            UpdateArrived();

            //Sends the server any kind of user inputs based off of the ControlCommand JSON.
            Networking.Send(playerSocket, JsonConvert.SerializeObject(command) + "\n");

            state.RemoveData(0, message.Length);

            state.OnNetworkAction = ProcessData;
            Networking.GetData(state);
        }

        /// <summary>
        /// Helper method to process the objects used for the game
        /// </summary>
        /// <param name="objects"></param>
        public void ProcessObjects(String o)
        {
            JObject curr = JObject.Parse(o);

            if (curr.ContainsKey("tank"))
            {
                Tank tank = JsonConvert.DeserializeObject<Tank>(o);

                world.SetTank(tank, tank.getID());
            }
            else if (curr.ContainsKey("power"))
            {
                PowerUps pow = JsonConvert.DeserializeObject<PowerUps>(o);

                world.SetPowerup(pow, pow.getID());
            }
            else if (curr.ContainsKey("proj"))
            {
                Projectile proj = JsonConvert.DeserializeObject<Projectile>(o);

                world.SetProjectile(proj, proj.getID());
            }
            else if (curr.ContainsKey("beam"))
            {
                Beams b = JsonConvert.DeserializeObject<Beams>(o);

                world.SetBeam(b, b.getOwner());
            }
            else if (curr.ContainsKey("wall"))
            {
                Walls newWall = JsonConvert.DeserializeObject<Walls>(o);

                world.SetWall(newWall, newWall.GetID());
            }

        }

        /// <summary>
        /// handling movement request: involes when button is down
        /// </summary>
        public void HandleMoveRequest(KeyEventArgs e)
        {
            Tank playertank = world.GetSpecificTank(PlayerID);

            switch (e.KeyCode)
            {
                case (Keys.W):
                    command.SetMovement("up");
                    break;
                case (Keys.A):
                    command.SetMovement("left");
                    break;
                case (Keys.S):
                    command.SetMovement("down");
                    break;
                case (Keys.D):
                    command.SetMovement("right");
                    break;
            }
        }

        /// <summary>
        /// handling movement request: cancels when button is up
        /// </summary>
        public void CancelMoveRequest(KeyEventArgs e)
        {
            Tank playertank = world.GetSpecificTank(PlayerID);

            switch (e.KeyCode)
            {
                case (Keys.W):
                    command.SetMovement("none");
                    break;
                case (Keys.A):
                    command.SetMovement("none");
                    break;
                case (Keys.S):
                    command.SetMovement("none");
                    break;
                case (Keys.D):
                    command.SetMovement("none");
                    break;
            }
        }

        /// <summary>
        /// Example of handling mouse request down
        /// </summary>
        public void HandleMouseRequest(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                command.SetFire("main");
            }
            else if (e.Button == MouseButtons.Right)
            {
                command.SetFire("alt");
            }
        }

        /// <summary>
        /// handling mouse request up
        /// </summary>
        public void CancelMouseRequest(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                command.SetFire("none");
            }
            else if (e.Button == MouseButtons.Right)
            {
                command.SetFire("none");
            }
        }

        /// <summary>
        /// Sets the aim of the turret based on the position of the mouse on the screen
        /// </summary>
        /// <param name="e"></param>
        public void MouseMoveRequest(MouseEventArgs e)
        {
            Vector2D aim = new Vector2D(e.X - 450, e.Y - 450);
            aim.Normalize();
            command.SetAim(aim);
        }
    }
}
