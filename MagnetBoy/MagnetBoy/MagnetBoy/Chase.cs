using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FuncWorks.XNA.XTiled;

namespace MagnetBoy
{
    class Chase : Attribute
    {
        double walkSwitchTimer = 0;
        bool walkingLeft = false;
        const float walkerSpeed = 0.09f;

        public Chase(Enemy parent)
        {
        }

        public void update(Enemy parent, GameTime currentTime)
        {
            if (walkSwitchTimer == 0)
            {
                walkSwitchTimer = currentTime.TotalGameTime.TotalMilliseconds;
            }

            foreach (Entity en in Entity.globalEntityList)
            {
                if (en is Player)
                {
                    if (parent.hitTest(en))
                    {
                        if (en.Position.X - parent.Position.X < 0)
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
            if (parent.onTheGround)
            {
                foreach (Entity en in Entity.globalEntityList)
                {
                    if (en is Player)
                    {
                        if (en.horizontal_pos - parent.horizontal_pos > -20 && en.horizontal_pos - parent.horizontal_pos < 0)
                        {
                            if (parent.velocity.X > 0)
                            {
                                
                            }
                        }
                    }
                    else
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
        }
    }
}
