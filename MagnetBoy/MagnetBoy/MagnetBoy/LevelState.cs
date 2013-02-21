using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using FuncWorks.XNA.XTiled;

namespace MagnetBoy
{
    class LevelState : IState
    {
        private ContentManager contentManager = null;

        private GameInput gameInput = null;

        private bool musicPlaying = false;

        /* this mutex is used to protect assets while loading the following:
         *  -- levelEntities
         *  -- levelCamera
         *  -- levelBulletPool
         *  -- levelParticlePool
         *  -- levelMap
         *  -- backgroundTile
         *  -- levelName
         */
        //private Semaphore assetResources;
        private bool grabbed;

        private List<Entity> levelEntities = null;
        public static Camera levelCamera = null;
        private BulletPool levelBulletPool = null;
        public static ParticlePool levelParticlePool = null;
        private static Map levelMap = null;
        private string levelName = null;

        public static float playerStamina = 100.0f;
        public const float playerStaminaMax = 100.0f;
        public const float playerStaminaDepleteRate = 2.5f;
        public const float playerStaminaGrowthRate = 2.5f;

        public static int maxPlayerHealth = 5;
        public static int currentPlayerHealth = 0;

        private static bool[] flags = null;
        private const int flagNum = 5;
        public enum FlagColor { Red = 0, Blue = 1, Green = 2, Yellow = 3, Purple = 4 };

        private bool paused;
        private bool startButtonDown;
        private int pausedSelect;

        private static double levelRecordTime;
        public static double LevelRecordTime { get { return levelRecordTime; } }

        private bool downPressed = false;
        private bool upPressed = false;
        private bool confirmPressed = false;
        private bool backPressed = false;

        private string tagLineA;
        private string tagLineB;

        public static bool fadingOut = false;
        private double fadingOutTimer = 0;
        private const double fadingOutDuration = 1000;

        public static bool showLevelCompleteText = false;

        private string[] loadingText = {"L", "O", "A", "D", "I", "N", "G", ".", ".", "."};
        private double loadingAnimTimePassed;
        private const double loadingAnimTime = 2000;
        private int magnetWopleyFrame;
        private double magnetWopleyLastUpdateTime;
        private string magnetWopleyAnim = "playerWalkRight";

        private int chaserFrame;
        private double chaserLastUpdateTime;
        private string chaserAnim = "angrySawWalkLeft";

        private string parallax1 = "testParallax1";
        private string parallax2 = "testParallax2";
        private string parallax3 = "testParallax3";

        private string levelSong = "songs/introTheme";

        public static bool checkPointTouched = false;
        public static Vector2 respawnPosition;

        public static Map CurrentLevel
        {
            get
            {
                return levelMap;
            }
        }

        private static bool endLevelFlag = false;

        public static Boolean EndLevelFlag
        {
            get
            {
                return endLevelFlag;
            }
            set
            {
                endLevelFlag = value;
            }
        }

        public static bool getFlag(FlagColor color)
        {
            return flags[(int)color];
        }
        public static void setFlag(FlagColor color, bool value)
        {
            flags[(int)color] = value;
        }
        public static Color getFlagXNAColor(FlagColor color)
        {
            switch (color)
            {
                case FlagColor.Blue:
                    return Color.Blue;
                case FlagColor.Green:
                    return Color.Green;
                case FlagColor.Red:
                    return Color.Red;
                case FlagColor.Yellow:
                    return Color.Yellow;
                case FlagColor.Purple:
                    return Color.Magenta;
                default:
                    return Color.Black;
            }
        }

        public LevelState(ContentManager newManager, string levelNameString)
        {
            EndLevelFlag = false;

            contentManager = newManager;

            gameInput = new GameInput(null);
            levelEntities = new List<Entity>(100);
            levelCamera = new Camera();
            levelBulletPool = new BulletPool();
            levelParticlePool = new ParticlePool(100);
            levelName = levelNameString;
            levelRecordTime = 0;

            paused = false;
            startButtonDown = false;

            showLevelCompleteText = false;

            loadingAnimTimePassed = 0;
            magnetWopleyFrame = 0;
            magnetWopleyLastUpdateTime = 0;
            chaserFrame = 0;
            chaserLastUpdateTime = 0;

            flags = new bool[flagNum];
            for (int i = 0; i < flagNum; i++)
            {
                flags[i] = false;
            }

            grabbed = false;
            new Thread(loadLevelThread).Start();

            Thread.Sleep(50);

            currentPlayerHealth = Game1.MagnetBoySaveData.defaultStartingHealth;

            fadingOut = false;
            fadingOutTimer = 0;

            IsUpdateable = true;

            GC.Collect();
        }

