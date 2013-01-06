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
        bool chasePlayer = false;

        private bool isEnabled = true;

        private Walk walkState = null;

        public Chase(Enemy parent)
        {
            walkState = new Walk(parent);
        }

        public void enableDisable(bool value)
        {
            isEnabled = value;
        }

        public void update(Enemy parent, GameTime currentTime)
        {
            if (!isEnabled)
            {
                return;
            }

            if (walkSwitchTimer == 0)
            {
                walkSwitchTimer = currentTime.TotalGameTime.TotalMilliseconds;
            }

            if (parent.onTheGround)
            {
                foreach (Entity en in Entity.globalEntityList)
                {
                    if (en is Player)
                    {
                        if (en.horizontal_pos - parent.horizontal_pos > -100 && en.horizontal_pos - parent.horizontal_pos < 100 && en.vertical_pos - parent.vertical_pos > -5 && en.vertical_pos - parent.vertical_pos < 5)
                        {
                            chasePlayer = true;
                        }
                        else
                        {
                            chasePlayer = false;
                        }

                        if (chasePlayer == true)
                        {
                            if (en.horizontal_pos - parent.horizontal_pos > -500 && en.horizontal_pos - parent.horizontal_pos < -30)
                            {

                                if (walkingLeft == false)
                                {
                                    walkingLeft = true;
                                }

                                parent.velocity.X = -2 * walkerSpeed;
                            }
                            else if (en.horizontal_pos - parent.horizontal_pos > -30 && en.horizontal_pos - parent.horizontal_pos < 30)
                            {
                                parent.velocity.X = 0.0f;
                            }
                            else if (en.horizontal_pos - parent.horizontal_pos > 30 && en.horizontal_pos - parent.horizontal_pos < 500)
                            {
                                if (walkingLeft == true)
                                {
                                    walkingLeft = false;
                                }

                                parent.velocity.X = 2 * walkerSpeed;
                            }
                        }
                    }
                }
               if( chasePlayer == false)
               {
                   walkState.update(parent, currentTime);
               }
            }
        }
    }
}
