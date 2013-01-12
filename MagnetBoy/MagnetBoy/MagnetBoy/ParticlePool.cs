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
        private const float blueSparkLiveTime = 350f;

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

            public Color color;
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

        public void pushParticle(ParticleType newType, Vector2 newPos, float direction)
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
                            pool[i].velocity.X = (float)(blueSparkVelocity * Math.Cos(direction));
                            pool[i].velocity.Y = (float)(blueSparkVelocity * Math.Sin(direction));
                            pool[i].accel.X = (float)(blueSparkAccel * Math.Cos(direction + 0.7 * ((Game1.gameRandom.Next() % 3) - 1)));
                            pool[i].accel.Y = (float)(blueSparkAccel * Math.Sin(direction + 0.7 * ((Game1.gameRandom.Next() % 3) - 1)));
                            pool[i].timeActive = 0;
                            pool[i].color = Game1.gameRandom.Next() % 2 == 0 ? Color.Cyan : Color.DarkCyan;
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
                    pool[i].pos += currentTime.ElapsedGameTime.Milliseconds * pool[i].velocity;
                    pool[i].velocity += pool[i].accel;
                    pool[i].timeActive += currentTime.ElapsedGameTime.Milliseconds;

                    if (pool[i].timeActive > blueSparkLiveTime)
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
                        AnimationFactory.drawAnimationFrame(sb, "dansParticle", (int)(p.timeActive / 100) % 2, p.pos - new Vector2(8f, 8f), p.color);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
