using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FuncWorks.XNA.XTiled;

namespace MagnetBoy
{
    class WallMagnet : Entity
    {
        public WallMagnet(float initialx, float initialy, Polarity newPolarity)
        {
            pole = newPolarity;

            magneticMoment = 1.5f;

            horizontal_pos = initialx;
            vertical_pos = initialy;
        }

        public override void update(GameTime currentTime)
        {
            //
        }

        public override void draw(SpriteBatch sb)
        {
            if (pole == Polarity.Positive)
            {
                sb.Draw(Game1.globalTestPositive, new Vector2(horizontal_pos, vertical_pos), Color.White);
            }
            else if (pole == Polarity.Negative)
            {
                sb.Draw(Game1.globalTestNegative, new Vector2(horizontal_pos, vertical_pos), Color.White);
            }
            else
            {
                sb.Draw(Game1.globalTestWalrus, new Vector2(horizontal_pos, vertical_pos), Color.White);
            }
        }
    }
}
