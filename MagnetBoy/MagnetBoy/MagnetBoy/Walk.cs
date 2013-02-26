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
    class WalkMarker : Entity
    {
        public WalkMarker(float newX, float newY)
        {
            creation();

            horizontal_pos = newX + 15;
            vertical_pos = newY;
            width = 2.0f;
            height = 32.0f;
        }

        public override void update(GameTime currentTime)
        {
            //
        }

        public override void draw(SpriteBatch sb)
        {
            //base.draw(sb);
        }
    }

    class Walk : Attribute
    {
        double walkSwitchTimer = 0;
        bool walkingLeft = false;
        const float walkerSpeed = 0.09f;

        private bool onWalkMarker;

        private bool isEnabled = true;

        public Walk(Enemy parent)
        {
            onWalkMarker = false;
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
                    //parent.velocity.X = -walkerSpeed;
                }
                else if (parent.velocity.X > walkerSpeed)
                {
                    //parent.velocity.X = walkerSpeed;
                }
            }

            bool touchingWalkMarker = false;
            foreach (Entity en in Entity.globalEntityList)
            {
                if (en is WalkMarker)
                {
                    if (parent.hitTest(en))
                    {
                        Entity player = null;
                        foreach (Entity p in Entity.globalEntityList)
                        {
                            if (p is Player)
                            {
                                player = p;
                            }
                        }

                        touchingWalkMarker = true;

                        if (!onWalkMarker && parent.PushTime <= 0)
                        {
                            onWalkMarker = true;

                            parent.velocity.X *= -1;
                        }
                    }
                }
            }

            if (!touchingWalkMarker && onWalkMarker)
            {
                onWalkMarker = false;
            }
        }
    }
}
