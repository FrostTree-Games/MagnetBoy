using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class FlagDoor : Entity
    {
        private LevelState.FlagColor doorColor;

        private const double doorClickDelay = 150;
        private double doorClickDelta = 0;

        private const float doorMaxHeight = 64.0f;
        private int doorHeight; //based on 8-pixel-high blocks
        public int DoorHeight
        {
            set
            {
                doorHeight = value;
            }
            get
            {
                return doorHeight;
            }
        }

        private bool doorIsClosed;

        public FlagDoor(float newX, float newY, LevelState.FlagColor color)
        {
            creation();

            removeFromGame = false;

            horizontal_pos = newX;
            vertical_pos = newY;
            doorColor = color;

            doorHeight = 8;
            width = 29.5f;
            height = 32f + (8f * doorHeight);

            doorIsClosed = true;
        }

        public override void update(GameTime currentTime)
        {
            if (!doorIsClosed)
            {
                if (doorHeight > 0)
                {
                    doorClickDelta += currentTime.ElapsedGameTime.Milliseconds;

                    if (doorClickDelta >= doorClickDelay)
                    {
                        doorClickDelta = 0;
                        doorHeight -= 1;
                        height = 32f + (8f * doorHeight);
                        AudioFactory.playSFX("sfx/doorClose");
                    }
                }
            }
            else
            {
                if (doorHeight < 8)
                {
                    doorClickDelta += currentTime.ElapsedGameTime.Milliseconds;

                    if (doorClickDelta >= doorClickDelay)
                    {
                        doorClickDelta = 0;
                        doorHeight += 1;
                        height = 32f + (8f * doorHeight);
                        AudioFactory.playSFX("sfx/doorClose");
                    }
                }
            }

            if (doorIsClosed && LevelState.getFlag(doorColor))
            {
                doorClickDelta = 0;
            }

            doorIsClosed = !(LevelState.getFlag(doorColor));
        }

        public override void draw(SpriteBatch sb)
        {
            AnimationFactory.drawAnimationFrame(sb, "flagDoor", 0, Position, LevelState.getFlagXNAColor(doorColor), AnimationFactory.DepthLayer1);
            AnimationFactory.drawAnimationFrame(sb, "flagDoorSymbol", (int)doorColor, Position, LevelState.getFlagXNAColor(doorColor), AnimationFactory.DepthLayer0);

            for (int i = 0; i < doorHeight; i++)
            {
                Vector2 pos = Position + new Vector2(0, 32 + (i * 8f));
                AnimationFactory.drawAnimationFrame(sb, "flagDoorWall", 0, pos, AnimationFactory.DepthLayer0);
            }
        }
    }
}
