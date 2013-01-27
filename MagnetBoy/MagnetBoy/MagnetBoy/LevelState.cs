using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
        private Camera levelCamera = null;
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

        private bool fadingOut = false;
        private double fadingOutTimer = 0;
        private const double fadingOutDuration = 1000;

        private string[] loadingText = {"L", "O", "A", "D", "I", "N", "G", ".", ".", "."};
        private double loadingAnimTimePassed;
        private const double loadingAnimTime = 2000;
        private int magnetWopleyFrame;
        private double magnetWopleyLastUpdateTime;
        private string magnetWopleyAnim = "playerWalkRight";

        private int chaserFrame;
        private double chaserLastUpdateTime;
        private string chaserAnim = "angrySawWalkLeft";

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

            GameInput.LockMostRecentPad = true;

            GC.Collect();
        }

        private void loadLevelThread()
        {
            /* add these lines when compiling for Xbox 360; sets thread explicity to extra core
                #ifdef XBOX
                Thread.SetProcessorAffinity(3); 
                #endif
             */
            Monitor.Enter(levelEntities);
            grabbed = true;

            levelMap = contentManager.Load<Map>(levelName);

            foreach (ObjectLayer layer in levelMap.ObjectLayers)
            {
                foreach (MapObject obj in layer.MapObjects)
                {
                    Entity en = null;

                    switch (obj.Name)
                    {
                        case "player":
                            en = new Player(obj.Bounds.X, obj.Bounds.Y);
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
                            levelEntities.Add(new LavaDumper(obj.Bounds.X, obj.Bounds.Y));
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
                        case"bossShield":
                            levelEntities.Add(new bossShield(obj.Bounds.X, obj.Bounds.Y));
                            break;
                        default:
                            break;
                    }
                }
            }

            //Thread.Sleep(5000);
            
            //assetResources.Release();
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
            //assetResources.Release();

            gameInput.update();

            if (!musicPlaying)
            {
                musicPlaying = true;

                AudioFactory.playSong("songs/song1");
            }

            if (currentPlayerHealth < 1 && !fadingOut)
            {
                AudioFactory.stopSong();

                AudioFactory.playSFX("sfx/lose");

                fadingOut = true;
            }

            if (fadingOut)
            {
                fadingOutTimer += currentTime.ElapsedGameTime.Milliseconds;

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

                //GameScreenManager.switchScreens(GameScreenManager.GameScreenType.Menu, "TitleScreenMenu");
                GameScreenManager.nextLevel();

                endLevelFlag = false;

                GameInput.LockMostRecentPad = false;

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

            Vector2 loadPos = new Vector2(350, 300);
            Vector2 wopleyPos = new Vector2(300, 300);
            Vector2 chaserPos = new Vector2(500, 300);

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
            AnimationFactory.drawAnimationFrame(spriteBatch, "testParallax3", 0, new Vector2(-1 * (levelCamera.getFocusPosition().X / levelMap.Bounds.Width) * (float)AnimationFactory.getAnimationFrameWidth("testParallax3"), 0f), AnimationFactory.DepthLayer3);
            AnimationFactory.drawAnimationFrame(spriteBatch, "testParallax3", 0, new Vector2((-1 * (levelCamera.getFocusPosition().X / levelMap.Bounds.Width) * (float)AnimationFactory.getAnimationFrameWidth("testParallax3")) + (float)AnimationFactory.getAnimationFrameWidth("testParallax3"), 0f), AnimationFactory.DepthLayer3);

            AnimationFactory.drawAnimationFrame(spriteBatch, "testParallax2", 0, new Vector2(-2 * (levelCamera.getFocusPosition().X / levelMap.Bounds.Width) * (float)AnimationFactory.getAnimationFrameWidth("testParallax2"), 0f), AnimationFactory.DepthLayer2);
            AnimationFactory.drawAnimationFrame(spriteBatch, "testParallax2", 0, new Vector2((-2 * (levelCamera.getFocusPosition().X / levelMap.Bounds.Width) * (float)AnimationFactory.getAnimationFrameWidth("testParallax2")) + (float)AnimationFactory.getAnimationFrameWidth("testParallax2"), 0f), AnimationFactory.DepthLayer2);
            AnimationFactory.drawAnimationFrame(spriteBatch, "testParallax2", 0, new Vector2((-2 * (levelCamera.getFocusPosition().X / levelMap.Bounds.Width) * (float)AnimationFactory.getAnimationFrameWidth("testParallax2")) + (float)(2f* AnimationFactory.getAnimationFrameWidth("testParallax2")), 0f), AnimationFactory.DepthLayer2);

            AnimationFactory.drawAnimationFrame(spriteBatch, "testParallax1", 0, new Vector2(-3 * (levelCamera.getFocusPosition().X / levelMap.Bounds.Width) * (float)AnimationFactory.getAnimationFrameWidth("testParallax1"), 0f), AnimationFactory.DepthLayer1);
            AnimationFactory.drawAnimationFrame(spriteBatch, "testParallax1", 0, new Vector2((-3 * (levelCamera.getFocusPosition().X / levelMap.Bounds.Width) * (float)AnimationFactory.getAnimationFrameWidth("testParallax1")) + (float)AnimationFactory.getAnimationFrameWidth("testParallax1"), 0f), AnimationFactory.DepthLayer1);
            AnimationFactory.drawAnimationFrame(spriteBatch, "testParallax1", 0, new Vector2((-3 * (levelCamera.getFocusPosition().X / levelMap.Bounds.Width) * (float)AnimationFactory.getAnimationFrameWidth("testParallax1")) + (float)(2f * AnimationFactory.getAnimationFrameWidth("testParallax1")), 0f), AnimationFactory.DepthLayer1);
            AnimationFactory.drawAnimationFrame(spriteBatch, "testParallax1", 0, new Vector2((-3 * (levelCamera.getFocusPosition().X / levelMap.Bounds.Width) * (float)AnimationFactory.getAnimationFrameWidth("testParallax1")) + (float)(3f * AnimationFactory.getAnimationFrameWidth("testParallax1")), 0f), AnimationFactory.DepthLayer1);
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

            //spriteBatch.DrawString(Game1.gameFontText, "Stamina: " + LevelState.playerStamina, new Vector2(32, 16), Color.White);
            AnimationFactory.drawAnimationFrame(spriteBatch, "gui_angledBoxB", 1, new Vector2(108, 76), new Vector2(10.0f * (playerStamina / playerStaminaMax), 1.0f), Color.Lerp(Color.DarkBlue, Color.LightBlue, (playerStamina / playerStaminaMax)), AnimationFactory.DepthLayer1);
            AnimationFactory.drawAnimationFrame(spriteBatch, "gui_angledBoxB", 1, new Vector2(108, 76), new Vector2(10.0f, 1.0f), Color.Black, AnimationFactory.DepthLayer2);
            spriteBatch.End();

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
                Matrix stretch = Matrix.CreateScale((float)(Game1.mapView.Width), (float)((fadingOutTimer / fadingOutDuration) * (Game1.mapView.Height)), 1.0f);

                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, stretch);
                spriteBatch.Draw(Game1.globalBlackPixel, Vector2.Zero, Color.White);
                spriteBatch.End();
            }
        }
    }
}
