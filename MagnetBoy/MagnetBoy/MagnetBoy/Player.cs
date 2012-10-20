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
    class Player: Entity
    {
        string currentAnimation = null;
        int currentFrame = 0;
        double lastFrameIncrement = 0;

        public Player()
        {
            creation();

            horizontal_pos = 0.0f;
            vertical_pos = 0.0f;

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;

            acceleration.Y = 0.001f;

            currentAnimation = "walkRight";
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

            currentAnimation = "walkRight";
        }

        public override void update(GameTime currentTime)
        {
            double delta = currentTime.ElapsedGameTime.Milliseconds;
            KeyboardState ks = Keyboard.GetState();

            //reset the acceleration vector and recompute it
            acceleration = Vector2.Zero;
            acceleration.Y = 0.001f;

            //we need to compute magnetism here and add it to acceleration

            foreach( Entity q2 in globalEntityList )
            {
                Vector2 magnetForce = Vector2.Zero;
                float Force = 0.0f;
                float angle = 0.0f;

                float distance = (float)(Math.Sqrt( Math.Pow((horizontal_pos - q2.Position.X), 2.0) + Math.Pow((vertical_pos - q2.Position.Y), 2.0)));

                angle = (float)(Math.Acos(( (horizontal_pos * q2.Position.X) + (vertical_pos * q2.Position.Y) )/( (Math.Sqrt( (Math.Pow(horizontal_pos, 2.0) + Math.Pow(vertical_pos, 2.0))) )*(Math.Sqrt( (Math.Pow(q2.Position.X, 2.0) + Math.Pow(q2.Position.Y, 2.0)))) )));

                Force = (float)((q2.MagneticValue.Value * magneticMoment) / (4 * Math.PI * Math.Pow(distance, 2.0)));

                magnetForce.X= (float)(Force * Math.Cos(angle));
                magnetForce.Y= (float)(Force * Math.Sin(angle));

                if (q2.MagneticValue.Key == Polarity.Positive && MagneticValue.Key== Polarity.Negative || q2.MagneticValue.Key == Polarity.Negative && MagneticValue.Key == Polarity.Positive)
                {
                    //attract
                    acceleration.X += magnetForce.X;
                    acceleration.Y += magnetForce.Y;
                    
                }
                else if (q2.MagneticValue.Key == Polarity.Positive && MagneticValue.Key == Polarity.Positive || q2.MagneticValue.Key == Polarity.Negative && MagneticValue.Key == Polarity.Negative)
                {
                    //repel
                    acceleration.X -= magnetForce.X;
                    acceleration.Y -= magnetForce.Y;
                }
            }

            Vector2 keyAcceleration = Vector2.Zero;
            Vector2 step = new Vector2(horizontal_pos, vertical_pos);

            if (ks.IsKeyDown(Keys.Right))
            {
                currentAnimation = "walkRight";

                if (velocity.X < 0.1f)
                {
                    keyAcceleration.X = 0.001f;
                }
            }
            else if (ks.IsKeyDown(Keys.Left))
            {
                currentAnimation = "walkLeft";

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

                if (currentAnimation == "walkRight")
                {
                    currentAnimation = "standingRight";
                }
                else if (currentAnimation == "walkLeft")
                {
                    currentAnimation = "standingLeft";
                }
            }

            if (ks.IsKeyDown(Keys.Up))
            {
                if (onTheGround)
                {
                    velocity.Y = -0.5f;
                }
            }

            Vector2 finalAcceleration = acceleration + keyAcceleration;

            velocity.X += (float)(finalAcceleration.X * delta);
            velocity.Y += (float)(finalAcceleration.Y * delta);

            step.X += (float)(((velocity.X) * delta) + (0.5) * (Math.Pow(delta, 2.0)) * finalAcceleration.X);
            step.Y += (float)(((velocity.Y) * delta) + (0.5) * (Math.Pow(delta, 2.0)) * finalAcceleration.Y);

            checkForWalls(Game1.map, ref step);

            horizontal_pos = step.X;
            vertical_pos = step.Y;

            // if the last frame time hasn't been set, set it now
            if (lastFrameIncrement == 0)
            {
                lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;
            }

            // update the current frame if needed
            if (currentTime.TotalGameTime.TotalMilliseconds - lastFrameIncrement > sheet.getAnimationSpeed(currentAnimation))
            {
                lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;

                currentFrame = (currentFrame + 1) % sheet.getAnimationFrameCount(currentAnimation);
            }
        }

        public override void draw(SpriteBatch sb)
        {
            if (sheet != null)
            {
                sheet.drawAnimationFrame(sb, currentAnimation, currentFrame, Position);
            }
            //sb.Draw(Game1.globalTestWalrus, new Vector2(horizontal_pos, vertical_pos), Color.Yellow);
        }
    }
}
