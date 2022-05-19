using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameModel
{
    /// <summary>
    /// Represents a Json which packages commands to be sent to the server
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ControlCommands
    {
        [JsonProperty(PropertyName = "moving")]
        private string move;

        [JsonProperty(PropertyName = "fire")]
        private string fire;

        [JsonProperty(PropertyName = "tdir")]
        private Vector2D aiming = new Vector2D(0, -1);

        public ControlCommands()
        {

        }

        public ControlCommands(string moveCmd, string fireCmd, Vector2D aimingCmd)
        {
            move = moveCmd;
            fire = fireCmd;
            aiming = aimingCmd;
        }

        public string getMovement()
        {
            return this.move;
        }

        public void SetMovement(string moveType)
        {
            this.move = moveType;
        }

        public string getFire()
        {
            return this.fire;
        }

        public void SetFire(string fireType)
        {
            this.fire = fireType;
        }

        public Vector2D getAim()
        {
            return this.aiming;
        }

        public void SetAim(Vector2D aimDir)
        {
            this.aiming = aimDir;
        }
    }
}