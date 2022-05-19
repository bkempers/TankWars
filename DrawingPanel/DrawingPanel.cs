using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using GameModel;

namespace GameView
{

    /// <summary>
    /// Authors: Anish Narayanaswamy and Ben
    /// 
    /// Class representing the panel on which the game is drawn.
    /// </summary>
    public class DrawingPanel : Panel
    {
        private World theWorld;
        public int PlayerID { get; set; }
        private int numFrames = 0;

        private Image wallSection = Image.FromFile(@"..\..\..\Resources\TankWars\Images\WallSprite.png");
        private Image backgroundImage = Image.FromFile(@"..\..\..\Resources\TankWars\Images\Background.png");
        private Image powerupImage = Image.FromFile(@"..\..\..\Resources\TankWars\Images\powerup.png");

        private Image tankTurrentBlue = Image.FromFile(@"..\..\..\Resources\TankWars\Images\BlueTurret.png");
        private Image tankImageBlue = Image.FromFile(@"..\..\..\Resources\TankWars\Images\BlueTank.png");
        private Image projImageBlue = Image.FromFile(@"..\..\..\Resources\TankWars\Images\shot-blue.png");

        private Image tankTurrentDark = Image.FromFile(@"..\..\..\Resources\TankWars\Images\DarkTurret.png");
        private Image tankImageDark = Image.FromFile(@"..\..\..\Resources\TankWars\Images\DarkTank.png");
        private Image projImageDark = Image.FromFile(@"..\..\..\Resources\TankWars\Images\shot-violet.png");

        private Image tankTurrentGreen = Image.FromFile(@"..\..\..\Resources\TankWars\Images\GreenTurret.png");
        private Image tankImageGreen = Image.FromFile(@"..\..\..\Resources\TankWars\Images\GreenTank.png");
        private Image projImageGreen = Image.FromFile(@"..\..\..\Resources\TankWars\Images\shot-green.png");

        private Image tankTurrentLightGreen = Image.FromFile(@"..\..\..\Resources\TankWars\Images\LightGreenTurret.png");
        private Image tankImageLightGreen = Image.FromFile(@"..\..\..\Resources\TankWars\Images\LightGreenTank.png");
        private Image projImageLightGreen = Image.FromFile(@"..\..\..\Resources\TankWars\Images\shot-blue.png");

        private Image tankTurrentOrange = Image.FromFile(@"..\..\..\Resources\TankWars\Images\OrangeTurret.png");
        private Image tankImageOrange = Image.FromFile(@"..\..\..\Resources\TankWars\Images\OrangeTank.png");
        private Image projImageOrange = Image.FromFile(@"..\..\..\Resources\TankWars\Images\shot-yellow.png");

        private Image tankTurrentPurple = Image.FromFile(@"..\..\..\Resources\TankWars\Images\PurpleTurret.png");
        private Image tankImagePurple = Image.FromFile(@"..\..\..\Resources\TankWars\Images\PurpleTank.png");
        private Image projImagePurple = Image.FromFile(@"..\..\..\Resources\TankWars\Images\shot-violet.png");

        private Image tankTurrentRed = Image.FromFile(@"..\..\..\Resources\TankWars\Images\RedTurret.png");
        private Image tankImageRed = Image.FromFile(@"..\..\..\Resources\TankWars\Images\RedTank.png");
        private Image projImageRed = Image.FromFile(@"..\..\..\Resources\TankWars\Images\shot-red.png");

        private Image tankTurrentYellow = Image.FromFile(@"..\..\..\Resources\TankWars\Images\YellowTurret.png");
        private Image tankImageYellow = Image.FromFile(@"..\..\..\Resources\TankWars\Images\YellowTank.png");
        private Image projImageYellow = Image.FromFile(@"..\..\..\Resources\TankWars\Images\shot-blue.png");

        private Image fullHealthBar = Image.FromFile(@"..\..\..\Resources\TankWars\Images\PS8FullHealth.png");
        private Image mediumHealthBar = Image.FromFile(@"..\..\..\Resources\TankWars\Images\PS866HealthBar.png");
        private Image lowHealthBar = Image.FromFile(@"..\..\..\Resources\TankWars\Images\PS8lowHealthBar.png");

        /// <summary>
        /// Only constructor, takes a worls object
        /// </summary>
        /// <param name="w"></param>
        public DrawingPanel(World w)
        {
            DoubleBuffered = true;
            theWorld = w;
        }


        // A delegate for DrawObjectWithTransform
        // Methods matching this delegate can draw whatever they want using e  
        public delegate void ObjectDrawer(object o, PaintEventArgs e);

