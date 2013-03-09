using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FuncWorks.XNA.XTiled;

namespace MagnetBoy
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private static bool exitGame = false;
        public static bool ExitGame
        {
            get { return exitGame; }
            set { exitGame = value; }
        }

        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameInput gameInput = null;

        // cpu identifiers for setting thread affinity on the Xbox 360
        public static int[] loadThread = { 3 };

        public struct LevelScoreStruct
        {
            public string levelBestTimeOwner;
            public double levelBestTime;
        }

        // this struct is serialized into Xbox 360 save data; it is global for all users
        public struct SaveGameData
        {
            public bool loaded;

            public int furthestLevelUnlocked;

            public int defaultStartingHealth;
            public bool showInGameTimer;
            public bool showInGameTopTime;

            public double levelBestTime_1;
            public string levelBestOwner_1;
            public double levelBestTime_2;
            public string levelBestOwner_2;
            public double levelBestTime_3;
            public string levelBestOwner_3;
            public double levelBestTime_4;
            public string levelBestOwner_4;
            public double levelBestTime_5;
            public string levelBestOwner_5;

            public double levelBestTime(int index)
            {
                switch (index)
                {
                    case 0:
                        return levelBestTime_1;
                    case 1:
                        return levelBestTime_2;
                    case 2:
                        return levelBestTime_3;
                    case 3:
                        return levelBestTime_4;
                    case 4:
                        return levelBestTime_5;
                    default:
                        return 99999999999;
                }
            }

            public string levelBestOwner(int index)
            {
                switch (index)
                {
                    case 0:
                        return levelBestOwner_1;
                    case 1:
                        return levelBestOwner_2;
                    case 2:
                        return levelBestOwner_3;
                    case 3:
                        return levelBestOwner_4;
                    case 4:
                        return levelBestOwner_5;
                    default:
                        return "";
                }
            }

            public void setLevelRecord(int index, double time, string owner)
            {
                switch (index)
                {
                    case 0:
                        levelBestTime_1 = time;
                        levelBestOwner_1 = owner;
                        break;
                    case 1:
                        levelBestTime_2 = time;
                        levelBestOwner_2 = owner;
                        break;
                    case 2:
                        levelBestTime_3 = time;
                        levelBestOwner_3 = owner;
                        break;
                    case 3:
                        levelBestTime_4 = time;
                        levelBestOwner_4 = owner;
                        break;
                    case 4:
                        levelBestTime_5 = time;
                        levelBestOwner_5 = owner;
                        break;
                    default:
                        return;
                }
            }
        }

        public static SaveGameData MagnetBoySaveData;
        public static double onScreenSaveSpin;

        public static Texture2D globalCompanyLogo = null;
        public static Texture2D globalGameLogo = null;
        public static Texture2D copyright = null;

        public static Texture2D globalTestWalrus = null;
        public static Texture2D globalTestPositive = null;
        public static Texture2D globalTestNegative = null;

        public static Texture2D globalCreditsList = null;

        public static Texture2D globalBlackPixel = null;
        public static Texture2D globalWhitePixel = null;

        public static SpriteFont gameFontText = null;

        public static Effect tintRedEffect = null;
        public static Effect grayCheckerBoard = null;
        public static Effect diamondWipe = null;

        private AnimationFactory aFac = null;
        private AudioFactory audFac = null;

        public static Random gameRandom = null;

        public static Rectangle mapView;

        private GameScreenManager screenManager = null;

        public static ContentManager levelLoader = null;

        // Game Level Information
        public static readonly string[] levelNames = { "Leave the Lab", "Cut the City", "Scrape the Sewer", "Fight the Factory", "WOPLEY" };
        public static readonly string[] levelFileNames = { "theLab2", "WillysMap", "sewer", "theLab", "climbTest" };

        //currentLevel and furthestLevelProgressed start from 0 and go to NumberOfLevels - 1
        private static int currentLevel;
        public static int CurrentLevel { get { return currentLevel; } set { currentLevel = value % Game1.NumberOfLevels; } }
        public static int FurthestLevelProgressed { get { return MagnetBoySaveData.furthestLevelUnlocked; } set { MagnetBoySaveData.furthestLevelUnlocked = value % Game1.NumberOfLevels; } }
        public static int NumberOfLevels { get { return levelNames.Length; } }

        private static bool assetsLoaded = false;
        public static bool AssetsLoaded { get { return assetsLoaded; } }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            //game runs in 480p widescreen
            graphics.PreferredBackBufferWidth = 720;
            graphics.PreferredBackBufferHeight = 480;

            //graphics.IsFullScreen = true;

