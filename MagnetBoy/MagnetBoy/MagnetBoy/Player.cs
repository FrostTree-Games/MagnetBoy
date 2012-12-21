using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FuncWorks.XNA.XTiled;

namespace MagnetBoy
{
    class Player: Entity
    {
        string currentAnimation = null;
        int currentFrame = 0;
        double lastFrameIncrement = 0;

        private const float knockBackForce = 0.5f;
        private Boolean isKnockedBack = false;
        private double knockBackStartTime = 0;

        // angle of window for direction magnetic force
        private const double aimWindow = 1.0471975512;

        //these should be removed when publishing final code
        private Vector2 aLine, bLine;

        public Player()
        {
            creation();

            horizontal_pos = 0.0f;
            vertical_pos = 0.0f;

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
            conveyer = Vector2.Zero;

            acceleration.Y = 0.001f;

            solid = true;

            currentAnimation = "playerWalkRight";
        }

        public Player(float initialx, float initialy)
        {
            creation();

            horizontal_pos = initialx;
            vertical_pos = initialy;

            width = 31.5f;
            height = 31.5f;

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;

            acceleration.Y = 0.001f;

            pole = Polarity.Neutral;
            magneticMoment = 0.5f;

            solid = true;

            currentAnimation = "playerWalkRight";
        }

        public override void update(GameTime currentTime)
        {
            double delta = currentTime.ElapsedGameTime.Milliseconds;
            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.X))
            {
                pole = Polarity.Positive;
            }
            else if (ks.IsKeyDown(Keys.C))
            {
                pole = Polarity.Negative;
            }
            else
            {
                pole = Polarity.Neutral;
            }

            //reset the acceleration vector and recompute it
            acceleration = Vector2.Zero;
            acceleration.Y = 0.001f;

            // compute cursor spread lines
            //aLine = GameInput.P1MouseDirectionNormal * 100;
            //bLine = GameInput.P1MouseDirectionNormal * 120;
            double directionAngle = Math.Atan2(GameInput.P1MouseDirectionNormal.Y, GameInput.P1MouseDirectionNormal.X);
            aLine = new Vector2((float)Math.Cos(directionAngle - aimWindow / 2), (float)Math.Sin(directionAngle - aimWindow / 2));
            bLine = new Vector2((float)Math.Cos(directionAngle + aimWindow / 2), (float)Math.Sin(directionAngle + aimWindow / 2));
            aLine.Normalize();
            bLine.Normalize();
            aLine = aLine * 100;
            bLine = bLine * 120;

            acceleration = acceleration + computeMagneticForce();

            if (isKnockedBack)
            {
                if (currentTime.TotalGameTime.TotalMilliseconds - knockBackStartTime > 500)
                {
                    isKnockedBack = false;
                }
            }

            Vector2 keyAcceleration = Vector2.Zero;
            Vector2 step = new Vector2(horizontal_pos, vertical_pos);

            if ((ks.IsKeyDown(Keys.Right) || GameInput.isButtonDown(GameInput.PlayerButton.RightDirection)) && !isKnockedBack)
            {
                currentAnimation = "playerWalkRight";

                if (velocity.X < 0.1f)
                {
                    keyAcceleration.X = 0.001f;
                }
            }
            else if ((ks.IsKeyDown(Keys.Left) || GameInput.isButtonDown(GameInput.PlayerButton.LeftDirection)) && !isKnockedBack)
            {
                currentAnimation = "playerWalkLeft";

                if (velocity.X > -0.1f)
                {
                    keyAcceleration.X = -0.001f;
                }
            }
            else
            {
                if (Math.Abs(velocity.X) > 0.01 && onTheGround)
                {
                    velocity.X = 0.0f;
                }

                if (currentAnimation == "playerWalkRight")
                {
                    currentAnimation = "playerIdleRight";
                }
                else if (currentAnimation == "playerWalkLeft")
                {
                    currentAnimation = "playerIdleLeft";
                }
            }

            if (ks.IsKeyDown(Keys.Up) || GameInput.isButtonDown(GameInput.PlayerButton.Jump))
            {
                if (onTheGround)
                {
                    velocity.Y = -0.5f;
                }
            }

