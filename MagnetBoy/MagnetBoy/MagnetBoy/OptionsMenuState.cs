using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class OptionsMenuState : IState
    {
        private class OptionsMenuOption
        {
            public string title;
            public string description;

            public bool selected;
            public double distanceOut;

            private const double minDistanceOut = 0.0;
            private const double maxDistanceOut = 1.0;
            private const double moveSpeed = 0.01;

            public OptionsMenuOption(string newTitle)
            {
                title = newTitle;
                selected = false;
                distanceOut = 0;

                switch (title)
                {
                    case "Starting Health":
                        description = "Change the health you start with on each level.";
                        break;
                    case "Erase Data":
                        description = "Confirm this to erase all previous save data.";
                        break;
                    case "Show Timer":
                        description = "Show a timer onscreen to see how long you've been playing the level for.";
                        break;
                    case "Show Record":
                        description = "Show a fixed timer onscreen to see the top record for completing a level.";
                        break;
                    default:
                        description = "...";
                        break;
                }
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

        private bool downPressed = false;
        private bool upPressed = false; /*
        private bool leftPressed = false;
        private bool rightPressed = false;
        private bool confirmPressed = false; */
        private bool cancelPressed = false;

        private int selectedMenuItem;
        private List<OptionsMenuOption> menuList;

        public OptionsMenuState(ContentManager newManager)
        {
            IsUpdateable = true;

            contentManager = newManager;
            gameInput = new GameInput(Game1.graphics.GraphicsDevice);

            selectedMenuItem = 0;
            menuList = new List<OptionsMenuOption>(10);

            menuList.Add(new OptionsMenuOption("Starting Health"));
            menuList.Add(new OptionsMenuOption("Erase Data"));
            menuList.Add(new OptionsMenuOption("Show Timer"));
        }

        protected override void doUpdate(GameTime currentTime)
        {
            gameInput.update();

            if (GameInput.isButtonDown(GameInput.PlayerButton.Cancel))
            {
                cancelPressed = true;
            }
            else if (cancelPressed == true)
            {
                GameScreenManager.switchScreens(GameScreenManager.GameScreenType.Menu, "TitleScreenMenu_fromOptions");
                cancelPressed = false;

                AudioFactory.playSFX("sfx/menu");
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
                    TitleScreenMenuState.checkerBoardSlide.X += GameInput.P1MouseDirectionNormal.X;
                }

                if (GameInput.P1MouseDirectionNormal.Y != float.NaN)
                {
                    TitleScreenMenuState.checkerBoardSlide.Y += GameInput.P1MouseDirectionNormal.Y;
                }
            }

            Game1.grayCheckerBoard.Parameters["slideX"].SetValue(TitleScreenMenuState.checkerBoardSlide.X);
            Game1.grayCheckerBoard.Parameters["slideY"].SetValue(TitleScreenMenuState.checkerBoardSlide.Y);
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
                MBQG.drawGUIBox(spriteBatch, new Vector2((float)(144 + (25 * menuList[i].distanceOut)), 100 + i * 64), 7, 3, Color.Purple, AnimationFactory.DepthLayer3);
                spriteBatch.DrawString(Game1.gameFontText, menuList[i].title, new Vector2((float)(156 + (25 * menuList[i].distanceOut)), 105 + (64 * i)), Color.Lerp(Color.Black, Color.White, (float)menuList[i].distanceOut));

                AnimationFactory.drawAnimationFrame(spriteBatch, "xboxButtons", 3, new Vector2(425, 400), AnimationFactory.DepthLayer0);
                spriteBatch.DrawString(Game1.gameFontText, "Back", new Vector2(450, 401), Color.Black);
            }
            spriteBatch.End();
        }
    }
}
