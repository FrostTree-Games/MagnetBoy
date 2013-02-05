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
        private bool confirmPressed = false;
        private bool backPressed = false;
        private bool anyButtonPressed = false;
        private bool xButtonPressed = false;

        public bool showPressButtonDialog;
        private double dialogTimer;

        private double timePassed;

        private bool fadingOut;
        private double fadeTimeElapsed;
        private const double fadeTimeMax = 1000;

        private List<TitleMenuOption> menuList;
        int selectedMenuItem;

        public static Vector2 checkerBoardSlide = new Vector2(720 * 100, 480 * 100);

        private int magnetWopleyFrame;
        private double magnetWopleyLastUpdateTime;
        private string magnetWopleyAnim = "playerWalkRight";
        private Vector2 magnetWopleyPosition = new Vector2(350, 244);

        private int enemyChaseFrame;
        private double enemyFrameLastUpdateTime;
        private string enemyFrameAnim = "angrySawWalkRight";
        private Vector2 enemyPosition = new Vector2(250, 244);

        private const int numberOfGrassTypes = 12;
        private int currentSpotInTiles = 0;
        private int[] grassTiles = { 0, 1, 2, 11, 0, 6, 7, 2, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private const double grassPixelsPerSecond = 10;
        private double grassPixelsElapsedTime = 0;
        private double grassPixelOffset = 0;

        public TitleScreenMenuState(ContentManager newManager, bool musicAlreadyPlaying, bool fade)
        {
            IsUpdateable = true;

            contentManager = newManager;

            gameInput = new GameInput(Game1.graphics.GraphicsDevice);

            musicPlaying = musicAlreadyPlaying;

            menuList = new List<TitleMenuOption>(5);
            menuList.Add(new TitleMenuOption("BEGIN"));
            menuList.Add(new TitleMenuOption("CONTINUE"));
            menuList.Add(new TitleMenuOption("OPTION"));
            menuList.Add(new TitleMenuOption("EXIT"));

            selectedMenuItem = 0;

            timePassed = 0;
            fadingOut = false;

            magnetWopleyAnim = "playerWalkRight";
            magnetWopleyFrame = 0;
            magnetWopleyLastUpdateTime = 0;

            switch (Game1.gameRandom.Next() % 3)
            {
                case 0:
                    enemyFrameAnim = "angrySawWalkLeft";
                    break;
                case 1:
                    enemyFrameAnim = "lolrusWalkRight";
                    break;
                case 2:
                    enemyFrameAnim = "goombaWalkRight";
                    break;
            }
            enemyChaseFrame = 0;
            enemyFrameLastUpdateTime = 0;

            for (int i = 0; i < grassTiles.Length; i++)
            {
                grassTiles[i] = Game1.gameRandom.Next() % numberOfGrassTypes;
            }

            if (musicAlreadyPlaying)
            {
                showPressButtonDialog = false;
            }
            else
            {
                showPressButtonDialog = true;
                GameInput.LockMostRecentPad = false;
            }

            if (!fade)
            {
                Game1.diamondWipe.Parameters["time"].SetValue(1.0f);
                timePassed = 1001;
            }
            else
            {
                Game1.diamondWipe.Parameters["time"].SetValue(0.0f);
            }

            Game1.CurrentLevel = 0;
        }

        protected override void doUpdate(GameTime currentTime)
        {
            gameInput.update();

            if (!musicPlaying)
            {
                musicPlaying = true;

                AudioFactory.playSong("songs/introTheme");
            }

            timePassed += currentTime.ElapsedGameTime.Milliseconds;

            if (fadingOut)
            {
                fadeTimeElapsed += currentTime.ElapsedGameTime.Milliseconds;
                Game1.diamondWipe.Parameters["time"].SetValue(1.0f - (float)(fadeTimeElapsed / fadeTimeMax));

                if (fadeTimeElapsed > fadeTimeMax)
                {
                    AudioFactory.stopSong();
                    GameScreenManager.switchScreens(GameScreenManager.GameScreenType.Level, Game1.levelFileNames[Game1.CurrentLevel]);
                }
            }

            if (!showPressButtonDialog)
            {
                if (GameInput.isButtonDown(GameInput.PlayerButton.XButton))
                {
                    xButtonPressed = true;
                }
                else if (xButtonPressed == true && GameInput.isButtonDown(GameInput.PlayerButton.LBumper))
                {
                    xButtonPressed = false;

                    if (Game1.MagnetBoySaveData.furthestLevelUnlocked < 4)
                    {
                        Game1.MagnetBoySaveData.furthestLevelUnlocked++;
                    }
                }

                if (GameInput.isButtonDown(GameInput.PlayerButton.DownDirection))
                {
                    downPressed = true;
                }
                else if (downPressed == true && !fadingOut)
                {
                    selectedMenuItem++;
                    downPressed = false;

                    AudioFactory.playSFX("sfx/menu");
                }

                if (GameInput.isButtonDown(GameInput.PlayerButton.UpDirection))
                {
                    upPressed = true;
                }
                else if (upPressed == true && !fadingOut)
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

                if (GameInput.isButtonDown(GameInput.PlayerButton.Cancel))
                {
                    backPressed = true;
                }
                else if (backPressed == true && !fadingOut)
                {
                    showPressButtonDialog = true;
                    GameInput.LockMostRecentPad = false;
                    dialogTimer = 0;

                    backPressed = false;

                    AudioFactory.playSFX("sfx/menuClose");
                }

                if (GameInput.isButtonDown(GameInput.PlayerButton.Confirm))
                {
                    confirmPressed = true;
                }
                else if (confirmPressed == true && !fadingOut)
                {
                    confirmPressed = false;

                    switch (menuList[selectedMenuItem].text)
                    {
                        case "BEGIN":
                            fadingOut = true;
                            fadeTimeElapsed = 0;
                            Game1.diamondWipe.Parameters["time"].SetValue(1.0f);
                            AudioFactory.playSFX("sfx/menu");
                            break;
                        case "CONTINUE":
                            AudioFactory.playSFX("sfx/menu");
                            GameScreenManager.switchScreens(GameScreenManager.GameScreenType.Menu, "LevelSelectMenu");
                            break;
                        case "OPTION":
                            AudioFactory.playSFX("sfx/menu");
                            GameScreenManager.switchScreens(GameScreenManager.GameScreenType.Menu, "GameOptionsMenu");
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
            }
            else
            {
                dialogTimer += currentTime.ElapsedGameTime.Milliseconds;

                if (GameInput.isButtonDown(GameInput.PlayerButton.AnyButton))
                {
                    anyButtonPressed = true;
                }
                else if (anyButtonPressed == true)
                {
                    showPressButtonDialog = false;

                    GameInput.LockMostRecentPad = true;
                    //apply any sign-in, storage device, or settings dialogs here

                    anyButtonPressed = false;

#if XBOX
                    SaveGameModule.selectStorageDevice();
                    SaveGameModule.loadGame();
#endif

                    AudioFactory.playSFX("sfx/menuOpen");
                }
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

            // if the last frame time hasn't been set, set it now
            if (magnetWopleyLastUpdateTime == 0)
            {
                magnetWopleyLastUpdateTime = currentTime.TotalGameTime.TotalMilliseconds;
            }

            // update the current frame if needed
            if (currentTime.TotalGameTime.TotalMilliseconds - magnetWopleyLastUpdateTime > AnimationFactory.getAnimationSpeed(magnetWopleyAnim))
            {
                magnetWopleyLastUpdateTime = currentTime.TotalGameTime.TotalMilliseconds;

                magnetWopleyFrame = (magnetWopleyFrame + 1) % AnimationFactory.getAnimationFrameCount(magnetWopleyAnim);
            }

            grassPixelsElapsedTime += currentTime.ElapsedGameTime.Milliseconds;
            if (grassPixelsElapsedTime > grassPixelsPerSecond)
            {
                grassPixelOffset += grassPixelsElapsedTime / grassPixelsPerSecond;
                grassPixelsElapsedTime = 0;
            }
            if (grassPixelOffset >= 16)
            {
                grassPixelOffset -= 16;
                currentSpotInTiles = (currentSpotInTiles + 1) % grassTiles.Length;

                int nextTile = currentSpotInTiles - 1;
                if (nextTile < 0)
                {
                    nextTile = grassTiles.Length - 1;
                }
                grassTiles[nextTile] = Game1.gameRandom.Next() % numberOfGrassTypes;
            }

            // if the last frame time hasn't been set, set it now
            if (enemyFrameLastUpdateTime == 0)
            {
                enemyFrameLastUpdateTime = currentTime.TotalGameTime.TotalMilliseconds;
            }

            // update the current frame if needed
            if (currentTime.TotalGameTime.TotalMilliseconds - enemyFrameLastUpdateTime > AnimationFactory.getAnimationSpeed(enemyFrameAnim))
            {
                enemyFrameLastUpdateTime = currentTime.TotalGameTime.TotalMilliseconds;

                enemyChaseFrame = (enemyChaseFrame + 1) % AnimationFactory.getAnimationFrameCount(enemyFrameAnim);
            }

            if (!fadingOut)
            {
                Game1.diamondWipe.Parameters["time"].SetValue(timePassed > 1000 ? 1.0f : (float)(timePassed / 1000));
            }
        } //doUpdate()

        public override void draw(SpriteBatch spriteBatch)
        {
            Game1.graphics.GraphicsDevice.Clear(Color.Lerp(Color.DarkGray, Color.White, 0.4f));

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, Game1.grayCheckerBoard, Matrix.CreateScale(720, 480, 0));
            spriteBatch.Draw(Game1.globalBlackPixel, Vector2.Zero, Color.White);
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw(Game1.globalWhitePixel, new Vector2(0, 48), null, Color.LightBlue, 0.0f, Vector2.Zero, new Vector2(720, 240), SpriteEffects.None, AnimationFactory.DepthLayer3);
            AnimationFactory.drawAnimationFrame(spriteBatch, magnetWopleyAnim, magnetWopleyFrame, magnetWopleyPosition, AnimationFactory.DepthLayer1);
            AnimationFactory.drawAnimationFrame(spriteBatch, enemyFrameAnim, enemyChaseFrame, enemyPosition, AnimationFactory.DepthLayer2);

            int v = 0;
            for (int i = currentSpotInTiles; v < grassTiles.Length ; i = (i + 1) % grassTiles.Length)
            {
                AnimationFactory.drawAnimationFrame(spriteBatch, "titleScreenGrass", grassTiles[i], new Vector2((v * 16) - (float)grassPixelOffset, 256), AnimationFactory.DepthLayer0);

                v++;
            }

            if (!showPressButtonDialog)
            {
                for (int i = 0; i < menuList.Count; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        AnimationFactory.drawAnimationFrame(spriteBatch, "gui_angledBoxA", j != 0 ? (j == 6 ? 2 : 1) : 0, new Vector2((float)(300 + (25 * menuList[i].distanceOut)) + (j * 16), 300 + (40 * i)), Color.Purple, AnimationFactory.DepthLayer3);
                        AnimationFactory.drawAnimationFrame(spriteBatch, "gui_angledBoxC", j != 0 ? (j == 6 ? 2 : 1) : 0, new Vector2((float)(300 + (25 * menuList[i].distanceOut)) + (j * 16), 300 + (40 * i) + 16), Color.Purple, AnimationFactory.DepthLayer3);
                    }

                    spriteBatch.DrawString(Game1.gameFontText, menuList[i].text, new Vector2((float)(308 + (25 * menuList[i].distanceOut)), 305 + (40 * i)), Color.Lerp(Color.Black, Color.White, (float)menuList[i].distanceOut));
                }
            }
            else
            {
                if ((dialogTimer / 750) % 2 < 1.0)
                {
                    spriteBatch.DrawString(Game1.gameFontText, "PRESS ANY BUTTON", new Vector2(275, 345), Color.Black);
                }
            }
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, Game1.diamondWipe, Matrix.CreateScale(720, 480, 0));
            spriteBatch.Draw(Game1.globalBlackPixel, Vector2.Zero, Color.White);
            spriteBatch.End();
        }
    }
}
