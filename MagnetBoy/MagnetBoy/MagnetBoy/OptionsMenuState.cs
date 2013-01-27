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
                        description = "Change the health you\nstart with on each level.";
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
                    case "Mute Music":
                        description = "Turn the music off if\nyou don't like it.";
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
        private bool upPressed = false; 
        private bool leftPressed = false;
        private bool rightPressed = false; /*
        private bool confirmPressed = false; */
        private bool cancelPressed = false;

        private Vector2 optionDescriptionText = new Vector2(400, 120);

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
            menuList.Add(new OptionsMenuOption("Mute Music"));
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

            if (GameInput.isButtonDown(GameInput.PlayerButton.RightDirection))
            {
                rightPressed = true;
            }
            else if (rightPressed == true)
            {
                switch (menuList[selectedMenuItem].title)
                {
                    case "Starting Health":
                        Game1.MagnetBoySaveData.defaultStartingHealth = Game1.MagnetBoySaveData.defaultStartingHealth + 1;
                        if (Game1.MagnetBoySaveData.defaultStartingHealth > LevelState.maxPlayerHealth)
                        {
                            Game1.MagnetBoySaveData.defaultStartingHealth = LevelState.maxPlayerHealth;
                        }
                        break;
                    case "Mute Music":
                        AudioFactory.Mute = !AudioFactory.Mute;
                        break;
                    default:
                        break;
                }

                rightPressed = false;

                AudioFactory.playSFX("sfx/menu");
            }

            if (GameInput.isButtonDown(GameInput.PlayerButton.LeftDirection))
            {
                leftPressed = true;
            }
            else if (leftPressed == true)
            {
                switch (menuList[selectedMenuItem].title)
                {
                    case "Starting Health":
                        Game1.MagnetBoySaveData.defaultStartingHealth = Game1.MagnetBoySaveData.defaultStartingHealth - 1;
                        if (Game1.MagnetBoySaveData.defaultStartingHealth < 1)
                        {
                            Game1.MagnetBoySaveData.defaultStartingHealth = 1;
                        }
                        break;
                    case "Mute Music":
                        AudioFactory.Mute = !AudioFactory.Mute;
                        break;
                    default:
                        break;
                }

                leftPressed = false;

                AudioFactory.playSFX("sfx/menu");
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

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            for (int i = 0; i < menuList.Count; i++)
            {
                MBQG.drawGUIBox(spriteBatch, new Vector2((float)(144 + (25 * menuList[i].distanceOut)), 100 + i * 64), 10, 3, Color.Purple, AnimationFactory.DepthLayer3);
                spriteBatch.DrawString(Game1.gameFontText, menuList[i].title, new Vector2((float)(156 + (25 * menuList[i].distanceOut)), 112 + (64 * i)), Color.Lerp(Color.Black, Color.White, (float)menuList[i].distanceOut));
            }

            AnimationFactory.drawAnimationFrame(spriteBatch, "xboxDPad", 0, new Vector2(210, 395), AnimationFactory.DepthLayer0);
            spriteBatch.DrawString(Game1.gameFontText, "Change Option", new Vector2(245, 401), Color.Black);

            AnimationFactory.drawAnimationFrame(spriteBatch, "xboxButtons", 3, new Vector2(425, 400), AnimationFactory.DepthLayer0);
            spriteBatch.DrawString(Game1.gameFontText, "Back", new Vector2(450, 401), Color.Black);

            spriteBatch.DrawString(Game1.gameFontText, menuList[selectedMenuItem].description, optionDescriptionText, Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, AnimationFactory.DepthLayer0);
            MBQG.drawGUIBox(spriteBatch, optionDescriptionText - new Vector2(14, 8), 16, 4, Color.Purple, AnimationFactory.DepthLayer1);

            switch (menuList[selectedMenuItem].title)
            {
                case "Starting Health":
                    for (int i = 0; i < LevelState.maxPlayerHealth; i++)
                    {
                        if (i < Game1.MagnetBoySaveData.defaultStartingHealth)
                        {
                            AnimationFactory.drawAnimationFrame(spriteBatch, "heartIdle", 0, new Vector2(432 + (i * 32), 193), new Vector2(0.8f, 0.8f), Color.White, AnimationFactory.DepthLayer0);
                        }
                        else
                        {
                            AnimationFactory.drawAnimationFrame(spriteBatch, "heartEmpty", 0, new Vector2(432 + (i * 32), 193), new Vector2(0.8f, 0.8f), Color.White, AnimationFactory.DepthLayer0);
                        }
                    }
                    break;
                case "Mute Music":
                    MBQG.drawGUIBox(spriteBatch, optionDescriptionText + new Vector2(14, 8) + new Vector2(16, 100), 4, 2, Color.Purple, AnimationFactory.DepthLayer3);
                    MBQG.drawGUIBox(spriteBatch, optionDescriptionText + new Vector2(14, 8) + new Vector2(132, 100), 4, 2, Color.Purple, AnimationFactory.DepthLayer3);
                    spriteBatch.DrawString(Game1.gameFontText, "ON", optionDescriptionText + new Vector2(32, 12) + new Vector2(16, 100), AudioFactory.Mute ? Color.Black : Color.White);
                    spriteBatch.DrawString(Game1.gameFontText, "OFF", optionDescriptionText + new Vector2(28, 12) + new Vector2(132, 100), AudioFactory.Mute ? Color.White : Color.Black);
                    break;
                default:
                    break;
            }
            spriteBatch.End();
        }
    }
}
