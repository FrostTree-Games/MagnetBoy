using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class AngrySaw : Enemy
    {
        public AngrySaw(float initialx, float initialy) : base(initialx, initialy)
        {
            list.Add(new Walk(this));

            currentFrame = 0;
            lastFrameIncrement = 0;

            removeFromGame = false;
        }

        protected override void enemyUpdate(GameTime currentTime)
        {
            if (deathAnimation == true)
            {
                if (deathAnimationSet == false)
                {
                    if (velocity.X < 0.0)
                    {
                        lastFrameIncrement = 0.0;
                        currentFrame = 0;
                        velocity.X = 0.0f;
                        currentAnimation = "angrySawDieLeft";
                        deathAnimationSet = true;
                    }
                    else
                    {
                        lastFrameIncrement = 0.0;
                        currentFrame = 0;
                        velocity.X = 0.0f;
                        currentAnimation = "angrySawDieRight";
                        deathAnimationSet = true;
                    }
                }

                
                if (deathTimer > 500)
                {
                    removeFromGame = true;
                    deathTimer = 0.0;
                }
                else
                {
                    deathTimer += currentTime.ElapsedGameTime.Milliseconds;
                }
            }
            else
            {
                if (Math.Abs(velocity.X) < 0.001)
                {
                    if (velocity.X > 0)
                    {
                        currentAnimation = "angrySawIdleRight";
                    }
                    else if (velocity.X < 0)
                    {
                        currentAnimation = "angrySawIdleLeft";
                    }
                }
                else if (velocity.X > 0)
                {
                    currentAnimation = "angrySawWalkLeft";
                }
                else if (velocity.X < 0)
                {
                    currentAnimation = "angrySawWalkRight";
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

                currentFrame = (currentFrame + 1) % AnimationFactory.getAnimationFrameCount(currentAnimation);
            }
            
        }

        public override void draw(SpriteBatch sb)
        {
            AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position);
        }

        public void dieAnimation()
        {
            //AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position);
            return;
        }
    }
}
