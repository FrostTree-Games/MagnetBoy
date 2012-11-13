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
    class WalkingEnemy : Enemy
    {
        double walkSwitchTimer = 0;
        bool walkingLeft = false;

        const float walkerSpeed = 0.09f;
        
        public WalkingEnemy(float initialx, float initialy, Polarity newPole)
        {
            creation();

            horizontal_pos = initialx;
            vertical_pos = initialy;

            width = 31.5f;
            height = 31.5f;

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;

            acceleration.X = 0.1f;
            acceleration.Y = 0.001f;

            pole = newPole;
            magneticMoment = 0.5f;

            solid = false;
        }

        protected override void enemyUpdate(GameTime currentTime)
        {
            if (walkSwitchTimer == 0)
            {
                walkSwitchTimer = currentTime.TotalGameTime.TotalMilliseconds;
            }

            foreach (Entity en in globalEntityList)
            {
                if (en is Player)
                {
                    if (hitTest(en))
                    {
                        ((Player)en).knockBack(new Vector2(en.Position.X - horizontal_pos, en.Position.Y - vertical_pos), currentTime.TotalGameTime.TotalMilliseconds);
                    }
                }
            }

            if (onTheGround)
            {
                if (Math.Abs(velocity.X) < 0.01f)
                {
                    walkingLeft = !walkingLeft;
                }

                if (walkingLeft && velocity.X > -walkerSpeed)
                {
                    acceleration.X = -0.001f;
                }
                else if (velocity.X < walkerSpeed)
                {
                    acceleration.X = 0.001f;
                }
                else if (velocity.X < -walkerSpeed)
                {
                    velocity.X = -walkerSpeed;
                }
                else if (velocity.X > walkerSpeed)
                {
                    velocity.X = walkerSpeed;
                }
            }
        }
    }
}
