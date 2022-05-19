using GameModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerController
{
    /// <summary>
    /// 
    /// Authors: Anish Narayanaswamy, Ben Kempers
    /// Version: April 23, 2021
    /// CS3500 - Prof. Kopta
    /// 
    /// Controls movements and checks for collisions on the game model for the game server
    /// 
    /// </summary>
    public class ServerController
    {

        /// <summary>
        /// No argument constructor for ServerController
        /// </summary>
        public ServerController()
        {
            
        }

        /// <summary>
        /// Handles the scoring system for the given tank
        /// </summary>
        /// <param name="tank"></param>
        public void incrementTankScore(Tank tank)
        {
            tank.incrementScore();
        }

        public Tank changeTankMovement(string controlCommands, Vector2D aim, Tank tank, double tankVelocity)
        {
            if (controlCommands.Contains("left"))
            {
                tank.changeTankPlacement("left", tankVelocity);
            }
            else if (controlCommands.Contains("right"))
            {
                tank.changeTankPlacement("right", tankVelocity);
            }
            else if (controlCommands.Contains("up"))
            {
                tank.changeTankPlacement("up", tankVelocity);
            }
            else if (controlCommands.Contains("down"))
            {
                tank.changeTankPlacement("down", tankVelocity);
            }

            tank.setAiming(aim);

            return tank;
        }

        /// <summary>
        /// Reverses the intended movement of a tank, used when the tank hits a wall
        /// </summary>
        /// <param name="controlCommands"></param>
        /// <param name="aim"></param>
        /// <param name="tank"></param>
        /// <returns></returns>
        public Tank OppositeChangeTankMovement(string controlCommands, Vector2D aim, Tank tank, double tankVelocity)
        {
            if (controlCommands.Contains("left"))
            {
                tank.changeTankPlacement("right", tankVelocity);
                tank.changeOrientation(new Vector2D(-1, 0));
            }
            else if (controlCommands.Contains("right"))
            {
                tank.changeTankPlacement("left", tankVelocity);
                tank.changeOrientation(new Vector2D(1, 0));
            }
            else if (controlCommands.Contains("up"))
            {
                tank.changeTankPlacement("down", tankVelocity);
                tank.changeOrientation(new Vector2D(0, -1));
            }
            else if (controlCommands.Contains("down"))
            {
                tank.changeTankPlacement("up", tankVelocity);
                tank.changeOrientation(new Vector2D(0, 1));
            }

            tank.setAiming(aim);

            return tank;
        }

        /// <summary>
        /// Removes a projectile if it's colliding with a wall
        /// </summary>
        /// <param name="p"></param>
        public void removeCollidingProjWall(Projectile p, IEnumerable<Walls> copyWalls, double wallSize)
        {
            foreach (Walls wall in new List<Walls>(copyWalls))
            {
                if (IsCollidingProjWall(p, wall, wallSize))
                {
                    p.setDead();
                }
            }
        }

        /// <summary>
        /// Remove a powerup from the board once hit, give the alternate attack to the tank
        /// </summary>
        /// <param name="tank"></param>
        /// <param name="copyPowerups"></param>
        /// <param name="tankSize"></param>
        /// <param name="gameMode"></param>
        public void removeCollidingPowUpTank(Tank tank, IEnumerable<PowerUps> copyPowerups, double tankSize, string gameMode)
        {
            foreach (PowerUps pow in new List<PowerUps>(copyPowerups))
            {
                if (IsCollidingPowerupTank(pow, tank, tankSize))
                {
                    tank.setAltAttack();
                    pow.setDead();
                }
            }
        }

        /// <summary>
        /// Clean up projectiles that were placed too far off the board
        /// </summary>
        /// <param name="p"></param>
        /// <param name="universeSize"></param>
        public void cleanupProjCheck(Projectile p, int universeSize)
        {
            //too left/right of the board
            if (Math.Abs(p.getLoc().GetX()) >= (universeSize / 2))
            {
                p.setDead();
            }
            //too low/high
            else if (Math.Abs(p.getLoc().GetY()) >= (universeSize / 2))
            {
                p.setDead();
            }
        }

        /// <summary>
        /// Loop helper to check if a given tank is colliding with any wall
        /// </summary>
        /// <param name="tank"></param>
        /// <returns></returns>
        public bool StopCollidingTankWall(Tank tank, IEnumerable<Walls> copyWalls, double tankSize, double wallSize)
        {
            foreach (Walls wall in new List<Walls>(copyWalls))
            {
                if (IsCollidingTankWall(tank, wall, tankSize, wallSize))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if a powerup is colliding with any of the walls
        /// </summary>
        /// <param name="tank"></param>
        /// <returns></returns>
        public bool StopCollidingPowWall(PowerUps pow, IEnumerable<Walls> copyWalls, double wallSize)
        {
            foreach (Walls wall in new List<Walls>(copyWalls))
            {
                if (IsCollidingPowerupWall(pow, wall, wallSize))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// returns true if a projectile and tank are colliding with each other
        /// </summary>
        /// <returns></returns>
        public bool IsCollidingProjTank(Projectile p, Tank t, double tankSize)
        {
            if ((p.getLoc() - t.getLoc()).Length() < tankSize / 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// returns true if a powerup and tank are colliding with each other
        /// </summary>
        /// <returns></returns>
        public bool IsCollidingPowerupTank(PowerUps p, Tank t, double tankSize)
        {
            if ((p.getLoc() - t.getLoc()).Length() < tankSize / 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///Method to determine if a projectile is colliding with a wall
        /// </summary>
        /// <returns></returns>
        public bool IsCollidingProjWall(Projectile p, Walls w, double wallSize)
        {
            //find the endpoint coordinates of the wall
            double x1 = w.GetP1().GetX();
            double y1 = w.GetP1().GetY();
            double x2 = w.GetP2().GetX();
            double y2 = w.GetP2().GetY();
            //check if wall is horizontal or vertical, based on this check the 4 corners

            //vertical, high y1
            if (x1 == x2 && y1 > y2)
            {
                if ((p.getLoc().GetX() > x1 - wallSize / 2) && (p.getLoc().GetX() < x1 + wallSize / 2)
                    && (p.getLoc().GetY() > y2 - wallSize / 2) && (p.getLoc().GetY() < y1 + wallSize / 2))
                {
                    return true;
                }
            }
            //vertical, high y2
            else if (x1 == x2 && y1 < y2)
            {
                if ((p.getLoc().GetX() > x1 - wallSize / 2) && (p.getLoc().GetX() < x1 + wallSize / 2)
                    && (p.getLoc().GetY() > y1 - wallSize / 2) && (p.getLoc().GetY() < y2 + wallSize / 2))
                {
                    return true;
                }
            }
            //horizontal, high x1
            else if (y1 == y2 && x1 > x2)
            {
                if ((p.getLoc().GetX() > x2 - wallSize / 2) && (p.getLoc().GetX() < x1 + wallSize / 2)
                    && (p.getLoc().GetY() > y1 - wallSize / 2) && (p.getLoc().GetY() < y1 + wallSize / 2))
                {
                    return true;
                }
            }
            //horizontal, high x2, all other cases
            else
            {
                if ((p.getLoc().GetX() > x1 - wallSize / 2) && (p.getLoc().GetX() < x2 + wallSize / 2)
                    && (p.getLoc().GetY() > y1 - wallSize / 2) && (p.getLoc().GetY() < y1 + wallSize / 2))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///Method to determine if a powerup is colliding with a wall
        /// </summary>
        /// <returns></returns>
        public bool IsCollidingPowerupWall(PowerUps p, Walls w, double wallSize)
        {
            //find the endpoint coordinates of the wall
            double x1 = w.GetP1().GetX();
            double y1 = w.GetP1().GetY();
            double x2 = w.GetP2().GetX();
            double y2 = w.GetP2().GetY();
            //check if wall is horizontal or vertical, based on this make 4 corner vectors

            //vertical, high y1
            if (x1 == x2 && y1 > y2)
            {
                if ((p.getLoc().GetX() > x1 - wallSize / 2) && (p.getLoc().GetX() < x1 + wallSize / 2)
                    && (p.getLoc().GetY() > y2 - wallSize / 2) && (p.getLoc().GetY() < y1 + wallSize / 2))
                {
                    return true;
                }
            }
            //vertical, high y2
            else if (x1 == x2 && y1 < y2)
            {
                if ((p.getLoc().GetX() > x1 - wallSize / 2) && (p.getLoc().GetX() < x1 + wallSize / 2)
                    && (p.getLoc().GetY() > y1 - wallSize / 2) && (p.getLoc().GetY() < y2 + wallSize / 2))
                {
                    return true;
                }
            }
            //horizontal, high x1
            else if (y1 == y2 && x1 > x2)
            {
                if ((p.getLoc().GetX() > x2 - wallSize / 2) && (p.getLoc().GetX() < x1 + wallSize / 2)
                    && (p.getLoc().GetY() > y1 - wallSize / 2) && (p.getLoc().GetY() < y1 + wallSize / 2))
                {
                    return true;
                }
            }
            //horizontal, high x2, all other cases
            else
            {
                if ((p.getLoc().GetX() > x1 - wallSize / 2) && (p.getLoc().GetX() < x2 + wallSize / 2)
                    && (p.getLoc().GetY() > y1 - wallSize / 2) && (p.getLoc().GetY() < y1 + wallSize / 2))
                {
                    return true;
                }
            }
            return false;

        }

        /// <summary>
        /// Determines if a  tank is hitting a wall
        /// </summary>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool IsCollidingTankWall(Tank t, Walls w, double tankSize, double wallSize)
        {
            //find the endpoint coordinates of the wall
            double x1 = w.GetP1().GetX();
            double y1 = w.GetP1().GetY();
            double x2 = w.GetP2().GetX();
            double y2 = w.GetP2().GetY();
            //check if wall is horizontal or vertical, based on this make 4 corner vectors

            //vertical, high y1
            if (x1 == x2 && y1 > y2)
            {
                if ((t.getLoc().GetX() > x1 - wallSize / 2 - tankSize / 2) && (t.getLoc().GetX() < x1 + wallSize / 2 + tankSize / 2)
                    && (t.getLoc().GetY() > y2 - wallSize / 2 - tankSize / 2) && (t.getLoc().GetY() < y1 + wallSize / 2))
                {
                    return true;
                }
            }
            //vertical, high y2
            else if (x1 == x2 && y1 < y2)
            {
                if ((t.getLoc().GetX() > x1 - wallSize / 2 - tankSize / 2) && (t.getLoc().GetX() < x1 + wallSize / 2 + tankSize / 2)
                    && (t.getLoc().GetY() > y1 - wallSize / 2 - tankSize / 2) && (t.getLoc().GetY() < y2 + wallSize / 2 + tankSize / 2))
                {
                    return true;
                }
            }
            //horizontal, high x1
            else if (y1 == y2 && x1 > x2)
            {
                if ((t.getLoc().GetX() > x2 - wallSize / 2 - tankSize / 2) && (t.getLoc().GetX() < x1 + wallSize / 2 + tankSize / 2)
                    && (t.getLoc().GetY() > y1 - wallSize / 2 - tankSize / 2) && (t.getLoc().GetY() < y1 + wallSize / 2 + tankSize / 2))
                {
                    return true;
                }
            }
            //horizontal, high x2, all other cases
            else
            {
                if ((t.getLoc().GetX() > x1 - wallSize / 2 - tankSize / 2) && (t.getLoc().GetX() < x2 + wallSize / 2 + tankSize / 2)
                    && (t.getLoc().GetY() > y1 - wallSize / 2 - tankSize / 2) && (t.getLoc().GetY() < y1 + wallSize / 2 + tankSize / 2))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if a beam is colliding with a tank
        /// </summary>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool IsCollidingBeamTank(Beams b, Tank t, double tankSize)
        {
            //call intersection with beam origin, beam orientation, tank location, tank radius 
            return this.Intersects(b.getOrigin(), b.getOrientation(), t.getLoc(), tankSize / 2);
        }

        /// <summary>
        /// Determines if a ray interescts a circle
        /// CODE FROM LECTURE 21
        /// </summary>
        /// <param name="rayOrig">The origin of the ray</param>
        /// <param name="rayDir">The direction of the ray</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="r">The radius of the circle</param>
        /// <returns></returns>
        public bool Intersects(Vector2D rayOrig, Vector2D rayDir, Vector2D center, double r)
        {
            // ray-circle intersection test
            // P: hit point
            // ray: P = O + tV
            // circle: (P-C)dot(P-C)-r^2 = 0
            // substituting to solve for t gives a quadratic equation:
            // a = VdotV
            // b = 2(O-C)dotV
            // c = (O-C)dot(O-C)-r^2
            // if the discriminant is negative, miss (no solution for P)
            // otherwise, if both roots are positive, hit

            double a = rayDir.Dot(rayDir);
            double b = ((rayOrig - center) * 2.0).Dot(rayDir);
            double c = (rayOrig - center).Dot(rayOrig - center) - r * r;

            // discriminant
            double disc = b * b - 4.0 * a * c;

            if (disc < 0.0)
                return false;

            // find the signs of the roots
            // technically we should also divide by 2a
            // but all we care about is the sign, not the magnitude
            double root1 = -b + Math.Sqrt(disc);
            double root2 = -b - Math.Sqrt(disc);

            return (root1 > 0.0 && root2 > 0.0);
        }

    }
}