        /// <summary>
        /// This method performs a translation and rotation to drawn an object in the world.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        /// <param name="worldX">The X coordinate of the object in world space</param>
        /// <param name="worldY">The Y coordinate of the object in world space</param>
        /// <param name="angle">The orientation of the objec, measured in degrees clockwise from "up"</param>
        /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
        private void DrawObjectWithTransform(PaintEventArgs e, object o, double worldX, double worldY, double angle, ObjectDrawer drawer)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

            e.Graphics.TranslateTransform((int)worldX, (int)worldY);
            e.Graphics.RotateTransform((float)angle);
            drawer(o, e);

            // "pop" the transform
            e.Graphics.Transform = oldMatrix;
        }

        /// <summary>
        /// TankDrawer delegate that is called after translate and transform has been preformed on the PaintEventArgs.
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void TankDrawer(object o, PaintEventArgs e)
        {
            Tank tank = o as Tank;

            //check if the tank is dead, and do not draw if it is
            if(tank.getHealth() == 0)
            {
                return;
            }

            if (tank.getID() % 8 == 0)
            {
                e.Graphics.DrawImage(tankImageBlue, (float)-(60 / 2), (float)-(60 / 2), 60, 60);
            }
            else if (tank.getID() % 8 == 1)
            {
                e.Graphics.DrawImage(tankImageDark, (float)-(60 / 2), (float)-(60 / 2), 60, 60);
            }
            else if (tank.getID() % 8 == 2)
            {
                e.Graphics.DrawImage(tankImageGreen, (float)-(60 / 2), (float)-(60 / 2), 60, 60);
            }
            else if (tank.getID() % 8 == 3)
            {
                e.Graphics.DrawImage(tankImageLightGreen, (float)-(60 / 2), (float)-(60 / 2), 60, 60);
            }
            else if (tank.getID() % 8 == 4)
            {
                e.Graphics.DrawImage(tankImageOrange, (float)-(60 / 2), (float)-(60 / 2), 60, 60);
            }
            else if (tank.getID() % 8 == 5)
            {
                e.Graphics.DrawImage(tankImagePurple, (float)-(60 / 2), (float)-(60 / 2), 60, 60);
            }
            else if (tank.getID() % 8 == 6)
            {
                e.Graphics.DrawImage(tankImageRed, (float)-(60 / 2), (float)-(60 / 2), 60, 60);
            }
            else if (tank.getID() % 8 == 7)
            {
                e.Graphics.DrawImage(tankImageYellow, (float)-(60 / 2), (float)-(60 / 2), 60, 60);
            }
        }

        /// <summary>
        /// TankInfoDrawer delegate that helps to display the health, name, and score of the tanks.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void TankInfoDrawer(object o, PaintEventArgs e)
        {
            Tank tank = o as Tank;
            
            //check if the tank is dead, and do not draw if it is
            if (tank.getHealth() == 0)
            {
                return;
            }

            SolidBrush tankBrush = new SolidBrush(Color.Black);
            StringFormat strFormat = new StringFormat();

            e.Graphics.DrawString("" + tank.getName() + " : " + tank.getScore(), Font, tankBrush, (float)-25, (float)30, strFormat);

            if (tank.getHealth().Equals(3))
            {
                e.Graphics.DrawImage(fullHealthBar, (float)-50, (float)-55, 110, 20);
            }
            else if (tank.getHealth().Equals(2))
            {
                e.Graphics.DrawImage(mediumHealthBar, (float)-50, (float)-55, 110, 20);
            }
            else if (tank.getHealth().Equals(1))
            {
                e.Graphics.DrawImage(lowHealthBar, (float)-50, (float)-55, 110, 20);
            }
        }

