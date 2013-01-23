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
    class HealthItem : Entity
    {
        protected string currentAnimation = null;
        protected int currentFrame = 0;
        protected double lastFrameIncrement = 0;

        public HealthItem(float initialx, float initialy)
        {
            creation();

            width = 31.5f;
            height = 31.5f;

            horizontal_pos = initialx;
            vertical_pos = initialy;
            currentAnimation = "heartIdle";
            removeFromGame = false;
        }

        public override void update(GameTime currentTime)
        {
            // update the current frame if needed
            if (currentTime.TotalGameTime.TotalMilliseconds - lastFrameIncrement > AnimationFactory.getAnimationSpeed(currentAnimation))
            {
                lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;

                currentFrame = (currentFrame + 1) % AnimationFactory.getAnimationFrameCount(currentAnimation);
            }

            foreach (Entity en in Entity.globalEntityList)
            {
                if (en is Player && hitTest(en))
                {
                    removeFromGame = true;
                    if (LevelState.currentPlayerHealth < 7 && LevelState.currentPlayerHealth > 0)
                    {
                        LevelState.currentPlayerHealth = LevelState.currentPlayerHealth + 1;

                        if (LevelState.currentPlayerHealth > LevelState.maxPlayerHealth)
                        {
                            LevelState.currentPlayerHealth = LevelState.maxPlayerHealth;
                        }
                    }
                }
            }
        }

        public virtual void enemyUpdate(GameTime currentTime)
        {
            return;
        }

        public override void draw(SpriteBatch sb)
        {
            AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position, AnimationFactory.DepthLayer2);
        }
    }
}