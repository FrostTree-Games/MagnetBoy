using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private List<Entity> levelEntities = null;
        private Camera levelCamera = null;
        private BulletPool levelBulletPool = null;

        private static Map levelMap = null;

        public static float playerStamina = 100.0f;

        private static int maxPlayerHealth = 7;
        public static int currentPlayerHealth = 7;

        private Texture2D backgroundTile = null;
        float backgroundDeltaX = 0.0f;
        float backgroundDeltaY = 0.0f;

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

        public LevelState(ContentManager newManager, string levelName)
        {
            EndLevelFlag = false;

            contentManager = newManager;

            gameInput = new GameInput(null);
            levelEntities = new List<Entity>();
            levelCamera = new Camera();
            levelBulletPool = new BulletPool();

            levelMap = newManager.Load<Map>(levelName);

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
                        default:
                            break;
                    }
                }
            }

            backgroundTile = contentManager.Load<Texture2D>("hackTile");
            backgroundDeltaX = 0.0f;
            backgroundDeltaY = 0.0f;

            IsUpdateable = true;

            GC.Collect();
        }

        protected override void doUpdate(GameTime currentTime)
        {
            gameInput.update();

            if (!musicPlaying)
            {
                musicPlaying = true;

                AudioFactory.playSong("songs/song1");
            }

            foreach (Entity en in levelEntities)
            {
                en.update(currentTime);

                if (endLevelFlag == true)
                {
                    break;
                }
            }

            if (endLevelFlag == true)
            {
                foreach (Entity en in levelEntities)
                {
                    en.death();
                }

                levelBulletPool.clearPool();

                GameScreenManager.switchScreens(GameScreenManager.GameScreenType.Menu, "BetaMenu");

                return;
            }

            levelBulletPool.updatePool(currentTime);

            backgroundDeltaX = 0.5f * levelCamera.getFocusPosition().X % backgroundTile.Bounds.Width;
            backgroundDeltaY = 0.25f * levelCamera.getFocusPosition().Y % backgroundTile.Bounds.Height;
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

        public override void draw(SpriteBatch spriteBatch)
        {
            Matrix mx = new Matrix();
            Rectangle rx = new Rectangle();
            levelCamera.getDrawTranslation(ref mx, ref Game1.mapView, ref levelMap);
            levelCamera.getDrawRectangle(ref rx, ref Game1.mapView, ref levelMap);

            Game1.graphics.GraphicsDevice.Clear(Color.Lerp(Color.DarkGray, Color.Black, 0.4f));

            spriteBatch.Begin();
            for (int i = -5; i < (Game1.mapView.Width / backgroundTile.Bounds.Width) + 5; i++)
            {
                for (int j = -5; j < (Game1.mapView.Height / backgroundTile.Bounds.Height) + 5; j++)
                {
                    spriteBatch.Draw(backgroundTile, new Vector2((i * backgroundTile.Bounds.Width) - backgroundDeltaX, (j * backgroundTile.Bounds.Height) + backgroundDeltaY), Color.White);
                }
            }

            spriteBatch.End();

            // draw map
            spriteBatch.Begin();
            levelMap.Draw(spriteBatch, rx);
            spriteBatch.End();

            // draw sprites
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, mx);
            foreach (Entity a in levelEntities)
            {
                a.draw(spriteBatch);
            }

            levelBulletPool.drawPool(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin();
            if (currentPlayerHealth <= maxPlayerHealth)
            {
                for (int i = 0; i < currentPlayerHealth; i++)
                {
                    spriteBatch.Draw(Game1.globalTestWalrus, new Vector2(i * 32, 50), Color.AntiqueWhite);
                }
            }
            spriteBatch.End();

            //draw game cursor
            Matrix arrowRotation = Matrix.Identity;
            arrowRotation = Matrix.Multiply(arrowRotation, Matrix.CreateTranslation(-16.0f, -16.0f, 0.0f));
            arrowRotation = Matrix.Multiply(arrowRotation, Matrix.CreateRotationZ(((float)(Math.PI / 2))));
            arrowRotation = Matrix.Multiply(arrowRotation, Matrix.CreateTranslation(GameInput.P1MouseDirection.Length(), 0.0f, 0.0f));
            arrowRotation = Matrix.Multiply(arrowRotation, Matrix.CreateRotationZ((float)Math.Atan2(GameInput.P1MouseDirectionNormal.Y, GameInput.P1MouseDirectionNormal.X)));
            arrowRotation = Matrix.Multiply(arrowRotation, Matrix.CreateTranslation(Game1.graphics.GraphicsDevice.Viewport.Bounds.Width / 2, Game1.graphics.GraphicsDevice.Viewport.Bounds.Height / 2, 0.0f));

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, arrowRotation);
            AnimationFactory.drawAnimationFrame(spriteBatch, "mouseArrow", 0, Vector2.Zero);
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.DrawString(Game1.gameFontText, "Stamina: " + LevelState.playerStamina, new Vector2(32, 16), Color.White);
            spriteBatch.End();
        }
    }
}
