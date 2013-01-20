using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class FlagKey : Entity
    {
        private const string keyAnimationA = "flagKeyA";
        private const string keyAnimationB = "flagKeyB";
        private const string keyAnimationC = "flagKeyC";
        private const string keyAnimationD = "flagKeyD";
        private const string keyAnimationE = "flagKeyE";

        private LevelState.FlagColor color;

        private string currentAnimation;
        private int currentFrame;
        private double lastFrameIncrement;

        public FlagKey(float newX, float newY, LevelState.FlagColor newColor)
        {
            creation();

            horizontal_pos = newX;
            vertical_pos = newY;
            width = 29.5f;
            height = 29.5f;

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;

            acceleration.Y = 0.001f;
            pole = Polarity.Neutral;
            magneticMoment = 0.5f;

            currentFrame = 0;
            lastFrameIncrement = 0;

            color = newColor;

            switch (color)
            {
                case LevelState.FlagColor.Blue:
                    currentAnimation = keyAnimationA;
                    break;
                case LevelState.FlagColor.Green:
                    currentAnimation = keyAnimationB;
                    break;
                case LevelState.FlagColor.Red:
                    currentAnimation = keyAnimationC;
                    break;
                case LevelState.FlagColor.Yellow:
                    currentAnimation = keyAnimationD;
                    break;
                case LevelState.FlagColor.Purple:
                    currentAnimation = keyAnimationE;
                    break;
            }
        }

        public override void update(GameTime currentTime)
        {
            double delta = currentTime.ElapsedGameTime.Milliseconds;

            if (onTheGround && LevelState.isSolidMap(new Vector2(horizontal_pos - 1, vertical_pos)) && velocity.X == 0.0f)
            {
                velocity.X = 0.15f;
            }
            else if (onTheGround && LevelState.isSolidMap(new Vector2(horizontal_pos + width + 3, vertical_pos)) && velocity.X == 0.0f)
            {
                velocity.X = -0.15f;
            }
            if (onTheGround)
            {
                velocity.X *= 0.9f;
            }

            //reset the acceleration vector and recompute it
            acceleration = Vector2.Zero;
            acceleration.Y = 0.001f;

            acceleration = acceleration + computeMagneticForce();

            Vector2 keyAcceleration = Vector2.Zero;
            Vector2 step = new Vector2(horizontal_pos, vertical_pos);
            Vector2 finalAcceleration = acceleration + keyAcceleration;

            velocity.X += (float)(finalAcceleration.X * delta);
            velocity.Y += (float)(finalAcceleration.Y * delta);

            step.X += (float)(((velocity.X) * delta) + (0.5) * (Math.Pow(delta, 2.0)) * finalAcceleration.X);
            step.Y += (float)(((velocity.Y) * delta) + (0.5) * (Math.Pow(delta, 2.0)) * finalAcceleration.Y);

            checkForWalls(LevelState.CurrentLevel, ref step);

            horizontal_pos = step.X;
            vertical_pos = step.Y;

            foreach (Entity en in globalEntityList)
            {
                if (en is FlagLock)
                {
                    FlagLock lk = (FlagLock)en;

                    if (lk.Color == color)
                    {
                        double dist = Math.Sqrt(Math.Pow(lk.CenterPosition.X - CenterPosition.X, 2) + Math.Pow(lk.CenterPosition.Y - CenterPosition.Y, 2));

                        if (dist < width / 2)
                        {
                            lk.open();
                            removeFromGame = true;
                            break;
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
            AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position, LevelState.getFlagXNAColor(color));
        }
    }

    class FlagLock : Entity
    {
        private const string lockAnimationA = "flagLockA";
        private const string lockAnimationB = "flagLockB";
        private const string lockAnimationC = "flagLockC";
        private const string lockAnimationD = "flagLockD";
        private const string lockAnimationE = "flagLockE";

        private LevelState.FlagColor color;
        public LevelState.FlagColor Color { get{ return color; } }

        private string currentAnimation;
        private int currentFrame;

        public FlagLock(float newX, float newY, LevelState.FlagColor newColor)
        {
            creation();

            horizontal_pos = newX;
            vertical_pos = newY;
            width = 29.5f;
            height = 29.5f;

            color = newColor;

            currentFrame = 0;

            switch (color)
            {
                case LevelState.FlagColor.Blue:
                    currentAnimation = lockAnimationA;
                    break;
                case LevelState.FlagColor.Green:
                    currentAnimation = lockAnimationB;
                    break;
                case LevelState.FlagColor.Red:
                    currentAnimation = lockAnimationC;
                    break;
                case LevelState.FlagColor.Yellow:
                    currentAnimation = lockAnimationD;
                    break;
                case LevelState.FlagColor.Purple:
                    currentAnimation = lockAnimationE;
                    break;
            }
        }

        public override void update(GameTime currentTime)
        {
            //
        }

        public override void draw(SpriteBatch sb)
        {
            AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position, LevelState.getFlagXNAColor(color));
        }

        public void open()
        {
            currentFrame = 1;

            LevelState.setFlag(color, true);
        }
    }
}
