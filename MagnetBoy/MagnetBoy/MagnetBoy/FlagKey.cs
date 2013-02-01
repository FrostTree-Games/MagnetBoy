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

        private float rotation;

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

            color = newColor;

            rotation = 0.0f;
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
            checkForSolidObjects(ref step);

            horizontal_pos = step.X;
            vertical_pos = step.Y;

            rotation += velocity.X;

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
        }

        public override void draw(SpriteBatch sb)
        {
            AnimationFactory.drawAnimationFrame(sb, "flagKey", 0, Position, new Vector2(32, 32), rotation, LevelState.getFlagXNAColor(color), AnimationFactory.DepthLayer2);
            AnimationFactory.drawAnimationFrame(sb, "flagDoorSymbol", (int)color, Position, new Vector2(32, 32), rotation, LevelState.getFlagXNAColor(color), AnimationFactory.DepthLayer1 + 0.01f);
        }
    }

    class FlagLock : Entity
    {
        private LevelState.FlagColor color;
        public LevelState.FlagColor Color { get{ return color; } }

        public FlagLock(float newX, float newY, LevelState.FlagColor newColor)
        {
            creation();

            horizontal_pos = newX;
            vertical_pos = newY;
            width = 29.5f;
            height = 29.5f;

            color = newColor;
        }

        public override void update(GameTime currentTime)
        {
            //
        }

        public override void draw(SpriteBatch sb)
        {
            AnimationFactory.drawAnimationFrame(sb, "flagLock", 0, Position, new Vector2(32, 32), 0.0f, LevelState.getFlagXNAColor(color), AnimationFactory.DepthLayer3);

            if (!LevelState.getFlag(color))
            {
                AnimationFactory.drawAnimationFrame(sb, "flagDoorSymbol", (int)color, Position, new Vector2(32, 32), 0.0f, LevelState.getFlagXNAColor(color), AnimationFactory.DepthLayer2);
            }
        }

        public void open()
        {
            AudioFactory.playSFX("sfx/unlockDoor");
            LevelState.setFlag(color, true);
        }
    }
}