        private void loadLevelThread()
        {
            #if XBOX
            Thread.CurrentThread.SetProcessorAffinity(Game1.loadThread); 
            #endif

            Monitor.Enter(levelEntities);
            grabbed = true;

            if (Entity.globalEntityList != null)
            {
                Entity.globalEntityList.Clear();
                Entity.globalEntityList.TrimExcess();
            }

            tagLineA = "LEVEL " + (Game1.CurrentLevel + 1);
            tagLineB = Game1.levelNames[Game1.CurrentLevel];

            switch (Game1.CurrentLevel)
            {
                case 0:
                    parallax1 = null;
                    parallax2 = "cityParallax2";
                    parallax3 = "cityParallax3";
                    levelSong = "songs/song0";
                    break;
                case 1:
                    levelSong = "songs/song1";
                    parallax1 = null;
                    parallax2 = "cityParallax2";
                    parallax3 = "cityParallax3";
                    break;
                case 2:
                    levelSong = "songs/song2";
                    parallax1 = "sewerParallax3";
                    parallax2 = "sewerParallax2";
                    parallax3 = "sewerParallax1";
                    break;
                case 3:
                    levelSong = "songs/song3";
                    parallax1 = "factoryParallax1";
                    parallax2 = "factoryParallax2";
                    parallax3 = "factoryParallax3";
                    break;
                case 4:
                    levelSong = "songs/song4";
                    break;
                default:
                    break;
            }

            levelMap = contentManager.Load<Map>(levelName);

            foreach (ObjectLayer layer in levelMap.ObjectLayers)
            {
                foreach (MapObject obj in layer.MapObjects)
                {
                    Entity en = null;

                    switch (obj.Name)
                    {
                        case "player":
                            if (checkPointTouched == false)
                            {
                                en = new Player(obj.Bounds.X, obj.Bounds.Y);
                            }
                            else
                            {
                                en = new Player(respawnPosition.X, respawnPosition.Y);
                            }
                            levelEntities.Add(en);
                            levelCamera.setNewFocus(ref en);
                            break;
                        case "wm_pos":
                            levelEntities.Add(new WallMagnet(obj.Bounds.X, obj.Bounds.Y, Entity.Polarity.Positive));
                            break;
                        case "wm_neg":
                            levelEntities.Add(new WallMagnet(obj.Bounds.X, obj.Bounds.Y, Entity.Polarity.Negative));
                            break;
                        case "walker_pos":
                            levelEntities.Add(new Enemy(obj.Bounds.X, obj.Bounds.Y));
                            break;
                        case "walker_neg":
                            levelEntities.Add(new Enemy(obj.Bounds.X, obj.Bounds.Y));
                            break;
                        case "jumper_pos":
                            levelEntities.Add(new JumpingEnemy(obj.Bounds.X, obj.Bounds.Y));
                            break;
                        case "factory_conveyer_left":
                            levelEntities.Add(new ConveyerBelt(obj.Bounds.X, obj.Bounds.Y, ConveyerBelt.ConveyerSpot.Left));
                            break;
                        case "factory_conveyer_mid":
                            levelEntities.Add(new ConveyerBelt(obj.Bounds.X, obj.Bounds.Y, ConveyerBelt.ConveyerSpot.Mid));
                            break;
                        case "factory_conveyer_right":
                            levelEntities.Add(new ConveyerBelt(obj.Bounds.X, obj.Bounds.Y, ConveyerBelt.ConveyerSpot.Right));
                            break;
                        case "spikes_up":
                            levelEntities.Add(new Spikes(obj.Bounds.X, obj.Bounds.Y));
                            break;
                        case "angrySaw":
                            levelEntities.Add(new AngrySaw(obj.Bounds.X, obj.Bounds.Y));
                            break;
                        case "lava":
                            levelEntities.Add(new Lava(obj.Bounds.X, obj.Bounds.Y));
                            break;
                        case "lavaDumper":
                            if (obj.Properties.ContainsKey("interval"))
                            {
                                levelEntities.Add(new LavaDumper(obj.Bounds.X, obj.Bounds.Y, (double)(obj.Properties["interval"].AsSingle)));
                            }
                            else
                            {
                                levelEntities.Add(new LavaDumper(obj.Bounds.X, obj.Bounds.Y));
                            }
                            break;
                        case "endLevelFlag":
                            levelEntities.Add(new EndLevelFlag(obj.Bounds.X, obj.Bounds.Y));
                            break;
                        case "lolrus":
                            levelEntities.Add(new Lolrus(obj.Bounds.X, obj.Bounds.Y));
                            break;
                        case "wallEntity":
                            levelEntities.Add(new climbWall(obj.Bounds.X, obj.Bounds.Y));
                            break;
                        case "shieldDudeRight":
                            levelEntities.Add(new ShieldDude(obj.Bounds.X, obj.Bounds.Y, true));
                            break;
                        case "shieldDudeLeft":
                            levelEntities.Add(new ShieldDude(obj.Bounds.X, obj.Bounds.Y, false));
                            break;
                        case "heartItem":
                            levelEntities.Add(new HealthItem(obj.Bounds.X, obj.Bounds.Y));
                            break;
                        case "boss":
                            levelEntities.Add(new Boss(obj.Bounds.X, obj.Bounds.Y));
                            break;
                        case "flagDoor":
                            if (obj.Properties["color"].Value == "blue")
                            {
                                levelEntities.Add(new FlagDoor(obj.Bounds.X, obj.Bounds.Y, FlagColor.Blue));
                            }
                            else if (obj.Properties["color"].Value == "green")
                            {
                                levelEntities.Add(new FlagDoor(obj.Bounds.X, obj.Bounds.Y, FlagColor.Green));
                            }
                            else if (obj.Properties["color"].Value == "red")
                            {
                                levelEntities.Add(new FlagDoor(obj.Bounds.X, obj.Bounds.Y, FlagColor.Red));
                            }
                            else if (obj.Properties["color"].Value == "yellow")
                            {
                                levelEntities.Add(new FlagDoor(obj.Bounds.X, obj.Bounds.Y, FlagColor.Yellow));
                            }
                            else if (obj.Properties["color"].Value == "purple")
                            {
                                levelEntities.Add(new FlagDoor(obj.Bounds.X, obj.Bounds.Y, FlagColor.Purple));
                            }
                            break;
                        case "flagLock":
                            if (obj.Properties["color"].Value == "blue")
                            {
                                levelEntities.Add(new FlagLock(obj.Bounds.X, obj.Bounds.Y, FlagColor.Blue));
                            }
                            else if (obj.Properties["color"].Value == "green")
                            {
                                levelEntities.Add(new FlagLock(obj.Bounds.X, obj.Bounds.Y, FlagColor.Green));
                            }
                            else if (obj.Properties["color"].Value == "red")
                            {
                                levelEntities.Add(new FlagLock(obj.Bounds.X, obj.Bounds.Y, FlagColor.Red));
                            }
                            else if (obj.Properties["color"].Value == "yellow")
                            {
                                levelEntities.Add(new FlagLock(obj.Bounds.X, obj.Bounds.Y, FlagColor.Yellow));
                            }
                            else if (obj.Properties["color"].Value == "purple")
                            {
                                levelEntities.Add(new FlagLock(obj.Bounds.X, obj.Bounds.Y, FlagColor.Purple));
                            }
                            break;
                        case "flagKey":
                            if (obj.Properties["color"].Value == "blue")
                            {
                                levelEntities.Add(new FlagKey(obj.Bounds.X, obj.Bounds.Y, FlagColor.Blue));
                            }
                            else if (obj.Properties["color"].Value == "green")
                            {
                                levelEntities.Add(new FlagKey(obj.Bounds.X, obj.Bounds.Y, FlagColor.Green));
                            }
                            else if (obj.Properties["color"].Value == "red")
                            {
                                levelEntities.Add(new FlagKey(obj.Bounds.X, obj.Bounds.Y, FlagColor.Red));
                            }
                            else if (obj.Properties["color"].Value == "yellow")
                            {
                                levelEntities.Add(new FlagKey(obj.Bounds.X, obj.Bounds.Y, FlagColor.Yellow));
                            }
                            else if (obj.Properties["color"].Value == "purple")
                            {
                                levelEntities.Add(new FlagKey(obj.Bounds.X, obj.Bounds.Y, FlagColor.Purple));
                            }
                            break;
                        case "tutorialSign":
                            if (obj.Properties["message"].Value == "message1")
                            {
                                levelEntities.Add(new TutorialSign(obj.Bounds.X, obj.Bounds.Y, (TutorialSign.SignMessage)1));
                            }
                            else if (obj.Properties["message"].Value == "message2")
                            {
                                levelEntities.Add(new TutorialSign(obj.Bounds.X, obj.Bounds.Y, (TutorialSign.SignMessage)2));
                            }
                            else if (obj.Properties["message"].Value == "message3")
                            {
                                levelEntities.Add(new TutorialSign(obj.Bounds.X, obj.Bounds.Y, (TutorialSign.SignMessage)3));
                            }
                            else if (obj.Properties["message"].Value == "message4")
                            {
                                levelEntities.Add(new TutorialSign(obj.Bounds.X, obj.Bounds.Y, (TutorialSign.SignMessage)4));
                            }
                            else if (obj.Properties["message"].Value == "message5")
                            {
                                levelEntities.Add(new TutorialSign(obj.Bounds.X, obj.Bounds.Y, (TutorialSign.SignMessage)5));
                            }
                            else if (obj.Properties["message"].Value == "message6")
                            {
                                levelEntities.Add(new TutorialSign(obj.Bounds.X, obj.Bounds.Y, (TutorialSign.SignMessage)6));
                            }
                            break;
                        case "bossShield":
                            levelEntities.Add(new bossShield(obj.Bounds.X, obj.Bounds.Y));
                            break;
                        case "walkMarker":
                            levelEntities.Add(new WalkMarker(obj.Bounds.X, obj.Bounds.Y));
                            break;
                        case "goomba":
                            levelEntities.Add(new Goomba(obj.Bounds.X, obj.Bounds.Y));
                            break;
                        default:
                            break;
                    }
                }
            }

            Thread.Sleep(1500);

            levelRecordTime = 0;

            Monitor.Exit(levelEntities);
        }

