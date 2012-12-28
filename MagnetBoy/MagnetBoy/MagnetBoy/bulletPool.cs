using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class Bullet : Entity
    {
        private const float testBulletVelocity = 0.25f;

        private bool inUse = false;
        private double timePassed = 0;
        private double maxLifeTime = 1500; 
        private float rotation = 0.0f; // 0.0 in rotation is considered to be right-facing, or "EAST"
        private BulletPool.BulletType type;

        public float Direction
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        public bool InUse
        {
            get
            {
                return inUse;
            }
            set
            {
                inUse = value;
            }
        }

        public Bullet()
        {
            inUse = false;
        }

        public void add(float newX, float newY, BulletPool.BulletType newType, GameTime entryTime, float direction)
        {
            creation();

            inUse = true;
            timePassed = 0;

            type = newType;
            horizontal_pos = newX;
            vertical_pos = newY;

            switch (newType)
            {
                case BulletPool.BulletType.TestBullet:
                    width= 31.5f;
                    height = 31.5f;
                    velocity.X = (float)(testBulletVelocity * Math.Cos(direction));
                    velocity.Y = (float)(testBulletVelocity * Math.Sin(direction));
                    rotation = direction;
                    break;
                default:
                    inUse = false;
                    break;
            }
        }

        public override void update(GameTime currentTime)
        {
            double delta = currentTime.ElapsedGameTime.Milliseconds;

            timePassed += delta;

            velocity.X = (float)(testBulletVelocity * Math.Cos(rotation));
            velocity.Y = (float)(testBulletVelocity * Math.Sin(rotation));

            horizontal_pos += (float)(velocity.X * delta);
            vertical_pos += (float)(velocity.Y * delta);

            if (timePassed > maxLifeTime)
            {
                death();

                inUse = false;
            }
        }

        public override void draw(SpriteBatch sb)
        {
            switch (type)
            {
                case BulletPool.BulletType.TestBullet:
                    AnimationFactory.drawAnimationFrame(sb, "testBullet", 0, Position, HitBox, rotation);
                    break;
                default:
                    AnimationFactory.drawAnimationFrame(sb, "testBullet", 0, Position);
                    break;
            }
        }
    }

    public class BulletPool
    {
        public enum BulletType
        {
            TestBullet,
            Laser,
            Rocket
        }

        private static Bullet[] pool = null;

        public BulletPool()
        {
            if (pool == null)
            {
                pool = new Bullet[50];

                for (int i = 0; i < pool.Length; i++)
                {
                    pool[i] = new Bullet();
                }
            }
            
        }

        public void updatePool(GameTime currentTime)
        {
            int count = 0;

            foreach (Bullet b in pool)
            {
                if (b.InUse)
                {
                    b.update(currentTime);
                    count++;
                }
            }
        }

        public static void pushBullet(BulletType type, float x, float y, GameTime currentTime, float direction)
        {
            for (int i = 0; i < pool.Length; i++)
            {
                if (pool[i].InUse)
                {
                    continue;
                }
                else
                {
                    pool[i].add(x, y, type, currentTime, direction);
                    break;
                }
            }
        }

        public void drawPool(SpriteBatch sb)
        {
            foreach (Bullet b in pool)
            {
                if (b.InUse)
                {
                    b.draw(sb);
                }
            }
        }

        public void clearPool()
        {
            foreach (Bullet b in pool)
            {
                b.InUse = false;
            }
        }
    }
}
