using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MagnetBoy
{
    public class ConveyerBelt : Entity
    {
        public enum ConveyerSpot
        {
            Left,
            Mid,
            Right
        }

        string currentAnimation = null;
        int currentFrame = 0;
        double lastFrameIncrement = 0;

        public ConveyerBelt(float initalX, float initalY, ConveyerSpot c)
        {
            creation();

            horizontal_pos = initalX;
            vertical_pos = initalY;

            width = 33f;
            height = 31.5f;

            solid = true;

            switch (c)
            {
                case ConveyerSpot.Left:
                    currentAnimation = "conveyerLeft";
                    break;
                case ConveyerSpot.Mid:
                    currentAnimation = "conveyerMid";
                    break;
                case ConveyerSpot.Right:
                    currentAnimation = "conveyerRight";
                    break;
                default:
                    break;
            }
            
        }

        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position);
        }
        
        public override void update(Microsoft.Xna.Framework.GameTime currentTime)
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

            foreach (Entity en in globalEntityList)
            {
                if (Math.Abs(vertical_pos - (en.Position.Y + en.HitBox.Y)) < 1.0f && (en.Position.X + en.HitBox.X > horizontal_pos && en.Position.X < horizontal_pos + width))
                {
                    en.convey(new Vector2(0.5f, 0.0f));
                }
            }
        }
    }
}
