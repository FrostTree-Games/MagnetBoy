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
    class climbWall : Entity
    {
        public climbWall(float initialx, float initialy)
        {
            creation();

            width = 1.0f;
            height = 31.5f;

            horizontal_pos = initialx;
            vertical_pos = initialy;
        }

        public override void update(GameTime currentTime)
        {
            return;
        }

        public virtual void enemyUpdate(GameTime currentTime)
        {
            return;
        }

        public override void draw(SpriteBatch sb)
        {
            //sb.Draw(Game1.globalTestWalrus, new Vector2(horizontal_pos, vertical_pos), Color.Blue);
        }
    }


}