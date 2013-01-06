using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class Enemy: Entity
    {
        public string currentAnimation = null;
        protected int currentFrame = 0;
        protected double lastFrameIncrement = 0;
        public List<Attribute> list = null;

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

            pole = Polarity.Neutral;
            magneticMoment = 0.5f;

            list = new List<Attribute>();

        }

        public override void update(GameTime currentTime)
        {
            double delta = currentTime.ElapsedGameTime.Milliseconds;

            //reset the acceleration vector and recompute it
            acceleration = Vector2.Zero;
            acceleration.Y = 0.001f;

            foreach( Attribute n in list)
            {
                n.update(this, currentTime);
            }

            foreach (Entity en in Entity.globalEntityList)
            {
                if (en is Player)
                {
                    if (hitTest(en))
                    {
                        if (en.Position.X - Position.X < 0)
                        {
                            ((Player)en).knockBack(new Vector2(-1, -5), currentTime.TotalGameTime.TotalMilliseconds);
                        }
                        else
                        {
                            ((Player)en).knockBack(new Vector2(1, -5), currentTime.TotalGameTime.TotalMilliseconds);
                        }

                        LevelState.currentPlayerHealth = LevelState.currentPlayerHealth - 1;
                    }
                }
            }

            acceleration = acceleration + computeMagneticForce();
            
            Vector2 keyAcceleration = Vector2.Zero;
            Vector2 step = new Vector2(horizontal_pos, vertical_pos);
            Vector2 finalAcceleration = acceleration + keyAcceleration;

            velocity.X += (float)(finalAcceleration.X * delta);
            velocity.Y += (float)(finalAcceleration.Y * delta);

            step.X += (float)(((velocity.X) * delta) + (0.5) * (Math.Pow(delta, 2.0)) * finalAcceleration.X);
            step.Y += (float)(((velocity.Y) * delta) + (0.5) * (Math.Pow(delta, 2.0)) * finalAcceleration.Y);

            enemyUpdate(currentTime);

            checkForWalls(LevelState.CurrentLevel, ref step);

            horizontal_pos = step.X;
            vertical_pos = step.Y;
        }

        protected virtual void enemyUpdate(GameTime currentTime)
        {
            return;
        }

        public override void draw(SpriteBatch sb)
        {
            sb.Draw(Game1.globalTestWalrus, new Vector2(horizontal_pos, vertical_pos), Color.Yellow);
        }

        public void addAttribute(Attribute attr)
        {
            list.Add(new Walk(this));
        }
    }
}
