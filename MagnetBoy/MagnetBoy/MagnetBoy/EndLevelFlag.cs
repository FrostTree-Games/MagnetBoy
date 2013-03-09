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
        private const double waitDuration = 5000;

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
                            if ((uint)(LevelState.LevelRecordTime) < Game1.MagnetBoySaveData.levelBestTime(Game1.CurrentLevel))
                            {
                                
#if WINDOWS
                                Game1.MagnetBoySaveData.setLevelRecord(Game1.CurrentLevel, LevelState.LevelRecordTime, System.Security.Principal.WindowsIdentity.GetCurrent().Name);
#endif
#if XBOX
                                Game1.MagnetBoySaveData.setLevelRecord(Game1.CurrentLevel, LevelState.LevelRecordTime, GameInput.LockedPlayerGamerTag);
#endif
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

                if (stateTimer % 1000 < 12)
                {
                    Color clr = Color.HotPink;

                    switch (Game1.gameRandom.Next() % 6)
                    {
                        case 0:
                            clr = Color.Violet;
                            break;
                        case 1:
                            clr = Color.Crimson;
                            break;
                        case 2:
                            clr = Color.LimeGreen;
                            break;
                        case 3:
                            clr = Color.Linen;
                            break;
                        case 4:
                            clr = Color.CornflowerBlue;
                            break;
                        default:
                            break;
                    }

                    Vector2 randPos = new Vector2(width / 2 + (float)((Game1.gameRandom.NextDouble() * width) - (width / 2)), height / -2 + (float)((Game1.gameRandom.NextDouble() * width) - (width / 2)));

                    for (int i = 0; i < 12; i++)
                    {
                        LevelState.levelParticlePool.pushParticle(ParticlePool.ParticleType.ColouredSpark, Position + randPos, Vector2.Zero, (float)(i * Math.PI / 6.0), 0.0f, clr);
                    }

                    switch (Game1.gameRandom.Next() % 3)
                    {
                        case 0:
                            AudioFactory.playSFX("sfx/firework");
                            break;
                        case 1:
                            AudioFactory.playSFX("sfx/firework2");
                            break;
                        default:
                            AudioFactory.playSFX("sfx/firework3");
                            break;
                    }
                    
                }

                if (stateTimer > waitDuration)
                {
                    state = EndLevelFlagState.EndGame;
                    stateTimer = 0;
                }
            }
            else if (state == EndLevelFlagState.EndGame)
            {
                //LevelState.EndLevelFlag = true;
                LevelState.fadingOut = true;

                stateTimer += currentTime.ElapsedGameTime.Milliseconds;
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
