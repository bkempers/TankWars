using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameModel
{
    /// <summary>
    /// Contains definition for Json object walls
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Walls
    {
        [JsonProperty(PropertyName = "wall")]
        private int ID;

        [JsonProperty(PropertyName = "p1")]
        private Vector2D endpoint1;

        [JsonProperty(PropertyName = "p2")]
        private Vector2D endpoint2;

        public Walls()
        {

        }

        public Walls(int iD, Vector2D P1, Vector2D P2)
        {
            this.ID = iD;
            this.endpoint1 = P1;
            this.endpoint2 = P2;
        }

        public Vector2D GetP1()
        {
            return endpoint1;
        }

        public Vector2D GetP2()
        {
            return endpoint2;
        }

        public int GetID()
        {
            return ID;
        }
    }
}