#if XBOX
            Components.Add(new GamerServicesComponent(this));
#endif

            Content.RootDirectory = "Content";

            levelLoader = new ContentManager(Services);
            levelLoader.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            gameRandom = new Random();

            base.Initialize();

            mapView = graphics.GraphicsDevice.Viewport.Bounds;

            currentLevel = 0;

            onScreenSaveSpin = 0;

#if WINDOWS

            if (Game1.MagnetBoySaveData.loaded == false)
            {
                resetSaveData();
            }

            MediaPlayer.Volume = 1.0f;

            //FurthestLevelProgressed = 4;
#endif
#if XBOX
            resetSaveData();
#endif

            gameInput = new GameInput(graphics.GraphicsDevice);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            gameFontText = Content.Load<SpriteFont>("testFont");

            aFac = new AnimationFactory(this.Content);
            audFac = new AudioFactory(this.Content);

            globalCompanyLogo = this.Content.Load<Texture2D>("FrostTreeLogo");
            globalGameLogo = this.Content.Load<Texture2D>("ZippyPushKidLogo");
            globalTestWalrus = this.Content.Load<Texture2D>("walrus");
            globalTestPositive = this.Content.Load<Texture2D>("posTest");
            globalTestNegative = this.Content.Load<Texture2D>("negTest");
            globalBlackPixel = this.Content.Load<Texture2D>("1x1BlackPixel");
            globalWhitePixel = this.Content.Load<Texture2D>("1x1WhitePixel");
            globalCreditsList = this.Content.Load<Texture2D>("credits");
            copyright = this.Content.Load<Texture2D>("copyright");

            tintRedEffect = this.Content.Load<Effect>("TintRed");
            tintRedEffect.CurrentTechnique = tintRedEffect.Techniques["Technique1"];

            grayCheckerBoard = this.Content.Load<Effect>("GrayCheckerBoard");
            grayCheckerBoard.CurrentTechnique = grayCheckerBoard.Techniques["Technique1"];

            diamondWipe = this.Content.Load<Effect>("DiamondWipe");
            diamondWipe.CurrentTechnique = diamondWipe.Techniques["Technique1"];

            assetsLoaded = false;

            screenManager = new GameScreenManager(this.Content);
            GameScreenManager.switchScreens(GameScreenManager.GameScreenType.SplashScreen, null);
        }

        public static void loadGameAssets()
        {
#if XBOX
            Thread.CurrentThread.SetProcessorAffinity(Game1.loadThread); 
#endif

            if (assetsLoaded)
            {
                return;
            }

            AnimationFactory aFac = new AnimationFactory(null);

            aFac.pushSheet("playerSheet");
            aFac.pushSheet("conveyer");
            aFac.pushSheet("spikes");
            aFac.pushSheet("cursorTarget");
            aFac.pushSheet("testBulletSheet");
            aFac.pushSheet("angrySawSheet");
            aFac.pushSheet("pushArrow");
            aFac.pushSheet("lolrusSheet");
            aFac.pushSheet("bucketSheet");
            aFac.pushSheet("lavaBottomSheet");
            aFac.pushSheet("fireballSheet");
            aFac.pushSheet("lavaDumperSheet");
            aFac.pushSheet("shieldGuySheet");
            aFac.pushSheet("heartSheet");
            aFac.pushSheet("dansParticleSheet");
            aFac.pushSheet("flagDoorSheet");
            aFac.pushSheet("flagKeySheet");
            aFac.pushSheet("wopleySheet");
            aFac.pushSheet("heartSheet2");
            aFac.pushSheet("brainSheet");
            aFac.pushSheet("lungSheet");
            aFac.pushSheet("tutorialSignsSheet");
            aFac.pushSheet("endLevelSheet");
            aFac.pushSheet("guiComponentSheet");
            aFac.pushSheet("testParallaxSheet");
            aFac.pushSheet("xboxButtonsSheet");
            aFac.pushSheet("goombaSheet");
            aFac.pushSheet("factoryParallaxSheet");
            aFac.pushSheet("wopleyShieldSheet");
            aFac.pushSheet("titleScreenGrass");
            aFac.pushSheet("sewerParallaxSheet");
            aFac.pushSheet("cityParallaxSheet");
            aFac.pushSheet("RSTutSheet");
            aFac.pushSheet("pushTutSheet");
            aFac.pushSheet("killTutSheet");
            aFac.pushSheet("enemyTutSheet");

            aFac.pushAnimation("playerAnims");
            aFac.pushAnimation("conveyerAnims");
            aFac.pushAnimation("spikesAnim");
            aFac.pushAnimation("cursorTargetAnims");
            aFac.pushAnimation("testBulletAnims");
            aFac.pushAnimation("angrySaw");
            aFac.pushAnimation("pushArrowAnims");
            aFac.pushAnimation("lolrus");
            aFac.pushAnimation("bucket");
            aFac.pushAnimation("lavaBottom");
            aFac.pushAnimation("fireball");
            aFac.pushAnimation("lavaDumper");
            aFac.pushAnimation("shieldGuy");
            aFac.pushAnimation("heart");
            aFac.pushAnimation("dansParticleAnims");
            aFac.pushAnimation("flagDoorAnims");
            aFac.pushAnimation("flagKeyAnims");
            aFac.pushAnimation("wopley");
            aFac.pushAnimation("heartProjectile");
            aFac.pushAnimation("brain");
            aFac.pushAnimation("lung");
            aFac.pushAnimation("guiComponentAnims");
            aFac.pushAnimation("tutorialSignsAnims");
            aFac.pushAnimation("endLevelAnims");
            aFac.pushAnimation("testParallaxAnims");
            aFac.pushAnimation("xboxButtonsAnims");
            aFac.pushAnimation("goomba");
            aFac.pushAnimation("factoryParallax");
            aFac.pushAnimation("wopleyShield");
            aFac.pushAnimation("titleScreenGrassAnims");
            aFac.pushAnimation("sewerParallax");
            aFac.pushAnimation("cityParallax");

            AudioFactory.pushNewSong("songs/song0");
            AudioFactory.pushNewSong("songs/song1");
            AudioFactory.pushNewSong("songs/song2");
            AudioFactory.pushNewSong("songs/song3");
            AudioFactory.pushNewSong("songs/song4");
            AudioFactory.pushNewSong("songs/introTheme");

            AudioFactory.pushNewSFX("sfx/lose");
            AudioFactory.pushNewSFX("sfx/menu");
            AudioFactory.pushNewSFX("sfx/explosion");
            AudioFactory.pushNewSFX("sfx/menuOpen");
            AudioFactory.pushNewSFX("sfx/menuClose");
            AudioFactory.pushNewSFX("sfx/menuDeny");
            AudioFactory.pushNewSFX("sfx/doorClose");
            AudioFactory.pushNewSFX("sfx/unlockDoor");
            AudioFactory.pushNewSFX("sfx/getHealth");
            AudioFactory.pushNewSFX("sfx/fanfare");
            AudioFactory.pushNewSFX("sfx/getHurt");
            AudioFactory.pushNewSFX("sfx/hurtGoomba");
            AudioFactory.pushNewSFX("sfx/wopleyDeath");
            AudioFactory.pushNewSFX("sfx/firework");
            AudioFactory.pushNewSFX("sfx/firework2");
            AudioFactory.pushNewSFX("sfx/firework3");

#if XBOX
            Thread.Sleep(2000);
#endif

            assetsLoaded = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            AudioFactory.stopSong();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //only use this for Windows testing now
#if WINDOWS
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                exitGame = true;
            }
