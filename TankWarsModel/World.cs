using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModel
{
    /// <summary>
    /// Representation of the game world, containing all game objects
    /// </summary>
    public class World
    {
        private Dictionary<int, Tank> Tanks;
        private Dictionary<int, PowerUps> Powerups;
        private Dictionary<int, Walls> Walls;
        private Dictionary<int, Projectile> Projectiles;
        private Dictionary<int, Beams> Beams;
        public int size
        { get; private set; }

        /// <summary>
        /// New no argument contructor for world class
        /// </summary>
        public World()
        {
            Tanks = new Dictionary<int, Tank>();
            Powerups = new Dictionary<int, PowerUps>();
            Walls = new Dictionary<int, Walls>();
            Projectiles = new Dictionary<int, Projectile>();
            Beams = new Dictionary<int, Beams>();
            this.size = 0;
        }

        public World(int size)
        {
            Tanks = new Dictionary<int, Tank>();
            Powerups = new Dictionary<int, PowerUps>();
            Walls = new Dictionary<int, Walls>();
            Projectiles = new Dictionary<int, Projectile>();
            Beams = new Dictionary<int, Beams>();
            this.size = size;
        }

        public void SetWall(Walls wall, int ID)
        {
            Walls[ID] = wall;
        }

        public void RemoveBeam(int ID)
        {
            Beams.Remove(ID);
        }

        public void SetBeam(Beams beam, int ID)
        {
            Beams[ID] = beam;
        }

        public void SetTank(Tank tank, int ID)
        {
            Tanks[ID] = tank;
        }

        public void SetPowerup(PowerUps pow, int ID)
        {
            Powerups[ID] = pow;
        }

        public void SetProjectile(Projectile proj, int ID)
        {
            Projectiles[ID] = proj;
        }

        public void RemoveTank(Tank tank)
        {
            Tanks.Remove(tank.getID());
        }

        public void RemoveTank(int tankID)
        {
            Tanks.Remove(tankID);
        }

        public void RemovePowerup(PowerUps pow)
        {
            Powerups.Remove(pow.getID());
        }

        public void RemoveBeam(Beams beam)
        {
            Beams.Remove(beam.getOwner());
        }

        public void RemoveProjectile(Projectile proj)
        {
            Projectiles.Remove(proj.getID());
        }

        public Dictionary<int, Walls>.ValueCollection GetWalls()
        {
            return Walls.Values;
        }

        public Dictionary<int, PowerUps>.ValueCollection GetPowerUps()
        {
            return Powerups.Values;
        }

        public Dictionary<int, Tank>.ValueCollection GetTanks()
        {
            return Tanks.Values;
        }

        public Dictionary<int, Beams>.ValueCollection GetBeams()
        {
            return Beams.Values;
        }

        public Dictionary<int, Projectile>.ValueCollection GetProjectiles()
        {
            return Projectiles.Values;
        }

        public Tank GetSpecificTank(int PlayerID)
        {
            Tanks.TryGetValue(PlayerID, out Tank tank);
            return tank;
        }

        public bool IsEmptyTanks()
        {
            if (Tanks.Count >= 1)
                return true;
            else
                return false;
        }



    }

}
