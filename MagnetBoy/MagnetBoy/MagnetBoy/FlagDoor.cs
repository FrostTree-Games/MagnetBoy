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
            //
        }

        public override void draw(SpriteBatch sb)
        {
            AnimationFactory.drawAnimationFrame(sb, "flagDoor", 0, Position);
        }
    }
}
