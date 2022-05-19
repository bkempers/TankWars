using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameModel
{
    /// <summary>
    /// Contains PowerUp representation Json object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class PowerUps
    {
        [JsonProperty(PropertyName = "power")]
        private int ID;

        [JsonProperty(PropertyName = "loc")]
        private Vector2D location;

        [JsonProperty(PropertyName = "died")]
        private bool died = false;

        public PowerUps()
        {

        }

        public PowerUps(int iD, Vector2D loc, bool died)
        {
            ID = iD;
            location = loc;
            this.died = died;
        }

        public int getID()
        {
            return this.ID;
        }

        public Vector2D getLoc()
        {
            return this.location;
        }

        public void setDead()
        {
            this.died = true;
        }

        public bool IsDead()
        {
            return this.died;
        }
    }
}