            if (GameInput.P1MouseDown == true || GameInput.isButtonDown(GameInput.PlayerButton.Push))
            {
                double aAngle = directionAngle - (aimWindow / 2);
                double bAngle = directionAngle + (aimWindow / 2);

                foreach (Entity en in globalEntityList)
                {
                    double distance = Math.Sqrt(Math.Pow(en.Position.X - horizontal_pos, 2) + Math.Pow(en.Position.Y - vertical_pos, 2));

                    if (en == this || distance > 250)
                    {
                        continue;
                    }

                    double enAngle = Math.Atan2(en.Position.Y - vertical_pos, en.Position.X - horizontal_pos);

                    if (enAngle > aAngle && enAngle < bAngle)
                    {
                        double force = (magneticMoment * en.MagneticValue.Value) / (4 * Math.PI * Math.Pow(distance, 2));
                        double angle = Math.Atan2(en.Position.X - horizontal_pos, vertical_pos - en.Position.Y);

                        Vector2 newForce = new Vector2((float)(force * Math.Cos(angle - (Math.PI / 2))), (float)(force * Math.Sin(angle - (Math.PI / 2))));

                        en.velocity += newForce * 10000;
                    }
                }

                //wall-pushing
                {
                    List<Point> closeTiles = new List<Point>();

                    foreach (TileLayer layer in Game1.map.TileLayers)
                    {
                        bool isSolid = false;

                        foreach (KeyValuePair<string, Property> p in layer.Properties)
                        {
                            if (p.Key.Equals("solid") && p.Value.AsInt32 == 1)
                            {
                                isSolid = true;
                            }
                        }

                        if (isSolid == true)
                        {
                            for (int i = 0; i < layer.Tiles.Length; i++)
                            {
                                for (int j = 0; j < layer.Tiles[i].Length; j++)
                                {
                                    if (Math.Sqrt(Math.Pow((Game1.map.TileWidth * i) - CenterPosition.X, 2) + Math.Pow((Game1.map.TileHeight * j) - CenterPosition.Y, 2)) < 96)
                                    {
                                        if (layer.Tiles[i][j] != null)
                                        {
                                            closeTiles.Add(new Point(Game1.map.TileWidth * i + (Game1.map.TileWidth / 2), Game1.map.TileHeight * j + (Game1.map.TileHeight / 2)));
                                            closeTiles.Add(new Point(Game1.map.TileWidth * i, Game1.map.TileHeight * j));
                                            closeTiles.Add(new Point((Game1.map.TileWidth * i), (Game1.map.TileHeight * j) + (Game1.map.TileHeight)));
                                            closeTiles.Add(new Point((Game1.map.TileWidth * i) + (Game1.map.TileWidth), (Game1.map.TileHeight * j)));
                                            closeTiles.Add(new Point((Game1.map.TileWidth * i) + (Game1.map.TileWidth), (Game1.map.TileHeight * j) + (Game1.map.TileHeight)));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (Point p in closeTiles)
                    {
                        double pAngle = Math.Atan2(p.Y - vertical_pos, p.X - horizontal_pos);

                        if (pAngle > aAngle && pAngle < bAngle)
                        {
                            double distance = Math.Sqrt(Math.Pow(p.X - Position.X, 2) + Math.Pow(p.Y - Position.Y, 2));
                            double force = 0.0055;
                            double angle = Math.Atan2(p.X - horizontal_pos, vertical_pos - p.Y);

                            Vector2 newForce = new Vector2((float)(force * Math.Cos(angle - (Math.PI / 2))), (float)(force * Math.Sin(angle - (Math.PI / 2))));

                            velocity += newForce * -1;

                            Vector2.Clamp(velocity, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
                        }
                    }
                }
            }

            Vector2 finalAcceleration = acceleration + keyAcceleration;

            velocity.X += (float)(finalAcceleration.X * delta);
            velocity.Y += (float)(finalAcceleration.Y * delta);

            step.X += (float)((((velocity.X + conveyer.X)) * delta) + (0.5) * (Math.Pow(delta, 2.0)) * finalAcceleration.X);
            step.Y += (float)((((velocity.Y + conveyer.Y)) * delta) + (0.5) * (Math.Pow(delta, 2.0)) * finalAcceleration.Y);

            checkForWalls(Game1.map, ref step);
            checkForSolidObjects(ref step);

            horizontal_pos = step.X;
            vertical_pos = step.Y;

            if (onTheGround)
            {
                conveyer = Vector2.Zero;
            }

            // if the last frame time hasn't been set, set it now
            if (lastFrameIncrement == 0)
            {
                lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;
            }

            // update the current frame if needed
            if (currentTime.TotalGameTime.TotalMilliseconds - lastFrameIncrement > AnimationFactory.getAnimationSpeed(currentAnimation))
            {
                lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;

                currentFrame = (currentFrame + 1) % AnimationFactory.getAnimationFrameCount(currentAnimation);
            }
        }

        public override void draw(SpriteBatch sb)
        {
            AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position);

            sb.Draw(Game1.globalTestWalrus, Position + aLine, Color.Aqua);
            sb.Draw(Game1.globalTestWalrus, Position + bLine, Color.Beige);
        }

        public void knockBack(Vector2 direction, double hitTime)
        {
            if (isKnockedBack)
            {
                return;
            }

            isKnockedBack = true;
            knockBackStartTime = hitTime;

            onTheGround = false;

            if (direction.X > 0)
            {
                velocity.X = 0.25f;
            }
            else
            {
                velocity.X = -0.25f;
            }

            if (direction.Y > 0)
            {
                velocity.Y = 0.35f;
            }
            else
            {
                velocity.Y = -0.35f;
            }
        }
    }
}
