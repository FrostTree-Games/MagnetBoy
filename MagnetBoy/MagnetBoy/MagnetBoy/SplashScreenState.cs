using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class SplashScreenState : IState
    {
        private enum SplashScreenVisualState
        {
            Loading = 0,
            FrostTreeLogo = 1,
            SaveNote = 2
        }

        private enum FadeState
        {
            FadeIn = 0,
            Stay = 1,
            FadeOut = 2
        }

        private ContentManager contentManager = null;
        private GameInput gameInput;

        private bool calledLoadAssets;

        private bool flashOffText;

        private SplashScreenVisualState state;
        private FadeState fadeState;
        private double fadeTimer = 0;
        private const double fadeDuration = 1000;

        private string autoSaveText1 = "This game features autosaving.";
        private string autoSaveText2 = "Do not turn off the power or";
        private string autoSaveText3 = "remove any memory devices";
        private string autoSaveText4 = "while you see this animation.";

        private bool startButtonDown;

        public SplashScreenState(ContentManager newManager)
        {
            contentManager = newManager;

            IsUpdateable = true;

            calledLoadAssets = false;
            flashOffText = false;

            startButtonDown = false;

            state = SplashScreenVisualState.Loading;
            fadeState = FadeState.FadeIn;
            fadeTimer = 0;

            gameInput = new GameInput(Game1.graphics.GraphicsDevice);
        }

        private Color FadeAmount
        {
            get
            {
                return (fadeState == FadeState.Stay) ? Color.White : (fadeState == FadeState.FadeOut) ? Color.Lerp(Color.White, new Color(0, 0, 0, 0), (float)(fadeTimer / fadeDuration)) : Color.Lerp(Color.White, new Color(0, 0, 0, 0), 1.0f - (float)(fadeTimer / fadeDuration));
            }
        }

        protected override void doUpdate(GameTime currentTime)
        {
            gameInput.update();

            if (state == SplashScreenVisualState.Loading)
            {
                if (!Game1.AssetsLoaded)
                {
                    if (!calledLoadAssets)
                    {
                        new Thread(Game1.loadGameAssets).Start();

                        calledLoadAssets = true;
                    }

                    flashOffText = currentTime.TotalGameTime.Milliseconds % 500 < 250;
                }
                else
                {
                    //GameScreenManager.switchScreens(GameScreenManager.GameScreenType.Menu, "TitleScreenMenu");
                    state = SplashScreenVisualState.FrostTreeLogo;
                }
            }
            else if (state == SplashScreenVisualState.FrostTreeLogo)
            {
                if (GameInput.isButtonDown(GameInput.PlayerButton.StartButton) && !startButtonDown)
                {
                    startButtonDown = true;
                }
                else if (startButtonDown && !GameInput.isButtonDown(GameInput.PlayerButton.StartButton))
                {
                    startButtonDown = false;

                    GameScreenManager.switchScreens(GameScreenManager.GameScreenType.Menu, "TitleScreenMenu");
                }

                fadeTimer += currentTime.ElapsedGameTime.Milliseconds;

                if (fadeTimer > fadeDuration)
                {
                    fadeState = (FadeState)((((int)fadeState) + 1) % 3);
                    fadeTimer = 0;

                    if (fadeState == FadeState.FadeIn)
                    {
                        state = SplashScreenVisualState.SaveNote;
                    }
                }
            }
            else if (state == SplashScreenVisualState.SaveNote)
            {
                if (GameInput.isButtonDown(GameInput.PlayerButton.StartButton) && !startButtonDown)
                {
                    startButtonDown = true;
                }
                else if (startButtonDown && !GameInput.isButtonDown(GameInput.PlayerButton.StartButton))
                {
                    startButtonDown = false;

                    GameScreenManager.switchScreens(GameScreenManager.GameScreenType.Menu, "TitleScreenMenu");
                }

                fadeTimer += currentTime.ElapsedGameTime.Milliseconds;

                if (fadeTimer > fadeDuration + (fadeState == FadeState.Stay ? 750 : 0))
                {
                    fadeState = (FadeState)((((int)fadeState) + 1) % 3);
                    fadeTimer = 0;

                    if (fadeState == FadeState.FadeIn)
                    {
                        GameScreenManager.switchScreens(GameScreenManager.GameScreenType.Menu, "TitleScreenMenu");
                    }
                }
            }
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            Game1.graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            if (state == SplashScreenVisualState.Loading)
            {
                if (flashOffText)
                {
                    spriteBatch.DrawString(Game1.gameFontText, "Loading...", new Vector2(400, 380), Color.White);
                }
            }
            else if (state == SplashScreenVisualState.FrostTreeLogo)
            {
                spriteBatch.Draw(Game1.globalCompanyLogo, new Vector2(720/2 - Game1.globalCompanyLogo.Bounds.Width/2, 480/2 - Game1.globalCompanyLogo.Bounds.Height/2), FadeAmount);
            }
            else if (state == SplashScreenVisualState.SaveNote)
            {
                spriteBatch.Draw(Game1.globalTestWalrus, new Vector2(360, 240), null, FadeAmount, (float)Math.Sin(Game1.onScreenSaveSpin / 200), new Vector2(16, 16), 1.0f, SpriteEffects.None, 0.5f);

                spriteBatch.DrawString(Game1.gameFontText, autoSaveText1, new Vector2(360 - (Game1.gameFontText.MeasureString(autoSaveText1).X/2), 175), FadeAmount);
                spriteBatch.DrawString(Game1.gameFontText, autoSaveText2, new Vector2(360 - (Game1.gameFontText.MeasureString(autoSaveText2).X / 2), 275), FadeAmount);
                spriteBatch.DrawString(Game1.gameFontText, autoSaveText3, new Vector2(360 - (Game1.gameFontText.MeasureString(autoSaveText3).X / 2), 292), FadeAmount);
                spriteBatch.DrawString(Game1.gameFontText, autoSaveText4, new Vector2(360 - (Game1.gameFontText.MeasureString(autoSaveText4).X / 2), 309), FadeAmount);
            }
            spriteBatch.End();
        }
    }
}
