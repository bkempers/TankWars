using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameModel
{
    /// <summary>
    /// Contains class representing beams as a Json objects
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Beams
    {
        [JsonProperty(PropertyName = "beam")]
        private int ID;

        [JsonProperty(PropertyName = "org")]
        private Vector2D origin;

        [JsonProperty(PropertyName = "dir")]
        private Vector2D orientation;

        [JsonProperty(PropertyName = "owner")]
        private int ownerID;

        public Beams()
        {

        }

        public Beams(int iD, Vector2D org, Vector2D dir, int ownerID)
        {
            ID = iD;
            origin = org;
            orientation = dir;
            this.ownerID = ownerID;
        }

        public int getID()
        {
            return this.ID;
        }

        public Vector2D getOrigin()
        {
            return this.origin;
        }

        public Vector2D getOrientation()
        {
            return this.orientation;
        }

        public int getOwner()
        {
            return this.ownerID;
        }
    }
}