        /// <summary>
        /// TurretDrawer delegate that draws the specific color of the tank's turret as well as the orientation of where the turret is facing.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void TurretDrawer(object o, PaintEventArgs e)
        {
            Tank tank = o as Tank;

            //check if the tank is dead, and do not draw if it is
            if (tank.getHealth() == 0)
            {
                return;
            }

            if (tank.getID() % 8 == 0)
            {
                e.Graphics.DrawImage(tankTurrentBlue, (float)-(50 / 2), (float)-(50 / 2), 50, 50);
            }
            else if (tank.getID() % 8 == 1)
            {
                e.Graphics.DrawImage(tankTurrentDark, (float)-(50 / 2), (float)-(50 / 2), 50, 50);
            }
            else if (tank.getID() % 8 == 2)
            {
                e.Graphics.DrawImage(tankTurrentGreen, (float)-(50 / 2), (float)-(50 / 2), 50, 50);
            }
            else if (tank.getID() % 8 == 3)
            {
                e.Graphics.DrawImage(tankTurrentLightGreen, (float)-(50 / 2), (float)-(50 / 2), 50, 50);
            }
            else if (tank.getID() % 8 == 4)
            {
                e.Graphics.DrawImage(tankTurrentOrange, (float)-(50 / 2), (float)-(50 / 2), 50, 50);
            }
            else if (tank.getID() % 8 == 5)
            {
                e.Graphics.DrawImage(tankTurrentPurple, (float)-(50 / 2), (float)-(50 / 2), 50, 50);
            }
            else if (tank.getID() % 8 == 6)
            {
                e.Graphics.DrawImage(tankTurrentRed, (float)-(50 / 2), (float)-(50 / 2), 50, 50);
            }
            else if (tank.getID() % 8 == 7)
            {
                e.Graphics.DrawImage(tankTurrentYellow, (float)-(50 / 2), (float)-(50 / 2), 50, 50);
            }
        }

        /// <summary>
        /// Draws the walls in the case that they are vertically oriented
        /// </summary>
        /// <param name="e"></param>
        /// <param name="o"></param>
        private void WallDrawerVertical(PaintEventArgs e, object o)
        {
            Walls wall = o as Walls;
            int WallID = wall.GetID();

            double x1 = wall.GetP1().GetX();
            double y1 = wall.GetP1().GetY();
            double y2 = wall.GetP2().GetY();

            if (y1 < y2)
            {
                int diff = (int)(y2 - y1) / 50;
                for (int i = 0; i <= diff; i++)
                {
                    e.Graphics.DrawImage(wallSection, (float)x1-25, (float)y1-25, 50, 50);
                    y1 += 50;
                }
            }
            else
            {
                int diff = (int)(y1 - y2) / 50;
                for (int i = 0; i <= diff; i++)
                {
                    e.Graphics.DrawImage(wallSection, (float)x1-25, (float)y2-25, 50, 50);
                    y2 += 50;
                }
            }
        }

        /// <summary>
        /// Draws the walls in the case that they are horizontally oriented
        /// </summary>
        /// <param name="e"></param>
        /// <param name="o"></param>
        private void WallDrawerHorizontal(PaintEventArgs e, object o)
        {
            Walls wall = o as Walls;
            int WallID = wall.GetID();

            double x1 = wall.GetP1().GetX();
            double y1 = wall.GetP1().GetY();
            double x2 = wall.GetP2().GetX();

            if (x1 < x2)
            {
                int diff = (int)(x2 - x1) / 50;
                for (int i = 0; i <= diff; i++)
                {
                    e.Graphics.DrawImage(wallSection, (float)x1-25, (float)y1-25, 50, 50);
                    x1 += 50;
                }
            }
            else
            {
                int diff = (int)(x1 - x2) / 50;
                for (int i = 0; i <= diff; i++)
                {
                    e.Graphics.DrawImage(wallSection, (float)x2-25, (float)y1-25, 50, 50);
                    x2 += 50;
                }
            }
        }

        /// <summary>
        /// PowerupDrawer delegate to draw a powerup based off of where the server has randomly placed it.
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void PowerupDrawer(object o, PaintEventArgs e)
        {
            PowerUps pow = o as PowerUps;

            e.Graphics.DrawImage(powerupImage, (float)-(30 / 2), (float)-(30 / 2), 30, 30);
        }

        /// <summary>
        /// ProjectileDrawer delegate that draws the specific tank's colored projectile based off of where it was shot from.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void ProjectileDrawer(object o, PaintEventArgs e)
        {
            Projectile proj = o as Projectile;

            if (proj.getOwner() % 8 == 0)
            {
                e.Graphics.DrawImage(projImageBlue, (float)-(50 / 2), (float)-(50 / 2), 50, 50);
            }
            else if (proj.getOwner() % 8 == 1)
            {
                e.Graphics.DrawImage(projImageDark, (float)-(50 / 2), (float)-(50 / 2), 50, 50);
            }
            else if (proj.getOwner() % 8 == 2)
            {
                e.Graphics.DrawImage(projImageGreen, (float)-(50 / 2), (float)-(50 / 2), 50, 50);
            }
            else if (proj.getOwner() % 8 == 3)
            {
                e.Graphics.DrawImage(projImageLightGreen, (float)-(50 / 2), (float)-(50 / 2), 50, 50);
            }
            else if (proj.getOwner() % 8 == 4)
            {
                e.Graphics.DrawImage(projImageOrange, (float)-(50 / 2), (float)-(50 / 2), 50, 50);
            }
            else if (proj.getOwner() % 8 == 5)
            {
                e.Graphics.DrawImage(projImagePurple, (float)-(50 / 2), (float)-(50 / 2), 50, 50);
            }
            else if (proj.getOwner() % 8 == 6)
            {
                e.Graphics.DrawImage(projImageRed, (float)-(50 / 2), (float)-(50 / 2), 50, 50);
            }
            else if (proj.getOwner() % 8 == 7)
            {
                e.Graphics.DrawImage(projImageYellow, (float)-(50 / 2), (float)-(50 / 2), 50, 50);
            }
        }

