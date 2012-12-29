using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class EndLevelFlag : Entity
    {
        public EndLevelFlag(float initialx, float initialy)
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
            bool endLevel = false;

            foreach (Entity en in Entity.globalEntityList)
            {
                if (en is Player)
                {
                    if (hitTest(en))
                    {
                        LevelState.EndLevelFlag = true;
                    }
                }
            }
        }

        public override void draw(SpriteBatch sb)
        {
            sb.Draw(Game1.globalTestWalrus, Position, Color.Black);
        }
    }
}
