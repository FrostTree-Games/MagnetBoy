using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MagnetBoy
{
    class IntervalShoot : Attribute
    {
        private double interval = 0;
        private BulletPool.BulletType bulletType;

        private bool isEnabled = true;

        private double timeSinceLastShot;

        public IntervalShoot(Enemy parent, double newInterval, BulletPool.BulletType type)
        {
            interval = newInterval;
            bulletType = type;

            timeSinceLastShot = 0;
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

            timeSinceLastShot += currentTime.ElapsedGameTime.Milliseconds;

            if (timeSinceLastShot > interval)
            {
                float direction = 0.0f;

                if (parent.velocity.X < 0.0f)
                {
                    direction += (float)(Math.PI);
                }

                Vector2 bulletPosition = parent.Position;

                if (parent is Lolrus)
                {
                    bulletPosition.Y += 4;

                    if (parent.velocity.X >= 0.0f)
                    {
                        bulletPosition.X += parent.HitBox.X;
                    }
                }

                BulletPool.pushBullet(bulletType, bulletPosition.X, bulletPosition.Y, currentTime, direction);

                timeSinceLastShot = 0;

                if (parent is Lolrus)
                {
                    ((Lolrus)parent).lolrusFire();
                }
            }
        }
    }
}
