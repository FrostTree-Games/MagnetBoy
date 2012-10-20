using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MagnetBoy
{
    class Enemy: Entity
    {
        string currentAnimation = null;
        int currentFrame = 0;
        double lastFrameIncrement = 0;

        public Enemy()
        {
            creation();

            horizontal_pos = 3.0f;
            vertical_pos = 3.0f;

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;

            acceleration.Y = 0.001f;
        }

        public Enemy(float initialx, float initialy)
        {
            creation();

            horizontal_pos = initialx;
            vertical_pos = initialy;

            width = 31.5f;
            height = 31.5f;

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;

            acceleration.Y = 0.001f;

            pole = Polarity.Positive;
            magneticMoment = 0.5f;
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

            //we need to compute magnetism here and add it to acceleration
            
            foreach (Entity q2 in globalEntityList)
            {
                if (q2 == this)
                {
                    continue;
                }

                if (pole != Polarity.Neutral && q2.MagneticValue.Key != Polarity.Neutral)
                {
                    double distance = Math.Sqrt(Math.Pow(q2.Position.X - horizontal_pos, 2) + Math.Pow(q2.Position.Y - vertical_pos, 2));
                    double force = (magneticMoment * q2.MagneticValue.Value)/(4 * Math.PI * Math.Pow(distance, 2));
                    double angle = Math.Atan2(q2.Position.X - horizontal_pos, vertical_pos - q2.Position.Y);

                    angle = (angle + (Math.PI / 2)) % (Math.PI * 2);

                    if (pole != q2.MagneticValue.Key)
                    {
                        angle += Math.PI;
                    }

                    Vector2 newForce = new Vector2((float)(force * Math.Cos(angle)) * 100, (float)(force * Math.Sin(angle)) * 100);

                    acceleration += newForce;
                }
            }
            
            Vector2 keyAcceleration = Vector2.Zero;
            Vector2 step = new Vector2(horizontal_pos, vertical_pos);
            Vector2 finalAcceleration = acceleration + keyAcceleration;

            velocity.X += (float)(finalAcceleration.X * delta);
            velocity.Y += (float)(finalAcceleration.Y * delta);

            step.X += (float)(((velocity.X) * delta) + (0.5) * (Math.Pow(delta, 2.0)) * finalAcceleration.X);
            step.Y += (float)(((velocity.Y) * delta) + (0.5) * (Math.Pow(delta, 2.0)) * finalAcceleration.Y);

            checkForWalls(Game1.map, ref step);

            horizontal_pos = step.X;
            vertical_pos = step.Y;
        }

        public override void draw(SpriteBatch sb)
        {
            sb.Draw(Game1.globalTestWalrus, new Vector2(horizontal_pos, vertical_pos), Color.Yellow);
        }
    }
}
