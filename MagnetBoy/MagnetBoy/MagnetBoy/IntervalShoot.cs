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

        private double timeSinceLastShot;

        public IntervalShoot(Enemy parent, double newInterval, BulletPool.BulletType type)
        {
            interval = newInterval;
            bulletType = type;

            timeSinceLastShot = 0;
        }

        public void update(Enemy parent, GameTime currentTime)
        {
            timeSinceLastShot += currentTime.ElapsedGameTime.Milliseconds;

            if (timeSinceLastShot > interval)
            {
                float direction = 0.0f;

                if (parent.velocity.X < 0.0f)
                {
                    direction += (float)(Math.PI);
                }

                BulletPool.pushBullet(bulletType, parent.Position.X, parent.Position.Y, currentTime, direction);

                timeSinceLastShot = 0;
            }
        }
    }
}
