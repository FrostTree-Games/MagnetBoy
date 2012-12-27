using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/* The GameScreenNode class is used to contain and organize various "scenes"
 * of the game. A scene is meant to represent a distinct area of content, such
 * as a title screen, level select, ingame level, etc.
 */

namespace MagnetBoy
{
    abstract class GameScreenNode
    {
        private bool isUpdateable = true;
        public bool IsUpdateable
        {
            get
            {
                return isUpdateable;
            }
            set
            {
                isUpdateable = value;
            }
        }


    }
}
