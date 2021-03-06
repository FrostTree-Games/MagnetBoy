﻿using System;
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
        private BulletPool.BulletType healthItem = BulletPool.BulletType.healthItem;

        //after he walks into the scene, enabled becomes true
        private double interval = 500;
        private double timeSinceLastShot = 0.0;

        private double deathTimer = 0.0;

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

            bossHealth = 15;

            solid = true;

            deathTimer = 0.0;
        }

        public override void update(GameTime currentTime)
        {
            timeSinceLastShot += currentTime.ElapsedGameTime.Milliseconds;

            if (deathAnimation == true)
            {
                if (deathAnimationSet == false)
                {
                    lastFrameIncrement = 0.0;
                    currentFrame = 0;
                    currentAnimation = "wopleyDie";
                    deathAnimationSet = true;
                }

                if (deathTimer > 500)
                {
                    removeFromGame = true;
                    deathAnimationSet = false;
                    deathTimer = 0.0f;
                }
                else
                {
                    deathTimer += currentTime.ElapsedGameTime.Milliseconds;
                }
            }
            else
            {
                if (timeSinceLastShot > interval)
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

                    if (Game1.gameRandom.Next() % 15 == 0)
                    {
                        BulletPool.pushBullet(healthItem, bulletPosition.X, bulletPosition.Y, currentTime, direction);
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

                    if (en is Bullet)
                    {
                        if (hitTest(en))
                        {
                            if (((Bullet)en).velocity.X > 0)
                            {
                                if (((Bullet)en).bulletUsed == false)
                                {
                                    bossHealth -= 1;
                                    ((Bullet)en).bulletUsed = true;
                                }
                                ((Bullet)en).deathAnimationSet = true;
                                ((Bullet)en).velocity.X = 0;
                                ((Bullet)en).velocity.Y = 0;
                                ((Bullet)en).acceleration.Y = 0;
                                ((Bullet)en).maxLifeTime = 0;
                                break;
                            }
                        }
                    }

                }

                if (bossHealth == 0)
                {
                    LevelState.setFlag(LevelState.FlagColor.Blue, true);
                    bossShield.shieldHealth = 0;
                    deathAnimation = true;
                    AudioFactory.playSFX("sfx/wopleyDeath");
                }
                // if the last frame time hasn't been set, set it now
                if (lastFrameIncrement == 0)
                {
                    lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;
                }
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
            AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position, AnimationFactory.DepthLayer1);

            return;
        }
    }

    public class bossShield : Entity
    {
        protected int currentFrame = 0;
        protected double lastFrameIncrement = 0;

        private string currentAnimation = "wopleyShieldIdle";

        private float interval = 10000;
        private float timeLastMoved = 0.0f;
        public static float yPosDisplacement = 0.0f;

        public static int shieldHealth = 0;
        public static int maxShieldHealth = 0;

        public bossShield()
        {
            creation();

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
        }

        public bossShield(float initialx, float initialy)
        {
            creation();

            shieldHealth = 10;
            maxShieldHealth = 10;

            horizontal_pos = initialx;
            vertical_pos = initialy;

            width = 31.5f;
            height = 127.5f;

            yPosDisplacement = 0f;

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
                if (yPosDisplacement <= 64)
                {
                    vertical_pos -= 0.05f;
                    yPosDisplacement += 0.05f;
                    timeLastMoved = 0.0f;
                }
            }

            if (BulletPool.shieldUp == false)
            {
                if (yPosDisplacement >= 0)
                {
                    vertical_pos += 0.1f;
                    yPosDisplacement -= 0.1f;
                    timeLastMoved = 0.0f;
                }
            }

            foreach (Entity b in globalEntityList)
            {
                if (b is Player)
                {
                    if (hitTest(b))
                    {
                        if (!(!b.onTheGround && b.velocity.Y > 0.001f && b.Position.Y < vertical_pos))
                        {
                            if (b.Position.X - Position.X < 0)
                            {
                                ((Player)b).knockBack(new Vector2(-1, -5), currentTime.TotalGameTime.TotalMilliseconds);
                            }
                            else
                            {
                                ((Player)b).knockBack(new Vector2(1, -5), currentTime.TotalGameTime.TotalMilliseconds);
                            }
                        }
                    }
                }

                if (b is Bullet)
                {
                    if (hitTest(b))
                    {
                        if (((Bullet)b).velocity.X < 0)
                        {
                            ((Bullet)b).inUse = false;
                            ((Bullet)b).velocity.X = 0;
                            ((Bullet)b).velocity.Y = 0;
                            ((Bullet)b).acceleration.Y = 0;
                            ((Bullet)b).bulletUsed = true;
                            ((Bullet)b).maxLifeTime = 0;
                            break;
                        }
                        else
                        {
                            if (((Bullet)b).bulletUsed == false)
                            {
                                shieldHealth -= 1;
                                ((Bullet)b).bulletUsed = true;
                            }
                            ((Bullet)b).deathAnimationSet = true;
                            ((Bullet)b).velocity.X = 0;
                            ((Bullet)b).velocity.Y = 0;
                            ((Bullet)b).acceleration.Y = 0;
                            ((Bullet)b).maxLifeTime = 0;
                            break;
                        }
                    }
                }
            }

            if (shieldHealth == maxShieldHealth * 0.8)
            {
                currentAnimation = "wopleyShieldHurt1";
            }

            else if (shieldHealth == maxShieldHealth * 0.6)
            {
                currentAnimation = "wopleyShieldHurt2";
            }

            else if (shieldHealth == maxShieldHealth * 0.4)
            {
                currentAnimation = "wopleyShieldHurt3";
            }

            else if (shieldHealth == maxShieldHealth * 0.2)
            {
                currentAnimation = "wopleyShieldHurt4";
            }

            else if (shieldHealth == 0)
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
            }
        }

        public override void draw(SpriteBatch sb)
        {
            AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position, AnimationFactory.DepthLayer1);
        }
    }
}