using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameModel
{
    /// <summary>
    /// Contains definition of projectile Json object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Projectile
    {
        [JsonProperty(PropertyName = "proj")]
        private int ID;

        [JsonProperty(PropertyName = "loc")]
        private Vector2D location;

        [JsonProperty(PropertyName = "dir")]
        private Vector2D orientation;

        [JsonProperty(PropertyName = "died")]
        private bool died = false;

        [JsonProperty(PropertyName = "owner")]
        private int ownerID;


        public Projectile()
        {

        }

        public Projectile(int iD, Vector2D loc, Vector2D dir, bool died, int ownerID)
        {
            ID = iD;
            location = loc;
            orientation = dir;
            this.died = died;
            this.ownerID = ownerID;
        }

        public int getID()
        {
            return this.ID;
        }

        public void setLoc(double projVelocity)
        {
            this.location += orientation * projVelocity;
        }

        public Vector2D getLoc()
        {
            return this.location;
        }

        public Vector2D getOrientation()
        {
            return this.orientation;
        }

        public int getOwner()
        {
            return this.ownerID;
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
