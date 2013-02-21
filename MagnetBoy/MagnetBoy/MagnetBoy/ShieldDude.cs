using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class ShieldDude : Enemy
    {
        private enum ShieldDudeState
        {
            DownShield,
            UpShield,
            MovingUp,
            MovingDown,
            Shooting
        }

        private ShieldDude.ShieldDudeState state;
        private double stateTimePassed;
        private bool facingRight;

        public Vector2 ShieldDir
        {
            get
            {
                float theta = 0.0f;

                if (state == ShieldDudeState.MovingUp || state == ShieldDudeState.MovingDown)
                {
                    theta = (float)(Math.PI / 4);
                }
                else if (state == ShieldDudeState.UpShield || state == ShieldDudeState.Shooting)
                {
                    theta = (float)(Math.PI / 2);
                }

                Vector2 dir = new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));

                if (!facingRight)
                {
                    dir.X *= -1;
                }

                return dir;
            }
        }

        public ShieldDude(float initalX, float initalY, bool isFacingRight) : base(initalX, initalY)
        {
            state = ShieldDudeState.DownShield;
            stateTimePassed = 0;
            facingRight = isFacingRight;

            if (facingRight)
            {
                currentAnimation = "shieldGuyIdleRight";
            }
            else
            {
                currentAnimation = "shieldGuyIdleLeft";
            }
        }

        protected override void enemyUpdate(GameTime currentTime)
        {
            if (onTheGround)
            {
                velocity.X *= 0.9f;
            }
            //if statements are chosen over a switch so the programmer may use scope if needed

            if (deathAnimation == true)
            {
                if (deathAnimationSet == false)
                {
                    if (velocity.X < 0.0)
                    {
                        lastFrameIncrement = 0.0;
                        currentFrame = 0;
                        velocity.X = 0.0f;
                        currentAnimation = "shieldGuyDieRight";
                        deathAnimationSet = true;
                    }
                    else
                    {
                        lastFrameIncrement = 0.0;
                        currentFrame = 0;
                        velocity.X = 0.0f;
                        currentAnimation = "shieldGuyDieLeft";
                        deathAnimationSet = true;
                    }
                }

                if (deathTimer > 1100)
                {
                    removeFromGame = true;
                    deathAnimationSet = false;
                    deathTimer = 0.0;
                }
                else
                {
                    deathTimer += currentTime.ElapsedGameTime.Milliseconds;
                }
            }

            else
            {
                if (state == ShieldDudeState.DownShield)
                {
                    stateTimePassed += currentTime.ElapsedGameTime.Milliseconds;

                    if (stateTimePassed > 3000)
                    {
                        stateTimePassed = 0;

                        state = ShieldDudeState.MovingUp;
                        currentFrame = 0;
                        lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;

                        if (facingRight)
                        {
                            currentAnimation = "shieldGuyRaiseShieldRight";
                        }
                        else
                        {
                            currentAnimation = "shieldGuyRaiseShieldLeft";
                        }
                    }

                    if (pushTime > 0 && Math.Abs(velocity.X) < 0.01f)
                    {
                        LevelState.levelParticlePool.pushParticle(ParticlePool.ParticleType.RedSpark, Position + new Vector2(facingRight ? width : 0, height / 2), Vector2.Zero, facingRight ? (float)((Math.PI/2) + (Game1.gameRandom.NextDouble() * Math.PI)) : (float)(-(Math.PI/2) + (Game1.gameRandom.NextDouble() * Math.PI)), 0.0f, Color.White);
                    }
                } //DownShield
                else if (state == ShieldDudeState.UpShield)
                {
                    stateTimePassed += currentTime.ElapsedGameTime.Milliseconds;

                    if (stateTimePassed > 10)
                    {
                        stateTimePassed = 0;

                        state = ShieldDudeState.Shooting;
                        currentFrame = 0;
                        lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;

                        if (facingRight)
                        {
                            currentAnimation = "shieldGuyShootRight";
                        }
                        else
                        {
                            currentAnimation = "shieldGuyShootLeft";
                        }
                    }
                } //UpShield
                else if (state == ShieldDudeState.MovingUp)
                {
                    stateTimePassed += currentTime.ElapsedGameTime.Milliseconds;

                    if (stateTimePassed >= AnimationFactory.getAnimationFrameCount(currentAnimation) * AnimationFactory.getAnimationSpeed(currentAnimation) - 50)
                    {
                        stateTimePassed = 0;

                        state = ShieldDudeState.UpShield;
                        currentFrame = 0;
                        lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;

                        if (facingRight)
                        {
                            currentAnimation = "shieldGuyRaisedIdleRight";
                        }
                        else
                        {
                            currentAnimation = "shieldGuyRaisedIdleLeft";
                        }
                    }
                } //MovingUp
                else if (state == ShieldDudeState.MovingDown)
                {
                    stateTimePassed += currentTime.ElapsedGameTime.Milliseconds;

                    if (stateTimePassed >= AnimationFactory.getAnimationFrameCount(currentAnimation) * AnimationFactory.getAnimationSpeed(currentAnimation) - 50)
                    {
                        stateTimePassed = 0;

                        state = ShieldDudeState.DownShield;
                        currentFrame = 0;
                        lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;

                        if (facingRight)
                        {
                            currentAnimation = "shieldGuyIdleRight";
                        }
                        else
                        {
                            currentAnimation = "shieldGuyIdleLeft";
                        }
                    }
                } //MovingDown
                else if (state == ShieldDudeState.Shooting)
                {
                    if (stateTimePassed == 0)
                    {
                        if (facingRight)
                        {
                            BulletPool.pushBullet(BulletPool.BulletType.Bucket, CenterPosition.X, CenterPosition.Y - 4, currentTime, 0.0f);
                        }
                        else
                        {
                            BulletPool.pushBullet(BulletPool.BulletType.Bucket, CenterPosition.X, CenterPosition.Y - 4, currentTime, (float)Math.PI);
                        }
                    }

                    stateTimePassed += currentTime.ElapsedGameTime.Milliseconds;

                    if (stateTimePassed >= AnimationFactory.getAnimationFrameCount(currentAnimation) * AnimationFactory.getAnimationSpeed(currentAnimation) - 50)
                    {
                        stateTimePassed = 0;

                        state = ShieldDudeState.MovingDown;
                        currentFrame = 0;
                        lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;

                        if (facingRight)
                        {
                            currentAnimation = "shieldGuyLowerShieldRight";
                        }
                        else
                        {
                            currentAnimation = "shieldGuyLowerShieldLeft";
                        }
                    }
                }
            }

            // if the last frame time hasn't been set, set it now
            if (lastFrameIncrement == 0)
            {
                lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;
            }

            // update the current frame if needed
            if (currentTime.TotalGameTime.TotalMilliseconds - lastFrameIncrement > AnimationFactory.getAnimationSpeed(currentAnimation))
            {
                lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;

                currentFrame = (currentFrame + 1) % AnimationFactory.getAnimationFrameCount(currentAnimation);
            }
        }

        public override void draw(SpriteBatch sb)
        {
            AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position, AnimationFactory.DepthLayer1);
        }
    }
}
