using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class GameScreenManager
    {
        private static ContentManager manager = null;

        private static GameScreenNode currentNode = null;
        public GameScreenNode CurrentNode
        {
            get
            {
                return currentNode;
            }
        }

        public enum GameScreenType
        {
            SplashScreen,
            Menu,
            Level,
            Credits
        }

        public enum TransitionType
        {
            NoTransition,
        }

        public GameScreenManager(ContentManager newManager)
        {
            if (manager == null)
            {
                manager = newManager;
            }
        }

        public static void nextLevel()
        {
            if (manager == null)
            {
                return;
            }

            if (!(currentNode is LevelState))
            {
                return;
            }

            Game1.CurrentLevel = (Game1.CurrentLevel + 1) % Game1.levelNames.Length;

            if (Game1.CurrentLevel > Game1.MagnetBoySaveData.furthestLevelUnlocked)
            {
                Game1.MagnetBoySaveData.furthestLevelUnlocked = Game1.CurrentLevel;
            }

            if (Game1.CurrentLevel != 0)
            {
                currentNode = new LevelState(manager, Game1.levelFileNames[Game1.CurrentLevel]);
            }
            else
            {
                currentNode = new TitleScreenMenuState(manager, false, false);
            }
        }

        public static void switchScreens(GameScreenType newType, string levelName)
        {
            if (manager == null)
            {
                return;
            }

            if (newType == GameScreenType.SplashScreen)
            {
                currentNode = new SplashScreenState(manager);
            }
            else if (newType == GameScreenType.Credits)
            {
                currentNode = new CreditsScreenState(manager);
            }
            else if (newType == GameScreenType.Level && levelName != null)
            {
                currentNode = new LevelState(manager, levelName);
            }
            else if (newType == GameScreenType.Menu)
            {
                switch (levelName)
                {
                    case "TitleScreenMenu":
                        currentNode = new TitleScreenMenuState(manager, false, true);
                        break;
                    case "TitleScreenMenu_fromOptions":
                        currentNode = new TitleScreenMenuState(manager, true, false);
                        break;
                    case "TitleScreenMenu_fromLevelSelect":
                        currentNode = new TitleScreenMenuState(manager, true, false);
                        break;
                    case "TitleScreenMenu_fromPause":
                        currentNode = new TitleScreenMenuState(manager, false, true);
                        ((TitleScreenMenuState)currentNode).showPressButtonDialog = false;
                        break;
                    case "GameOptionsMenu":
                        currentNode = new OptionsMenuState(manager);
                        break;
                    case "LevelSelectMenu":
                        currentNode = new LevelSelectMenuState(manager);
                        break;
                    case "BetaMenu":
                        currentNode = new BetaMenuState(manager);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                //
            }
        }

    }
}
