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

        public bool inUse = false;
        private double timePassed = 0;
        private float rotation = 0.0f; // 0.0 in rotation is considered to be right-facing, or "EAST"
        public BulletPool.BulletType type;
        public double maxLifeTime = 1500;
        public bool bulletUsed = false;

        private bool exploding = false;
        private double explodingTime = 0;

        public bool playerEnact = false;

        //animation data
        string currentAnimation = null;
        int currentFrame = 0;
        double lastFrameIncrement = 0;

        public Vector2 initialBucketVelocity = Vector2.Zero;

        float organVelocityX = 0.0f;
        float organVelocityY = 0.0f;

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
            removeFromGame = true;
        }

        public void add(float newX, float newY, BulletPool.BulletType newType, GameTime entryTime, float direction)
        {
            if (newType == BulletPool.BulletType.TestBullet || newType == BulletPool.BulletType.Bucket || newType == BulletPool.BulletType.Heart || newType == BulletPool.BulletType.Brain || newType == BulletPool.BulletType.Lung)
            {
                creation();
            }

            initialBucketVelocity = Vector2.Zero;

            inUse = true;
            timePassed = 0;

            type = newType;
            horizontal_pos = newX;
            vertical_pos = newY;

            exploding = false;
            explodingTime = 0;

            removeFromGame = false;

            pole = Polarity.Neutral;
            magneticMoment = 0.5f;

            if (type == BulletPool.BulletType.Lung)
            {
                organVelocityX = (float)((Game1.gameRandom.Next() % 1.2 + 1.0) * 0.12f);
                organVelocityY = (float)((Game1.gameRandom.Next() % 1 + 1.0) * 0.32f);
            }
            if (type == BulletPool.BulletType.Heart || type == BulletPool.BulletType.healthItem)
            {
                organVelocityX = (float)((Game1.gameRandom.Next() % 1.2 + 1.0) * 0.13f);
                organVelocityY = (float)((Game1.gameRandom.Next() % 1 + 1.0) * 0.4f);
            }
            if (type == BulletPool.BulletType.Brain)
            {
                organVelocityX = (float)((Game1.gameRandom.Next() % 1.2 + 1.0) * 0.14f);
                organVelocityY = (float)((Game1.gameRandom.Next() % 1 + 1.0) * 0.4f);
            }
                

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
                    velocity.Y = 0.375f;
                    currentAnimation = "fireball";
                    rotation = 0.0f;
                    break;
                case BulletPool.BulletType.Bucket:
                    width = 16f;
                    height = 16f;

                    velocity.X = (float)(testBulletVelocity * Math.Cos(direction));
                    velocity.Y = (float)(testBulletVelocity * Math.Sin(direction));

                    initialBucketVelocity.X = velocity.X;
                    initialBucketVelocity.Y = velocity.Y;

                    rotation = direction;
                    maxLifeTime = 1500;
                    currentAnimation = "bucket";
                    currentFrame = 0;
                    lastFrameIncrement = entryTime.TotalGameTime.Milliseconds;
                    break;
                case BulletPool.BulletType.Heart:
                    width = 31.5f;
                    height = 31.5f;
                    velocity.X = (float)(organVelocityX * Math.Cos(direction));
                    velocity.Y = (float)(organVelocityY * Math.Sin(direction));
                    rotation = direction;
                    currentAnimation = "heartIdle2";
                    acceleration.Y = 0.0005f;
                    maxLifeTime = 50000;
                    bulletUsed = false;
                    currentFrame = 0;
                    lastFrameIncrement = entryTime.TotalGameTime.Milliseconds;
                    break;
                case BulletPool.BulletType.Brain:
                    width = 31.5f;
                    height = 31.5f;
                    velocity.X = (float)(organVelocityX * Math.Cos(direction));
                    velocity.Y = (float)(organVelocityY * Math.Sin(direction));
                    rotation = direction;
                    currentAnimation = "brainIdle";
                    maxLifeTime = 50000;
                    acceleration.Y = 0.0007f;
                    bulletUsed = false;
                    currentFrame = 0;
                    lastFrameIncrement = entryTime.TotalGameTime.Milliseconds;
                    
                    break;
                case BulletPool.BulletType.Lung:
                    width = 31.5f;
                    height = 31.5f;
                    velocity.X = (float)(organVelocityX * Math.Cos(direction));
                    velocity.Y = (float)(organVelocityY * Math.Sin(direction));
                    rotation = direction;
                    currentAnimation = "lungIdle";
                    currentFrame = 0;
                    acceleration.Y = 0.0003f;
                    bulletUsed = false;
                    lastFrameIncrement = entryTime.TotalGameTime.Milliseconds;
                    maxLifeTime = 50000;
                    break;
                case BulletPool.BulletType.healthItem:
                    width = 31.5f;
                    height = 31.5f;
                    velocity.X = (float)(organVelocityX * Math.Cos(direction));
                    velocity.Y = (float)(organVelocityY * Math.Sin(direction));
                    rotation = direction;
                    currentAnimation = "heartIdle";
                    acceleration.Y = 0.0005f;
                    maxLifeTime = 2000;
                    bulletUsed = false;
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
            

            // redirection
            if (type == BulletPool.BulletType.TestBullet || (type == BulletPool.BulletType.Bucket && !exploding) || type == BulletPool.BulletType.Heart || type == BulletPool.BulletType.Lung || type == BulletPool.BulletType.Brain)
            {
                if (playerEnact == true)
                {
                    velocity.X = (float)(testBulletVelocity * Math.Cos(rotation));
                    velocity.Y = (float)(testBulletVelocity * Math.Sin(rotation));
                    playerEnact = false;
                }
            }

            // damage player
            if(!exploding)
            {
                foreach (Entity en in globalEntityList)
                {
                    if (en is Player)
                    {
                        if (hitTestPlayerVitals((Player)en))
                        {
                            if (type == BulletPool.BulletType.healthItem)
                            {
                                if (bulletUsed == false)
                                {
                                    if (LevelState.currentPlayerHealth < 5 && LevelState.currentPlayerHealth > 0)
                                    {
                                        LevelState.currentPlayerHealth += 1;
                                        maxLifeTime = 0;
                                    }

                                    bulletUsed = true;
                                }

                            }
                            else
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

                    if (en is Enemy && type == BulletPool.BulletType.Bucket)
                    {
                        if (hitTest(en))
                        {
                            float dotProduct = Vector2.Dot(velocity, initialBucketVelocity);

                            float magnitude = (float)(Math.Sqrt(Math.Pow(velocity.X,2.0) + Math.Pow(velocity.Y,2.0)) * Math.Sqrt(Math.Pow(initialBucketVelocity.X,2.0)+Math.Pow(initialBucketVelocity.Y, 2.0)));
                            float angleBetween = (float)Math.Acos((dotProduct) / magnitude);

                            if (angleBetween >= Math.PI / 2 && angleBetween != 0 || angleBetween <= Math.PI / 2 && angleBetween != 0)
                            {
                                en.deathAnimation = true;
                                en.killedByBullet = true;
                                maxLifeTime = 0;
                            }
                        }
                    }
                    if (en is Enemy && type == BulletPool.BulletType.LavaBlob)
                    {
                        if (hitTest(en))
                        {
                            en.deathAnimation = true;
                            en.killedByBullet = true;
                        }
                    }
                }
            }   

            horizontal_pos += (float)(velocity.X * delta);
            vertical_pos += (float)(velocity.Y * delta);

            if (type == BulletPool.BulletType.Heart || type == BulletPool.BulletType.Brain || type == BulletPool.BulletType.Lung || type == BulletPool.BulletType.healthItem)
            {
                Vector2 keyAcceleration = Vector2.Zero;
                Vector2 step = new Vector2(horizontal_pos, vertical_pos);
                Vector2 finalAcceleration = acceleration + keyAcceleration;

                velocity.X += (float)(finalAcceleration.X * delta);
                velocity.Y += (float)(finalAcceleration.Y * delta);

                step.X += (float)(((velocity.X) * delta) + (0.5) * (Math.Pow(delta, 2.0)) * finalAcceleration.X);
                step.Y += (float)(((velocity.Y) * delta) + (0.5) * (Math.Pow(delta, 2.0)) * finalAcceleration.Y);
                horizontal_pos = step.X;
                vertical_pos = step.Y;
            }

            if ((timePassed > maxLifeTime || LevelState.isSolidMap(CenterPosition)) && !exploding)
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
                else if (type == BulletPool.BulletType.Heart)
                {
                    exploding = true;

                    velocity.X = 0f;
                    velocity.Y = 0f;
                    acceleration.Y = 0f;

                    currentFrame = 0;
                    currentAnimation = "heartDie2";
                    lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;
                }
                else if (type == BulletPool.BulletType.Brain)
                {
                    exploding = true;

                    velocity.X = 0f;
                    velocity.Y = 0f;
                    acceleration.Y = 0f;

                    currentFrame = 0;
                    currentAnimation = "brainDie";
                    lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;
                }
                else if (type == BulletPool.BulletType.Lung)
                {
                    exploding = true;

                    velocity.X = 0f;
                    velocity.Y = 0f;
                    acceleration.Y = 0f;

                    currentFrame = 0;
                    currentAnimation = "lungDie";
                    lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;
                }
                else
                {
                    if (type == BulletPool.BulletType.LavaBlob)
                    {
                        LevelState.levelParticlePool.pushParticle(ParticlePool.ParticleType.SweatDrops, new Vector2(horizontal_pos + width / 2, vertical_pos), Vector2.Zero, (float)((7 * (Math.PI / 4))), 0.0f, Color.Lerp(Color.Yellow, Color.Red, (float)(Game1.gameRandom.NextDouble())));
                        LevelState.levelParticlePool.pushParticle(ParticlePool.ParticleType.SweatDrops, new Vector2(horizontal_pos + width / 2, vertical_pos), Vector2.Zero, (float)((5.4 * (Math.PI / 4))), 0.0f, Color.Lerp(Color.Yellow, Color.Red, (float)(Game1.gameRandom.NextDouble())));
                        LevelState.levelParticlePool.pushParticle(ParticlePool.ParticleType.SweatDrops, new Vector2(horizontal_pos + width / 2, vertical_pos), Vector2.Zero, (float)((5 * (Math.PI / 4))), 0.0f, Color.Lerp(Color.Yellow, Color.Red, (float)(Game1.gameRandom.NextDouble())));
                    }

                    removeFromGame = true;
                    death();
                    inUse = false;
                }
            }
            else if (exploding)
            {
                explodingTime += currentTime.ElapsedGameTime.TotalMilliseconds;

                if (explodingTime > AnimationFactory.getAnimationFrameCount(currentAnimation) * AnimationFactory.getAnimationSpeed(currentAnimation))
                {
                    removeFromGame = true;
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
                    AnimationFactory.drawAnimationFrame(sb, "testBullet", 0, Position, HitBox, rotation, AnimationFactory.DepthLayer0);
                    break;
                case BulletPool.BulletType.LavaBlob:
                    AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position, HitBox, rotation, AnimationFactory.DepthLayer0);
                    break;
                case BulletPool.BulletType.Bucket:
                    AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position, HitBox, rotation, AnimationFactory.DepthLayer0);
                    break;
                case BulletPool.BulletType.Heart:
                    AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position, HitBox, rotation, AnimationFactory.DepthLayer0);
                    break;
                case BulletPool.BulletType.Lung:
                    AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position, HitBox, rotation, AnimationFactory.DepthLayer0);
                    break;
                case BulletPool.BulletType.Brain:
                    AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position, HitBox, rotation, AnimationFactory.DepthLayer0);
                    break;
                case BulletPool.BulletType.healthItem:
                    AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position, HitBox, rotation, AnimationFactory.DepthLayer0);
                    break;
                default:
                    AnimationFactory.drawAnimationFrame(sb, "testBullet", 0, Position, AnimationFactory.DepthLayer0);
                    break;
            }
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

    public class BulletPool
    {
        public enum BulletType
        {
            TestBullet,
            LavaBlob,
            Bucket,
            Heart,
            Brain,
            Lung,
            healthItem
        }

        private static Bullet[] pool = null;

        public static bool shieldUp = false;

        public BulletPool()
        {
            if (pool == null)
            {
                pool = new Bullet[150];

                for (int i = 0; i < pool.Length; i++)
                {
                    pool[i] = new Bullet();
                }
            }
            
        }

        public static void shieldStatus(bool val)
        {
            shieldUp = val;
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

            if (count == pool.Length)
            {
                Console.WriteLine("full2");
            }
        }

        public static void pushBullet(BulletType type, float x, float y, GameTime currentTime, float direction)
        {
            if (Math.Sqrt(Math.Pow(y - LevelState.levelCamera.getFocusPosition().Y, 2) + Math.Pow(x - LevelState.levelCamera.getFocusPosition().X, 2)) > 1000)
            {
                return;
            }

            bool full = true;

            for (int i = 0; i < pool.Length; i++)
            {
                if (pool[i].InUse)
                {
                    continue;
                }
                else
                {
                    full = false;
                    pool[i].add(x, y, type, currentTime, direction);
                    break;
                }
            }

            if (full)
            {
                Console.WriteLine("full");
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
