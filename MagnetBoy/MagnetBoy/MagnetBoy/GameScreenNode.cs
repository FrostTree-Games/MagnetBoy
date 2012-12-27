using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/* The GameScreenNode class is used to contain and organize various "scenes"
 * of the game. A scene is meant to represent a distinct area of content, such
 * as a title screen, level select, ingame level, etc.
 */

namespace MagnetBoy
{
    public abstract class GameScreenNode
    {
        public bool IsUpdateable { get; set; }

        public virtual void update(GameTime currentTime)
        {
            if (!IsUpdateable)
            {
                return;
            }

            doUpdate(currentTime);
        }

        protected abstract void doUpdate(GameTime currentTime);

        public abstract void draw();
    }

    public interface IState : GameScreenNode
    {
        public String StateName;
    }

    public interface ITransition : GameScreenNode
    {
        public IState OldState;
        public IState NewState;

        public void enter();
        public void exit();
    }
}