        private void loadingDoUpdate(GameTime currentTime)
        {
            loadingAnimTimePassed += currentTime.ElapsedGameTime.Milliseconds;

            if (loadingAnimTimePassed > loadingAnimTime)
            {
                loadingAnimTimePassed = 0;
            }

            //updates for anims
            if (magnetWopleyLastUpdateTime == 0)
            {
                magnetWopleyLastUpdateTime = currentTime.TotalGameTime.TotalMilliseconds;
            }
            if (currentTime.TotalGameTime.TotalMilliseconds - magnetWopleyLastUpdateTime > AnimationFactory.getAnimationSpeed(magnetWopleyAnim))
            {
                magnetWopleyLastUpdateTime = currentTime.TotalGameTime.TotalMilliseconds;

                magnetWopleyFrame = (magnetWopleyFrame + 1) % AnimationFactory.getAnimationFrameCount(magnetWopleyAnim);
            }

            //updates for anims
            if (chaserLastUpdateTime == 0)
            {
                chaserLastUpdateTime = currentTime.TotalGameTime.TotalMilliseconds;
            }
            if (currentTime.TotalGameTime.TotalMilliseconds - chaserLastUpdateTime > AnimationFactory.getAnimationSpeed(chaserAnim))
            {
                chaserLastUpdateTime = currentTime.TotalGameTime.TotalMilliseconds;

                chaserFrame = (magnetWopleyFrame + 1) % AnimationFactory.getAnimationFrameCount(chaserAnim);
            }
        }

