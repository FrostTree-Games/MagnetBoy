using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class Boss : Entity
    {
        protected int currentFrame = 0;
        protected double lastFrameIncrement = 0;

        private string currentAnimation = "wopleyIdle";

        public bool dying = false;

        double walkSwitchTimer = 0;
        bool walkingLeft = false;
        const float walkerSpeed = 0.09f;

        private BulletPool.BulletType Heart = BulletPool.BulletType.Heart;
        private BulletPool.BulletType Brain = BulletPool.BulletType.Brain;
        private BulletPool.BulletType Lung = BulletPool.BulletType.Lung;

        //after he walks into the scene, enabled becomes true
        private bool isEnabled = false;
        private double interval = 500;
        private double timeSinceLastShot= 0.0;

        public int bossHealth = 0;

        public Boss()
        {
            creation();

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
        }

        public Boss(float initialx, float initialy)
        {
            creation();

            horizontal_pos = initialx;
            vertical_pos = initialy;

            width = 160;
            height = 160;

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;

            acceleration.Y = 0.001f;

            pole = Polarity.Neutral;

            solid = true;
        }

        public override void update(GameTime currentTime)
        {
            timeSinceLastShot += currentTime.ElapsedGameTime.Milliseconds;

            if( timeSinceLastShot > interval )
            {
                float direction = 0.0f;
                
                direction += (float)(-0.75 * Math.PI);

                Vector2 bulletPosition = Position;

                bulletPosition.Y += 4;

                if (Game1.gameRandom.Next() % 7 == 0)
                {
                    BulletPool.pushBullet(Heart, bulletPosition.X, bulletPosition.Y, currentTime, direction);
                }

                if (Game1.gameRandom.Next() % 6 == 0)
                {
                    BulletPool.pushBullet(Brain, bulletPosition.X, bulletPosition.Y, currentTime, direction);
                }

                if (Game1.gameRandom.Next() % 8 == 0)
                {
                    BulletPool.pushBullet(Lung, bulletPosition.X, bulletPosition.Y, currentTime, direction);
                }
             
                timeSinceLastShot = 0;
            }

            foreach (Entity en in Entity.globalEntityList)
            {
                if (en is Player)
                {
                    if (hitTest(en))
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

            return;
        }

        public override void draw(SpriteBatch sb)
        {
            AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position, AnimationFactory.DepthLayer1);

            return;
        }

        private void walk(GameTime currentTime)
        {
            if (walkSwitchTimer == 0)
            {
                walkSwitchTimer = currentTime.TotalGameTime.TotalMilliseconds;
            }

            if (onTheGround)
            {
                if (Math.Abs(velocity.X) < 0.01f)
                {
                    walkingLeft = !walkingLeft;
                }

                if (walkingLeft && velocity.X > -walkerSpeed)
                {
                    acceleration.X = -0.001f;
                }
                else if (velocity.X < walkerSpeed)
                {
                    acceleration.X = 0.001f;
                }
                else if (velocity.X < -walkerSpeed)
                {
                    velocity.X = -walkerSpeed;
                }
                else if (velocity.X > walkerSpeed)
                {
                    velocity.X = walkerSpeed;

                }
            }
        }
    }

    public class bossShield : Entity
    {
        protected int currentFrame = 0;
        protected double lastFrameIncrement = 0;

        private string currentAnimation = null;

        private float interval = 10000;
        private float timeLastMoved = 0.0f;
        public static float yPosDisplacement = 1.0f;

        public static int shieldHealth = 0;

        public bossShield()
        {
            creation();

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
        }

        public bossShield(float initialx, float initialy)
        {
            creation();

            shieldHealth = 21;

            horizontal_pos = initialx;
            vertical_pos = initialy;

            width = 31.5f;
            height = 31.5f;

            solid = true;
        }

        public override void update(GameTime currentTime)
        {
            timeLastMoved += currentTime.ElapsedGameTime.Milliseconds;

            if (timeLastMoved > interval)
            {
                BulletPool.shieldUp = !BulletPool.shieldUp;
            }

            if (BulletPool.shieldUp == true)
            {
                if (yPosDisplacement < 64)
                {
                    vertical_pos -= 0.1f;
                    yPosDisplacement += 0.1f;
                    timeLastMoved = 0.0f;
                }
            }

            if (BulletPool.shieldUp == false)
            {
                if (yPosDisplacement > 0)
                {
                    vertical_pos += 0.1f;
                    yPosDisplacement -= 0.1f;
                    timeLastMoved = 0.0f;
                }
            }

            foreach(Entity b in globalEntityList)
            {
                
                if (b is Bullet)
                {
                    if (hitTest(b))
                    {
                        Console.WriteLine(((Bullet)b).type);
                        if (((Bullet)b).velocity.X < 0)
                        {
                            ((Bullet)b).inUse = false;
                            ((Bullet)b).removeFromGame = true;
                            ((Bullet)b).death();
                            death();
                            break;
                        }
                        else
                        {
                            shieldHealth -= 1;
                            ((Bullet)b).inUse = false;
                            ((Bullet)b).removeFromGame = true;
                            ((Bullet)b).death();
                            break;
                        }
                    }
                }
            }

            if (shieldHealth == 0)
            {
                removeFromGame = true;
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

                //currentFrame = (currentFrame + 1) % AnimationFactory.getAnimationFrameCount(currentAnimation);
            }
        }

        public override void draw(SpriteBatch sb)
        {
            sb.Draw(Game1.globalTestWalrus, new Vector2(horizontal_pos, vertical_pos), Color.Yellow);
        }
    }
}