        /// <summary>
        /// Draws beams, which can be used when a powerup is picked up
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void BeamDrawer(object o, PaintEventArgs e)
        {
            Beams beam = o as Beams;

            using (Pen pen = new Pen(Color.White, 23.0f - numFrames))
            {
                e.Graphics.DrawLine(pen, new Point(0, 0), new Point(0, -2000));
                numFrames++;
            }
        }

        /// <summary>
        /// This method is invoked when the DrawingPanel needs to be re-drawn
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (theWorld.IsEmptyTanks() && !(theWorld.GetSpecificTank(PlayerID) is null))
            {
                double playerX = theWorld.GetSpecificTank(PlayerID).getLoc().GetX();
                double playerY = theWorld.GetSpecificTank(PlayerID).getLoc().GetY();

                // Center the view on the middle of the world,
                // since the image and world use different coordinate systems
                int viewSize = Size.Width; // view is square, so we can just use width

                e.Graphics.TranslateTransform((float)(-playerX + (viewSize / 2)), (float)(-playerY + (viewSize / 2)));
                e.Graphics.DrawImage(backgroundImage, (float)(-2000 / 2), (float)(-2000 / 2), 2000, 2000);
            }

            lock (theWorld)
            {
                foreach (Walls wall in theWorld.GetWalls())
                {
                    double x = wall.GetP1().GetX();
                    double y = wall.GetP1().GetY();

                    //case when it is vertical
                    if (wall.GetP1().GetX() == wall.GetP2().GetX())
                    {
                        WallDrawerVertical(e, wall);
                    }
                    //case when vector is horizontal
                    if (wall.GetP1().GetY() == wall.GetP2().GetY())
                    {
                        WallDrawerHorizontal(e, wall);
                    }
                }

                // Draw the tanks
                foreach (Tank tank in theWorld.GetTanks())
                {
                    if (!(tank.getLoc() is null))
                    {
                        DrawObjectWithTransform(e, tank, tank.getLoc().GetX(), tank.getLoc().GetY(), tank.getOrientation().ToAngle(), TankDrawer);

                        Vector2D originalTank = new Vector2D(0, -1);
                        DrawObjectWithTransform(e, tank, tank.getLoc().GetX(), tank.getLoc().GetY(), originalTank.ToAngle(), TankInfoDrawer);

                        DrawObjectWithTransform(e, tank, tank.getLoc().GetX(), tank.getLoc().GetY(), tank.getAiming().ToAngle(), TurretDrawer);
                    }
                }

                // Draw the powerups
                foreach (PowerUps pow in theWorld.GetPowerUps())
                {
                    if (!(pow.getLoc() is null))
                    {
                        DrawObjectWithTransform(e, pow, pow.getLoc().GetX(), pow.getLoc().GetY(), 0, PowerupDrawer);
                    }
                }

                // Draw the projectiles
                foreach (Projectile proj in theWorld.GetProjectiles())
                {
                    if (!(proj.getLoc() is null))
                    {
                        DrawObjectWithTransform(e, proj, proj.getLoc().GetX(), proj.getLoc().GetY(), proj.getOrientation().ToAngle(), ProjectileDrawer);
                    }
                }

                //Draw the beams
                IEnumerable<Beams> copyBeams = theWorld.GetBeams();
                foreach (Beams beam in new List<Beams>(copyBeams))
                {
                    if (!(beam.getOrigin() is null))
                    {
                        DrawObjectWithTransform(e, beam, beam.getOrigin().GetX(), beam.getOrigin().GetY(), beam.getOrientation().ToAngle(), BeamDrawer);
                        if(numFrames > 23)
                        {
                            theWorld.RemoveBeam(beam);
                            numFrames = 0;
                        }
                    }
                }
            }

            // Do anything that Panel (from which we inherit) needs to do
            base.OnPaint(e);
        }

    }
}