        protected override void doUpdate(GameTime currentTime)
        {
            if (grabbed == false || Monitor.TryEnter(levelEntities) == false)
            {
                loadingDoUpdate(currentTime);
                return;
            }
            Monitor.Exit(levelEntities);

            gameInput.update();

            if (!musicPlaying)
            {
                musicPlaying = true;

                AudioFactory.playSong(levelSong);
            }

            if (currentPlayerHealth < 1 && !fadingOut)
            {
                AudioFactory.stopSong();

                AudioFactory.playSFX("sfx/lose");

                fadingOut = true;
            }

            if (paused)
            {
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

                    Game1.grayCheckerBoard.Parameters["slideX"].SetValue(TitleScreenMenuState.checkerBoardSlide.X);
                    Game1.grayCheckerBoard.Parameters["slideY"].SetValue(TitleScreenMenuState.checkerBoardSlide.Y);
                }

                if (GameInput.isButtonDown(GameInput.PlayerButton.UpDirection))
                {
                    upPressed = true;
                }
                else if (!(GameInput.isButtonDown(GameInput.PlayerButton.UpDirection)) && upPressed)
                {
                    pausedSelect--;
                    if (pausedSelect < 0)
                    {
                        pausedSelect = 1;
                    }

                    upPressed = false;
                    AudioFactory.playSFX("sfx/menu");
                }

                if (GameInput.isButtonDown(GameInput.PlayerButton.DownDirection))
                {
                    downPressed = true;
                }
                else if (!(GameInput.isButtonDown(GameInput.PlayerButton.DownDirection)) && downPressed)
                {
                    pausedSelect++;
                    if (pausedSelect > 1)
                    {
                        pausedSelect = 0;
                    }

                    downPressed = false;
                    AudioFactory.playSFX("sfx/menu");
                }

                if (GameInput.isButtonDown(GameInput.PlayerButton.Confirm))
                {
                    confirmPressed = true;
                }
                else if (!(GameInput.isButtonDown(GameInput.PlayerButton.Confirm)) && confirmPressed)
                {
                    switch (pausedSelect)
                    {
                        case 0:
                            paused = false;
                            MediaPlayer.Volume = 1.0f;
                            AudioFactory.playSFX("sfx/menuClose");
                            break;
                        case 1:
                            paused = false;
                            checkPointTouched = false;
                            MediaPlayer.Volume = 1.0f;
                            AudioFactory.playSFX("sfx/menuOpen");
                            GameScreenManager.switchScreens(GameScreenManager.GameScreenType.Menu, "TitleScreenMenu_fromPause");
                            break;
                        default:
                            break;
                    }

                    confirmPressed = false;
                }

                if (GameInput.isButtonDown(GameInput.PlayerButton.StartButton))
                {
                    startButtonDown = true;
                }
                else if (!(GameInput.isButtonDown(GameInput.PlayerButton.StartButton)) && startButtonDown)
                {
                    startButtonDown = false;

                    paused = false;
                    MediaPlayer.Volume = 1.0f;

                    AudioFactory.playSFX("sfx/menuClose");
                }

                if (GameInput.isButtonDown(GameInput.PlayerButton.Cancel))
                {
                    backPressed = true;
                }
                else if (!(GameInput.isButtonDown(GameInput.PlayerButton.Cancel)) && backPressed)
                {
                    backPressed = false;

                    paused = false;
                    MediaPlayer.Volume = 1.0f;

                    AudioFactory.playSFX("sfx/menuClose");
                }
            }
            else
            {
                levelRecordTime += currentTime.ElapsedGameTime.Milliseconds;

                if (GameInput.isButtonDown(GameInput.PlayerButton.StartButton))
                {
                    startButtonDown = true;
                }
                else if (!(GameInput.isButtonDown(GameInput.PlayerButton.StartButton)) && startButtonDown)
                {
                    startButtonDown = false;

                    if (!fadingOut)
                    {
                        MediaPlayer.Volume = 0.25f;
                        pausedSelect = 0;
                        paused = true;

                        AudioFactory.playSFX("sfx/menuOpen");
                    }
                }

                if (fadingOut)
                {
                    fadingOutTimer += currentTime.ElapsedGameTime.Milliseconds;

                    if (currentPlayerHealth > 0)
                    {
                        Game1.diamondWipe.Parameters["time"].SetValue(1.0f - (float)(fadingOutTimer/fadingOutDuration));
                    }

                    if (fadingOutTimer > fadingOutDuration)
                    {
                        endLevelFlag = true;
                    }
                }

                if (endLevelFlag == true)
                {
                    foreach (Entity en in levelEntities)
                    {
                        en.removeFromGame = true;
                    }

                    foreach (Entity en in levelEntities)
                    {
                        en.death();
                    }

                    levelBulletPool.clearPool();

                    if (currentPlayerHealth < 1)
                    {
                        GameScreenManager.switchScreens(GameScreenManager.GameScreenType.Level, levelName);
                    }
                    else
                    {
                        GameScreenManager.nextLevel();
                    }

                    endLevelFlag = false;

                    return;
                }

                foreach (Entity en in levelEntities)
                {
                    en.update(currentTime);

                    if (endLevelFlag == true)
                    {
                        break;
                    }
                }

                levelBulletPool.updatePool(currentTime);

                levelParticlePool.updatePool(currentTime);

                //levelEntities.RemoveAll(en => en.removeFromGame == true);
                XboxListTools.RemoveAll<Entity>(levelEntities, XboxListTools.isShouldBeRemoved);

                foreach (Entity en in levelEntities)
                {
                    en.death();
                    break;
                }
            }
        }

