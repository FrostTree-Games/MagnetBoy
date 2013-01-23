using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class TitleScreenMenuState : IState
    {
        private class TitleMenuOption
        {
            public string text;

            public bool selected;
            public double distanceOut;

            private const double minDistanceOut = 0.0;
            private const double maxDistanceOut = 1.0;
            private const double moveSpeed = 0.01;

            public TitleMenuOption(string newText)
            {
                text = newText;
                selected = false;
                distanceOut = minDistanceOut;
            }

            public void update(GameTime currentTime)
            {
                if (selected)
                {
                    if (distanceOut < maxDistanceOut)
                    {
                        distanceOut += moveSpeed * currentTime.ElapsedGameTime.Milliseconds;

                        if (distanceOut > maxDistanceOut)
                        {
                            distanceOut = maxDistanceOut;
                        }
                    }
                }
                else
                {
                    if (distanceOut > minDistanceOut)
                    {
                        distanceOut -= moveSpeed * currentTime.ElapsedGameTime.Milliseconds;

                        if (distanceOut < minDistanceOut)
                        {
                            distanceOut = minDistanceOut;
                        }
                    }
                }
            }
        }

        private ContentManager contentManager = null;
        private GameInput gameInput;

        private bool musicPlaying;

        private bool downPressed = false;
        private bool upPressed = false;
        private bool leftPressed = false;
        private bool rightPressed = false;
        private bool confirmPressed = false;

        private List<TitleMenuOption> menuList;
        int selectedMenuItem;

        private Vector2 checkerBoardSlide;

        public TitleScreenMenuState(ContentManager newManager)
        {
            IsUpdateable = true;

            contentManager = newManager;

            gameInput = new GameInput(Game1.graphics.GraphicsDevice);

            musicPlaying = false;

            menuList = new List<TitleMenuOption>(5);
            menuList.Add(new TitleMenuOption("BEGIN"));
            menuList.Add(new TitleMenuOption("CONTINUE"));
            menuList.Add(new TitleMenuOption("OPTION"));
            menuList.Add(new TitleMenuOption("EXIT"));

            checkerBoardSlide = new Vector2(-720/2, -480/2);

            selectedMenuItem = 0;
        }

        protected override void doUpdate(GameTime currentTime)
        {
            gameInput.update();

            if (!musicPlaying)
            {
                musicPlaying = true;

                AudioFactory.playSong("songs/introTheme");
            }

            if (GameInput.isButtonDown(GameInput.PlayerButton.DownDirection))
            {
                downPressed = true;
            }
            else if (downPressed == true)
            {
                selectedMenuItem++;
                downPressed = false;

                AudioFactory.playSFX("sfx/menu");
            }

            if (GameInput.isButtonDown(GameInput.PlayerButton.UpDirection))
            {
                upPressed = true;
            }
            else if (upPressed == true)
            {
                selectedMenuItem--;
                upPressed = false;

                AudioFactory.playSFX("sfx/menu");
            }

            if (selectedMenuItem >= menuList.Count)
            {
                selectedMenuItem = selectedMenuItem % menuList.Count;
            }
            else if (selectedMenuItem < 0)
            {
                selectedMenuItem += menuList.Count;
            }

            if (GameInput.isButtonDown(GameInput.PlayerButton.Confirm))
            {
                confirmPressed = true;
            }
            else if (confirmPressed == true)
            {
                confirmPressed = false;

                switch (menuList[selectedMenuItem].text)
                {
                    case "CONTINUE":
                        AudioFactory.stopSong();
                        AudioFactory.playSFX("sfx/menu");
                        GameScreenManager.switchScreens(GameScreenManager.GameScreenType.Menu, "BetaMenu");
                        break;
                    case "EXIT":
                        Game1.ExitGame = true;
                        break;
                    default:
                        break;
                }
            }

            for (int i = 0; i < menuList.Count; i++)
            {
                if (i == selectedMenuItem)
                {
                    menuList[i].selected = true;
                }
                else
                {
                    menuList[i].selected = false;
                }

                menuList[i].update(currentTime);
            }

            if (GameInput.isButtonDown(GameInput.PlayerButton.Push))
            {
                if (GameInput.P1MouseDirectionNormal.X != float.NaN)
                {
                    checkerBoardSlide.X += GameInput.P1MouseDirectionNormal.X;
                }

                if (GameInput.P1MouseDirectionNormal.Y != float.NaN)
                {
                    checkerBoardSlide.Y += GameInput.P1MouseDirectionNormal.Y;
                }
            }

            Game1.grayCheckerBoard.Parameters["slideX"].SetValue(checkerBoardSlide.X);
            Game1.grayCheckerBoard.Parameters["slideY"].SetValue(checkerBoardSlide.Y);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            Game1.graphics.GraphicsDevice.Clear(Color.Lerp(Color.DarkGray, Color.White, 0.4f));

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, Game1.grayCheckerBoard, Matrix.CreateScale(720, 480, 0));
            spriteBatch.Draw(Game1.globalBlackPixel, Vector2.Zero, Color.White);
            spriteBatch.End();

            spriteBatch.Begin();
            for (int i = 0; i < menuList.Count; i++)
            {
                spriteBatch.DrawString(Game1.gameFontText, menuList[i].text, new Vector2((float)(300 + (25 * menuList[i].distanceOut)), 300 + (32 * i)), Color.Lerp(Color.Black, Color.White, (float)menuList[i].distanceOut));
            }
            spriteBatch.End();
        }
    }
}
