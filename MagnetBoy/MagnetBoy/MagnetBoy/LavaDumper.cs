using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class LavaDumper : Entity
    {
        private double interval = 2000; //milliseconds
        private double timeSinceLastInterval = 0;

        public LavaDumper(float initalX, float initalY)
        {
            horizontal_pos = initalX;
            vertical_pos = initalY;
        }

        public override void update(GameTime currentTime)
        {
            timeSinceLastInterval += currentTime.ElapsedGameTime.Milliseconds;

            if (timeSinceLastInterval > interval)
            {
                BulletPool.pushBullet(BulletPool.BulletType.LavaBlob, horizontal_pos, vertical_pos, currentTime, 0.0f);

                timeSinceLastInterval = 0;
            }
        }

        public override void draw(SpriteBatch sb)
        {
            base.draw(sb);
        }
    }
}
