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
    class Entity
    {
        //sexy class data for our game entity goes here

        virtual void update(GameTime currentTime)
        {
            return;
        }

        virtual void draw(SpriteBatch sb)
        {
            return;
        }
    }
}
