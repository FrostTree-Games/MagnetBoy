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
        private enum CheckPointState
        {
            Untouched,
            Touched
        }

        protected string currentAnimation = null;
        protected int currentFrame = 0;
        protected double lastFrameIncrement = 0;

        private CheckPointState state;

        public HealthItem(float initialx, float initialy)
        {
            creation();

            width = 31.5f;
            height = 31.5f;

            horizontal_pos = initialx;
            vertical_pos = initialy;
            currentFrame = 0;
            lastFrameIncrement = 0;
            currentAnimation = "heartAnimation";
            removeFromGame = false;

            state = CheckPointState.Untouched;
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
                    if (state == CheckPointState.Untouched)
                    {
                        LevelState.currentPlayerHealth = LevelState.maxPlayerHealth;

                        for (int i = 0; i < 12; i++)
                        {
                            LevelState.levelParticlePool.pushParticle(ParticlePool.ParticleType.ColouredSpark, CenterPosition, Vector2.Zero, (float)(i * Math.PI / 6.0), 0.0f, Color.Red);
                        }

                        AudioFactory.playSFX("sfx/getHealth");

                        LevelState.checkPointTouched = true;
                        LevelState.respawnPosition = Position;

                        state = CheckPointState.Touched;
                        currentAnimation = "heartEmpty";
                    }
                }
            }

            if (state == CheckPointState.Touched)
            {
                if (LevelState.respawnPosition != Position)
                {
                    currentAnimation = "heartAnimation";
                    currentFrame = 0;
                    lastFrameIncrement = 0;

                    state = CheckPointState.Untouched;
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