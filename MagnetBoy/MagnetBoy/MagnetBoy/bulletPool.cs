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

        private bool exploding = false;
        private double explodingTime = 0;

        //animation data
        string currentAnimation = null;
        int currentFrame = 0;
        double lastFrameIncrement = 0;

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
            if (newType == BulletPool.BulletType.TestBullet || newType == BulletPool.BulletType.Bucket)
            {
                creation();
            }

            inUse = true;
            timePassed = 0;

            type = newType;
            horizontal_pos = newX;
            vertical_pos = newY;

            exploding = false;
            explodingTime = 0;

            switch (newType)
            {
                case BulletPool.BulletType.TestBullet:
                    width= 31.5f;
                    height = 31.5f;
                    velocity.X = (float)(testBulletVelocity * Math.Cos(direction));
                    velocity.Y = (float)(testBulletVelocity * Math.Sin(direction));
                    rotation = direction;
                    break;
                case BulletPool.BulletType.LavaBlob:
                    width = 31.5f;
                    height = 31.5f;
                    velocity.X = 0.0f;
                    velocity.Y = 0.35f;
                    rotation = (float)(Math.PI/2);
                    break;
                case BulletPool.BulletType.Bucket:
                    width = 16f;
                    height = 16f;
                    velocity.X = (float)(testBulletVelocity * Math.Cos(direction));
                    velocity.Y = (float)(testBulletVelocity * Math.Sin(direction));
                    rotation = direction;
                    currentAnimation = "bucket";
                    currentFrame = 0;
                    lastFrameIncrement = entryTime.TotalGameTime.Milliseconds;
                    break;
                default:
                    inUse = false;
                    death();
                    break;
            }
        }

        public override void update(GameTime currentTime)
        {
            double delta = currentTime.ElapsedGameTime.Milliseconds;

            timePassed += delta;

            // animation
            if (type == BulletPool.BulletType.Bucket)
            {
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
            }

            // redirection
            if (type == BulletPool.BulletType.TestBullet || (type == BulletPool.BulletType.Bucket && !exploding))
            {
                velocity.X = (float)(testBulletVelocity * Math.Cos(rotation));
                velocity.Y = (float)(testBulletVelocity * Math.Sin(rotation));
            }

            // damage player
            if (type == BulletPool.BulletType.TestBullet || type == BulletPool.BulletType.LavaBlob)
            {
                foreach (Entity en in globalEntityList)
                {
                    if (en is Player)
                    {
                        if (hitTest(en))
                        {
                            if (en.CenterPosition.X - CenterPosition.X < 0)
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

            horizontal_pos += (float)(velocity.X * delta);
            vertical_pos += (float)(velocity.Y * delta);

            if ((timePassed > maxLifeTime || LevelState.isSolidMap(Position)) && !exploding)
            {
                if (type == BulletPool.BulletType.Bucket)
                {
                    exploding = true;

                    velocity.X = 0f;
                    velocity.Y = 0f;

                    currentFrame = 0;
                    currentAnimation = "bucketExplode";
                    lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;
                }
                else
                {
                    death();
                    inUse = false;
                }
            }
            else if (exploding)
            {
                explodingTime += currentTime.ElapsedGameTime.TotalMilliseconds;

                if (explodingTime > AnimationFactory.getAnimationFrameCount(currentAnimation) * AnimationFactory.getAnimationSpeed(currentAnimation))
                {
                    death();
                    inUse = false;
                }
            }
        }

        public override void draw(SpriteBatch sb)
        {
            switch (type)
            {
                case BulletPool.BulletType.TestBullet:
                    AnimationFactory.drawAnimationFrame(sb, "testBullet", 0, Position, HitBox, rotation);
                    break;
                case BulletPool.BulletType.Bucket:
                    AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position, HitBox, rotation);
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
            LavaBlob,
            Bucket
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
                b.death();
            }
        }
    }
}