#endif
            onScreenSaveSpin += gameTime.ElapsedGameTime.Milliseconds;

            // Allows the game to exit
            if (exitGame)
            {
                this.Exit();
            }

            gameInput.update();

            screenManager.CurrentNode.update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            screenManager.CurrentNode.draw(spriteBatch);

            if (SaveGameModule.TouchingStorageDevice)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(Game1.globalTestWalrus, new Vector2(560, 380), null, Color.White, (float)Math.Sin(onScreenSaveSpin / 200), new Vector2(16, 16), 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        public static void resetSaveData()
        {
            MagnetBoySaveData.loaded = true;
            MagnetBoySaveData.furthestLevelUnlocked = 0;
            MagnetBoySaveData.defaultStartingHealth = 5;
            MagnetBoySaveData.showInGameTimer = false;
            MagnetBoySaveData.showInGameTopTime = false;

            for (int i = 0; i < NumberOfLevels; i++)
            {
                switch (i)
                {
                    case 0:
                        MagnetBoySaveData.setLevelRecord(i, 120 * 1000, "Dan");
                        break;
                    case 1:
                        MagnetBoySaveData.setLevelRecord(i, 400 * 1000, "Wilson");
                        break;
                    case 2:
                        MagnetBoySaveData.setLevelRecord(i, 350 * 1000, "Eric");
                        break;
                    case 3:
                        MagnetBoySaveData.setLevelRecord(i, 1000 * 1000, "Wopley");
                        break;
                    case 4:
                        MagnetBoySaveData.setLevelRecord(i, 1200 * 1000, "Zippy");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
