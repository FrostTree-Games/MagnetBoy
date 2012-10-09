using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FuncWorks.XNA.XTiled;

namespace MagnetBoy
{
    class Camera
    {
        Entity focusEntity = null;

        //this method allows us to set what entity the camera is to focus on
        public bool setNewFocus(ref Entity newFocus)
        {
            if (newFocus == null)
            {
                return false;
            }
            else
            {
                focusEntity = newFocus;

                return true;
            }
        }

        public void getDrawTranslation(ref Matrix mx, ref Rectangle viewPort, ref Map mp)
        {
            float cameraX, cameraY;

            if (mx == null || focusEntity == null)
            {
                return;
            }

            if (focusEntity.Position.X < viewPort.Width / 2)
            {
                cameraX = -viewPort.Width / 2;
            }
            else if (focusEntity.Position.X > (mp.TileWidth * mp.Width) - viewPort.Width / 2)
            {
                cameraX = (-1)*((mp.TileWidth * mp.Width) - viewPort.Width / 2);
            }
            else
            {
                cameraX = -focusEntity.Position.X;
            }

            if (focusEntity.Position.Y < viewPort.Height / 2)
            {
                cameraY = -viewPort.Height / 2;
            }
            else if (focusEntity.Position.Y > (mp.TileHeight * mp.Height) - viewPort.Height / 2)
            {
                cameraY = (-1) * ((mp.TileHeight * mp.Height) - viewPort.Height / 2);
            }
            else
            {
                cameraY = -focusEntity.Position.Y;
            }

            mx = Matrix.CreateTranslation(cameraX, cameraY, 0.0f) * Matrix.CreateScale(1.0f) * Matrix.CreateTranslation(viewPort.Width / 2, viewPort.Height / 2, 0.0f);
        }


        public void getDrawRectangle(ref Rectangle rx, ref Rectangle viewPort, ref Map mp)
        {
            float cameraX, cameraY;

            if (rx == null || focusEntity == null)
            {
                return;
            }

            if (focusEntity.Position.X < viewPort.Width / 2)
            {
                cameraX = 0;
            }
            else if (focusEntity.Position.X > (mp.TileWidth * mp.Width) - viewPort.Width / 2)
            {
                cameraX = ((mp.TileWidth * mp.Width) - viewPort.Width);
            }
            else
            {
                cameraX = focusEntity.Position.X - viewPort.Width / 2;
            }

            if (focusEntity.Position.Y < viewPort.Height / 2)
            {
                cameraY = 0;
            }
            else if (focusEntity.Position.Y > (mp.TileHeight * mp.Height) - viewPort.Height / 2)
            {
                cameraY = ((mp.TileHeight * mp.Height) - viewPort.Height);
            }
            else
            {
                cameraY = focusEntity.Position.Y - viewPort.Height / 2;
            }

            rx = new Rectangle((int)cameraX, (int)cameraY, viewPort.Width, viewPort.Height);
        }
    }
}
