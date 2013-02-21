using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
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

        private const double spinningDuration = 1400;
        private const double waitDuration = 8000;

        private string currentAnimation = "flagIdle";
        private int currentFrame;
        private double lastFrameIncrement;

        public EndLevelFlag(float initialx, float initialy)
        {
            creation();

            horizontal_pos = initialx;
            vertical_pos = initialy;

            width = 128f;
            height = 128f;

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
                            if ((uint)(LevelState.LevelRecordTime) < Game1.MagnetBoySaveData[Game1.CurrentLevel].levelBestTime)
                            {
                                Game1.LevelScoreStruct newHighScore;
                                newHighScore.levelBestTime = LevelState.LevelRecordTime;
#if WINDOWS
                                newHighScore.levelBestTimeOwner = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
#endif
#if XBOX
                                newHighScore.levelBestTimeOwner = GameInput.LockedPlayerGamerTag;
#endif

                                Game1.MagnetBoySaveData[Game1.CurrentLevel] = newHighScore;
                            }

                            
#if XBOX
                            SaveGameModule.saveGame();
#endif
                            AudioFactory.playSFX("sfx/fanfare");
                            AudioFactory.stopSong();

                            LevelState.showLevelCompleteText = true;

                            state = EndLevelFlagState.Spinning;
                            currentAnimation = "flagDie";
                            currentFrame = 0;
                            lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;
                            stateTimer = 0;
                            break;
                        }
                    }
                }
            }
            else if (state == EndLevelFlagState.Spinning)
            {
                stateTimer += currentTime.ElapsedGameTime.Milliseconds;

                if (currentFrame == AnimationFactory.getAnimationFrameCount(currentAnimation) - 1)
                {
                    currentAnimation = "flagMagnetIdle";
                    currentFrame = 0;
                    lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;
                    stateTimer = 0;
                    state = EndLevelFlagState.Touched;
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
                //LevelState.EndLevelFlag = true;
                LevelState.fadingOut = true;
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
            AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position, AnimationFactory.DepthLayer2);
        }
    }
}
