using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class EndLevelFlag : Entity
    {
        private enum EndLevelFlagState
        {
            Untouched,
            Spinning,
            Touched,
            EndGame
        }

        private EndLevelFlagState state;
        private double stateTimer;

        private const double spinningDuration = 500;
        private const double waitDuration = 750;

        private string currentAnimation = "endLevelFlag";
        private int currentFrame;
        private double lastFrameIncrement;

        public EndLevelFlag(float initialx, float initialy)
        {
            creation();

            horizontal_pos = initialx;
            vertical_pos = initialy;

            width = 32f;
            height = 32f;

            state = EndLevelFlagState.Untouched;
            currentFrame = 0;
            lastFrameIncrement = 0;

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
        }

        public override void update(GameTime currentTime)
        {
            if (state == EndLevelFlagState.Untouched)
            {
                foreach (Entity en in Entity.globalEntityList)
                {
                    if (en is Player)
                    {
                        if (hitTest(en))
                        {
#if XBOX
                            if ((uint)(LevelState.LevelRecordTime / 1000) < Game1.MagnetBoySaveData[Game1.CurrentLevel].levelBestTime)
                            {
                                Game1.LevelScoreStruct newHighScore;
                                newHighScore.levelBestTime = (uint)(LevelState.LevelRecordTime / 1000);
                                newHighScore.levelBestTimeOwner = GameInput.LockedPlayerGamerTag;

                                Game1.MagnetBoySaveData[Game1.CurrentLevel] = newHighScore;
                            }
#endif

                            state = EndLevelFlagState.Spinning;
                            stateTimer = 0;
                            break;
                        }
                    }
                }
            }
            else if (state == EndLevelFlagState.Spinning)
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

                stateTimer += currentTime.ElapsedGameTime.Milliseconds;

                if (stateTimer > spinningDuration)
                {
                    stateTimer = 0;
                    state = EndLevelFlagState.Touched;
                    currentFrame = AnimationFactory.getAnimationFrameCount(currentAnimation) - 1;
                }
            }
            else if (state == EndLevelFlagState.Touched)
            {
                stateTimer += currentTime.ElapsedGameTime.Milliseconds;

                if (stateTimer > waitDuration)
                {
                    state = EndLevelFlagState.EndGame;
                }
            }
            else if (state == EndLevelFlagState.EndGame)
            {
                LevelState.EndLevelFlag = true;
            }
        }

        public override void draw(SpriteBatch sb)
        {
            AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position + new Vector2(-8, -16), AnimationFactory.DepthLayer2);
        }
    }
}
