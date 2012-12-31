using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class Lolrus : Enemy
    {
        private bool shooting = false;
        private double shootingTime = 0;

        public Lolrus(int initalX, int initalY) : base(initalX, initalY)
        {
            list.Add(new Walk(this));
            list.Add(new IntervalShoot(this, 500, BulletPool.BulletType.TestBullet));
        }

        protected override void enemyUpdate(GameTime currentTime)
        {
            if (shooting)
            {
                velocity.X = 0.0f;

                shootingTime += currentTime.ElapsedGameTime.Milliseconds;

                if (shootingTime > AnimationFactory.getAnimationFrameCount(currentAnimation) * AnimationFactory.getAnimationSpeed(currentAnimation))
                {
                    shooting = false;

                    foreach (Attribute a in list)
                    {
                        if (a is Walk)
                        {
                            a.enableDisable(true);
                        }
                    }
                }
            }
        }

        public void lolrusFire()
        {
            shooting = true;
            shootingTime = 0;

            velocity.X = 0.0f;

            foreach (Attribute a in list)
            {
                if (a is Walk)
                {
                    a.enableDisable(false);
                }
            }
        }

        public override void draw(SpriteBatch sb)
        {
            base.draw(sb);
        }
    }
}
