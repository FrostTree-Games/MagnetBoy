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

        //dead, as in has been "killed by ingame action", such as the player, spikes, etc.
        public bool dying = false;
        public double deathTimer = 0.0;

        protected double pushTime;
        public double PushTime
        {
            get
            {
                return pushTime;
            }
            set
            {
                pushTime = value;
            }

        }

        public Enemy()
        {
            creation();

            horizontal_pos = 3.0f;
            vertical_pos = 3.0f;

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;

            acceleration.Y = 0.001f;

            pushTime = 0;
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

            pushTime = 0;

            list = new List<Attribute>();

        }

        public override void update(GameTime currentTime)
        {
            double delta = currentTime.ElapsedGameTime.Milliseconds;

            //reset the acceleration vector and recompute it
            acceleration = Vector2.Zero;
            acceleration.Y = 0.001f;

            if (pushTime > 0)
            {
                pushTime -= delta;
            }

            if (!deathAnimation)
            {
                foreach (Attribute n in list)
                {
                    n.update(this, currentTime);
                }
            }

            if(deathAnimationSet == false)
            {
                foreach (Entity en in Entity.globalEntityList)
                {
                    if (en is Player)
                    {
                        if (hitTestPlayerVitals((Player)en))
                        {
                            if (!(!en.onTheGround && en.velocity.Y > 0.001f && en.Position.Y < vertical_pos))
                            {
                                if (en.Position.X - Position.X < 0)
                                {
                                    ((Player)en).knockBack(new Vector2(-1, -5), currentTime.TotalGameTime.TotalMilliseconds);
                                }
                                else
                                {
                                    ((Player)en).knockBack(new Vector2(1, -5), currentTime.TotalGameTime.TotalMilliseconds);
                                }
                            }
                        }
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

        public void dieAnimation(SpriteBatch sb)
        {
            return;
        }

        public override void draw(SpriteBatch sb)
        {
            sb.Draw(Game1.globalTestWalrus, new Vector2(horizontal_pos, vertical_pos), Color.Yellow);
        }

        //this method is interesting
        public void addAttribute(Attribute attr)
        {
            list.Add(new Walk(this));
        }

        private bool hitTestPlayerVitals(Player pl)
        {
            Vector2 vitalsPos = pl.Position;
            vitalsPos.X += (pl.HitBox.X - pl.VitalsBox.X) / 2;
            vitalsPos.Y += (pl.HitBox.Y - pl.VitalsBox.Y) / 2;

            if (horizontal_pos > vitalsPos.X + pl.VitalsBox.X || horizontal_pos + width < vitalsPos.X || vertical_pos > vitalsPos.Y + pl.VitalsBox.Y || vertical_pos + height < vitalsPos.Y)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    } 
}
