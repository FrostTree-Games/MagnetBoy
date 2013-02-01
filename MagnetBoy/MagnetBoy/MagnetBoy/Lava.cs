using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class Lava : Entity
    {
        protected string currentAnimation = null;
        protected int currentFrame = 0;
        protected double lastFrameIncrement = 0;

        public Lava(float initialx, float initialy)
        {
            creation();

            horizontal_pos = initialx;
            vertical_pos = initialy;

            width = 32f;
            height = 32f;

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;

            currentAnimation = "lavaBottom";
        }

        public override void update(GameTime currentTime)
        {
            foreach (Entity en in Entity.globalEntityList)
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
                if (en is Enemy)
                {
                    if (hitTest(en))
                    {
                        en.deathAnimation = true;
                    }
                }
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
            AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position, AnimationFactory.DepthLayer0);
        }
    }
}
