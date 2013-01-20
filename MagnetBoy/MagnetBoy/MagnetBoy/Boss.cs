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

        private BulletPool.BulletType organBullet = BulletPool.BulletType.Heart;

        //after he walks into the scene, enabled becomes true
        private bool isEnabled = false;
        private double interval = 0.0;
        private double timeSinceLastShot= 0.0;

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
        }

        public override void update(GameTime currentTime)
        {
            timeSinceLastShot += currentTime.ElapsedGameTime.Milliseconds;

            if( timeSinceLastShot > interval)
            {
                float direction = 0.0f;

                if (velocity.X < 0.0f)
                {
                    direction += (float)(Math.PI);
                }

                Vector2 bulletPosition = Position;

                bulletPosition.Y += 4;

                bulletPosition.X += HitBox.X;
              
                BulletPool.pushBullet(organBullet, bulletPosition.X, bulletPosition.Y, currentTime, direction);

                timeSinceLastShot = 0;
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
<<<<<<< HEAD
=======


>>>>>>> origin/Eric-Dev
            return;
        }

        public override void draw(SpriteBatch sb)
        {
            AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position);

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

        public bossShield()
        {
            creation();

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
        }

        public bossShield(float initialx, float initialy)
        {
            creation();

            horizontal_pos = initialx;
            vertical_pos = initialy;
        }
    }
}
