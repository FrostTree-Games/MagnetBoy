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
        private const float blueSparkVelocity = 0.2f;
        private const float blueSparkAccel = 0.005f;
        private const float blueSparkLiveTime = 500f;

        public enum ParticleType
        {
            BlueSpark
        }

        private struct Particle
        {
            public bool active;
            public ParticleType type;
            public double timeActive;

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

        public void pushParticle(ParticleType newType, Vector2 newPos, float direction, float acceldir)
        {
            for (int i = 0; i < pool.Length; i++)
            {
                if (pool[i].active == false)
                {
                    pool[i].active = true;
                    pool[i].type = newType;

                    switch (newType)
                    {
                        case ParticleType.BlueSpark:
                            pool[i].pos = newPos;
                            pool[i].posPrevA = newPos;
                            pool[i].posPrevB = newPos;
                            pool[i].posPrevC = newPos;
                            pool[i].posPrevD = newPos;
                            pool[i].velocity.X = (float)(blueSparkVelocity * Math.Cos(direction));
                            pool[i].velocity.Y = (float)(blueSparkVelocity * Math.Sin(direction));
                            pool[i].accel.X = (float)(blueSparkAccel * Math.Cos(acceldir + 0.7));
                            pool[i].accel.Y = (float)(blueSparkAccel * Math.Sin(acceldir + 0.7));
                            pool[i].timeActive = 0;
                            pool[i].color = Game1.gameRandom.Next() % 2 == 0 ? Color.Cyan : Color.DarkCyan;
                            pool[i].colorPrevA = pool[i].color;
                            pool[i].colorPrevB = pool[i].color;
                            pool[i].colorPrevC = pool[i].color;
                            pool[i].colorPrevD = pool[i].color;
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
                    pool[i].posPrevD = pool[i].posPrevC;
                    pool[i].posPrevC = pool[i].posPrevB;
                    pool[i].posPrevB = pool[i].posPrevA;
                    pool[i].posPrevA = pool[i].pos;
                    pool[i].pos += currentTime.ElapsedGameTime.Milliseconds * pool[i].velocity;
                    pool[i].velocity += pool[i].accel;
                    pool[i].timeActive += currentTime.ElapsedGameTime.Milliseconds;

                    if (pool[i].timeActive > blueSparkLiveTime || LevelState.isSolidMap(pool[i].pos))
                    {
                        pool[i].active = false;
                    }
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
                        AnimationFactory.drawAnimationFrame(sb, "dansParticle", (int)(p.timeActive / 100) % 2, p.posPrevD - new Vector2(4f, 4f), p.colorPrevD);
                        AnimationFactory.drawAnimationFrame(sb, "dansParticle", (int)(p.timeActive / 100) % 2, p.posPrevC - new Vector2(4f, 4f), p.colorPrevC);
                        AnimationFactory.drawAnimationFrame(sb, "dansParticle", (int)(p.timeActive / 100) % 2, p.posPrevB - new Vector2(4f, 4f), p.colorPrevB);
                        AnimationFactory.drawAnimationFrame(sb, "dansParticle", (int)(p.timeActive / 100) % 2, p.posPrevA - new Vector2(4f, 4f), p.colorPrevA);
                        AnimationFactory.drawAnimationFrame(sb, "dansParticle", (int)(p.timeActive / 100) % 2, p.pos - new Vector2(4f, 4f), p.color);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
