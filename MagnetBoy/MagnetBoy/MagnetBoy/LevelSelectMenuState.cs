using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class LevelSelectMenuState : IState
    {
        private class LevelSelectMenuOption
        {
            public bool selected;
            public double distanceOut;

            public const double minDistanceOut = 0.0;
            public const double maxDistanceOut = 1.0;
            public const double moveSpeed = 0.01;

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
        private bool upPressed = false;
        private bool confirmPressed = false;
        private bool cancelPressed = false;

        private int selectedMenuItem;
        private List<LevelSelectMenuOption> menuList = null;

        public LevelSelectMenuState(ContentManager newManager)
        {
            IsUpdateable = true;

            contentManager = newManager;
            gameInput = new GameInput(Game1.graphics.GraphicsDevice);

            selectedMenuItem = 0;
            menuList = new List<LevelSelectMenuOption>(5);
            for (int i = 0; i < 5; i++)
            {
                menuList.Add(new LevelSelectMenuOption());
            }
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
                GameScreenManager.switchScreens(GameScreenManager.GameScreenType.Menu, "TitleScreenMenu_fromLevelSelect");
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

            if (GameInput.isButtonDown(GameInput.PlayerButton.Confirm))
            {
                confirmPressed = true;
            }
            else if (confirmPressed == true)
            {
                confirmPressed = false;

                Game1.CurrentLevel = selectedMenuItem;
                GameScreenManager.switchScreens(GameScreenManager.GameScreenType.Level, Game1.levelFileNames[selectedMenuItem]);
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
                MBQG.drawGUIBox(spriteBatch, new Vector2((float)(144 + (25 * menuList[i].distanceOut)), 81 + i * 64), 28, 3, Color.Purple, AnimationFactory.DepthLayer3);
                spriteBatch.DrawString(Game1.gameFontText, Game1.levelNames[i], new Vector2((float)(152 + (25 * menuList[i].distanceOut)), 85 + i * 64), Color.Lerp(Color.Black, Color.White, (float)menuList[i].distanceOut));
                spriteBatch.DrawString(Game1.gameFontText, "Completed in " + Game1.MagnetBoySaveData[i].levelBestTime + " by " + Game1.MagnetBoySaveData[i].levelBestTimeOwner, new Vector2((float)(168 + (25 * menuList[i].distanceOut)), 106 + i * 64), Color.Lerp(new Color(40, 40, 40), Color.DarkGray, (float)menuList[i].distanceOut));
            }

            AnimationFactory.drawAnimationFrame(spriteBatch, "xboxButtons", 0, new Vector2(220, 400), AnimationFactory.DepthLayer0);
            spriteBatch.DrawString(Game1.gameFontText, "Begin Level", new Vector2(245, 401), Color.Black);

            AnimationFactory.drawAnimationFrame(spriteBatch, "xboxButtons", 3, new Vector2(425, 400), AnimationFactory.DepthLayer0);
            spriteBatch.DrawString(Game1.gameFontText, "Back", new Vector2(450, 401), Color.Black);
            spriteBatch.End();
        }
    }
}