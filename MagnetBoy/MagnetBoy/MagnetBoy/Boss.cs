using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class Boss : Entity
    {
        protected int currentFrame = 0;
        protected double lastFrameIncrement = 0;

        public bool dying = false;

        private BulletPool.BulletType organBullet;

        public Boss()
        {
            creation();

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
        }

        public Boss(float initialx, float initialy)
        {
            creation();

            horizontal_pos = initialx;
            vertical_pos = initialy;

            width = 160;
            height = 160;

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;

            acceleration.Y = 0.001f;

            pole = Polarity.Neutral;
        }

        public override void update(GameTime currentTime)
        {
            return;
        }

        public override void draw(SpriteBatch sb)
        {
            sb.Draw(Game1.globalTestWalrus, new Vector2(horizontal_pos, vertical_pos), Color.Yellow);

            return;
        }

        private void walk()
        {
            if (walkSwitchTimer == 0)
            {
                walkSwitchTimer = currentTime.TotalGameTime.TotalMilliseconds;
            }

            if (parent.onTheGround)
            {
                if (Math.Abs(parent.velocity.X) < 0.01f)
                {
                    walkingLeft = !walkingLeft;
                }

                if (walkingLeft && parent.velocity.X > -walkerSpeed)
                {
                    parent.acceleration.X = -0.001f;
                }
                else if (parent.velocity.X < walkerSpeed)
                {
                    parent.acceleration.X = 0.001f;
                }
                else if (parent.velocity.X < -walkerSpeed)
                {
                    parent.velocity.X = -walkerSpeed;
                }
                else if (parent.velocity.X > walkerSpeed)
                {
                    parent.velocity.X = walkerSpeed;

                }
            }
        }
    }

    private class bossShield : Entity
    {
        protected int currentFrame = 0;
        protected double lastFrameIncrement = 0;

        public bossShield()
        {
            creation();

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
        }

        public bossShield(float initialx, float initialy)
        {
            creation();

            horizontal_pos = initialx;
            vertical_pos = initialy;
        }
    }
}
