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

        float horizontal_pos = 0.0f;
        float vertical_pos = 0.0f;

        float width = 0.0f;
        float height = 0.0f;

        public Entity(float initialx, float initialy)
        {
            horizontal_pos = initialx;
            vertical_pos = initialy;
        }

        public virtual void update(GameTime currentTime)
        {
            return;
        }

        public virtual void draw(SpriteBatch sb)
        {
            return;
        }
    }
}
