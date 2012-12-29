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

        public enum GameScreenType
        {
            TitleScreen,
            Menu,
            Level,
            Credits
        }

        public enum TransitionType
        {
            NoTransition,
        }

        private static GameScreenNode currentNode = null;
        public GameScreenNode CurrentNode
        {
            get
            {
                return currentNode;
            }
        }

        public GameScreenManager(ContentManager newManager)
        {
            if (manager == null)
            {
                manager = newManager;
            }
        }

        public static void switchScreens(GameScreenType newType, string levelName)
        {
            Console.WriteLine("ping!");

            if (manager == null)
            {
                return;
            }

            if (newType == GameScreenType.Level && levelName != null)
            {
                currentNode = new LevelState(manager, levelName);
            }
            else if (newType == GameScreenType.Menu)
            {
                //
            }
            else
            {
                //
            }
        }

    }
}