        public static bool isSolidMap(Vector2 point)
        {
            foreach (TileLayer layer in levelMap.TileLayers)
            {
                bool isSolid = false;

                foreach (KeyValuePair<string, Property> p in layer.Properties)
                {
                    if (p.Key.Equals("solid") && p.Value.AsInt32 == 1)
                    {
                        isSolid = true;
                    }
                }

                if (isSolid == true)
                {
                    if (point.X < 0 || point.X >= levelMap.Bounds.Width || point.Y < 0 || point.Y >= levelMap.Bounds.Height)
                    {
                        //
                    }

                    int posX = (int)(point.X / levelMap.TileWidth);
                    int posY = (int)(point.Y / levelMap.TileHeight);

                    try
                    {
                        if (layer.Tiles[posX][posY] != null)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        private void loadingDraw(SpriteBatch spriteBatch)
        {
            Game1.graphics.GraphicsDevice.Clear(Color.Black);

            Vector2 loadPos = new Vector2(300, 300);
            Vector2 wopleyPos = new Vector2(250, 300);
            Vector2 chaserPos = new Vector2(450, 300);

            spriteBatch.Begin();
            for (int i = 0; i < loadingText.Length; i++)
            {
                if ((int)(10 * (loadingAnimTimePassed / loadingAnimTime)) == i)
                {
                    loadPos.Y -= 15;
                }

                spriteBatch.DrawString(Game1.gameFontText, loadingText[i], loadPos, Color.White);

                loadPos.X += 15;

                if ((int)(10 * (loadingAnimTimePassed / loadingAnimTime)) == i)
                {
                    loadPos.Y += 15;
                }
            }

            AnimationFactory.drawAnimationFrame(spriteBatch, magnetWopleyAnim, magnetWopleyFrame, wopleyPos, AnimationFactory.DepthLayer0);
            AnimationFactory.drawAnimationFrame(spriteBatch, chaserAnim, chaserFrame, chaserPos, AnimationFactory.DepthLayer0);
            
            spriteBatch.End();
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            if (grabbed == false || Monitor.TryEnter(levelEntities) == false)
            {
                loadingDraw(spriteBatch);
                return;
            }
            Monitor.Exit(levelEntities);

            Matrix mx = new Matrix();
            Rectangle rx = new Rectangle();
            levelCamera.getDrawTranslation(ref mx, ref Game1.mapView, ref levelMap);
            levelCamera.getDrawRectangle(ref rx, ref Game1.mapView, ref levelMap);

            Game1.graphics.GraphicsDevice.Clear(Color.Lerp(Color.DarkGray, Color.Pink, 0.4f));

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            AnimationFactory.drawAnimationFrame(spriteBatch, parallax3, 0, new Vector2(-1 * (levelCamera.getFocusPosition().X / levelMap.Bounds.Width) * (float)AnimationFactory.getAnimationFrameWidth(parallax3), 0f), AnimationFactory.DepthLayer3);
            AnimationFactory.drawAnimationFrame(spriteBatch, parallax3, 0, new Vector2((-1 * (levelCamera.getFocusPosition().X / levelMap.Bounds.Width) * (float)AnimationFactory.getAnimationFrameWidth(parallax3)) + (float)AnimationFactory.getAnimationFrameWidth(parallax3), 0f), AnimationFactory.DepthLayer3);

            AnimationFactory.drawAnimationFrame(spriteBatch, parallax2, 0, new Vector2(-2 * (levelCamera.getFocusPosition().X / levelMap.Bounds.Width) * (float)AnimationFactory.getAnimationFrameWidth(parallax2), 0f), AnimationFactory.DepthLayer2);
            AnimationFactory.drawAnimationFrame(spriteBatch, parallax2, 0, new Vector2((-2 * (levelCamera.getFocusPosition().X / levelMap.Bounds.Width) * (float)AnimationFactory.getAnimationFrameWidth(parallax2)) + (float)AnimationFactory.getAnimationFrameWidth(parallax2), 0f), AnimationFactory.DepthLayer2);
            AnimationFactory.drawAnimationFrame(spriteBatch, parallax2, 0, new Vector2((-2 * (levelCamera.getFocusPosition().X / levelMap.Bounds.Width) * (float)AnimationFactory.getAnimationFrameWidth(parallax2)) + (float)(2f * AnimationFactory.getAnimationFrameWidth(parallax2)), 0f), AnimationFactory.DepthLayer2);

            AnimationFactory.drawAnimationFrame(spriteBatch, parallax1, 0, new Vector2(-3 * (levelCamera.getFocusPosition().X / levelMap.Bounds.Width) * (float)AnimationFactory.getAnimationFrameWidth(parallax1), 0f), AnimationFactory.DepthLayer1);
            AnimationFactory.drawAnimationFrame(spriteBatch, parallax1, 0, new Vector2((-3 * (levelCamera.getFocusPosition().X / levelMap.Bounds.Width) * (float)AnimationFactory.getAnimationFrameWidth(parallax1)) + (float)AnimationFactory.getAnimationFrameWidth(parallax1), 0f), AnimationFactory.DepthLayer1);
            AnimationFactory.drawAnimationFrame(spriteBatch, parallax1, 0, new Vector2((-3 * (levelCamera.getFocusPosition().X / levelMap.Bounds.Width) * (float)AnimationFactory.getAnimationFrameWidth(parallax1)) + (float)(2f * AnimationFactory.getAnimationFrameWidth(parallax1)), 0f), AnimationFactory.DepthLayer1);
            AnimationFactory.drawAnimationFrame(spriteBatch, parallax1, 0, new Vector2((-3 * (levelCamera.getFocusPosition().X / levelMap.Bounds.Width) * (float)AnimationFactory.getAnimationFrameWidth(parallax1)) + (float)(3f * AnimationFactory.getAnimationFrameWidth(parallax1)), 0f), AnimationFactory.DepthLayer1);
            spriteBatch.End();

            // draw map
            spriteBatch.Begin();
            levelMap.Draw(spriteBatch, rx);
            spriteBatch.End();

            // draw sprites
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, Game1.tintRedEffect, mx);
            foreach (Entity a in levelEntities)
            {
                a.draw(spriteBatch);
            }

            levelBulletPool.drawPool(spriteBatch);

            levelParticlePool.drawPool(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            if (currentPlayerHealth <= maxPlayerHealth)
            {
                for (int i = 0; i < maxPlayerHealth; i++)
                {
                    AnimationFactory.drawAnimationFrame(spriteBatch, i < currentPlayerHealth ? "heartIdle" : "heartEmpty", 0, new Vector2(112 + (i * 32), 96), new Vector2(0.8f, 0.8f), Color.White, AnimationFactory.DepthLayer0);
                }
            }

            AnimationFactory.drawAnimationFrame(spriteBatch, "gui_angledBoxB", 1, new Vector2(108, 76), new Vector2(10.0f * (playerStamina / playerStaminaMax), 1.0f), Color.Lerp(Color.DarkBlue, Color.Cyan, (playerStamina / playerStaminaMax)), AnimationFactory.DepthLayer0);
            AnimationFactory.drawAnimationFrame(spriteBatch, "gui_angledBoxB", 1, new Vector2(108, 76), new Vector2(10f, 1f), Color.Gray, AnimationFactory.DepthLayer1);
            AnimationFactory.drawAnimationFrame(spriteBatch, "gui_angledBoxB", 1, new Vector2(107, 75), new Vector2(10.1f, 1.1f), Color.Black, AnimationFactory.DepthLayer2);

            if (levelRecordTime < 1500)
            {
                Vector2 posA = new Vector2((float)(levelRecordTime / 1500) * 400, 90);
                Vector2 posB = new Vector2((float)(levelRecordTime / 1500) * 400, 104);

                for (int i = -1; i < 2; i++)
                {
                    spriteBatch.DrawString(Game1.gameFontText, tagLineA, posA + new Vector2(i, i), (i == 0) ? Color.White : Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, (i == 0) ? AnimationFactory.DepthLayer0 : AnimationFactory.DepthLayer3);
                    spriteBatch.DrawString(Game1.gameFontText, tagLineB, posB + new Vector2(i, i), (i == 0) ? Color.White : Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, (i == 0) ? AnimationFactory.DepthLayer0 : AnimationFactory.DepthLayer3);
                    spriteBatch.DrawString(Game1.gameFontText, tagLineA, posA + new Vector2(-1 * i, i), (i == 0) ? Color.White : Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, (i == 0) ? AnimationFactory.DepthLayer0 : AnimationFactory.DepthLayer3);
                    spriteBatch.DrawString(Game1.gameFontText, tagLineB, posB + new Vector2(-1 * i, i), (i == 0) ? Color.White : Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, (i == 0) ? AnimationFactory.DepthLayer0 : AnimationFactory.DepthLayer3);
                }
            }
            else if (levelRecordTime < 5000)
            {
                Vector2 posA = new Vector2(400, 90);
                Vector2 posB = new Vector2(400, 104);

                for (int i = -1; i < 2; i++)
                {
                    spriteBatch.DrawString(Game1.gameFontText, tagLineA, posA + new Vector2(i, i), (i == 0) ? Color.White : Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, (i == 0) ? AnimationFactory.DepthLayer0 : AnimationFactory.DepthLayer3);
                    spriteBatch.DrawString(Game1.gameFontText, tagLineB, posB + new Vector2(i, i), (i == 0) ? Color.White : Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, (i == 0) ? AnimationFactory.DepthLayer0 : AnimationFactory.DepthLayer3);
                    spriteBatch.DrawString(Game1.gameFontText, tagLineA, posA + new Vector2(-1 * i, i), (i == 0) ? Color.White : Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, (i == 0) ? AnimationFactory.DepthLayer0 : AnimationFactory.DepthLayer3);
                    spriteBatch.DrawString(Game1.gameFontText, tagLineB, posB + new Vector2(-1 * i, i), (i == 0) ? Color.White : Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, (i == 0) ? AnimationFactory.DepthLayer0 : AnimationFactory.DepthLayer3);
                }
            }
            else if (levelRecordTime < 7000)
            {
                Vector2 posA = new Vector2(400 + (float)((levelRecordTime - 5000) / 2000) * 900, 90);
                Vector2 posB = new Vector2(400 + (float)((levelRecordTime - 5000) / 2000) * 900, 104);

                for (int i = -1; i < 2; i++)
                {
                    spriteBatch.DrawString(Game1.gameFontText, tagLineA, posA + new Vector2(i, i), (i == 0) ? Color.White : Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, (i == 0) ? AnimationFactory.DepthLayer0 : AnimationFactory.DepthLayer3);
                    spriteBatch.DrawString(Game1.gameFontText, tagLineB, posB + new Vector2(i, i), (i == 0) ? Color.White : Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, (i == 0) ? AnimationFactory.DepthLayer0 : AnimationFactory.DepthLayer3);
                    spriteBatch.DrawString(Game1.gameFontText, tagLineA, posA + new Vector2(-1 * i, i), (i == 0) ? Color.White : Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, (i == 0) ? AnimationFactory.DepthLayer0 : AnimationFactory.DepthLayer3);
                    spriteBatch.DrawString(Game1.gameFontText, tagLineB, posB + new Vector2(-1 * i, i), (i == 0) ? Color.White : Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, (i == 0) ? AnimationFactory.DepthLayer0 : AnimationFactory.DepthLayer3);
                }
            }

            if (showLevelCompleteText)
            {
                MBQG.drawBlackBorderText(spriteBatch, new Vector2(360 - Game1.gameFontText.MeasureString("LEVEL COMPLETE").X/2, 240), Color.White, "LEVEL COMPLETE",  AnimationFactory.DepthLayer0);
            }
            spriteBatch.End();

            if (paused)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, Game1.grayCheckerBoard, Matrix.CreateScale(720, 480, 0));
                spriteBatch.Draw(Game1.globalBlackPixel, Vector2.Zero, Color.White);
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                MBQG.drawGUIBox(spriteBatch, new Vector2(284, 200), 10, 5, Color.Purple, AnimationFactory.DepthLayer2);
                spriteBatch.DrawString(Game1.gameFontText, "PAUSED", new Vector2(330, 204), Color.White);
                spriteBatch.DrawString(Game1.gameFontText, "Continue", new Vector2(322, 228), pausedSelect == 0 ? Color.LightGray : Color.Black);
                spriteBatch.DrawString(Game1.gameFontText, "Return to Title", new Vector2(298, 252), pausedSelect == 1 ? Color.White : Color.Black);
                spriteBatch.End();
            }

            /*
            //draw game cursor
            Matrix arrowRotation = Matrix.Identity;
            arrowRotation = Matrix.Multiply(arrowRotation, Matrix.CreateTranslation(-16.0f, -16.0f, 0.0f));
            arrowRotation = Matrix.Multiply(arrowRotation, Matrix.CreateRotationZ(((float)(Math.PI / 2))));
            arrowRotation = Matrix.Multiply(arrowRotation, Matrix.CreateTranslation(GameInput.P1MouseDirection.Length(), 0.0f, 0.0f));
            arrowRotation = Matrix.Multiply(arrowRotation, Matrix.CreateRotationZ((float)Math.Atan2(GameInput.P1MouseDirectionNormal.Y, GameInput.P1MouseDirectionNormal.X)));
            arrowRotation = Matrix.Multiply(arrowRotation, Matrix.CreateTranslation(Game1.graphics.GraphicsDevice.Viewport.Bounds.Width / 2, Game1.graphics.GraphicsDevice.Viewport.Bounds.Height / 2, 0.0f));
            

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, arrowRotation);
            AnimationFactory.drawAnimationFrame(spriteBatch, "mouseArrow", 0, Vector2.Zero, AnimationFactory.DepthLayer0); 
            spriteBatch.End();*/

            if (fadingOut)
            {
                if (currentPlayerHealth < 1)
                {
                    Matrix stretch = Matrix.CreateScale((float)(Game1.mapView.Width), (float)((fadingOutTimer / fadingOutDuration) * (Game1.mapView.Height)), 1.0f);

                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, stretch);
                    spriteBatch.Draw(Game1.globalBlackPixel, Vector2.Zero, Color.White);
                    spriteBatch.End();
                }
                else
                {
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, Game1.diamondWipe, Matrix.CreateScale(720f, 480f, 0f));
                    spriteBatch.Draw(Game1.globalBlackPixel, Vector2.Zero, Color.White);
                    spriteBatch.End();
                }
            }
        }
    }
}
