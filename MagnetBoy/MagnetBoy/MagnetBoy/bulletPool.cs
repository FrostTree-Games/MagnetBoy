using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class Bullet : Entity
    {
        private bool inUse = false;
        private double startTime = 0;
        private double maxLifeTime = 5000; 
        private double rotation = 0.0; // 0.0 in rotation is considered to be right-facing, or "EAST"

        public void add(float newX, float newY, BulletPool.BulletType type, GameTime entryTime)
        {
            inUse = true;
            startTime = entryTime.ElapsedGameTime.Milliseconds;

            horizontal_pos = newX;
            vertical_pos = newY;
        }

        public override void update(GameTime currentTime)
        {
            // bullet functionality

            if (currentTime.ElapsedGameTime.Milliseconds - startTime > maxLifeTime)
            {
                inUse = false;
            }
        }

        public override void draw(SpriteBatch sb)
        {
            //
        }
    }

    class BulletPool
    {
        public enum BulletType
        {
            Laser,
            Rocket
        }

        private Bullet[] pool = null;

        public BulletPool()
        {
            pool = new Bullet[15];
        }

        public void updatePool(GameTime currentTime)
        {
            //
        }

        public void pushBullet(GameTime currentTime)
        {
            //
        }
    }
}
