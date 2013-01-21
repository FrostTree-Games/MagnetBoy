using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class BetaMenuState : IState
    {
        private class LevelMenuOption
        {
            public string levelName;

            public LevelMenuOption(string newName)
            {
                levelName = newName;
            }
        }

        private ContentManager contentManager = null;

        private int menuOption = 0;
        private List<LevelMenuOption> levelList = null;

        private GameInput gameInput = null;

        private bool musicPlaying = false;

        private bool downPressed = false;
        private bool upPressed = false;

        public BetaMenuState(ContentManager newManager)
        {
            IsUpdateable = true;

            contentManager = newManager;

            gameInput = new GameInput(Game1.graphics.GraphicsDevice);

            levelList = new List<LevelMenuOption>();

            musicPlaying = false;

            levelList.Add(new LevelMenuOption("testMap1"));
            levelList.Add(new LevelMenuOption("testMap2"));
            levelList.Add(new LevelMenuOption("climbTest"));
            levelList.Add(new LevelMenuOption("theLab"));
            levelList.Add(new LevelMenuOption("theLab2"));
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
                menuOption = (menuOption + 1) % (levelList.Count);
                downPressed = false;

                AudioFactory.playSFX("sfx/menu");
            }

            if (GameInput.isButtonDown(GameInput.PlayerButton.UpDirection))
            {
                upPressed = true;
            }
            else if (upPressed == true)
            {
                menuOption--;
                if (menuOption < 0)
                {
                    menuOption = levelList.Count - 1;
                }
                upPressed = false;

                AudioFactory.playSFX("sfx/menu");
            }

            if (GameInput.isButtonDown(GameInput.PlayerButton.Confirm))
            {
                GameScreenManager.switchScreens(GameScreenManager.GameScreenType.Level, levelList[menuOption].levelName);
            }
        }

        public override void draw(SpriteBatch sb)
        {
            Game1.graphics.GraphicsDevice.Clear(Color.Lerp(Color.DarkGray, Color.White, 0.4f));

            sb.Begin();
            for (int i = 0; i < levelList.Count; i++)
            {
                sb.DrawString(Game1.gameFontText, levelList[i].levelName, new Vector2(64, 64 * (i + 1)), Color.Black);
            }

            AnimationFactory.drawAnimationFrame(sb, "playerWalkRight", 0, new Vector2(32, 64 * (menuOption + 1)), AnimationFactory.DepthLayer0);
            sb.End();
        }
    }
}
