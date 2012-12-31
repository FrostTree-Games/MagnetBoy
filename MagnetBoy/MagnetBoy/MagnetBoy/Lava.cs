using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class Lava : Entity
    {
        public Lava(float initialx, float initialy)
        {
            creation();

            horizontal_pos = initialx;
            vertical_pos = initialy;

            width = 32f;
            height = 32f;

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
        }

        public override void update(GameTime currentTime)
        {
            foreach (Entity en in Entity.globalEntityList)
            {
                if (en is Player)
                {
                    if (hitTest(en))
                    {
                        if (en.CenterPosition.X - CenterPosition.X < 0)
                        {
                            ((Player)en).knockBack(new Vector2(-1, -5), currentTime.TotalGameTime.TotalMilliseconds);
                        }
                        else
                        {
                            ((Player)en).knockBack(new Vector2(1, -5), currentTime.TotalGameTime.TotalMilliseconds);
                        }
                    }
                }
            }
        }

        public override void draw(SpriteBatch sb)
        {
            base.draw(sb);
        }
    }
}
