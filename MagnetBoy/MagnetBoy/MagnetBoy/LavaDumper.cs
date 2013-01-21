using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class LavaDumper : Entity
    {
        protected string currentAnimation = "lavaDumper";
        protected int currentFrame = 0;
        protected double lastFrameIncrement = 0;

        private double interval = 2000; //milliseconds
        private double timeSinceLastInterval = 0;

        public LavaDumper(float initalX, float initalY)
        {
            horizontal_pos = initalX;
            vertical_pos = initalY;
        }

        public override void update(GameTime currentTime)
        {
            timeSinceLastInterval += currentTime.ElapsedGameTime.Milliseconds;

            if (timeSinceLastInterval > interval)
            {
                BulletPool.pushBullet(BulletPool.BulletType.LavaBlob, horizontal_pos, vertical_pos, currentTime, 0.0f);

                timeSinceLastInterval = 0;
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
            Vector2 newPos = Position;
            newPos.Y -= 3;

            AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, newPos, AnimationFactory.DepthLayer2);
        }
    }
}
