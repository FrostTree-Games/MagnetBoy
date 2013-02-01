using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MagnetBoy
{
    class Spikes : Entity
    {
        public Spikes(float initialx, float initialy)
        {
            creation();

            horizontal_pos = initialx;
            vertical_pos = initialy;

            solid = false;
        }

        public override void update(GameTime currentTime)
        {
            foreach (Entity en in globalEntityList)
            {
                if (en is Player)
                {
                    if (hitTest(en) && en.Position.Y < vertical_pos && en.velocity.Y > 0)
                    {
                        ((Player)en).knockBack(new Vector2(en.Position.X - horizontal_pos, -5), currentTime.TotalGameTime.TotalMilliseconds);
                    }
                }
                if (en is Enemy)
                {
                    if (hitTest(en) && en.Position.Y < vertical_pos && en.velocity.Y > 0)
                    {
                        en.deathAnimation = true;
                    }
                }
            }
        }

        public override void draw(SpriteBatch sb)
        {
            AnimationFactory.drawAnimationFrame(sb, "spikes", 0, Position, AnimationFactory.DepthLayer2);
        }
    }
}
