using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MagnetBoy
{
    class Jump : Attribute
    {
        const float walkerSpeed = 0.09f;

        private bool isEnabled = false;

        public Jump(Enemy parent)
        {
        }

        public void enableDisable(bool value)
        {
            isEnabled = value;
        }

        public void update(Enemy parent, GameTime currentTime)
        {
            if (parent.onTheGround)
            {
                if (Game1.gameRandom.Next() % 30 == 0)
                {
                    parent.velocity.Y = -0.4f;
                }
            }

        }
    }
}
