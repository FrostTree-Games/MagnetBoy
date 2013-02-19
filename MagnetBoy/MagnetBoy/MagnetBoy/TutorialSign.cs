using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class TutorialSign : Entity
    {
        public enum SignMessage
        {
            Move = 1,
            MagnetPowers = 2,
            Jumping = 3,
            PushObjects = 4,
            PushBadGuys = 5,
            JumpBadGuys = 6
        }

        private const string messageAnimation1 = "RSTutorial";
        private const string messageAnimation2 = "tutorialSign2";
        private const string messageAnimation3 = "tutorialSign3";
        private const string messageAnimation4 = "tutorialSign4";
        private const string messageAnimation5 = "tutorialSign5";
        private const string messageAnimation6 = "tutorialSign6";

        private bool playerClose;
        private double lightValue;
        private const double lightChangeSpeed = 0.0035;
        private const double lightValueMin = 0.0;
        private const double lightValueMax = 1.0;

        private string currentAnimation;
        private int currentFrame;
        private double lastFrameIncrement = 0;

        public TutorialSign(float newX, float newY, TutorialSign.SignMessage messageType)
        {
            horizontal_pos = newX;
            vertical_pos = newY;

            width = 160;
            height = 96;

            playerClose = false;
            lightValue = 0;

            currentFrame = 0;
            lastFrameIncrement = 0;

            switch (messageType)
            {
                case SignMessage.Move:
                    currentAnimation = messageAnimation1;
                    break;
                case SignMessage.MagnetPowers:
                    currentAnimation = messageAnimation2;
                    break;
                case SignMessage.Jumping:
                    currentAnimation = messageAnimation3;
                    break;
                case SignMessage.PushObjects:
                    currentAnimation = messageAnimation4;
                    break;
                case SignMessage.PushBadGuys:
                    currentAnimation = messageAnimation5;
                    break;
                case SignMessage.JumpBadGuys:
                    currentAnimation = messageAnimation6;
                    break;

            }
        }

        public override void update(GameTime currentTime)
        {
            foreach (Entity en in globalEntityList)
            {
                if (en is Player)
                {
                    if (Math.Abs(en.CenterPosition.X - CenterPosition.X) > width || Math.Abs(en.CenterPosition.Y - CenterPosition.Y) > height)
                    {
                        playerClose = false;
                    }
                    else
                    {
                        playerClose = true;
                    }

                    break;
                }
            }

            if (playerClose)
            {
                if (lightValue < lightValueMax)
                {
                    lightValue += lightChangeSpeed * currentTime.ElapsedGameTime.Milliseconds;

                    if (lightValue > lightValueMax)
                    {
                        lightValue = lightValueMax;
                    }
                }
            }
            else
            {
                if (lightValue > lightValueMin)
                {
                    lightValue -= lightChangeSpeed * currentTime.ElapsedGameTime.Milliseconds;

                    if (lightValue < lightValueMin)
                    {
                        lightValue = lightValueMin;
                    }
                }
            }

            // if the last frame time hasn't been set, set it now
            if (lastFrameIncrement == 0)
            {
                lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;
            }

            if (Math.Abs(lightValue - lightValueMax) < 0.5f)
            {
                // update the current frame if needed
                if (currentTime.TotalGameTime.TotalMilliseconds - lastFrameIncrement > AnimationFactory.getAnimationSpeed(currentAnimation))
                {
                    lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;

                    currentFrame = (currentFrame + 1) % AnimationFactory.getAnimationFrameCount(currentAnimation);
                }
            }
            else
            {
                currentFrame = 0;
                lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;
            }
        }

        public override void draw(SpriteBatch sb)
        {
            AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position, Color.Lerp(Color.Lerp(Color.Black, Color.DarkGray, 0.5f), Color.White, (float)lightValue), AnimationFactory.DepthLayer3);
        }
    }
}
