﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class Lolrus : Enemy
    {
        private bool shooting = false;
        private double shootingTime = 0;

        public Lolrus(int initalX, int initalY) : base(initalX, initalY)
        {
            list.Add(new Walk(this));
            list.Add(new IntervalShoot(this, 3000, BulletPool.BulletType.TestBullet));
        }

        protected override void enemyUpdate(GameTime currentTime)
        {
            if (shooting)
            {
                velocity.X = 0.0f;

                if (currentAnimation == "lolrusWalkLeft")
                {
                    currentAnimation = "lolrusShootRight";
                }
                else if (currentAnimation == "lolrusWalkRight")
                {
                    currentAnimation = "lolrusShootLeft";
                }

                shootingTime += currentTime.ElapsedGameTime.Milliseconds;

                if (shootingTime > AnimationFactory.getAnimationFrameCount(currentAnimation) * AnimationFactory.getAnimationSpeed(currentAnimation))
                {
                    shooting = false;

                    foreach (Attribute a in list)
                    {
                        if (a is Walk)
                        {
                            a.enableDisable(true);
                        }
                    }
                }
            }
            else
            {
                if (velocity.X < 0.0f)
                {
                    currentAnimation = "lolrusWalkLeft";
                }
                else if (velocity.X > 0.0f)
                {
                    currentAnimation = "lolrusWalkRight";
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

                if (currentAnimation == "lolrusWalkRight" || currentAnimation == "lolrusShootLeft")
                {
                    currentFrame--;
                    if (currentFrame < 0)
                    {
                        currentFrame = AnimationFactory.getAnimationFrameCount(currentAnimation) - 1;
                    }
                }
                else
                {
                    currentFrame = (currentFrame + 1) % AnimationFactory.getAnimationFrameCount(currentAnimation);
                }
            }
        }

        public void lolrusFire()
        {
            shooting = true;
            shootingTime = 0;

            currentFrame = 0;

            velocity.X = 0.0f;

            foreach (Attribute a in list)
            {
                if (a is Walk)
                {
                    a.enableDisable(false);
                }
            }
        }

        public override void draw(SpriteBatch sb)
        {
            AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position);
        }
    }
}
