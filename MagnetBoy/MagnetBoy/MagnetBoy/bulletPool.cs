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
        private double startTime = 0;
        private double maxLifeTime = 2500; 
        private float rotation = 0.0f; // 0.0 in rotation is considered to be right-facing, or "EAST"
        private BulletPool.BulletType type;

        public bool InUse
        {
            get
            {
                return inUse;
            }
        }

        public Bullet()
        {
            inUse = false;
        }

        public void add(float newX, float newY, BulletPool.BulletType newType, GameTime entryTime, float direction)
        {
            inUse = true;
            startTime = entryTime.TotalGameTime.Milliseconds;

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

            horizontal_pos += (float)(velocity.X * delta);
            vertical_pos += (float)(velocity.Y * delta);

            if (currentTime.TotalGameTime.Milliseconds - startTime > maxLifeTime)
            {
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

    class BulletPool
    {
        public enum BulletType
        {
            TestBullet,
            Laser,
            Rocket
        }

        private Bullet[] pool = new Bullet[20];

        public BulletPool()
        {
            for (int i = 0; i < pool.Length; i++)
            {
                pool[i] = new Bullet();
            }
        }

        public void updatePool(GameTime currentTime)
        {
            foreach (Bullet b in pool)
            {
                if (!b.InUse)
                {
                    //
                }
            }
        }

        public void pushBullet(BulletType type, float x, float y, GameTime currentTime, float direction)
        {
            //
        }
    }
}
