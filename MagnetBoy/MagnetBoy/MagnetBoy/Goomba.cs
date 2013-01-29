using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class Goomba : Enemy
    {
        public Goomba(float initialx, float initialy) : base(initialx, initialy)
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
                        currentAnimation = "goombaDieRight";
                        deathAnimationSet = true;
                    }
                    else
                    {
                        lastFrameIncrement = 0.0;
                        currentFrame = 0;
                        velocity.X = 0.0f;
                        currentAnimation = "goombaDieLeft";
                        deathAnimationSet = true;
                    }
                }


                if (deathTimer > 500)
                {
                    removeFromGame = true;
                    deathAnimationSet = false;
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
                        currentAnimation = "goombaIdleRight";
                    }
                    else if (velocity.X < 0)
                    {
                        currentAnimation = "goombaIdleLeft";
                    }
                }
                else if (velocity.X > 0)
                {
                    currentAnimation = "goombaWalkRight";
                }
                else if (velocity.X < 0)
                {
                    currentAnimation = "goombaWalkLeft";
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
            AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position, AnimationFactory.DepthLayer1);
        }

        public void dieAnimation()
        {
            //AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position);
            return;
        }
    }
}
