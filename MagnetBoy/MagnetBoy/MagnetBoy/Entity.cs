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

        protected float horizontal_pos = 0.0f;
        protected float vertical_pos = 0.0f;

        protected float width = 0.0f;
        protected float height = 0.0f;

        protected Vector2 velocity;
        protected Vector2 acceleration;
        
        public Entity()
        {
            horizontal_pos = 0.0f;
            vertical_pos = 0.0f;
        }
        
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
            sb.Draw(Game1.globalTestWalrus, new Vector2(horizontal_pos, vertical_pos), Color.White);
        }
    }
}
