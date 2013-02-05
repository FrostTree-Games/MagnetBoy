using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class ParticlePool
    {
        private const float blueSparkVelocity = 0.3f;
        private const float blueSparkAccel = 0.005f;
        private const float blueSparkLiveTime = 750f;

        private const float sweatDropVelocity = 0.5f;
        private const float sweatDropAccel = 0.019f;
        private const float sweatDropLiveTime = 100;

        private const float colouredSparkVelocity = 0.3f;

        string dansAnim = "dansParticle";

        public enum ParticleType
        {
            BlueSpark,
            SweatDrops,
            ColouredSpark
        }

        private struct Particle
        {
            public bool active;
            public ParticleType type;
            public double timeActive;

            public bool bounce;

            public Vector2 startPosition;

            public Vector2 pos;
            public Vector2 velocity;
            public Vector2 accel;

            public Vector2 posPrevA;
            public Vector2 posPrevB;
            public Vector2 posPrevC;
            public Vector2 posPrevD;

            public Color color;
            public Color colorPrevA;
            public Color colorPrevB;
            public Color colorPrevC;
            public Color colorPrevD;

            public int currentFrame;
            public double lastFrameIncrement;
            public int frameCount;
            public double frameSpeed;
        }

        private Particle[] pool = null;

        public ParticlePool(int size)
        {
            pool = new Particle[size];

            for (int i = 0; i < pool.Length; i++)
            {
                pool[i].active = false;
            }
        }

        public void pushParticle(ParticleType newType, Vector2 newPos, Vector2 offsetVelocity, float direction, float acceldir, Color tint)
        {
            for (int i = 0; i < pool.Length; i++)
            {
                if (pool[i].active == false)
                {
                    pool[i].active = true;
                    pool[i].type = newType;

                    pool[i].startPosition = newPos;

                    switch (newType)
                    {
                        case ParticleType.BlueSpark:
                            pool[i].bounce = true;
                            pool[i].pos = newPos;
                            pool[i].posPrevA = newPos;
                            pool[i].posPrevB = newPos;
                            pool[i].posPrevC = newPos;
                            pool[i].posPrevD = newPos;
                            pool[i].velocity.X = (float)(blueSparkVelocity * Math.Cos(direction));
                            pool[i].velocity.Y = (float)(blueSparkVelocity * Math.Sin(direction));
                            pool[i].accel = Vector2.Zero;
                            pool[i].timeActive = 0;
                            pool[i].color = Game1.gameRandom.Next() % 2 == 0 ? Color.Cyan : Color.DarkCyan;
                            pool[i].colorPrevA = pool[i].color;
                            pool[i].colorPrevB = pool[i].color;
                            pool[i].colorPrevC = pool[i].color;
                            pool[i].colorPrevD = pool[i].color;
                            pool[i].currentFrame = 0;
                            pool[i].lastFrameIncrement = 0;
                            pool[i].frameCount = AnimationFactory.getAnimationFrameCount(dansAnim);
                            pool[i].frameSpeed = AnimationFactory.getAnimationSpeed(dansAnim);
                            if (pool[i].velocity.X * offsetVelocity.X >= 0)
                            {
                                pool[i].velocity.X += offsetVelocity.X;
                            }
                            break;
                        case ParticleType.SweatDrops:
                            pool[i].bounce = false;
                            pool[i].pos = newPos;
                            pool[i].posPrevA = newPos;
                            pool[i].posPrevB = newPos;
                            pool[i].posPrevC = newPos;
                            pool[i].posPrevD = newPos;
                            pool[i].velocity.X = (float)((sweatDropVelocity * Math.Cos(direction)) / 2.5f);
                            pool[i].velocity.Y = (float)(sweatDropVelocity * Math.Sin(direction));
                            pool[i].accel = new Vector2(0, sweatDropAccel);
                            pool[i].timeActive = 0;
                            pool[i].color = Color.White;
                            pool[i].colorPrevA = pool[i].color;
                            pool[i].colorPrevB = pool[i].color;
                            pool[i].colorPrevC = pool[i].color;
                            pool[i].colorPrevD = pool[i].color;
                            pool[i].currentFrame = 2;
                            pool[i].lastFrameIncrement = 0;
                            pool[i].frameCount = AnimationFactory.getAnimationFrameCount(dansAnim);
                            pool[i].frameSpeed = AnimationFactory.getAnimationSpeed(dansAnim);
                            if (pool[i].velocity.Y > 0)
                            {
                                pool[i].velocity.Y *= -1;
                            }
                            break;
                        case ParticleType.ColouredSpark:
                            pool[i].bounce = false;
                            pool[i].pos = newPos;
                            pool[i].color = tint;
                            pool[i].velocity.X = (float)(colouredSparkVelocity * Math.Cos(direction));
                            pool[i].velocity.Y = (float)(colouredSparkVelocity * Math.Sin(direction));
                            pool[i].accel = Vector2.Zero;
                            pool[i].timeActive = 0;
                            pool[i].currentFrame = 2;
                            pool[i].lastFrameIncrement = 0;
                            pool[i].frameCount = AnimationFactory.getAnimationFrameCount(dansAnim);
                            pool[i].frameSpeed = AnimationFactory.getAnimationSpeed(dansAnim);
                            break;
                        default:
                            break;
                    }

                    break;
                }
            }
        }

        public void updatePool(GameTime currentTime)
        {
            for (int i = 0; i < pool.Length; i++)
            {
                if (pool[i].active == false)
                {
                    continue;
                }
                else
                {
                    pool[i].lastFrameIncrement += currentTime.ElapsedGameTime.Milliseconds;

                    if (pool[i].lastFrameIncrement > pool[i].frameSpeed)
                    {
                        pool[i].lastFrameIncrement = 0;
                        pool[i].currentFrame = (pool[i].currentFrame + 1) % pool[i].frameCount;
                    }

                    pool[i].timeActive += currentTime.ElapsedGameTime.Milliseconds;
                    if (pool[i].timeActive > ((pool[i].type == ParticleType.BlueSpark) ? blueSparkLiveTime : blueSparkLiveTime/2) || (Vector2.Distance(pool[i].pos, pool[i].startPosition) > 128 && pool[i].type == ParticleType.BlueSpark))
                    {
                        pool[i].active = false;
                        continue;
                    }

                    Vector2 stepX = currentTime.ElapsedGameTime.Milliseconds * pool[i].velocity;
                    Vector2 stepY = stepX;
                    stepX.Y = 0.0f;
                    stepY.X = 0.0f;

                    if (pool[i].bounce)
                    {
                        if (LevelState.isSolidMap(pool[i].pos + stepX))
                        {
                            pool[i].velocity.X *= -1;
                            pool[i].accel.X *= -1;
                        }

                        if (LevelState.isSolidMap(pool[i].pos + stepY))
                        {
                            pool[i].velocity.Y *= -1;
                            pool[i].accel.Y *= -1;
                        }
                    }

                    pool[i].posPrevD = pool[i].posPrevC;
                    pool[i].posPrevC = pool[i].posPrevB;
                    pool[i].posPrevB = pool[i].posPrevA;
                    pool[i].posPrevA = pool[i].pos;
                    pool[i].pos += currentTime.ElapsedGameTime.Milliseconds * pool[i].velocity;
                    pool[i].velocity += pool[i].accel;
                }
            }
        }

        public void drawPool(SpriteBatch sb)
        {
            foreach (Particle p in pool)
            {
                if (!p.active)
                {
                    continue;
                }

                switch (p.type)
                {
                    case ParticleType.BlueSpark:
                        AnimationFactory.drawAnimationFrame(sb, dansAnim, p.currentFrame, p.posPrevD - new Vector2(4f, 4f), p.colorPrevD, AnimationFactory.DepthLayer0);
                        AnimationFactory.drawAnimationFrame(sb, dansAnim, p.currentFrame, p.posPrevC - new Vector2(4f, 4f), p.colorPrevC, AnimationFactory.DepthLayer0);
                        AnimationFactory.drawAnimationFrame(sb, dansAnim, p.currentFrame, p.posPrevB - new Vector2(4f, 4f), p.colorPrevB, AnimationFactory.DepthLayer0);
                        AnimationFactory.drawAnimationFrame(sb, dansAnim, p.currentFrame, p.posPrevA - new Vector2(4f, 4f), p.colorPrevA, AnimationFactory.DepthLayer0);
                        AnimationFactory.drawAnimationFrame(sb, dansAnim, p.currentFrame, p.pos - new Vector2(4f, 4f), p.color, AnimationFactory.DepthLayer0);
                        break;
                    case ParticleType.SweatDrops:
                        AnimationFactory.drawAnimationFrame(sb, dansAnim, 0, p.posPrevB - new Vector2(4f, 4f), p.colorPrevB, AnimationFactory.DepthLayer0);
                        AnimationFactory.drawAnimationFrame(sb, dansAnim, 1, p.posPrevA - new Vector2(4f, 4f), p.colorPrevA, AnimationFactory.DepthLayer0);
                        AnimationFactory.drawAnimationFrame(sb, dansAnim, 2, p.pos - new Vector2(4f, 4f), p.color, AnimationFactory.DepthLayer0);
                        break;
                    case ParticleType.ColouredSpark:
                        AnimationFactory.drawAnimationFrame(sb, dansAnim, 2, p.pos - new Vector2(4f, 4f), p.color, AnimationFactory.DepthLayer0);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
