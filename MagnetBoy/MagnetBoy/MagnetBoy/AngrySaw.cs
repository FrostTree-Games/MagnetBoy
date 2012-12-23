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
        }

        protected override void enemyUpdate(GameTime currentTime)
        {
            if (Math.Abs(velocity.X) < 0.001)
            {
                if (velocity.X >= 0)
                {
                    currentAnimation = "angrySawIdleRight";
                }
                else
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
    }
}
