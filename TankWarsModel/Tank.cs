using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameModel
{
    /// <summary>
    /// Tank game object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Tank
    {
        [JsonProperty(PropertyName = "tank")]
        private int ID;

        [JsonProperty(PropertyName = "loc")]
        private Vector2D location;

        [JsonProperty(PropertyName = "bdir")]
        private Vector2D orientation;

        [JsonProperty(PropertyName = "tdir")]
        private Vector2D aiming = new Vector2D(0, -1);

        [JsonProperty(PropertyName = "name")]
        private string name;

        [JsonProperty(PropertyName = "hp")]
        private int hitPoints = 3;

        [JsonProperty(PropertyName = "score")]
        private int score = 0;

        [JsonProperty(PropertyName = "died")]
        private bool died = false;

        [JsonProperty(PropertyName = "dc")]
        private bool disconnected = false;

        [JsonProperty(PropertyName = "join")]
        private bool joined = false;

        private bool altAttack = false;
        private bool mainAttack = true;
        private int attackDelayCount = 0;
        private int respawnDelayCount = 0;

        private string userInputString = "";

        private bool worldDied = false;
        private bool speedBoost = false;

        public Tank()
        {

        }

        public Tank(int iD, Vector2D loc, Vector2D bdir, Vector2D tdir, string name, int HP, int score, bool died, bool disconnected, bool joined)
        {
            ID = iD;
            location = loc;
            orientation = bdir;
            aiming = tdir;
            this.name = name;
            hitPoints = HP;
            this.score = score;
            this.died = died;
            this.disconnected = disconnected;
            this.joined = joined;
        }

        public Tank(int iD)
        {
            ID = iD;
        }

        /// <summary>
        /// Getter for if this Tank has an active speed boost
        /// </summary>
        /// <returns></returns>
        public bool hasSpeedBoost()
        {
            if (speedBoost)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Setter to give this Tank an active speed boost
        /// </summary>
        /// <param name="val"></param>
        public void setSpeedBoost(bool val)
        {
            speedBoost = val;
        }

        /// <summary>
        /// Setter to give this tank the given hitpoints
        /// </summary>
        /// <param name="hp"></param>
        public void SetHP(int hp)
        {
            this.hitPoints = hp;
        }

        /// <summary>
        /// Getter to see if this tank is 'dead'
        /// </summary>
        /// <returns></returns>
        public bool getWorldDied()
        {
            return worldDied;
        }

        /// <summary>
        /// Setter to make this tank 'dead'
        /// </summary>
        /// <param name="var"></param>
        public void setWorldDied(bool var)
        {
            worldDied = var;
        }

        /// <summary>
        /// Sets user input of the tank attack
        /// </summary>
        /// <param name="userInput"></param>
        public void setTankUserInput(string userInput)
        {
            userInputString = userInput;
        }

        /// <summary>
        /// Checks what user attack input is
        /// </summary>
        /// <returns></returns>
        public string checkTankUserInput()
        {
            return userInputString;
        }

        /// <summary>
        /// Increment the respawn delay
        /// </summary>
        public void incrementRespawnDelay()
        {
            if(worldDied == true)
                respawnDelayCount++;
        }

        /// <summary>
        /// Checks the respawn delay to see if it has finished
        /// </summary>
        /// <param name="respawnDelayTime"></param>
        /// <returns></returns>
        public bool checkRespawnDelay(double respawnDelayTime)
        {
            if (respawnDelayCount == respawnDelayTime)
            {
                respawnDelayCount = 0;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Checks the attack delay to see if it has finished
        /// </summary>
        /// <param name="attackDelayTime"></param>
        /// <returns></returns>
        public bool checkAttackDelay(double attackDelayTime)
        {
            if (attackDelayCount == attackDelayTime)
            {
                attackDelayCount = 0;
                setMainAttack(true);
                return true;
            }
            else
            {
                attackDelayCount++;
                return false;
            }
        }

        /// <summary>
        /// Sets the alt attack to true
        /// </summary>
        public void setAltAttack()
        {
            this.altAttack = true;
        }

        /// <summary>
        /// Checks the alt attack and puts it to false if true and returns true.
        /// </summary>
        /// <returns></returns>
        public bool checkAltAttack()
        {
            if (altAttack)
            {
                altAttack = false;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Sets the main attack to the given parameter
        /// </summary>
        /// <param name="val"></param>
        public void setMainAttack(bool val)
        {
            this.mainAttack = val;
        }

        /// <summary>
        /// Checks the main attack and puts it to false if true and returns true
        /// </summary>
        /// <returns></returns>
        public bool checkMainAttack()
        {
            if (mainAttack)
            {
                mainAttack = false;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Damages the tank, if the tank has 0 hitpoints it sets the tank to dead - returns this tank with updated health
        /// </summary>
        /// <param name="tank"></param>
        /// <returns></returns>
        public Tank tankHit(Tank tank)
        {
            tank.hitPoints--;

            if (tank.hitPoints == 0)
            {
                tank.worldDied = true;
                tank.died = true;
                tank.hitPoints = 0;
            }
            return tank;
        }

        /// <summary>
        /// Damages the tank by a beam which kills it regardless of health - returns this tank with updated health
        /// </summary>
        /// <param name="tank"></param>
        /// <returns></returns>
        public Tank tankBeamHit(Tank tank)
        {
            tank.worldDied = true;
            tank.died = true;
            tank.hitPoints = 0;

            return tank;
        }

        /// <summary>
        /// Sets the location of this tank with the given loc parameter
        /// </summary>
        /// <param name="loc"></param>
        public void setLoc(Vector2D loc)
        {
            location = loc;
        }

        /// <summary>
        /// Returns the ID associated with this tank
        /// </summary>
        /// <returns></returns>
        public int getID()
        {
            return this.ID;
        }

        /// <summary>
        /// Returns the name associated with this tank
        /// </summary>
        /// <returns></returns>
        public string getName()
        {
            return this.name;
        }

        /// <summary>
        /// Returns the score associated with this tank
        /// </summary>
        /// <returns></returns>
        public int getScore()
        {
            return this.score;
        }

        /// <summary>
        /// Increments the score of this tank by 1
        /// </summary>
        public void incrementScore()
        {
            this.score++;
        }

        /// <summary>
        /// Returns the current hitpoints of this tank
        /// </summary>
        /// <returns></returns>
        public int getHealth()
        {
            return this.hitPoints;
        }

        /// <summary>
        /// Returns the current location of this tank
        /// </summary>
        /// <returns></returns>
        public Vector2D getLoc()
        {
            return this.location;
        }

        /// <summary>
        /// Returns the orientation of the tanks body 
        /// </summary>
        /// <returns></returns>
        public Vector2D getOrientation()
        {
            return this.orientation;
        }

        /// <summary>
        /// Changes the orientation of this tanks body
        /// </summary>
        /// <param name="dir"></param>
        public void changeOrientation(Vector2D dir)
        {
            this.orientation = dir;
        }

        /// <summary>
        /// Returns the turret aiming direction of this tank
        /// </summary>
        /// <returns></returns>
        public Vector2D getAiming()
        {
            return this.aiming;
        }

        /// <summary>
        /// Sets the turret aiming direction with the given parameter
        /// </summary>
        /// <param name="aim"></param>
        public void setAiming(Vector2D aim)
        {
            this.aiming = aim;
        }

        /// <summary>
        /// Returns the dead bool value of this tank
        /// </summary>
        /// <returns></returns>
        public bool IsDead()
        {
            return this.died;
        }

        /// <summary>
        /// Sets this tank to dead
        /// </summary>
        /// <param name="value"></param>
        public void SetDead(bool value)
        {
            this.died = value;
        }
        
        /// <summary>
        /// Sets this tank to have a true disconnected value
        /// </summary>
        public void setDisconnected()
        {
            this.disconnected = true;
        }

        /// <summary>
        /// Helper method to update the movement of the tank based off of the user inputs and the tank's engine speed
        /// </summary>
        /// <param name="movementDir"></param>
        /// <param name="tankVelocity"></param>
        public void changeTankPlacement(string movementDir, double tankVelocity)
        {
            if (movementDir.Equals("left"))
            {
                Vector2D moveLeft = new Vector2D(-tankVelocity, 0);
                this.location += moveLeft;
                this.orientation = new Vector2D(-1, 0);
            }
            if (movementDir.Equals("right"))
            {
                Vector2D moveRight = new Vector2D(tankVelocity, 0);
                this.location += moveRight;
                this.orientation = new Vector2D(1, 0);
            }
            if (movementDir.Equals("up"))
            {
                Vector2D moveUp = new Vector2D(0, -tankVelocity);
                this.location += moveUp;
                this.orientation = new Vector2D(0, -1);
            }
            if (movementDir.Equals("down"))
            {
                Vector2D moveDown = new Vector2D(0, tankVelocity);
                this.location += moveDown;
                this.orientation = new Vector2D(0, 1);
            }
        }
    }
}
