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
    class Player: Entity
    {
        public Player()
        {
            horizontal_pos = 0.0f;
            vertical_pos = 0.0f;

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
        }

        public Player(float initialx, float initialy)
        {
            horizontal_pos = initialx;
            vertical_pos = initialy;

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
        }

        public override void update(GameTime currentTime)
        {
            double delta = currentTime.ElapsedGameTime.Milliseconds;
            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.Right))
            {
                velocity.X = 0.1f;
            }
            else if (ks.IsKeyDown(Keys.Left))
            {
                velocity.X = -0.1f;
            }
            else
            {
                velocity.X = 0.0f;
            }

            if (ks.IsKeyDown(Keys.Up))
            {
                velocity.Y = -0.1f;
            }
            else if (ks.IsKeyDown(Keys.Down))
            {
                velocity.Y = 0.1f;
            }
            else
            {
                velocity.Y = 0.0f;
            }
                
            horizontal_pos += (float)(((velocity.X)*delta) + (0.5)*(Math.Pow(delta,2.0))*acceleration.X);
            vertical_pos += (float)(((velocity.Y) * delta) + (0.5) * (Math.Pow(delta, 2.0)) * acceleration.Y);
        }

        public override void draw(SpriteBatch sb)
        {
            sb.Draw(Game1.globalTestWalrus, new Vector2(horizontal_pos, vertical_pos), Color.Yellow);
        }
    }
}
