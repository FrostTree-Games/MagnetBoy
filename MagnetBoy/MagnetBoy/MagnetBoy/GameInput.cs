using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

/* This class is to be a public abstraction for keyboard/mouse input.
 * It is to follow a singleton pattern.
 */

namespace MagnetBoy
{
    class GameInput
    {
        private static GraphicsDevice graphicsDevice = null;

        private static MouseState mouse;
        private static Vector2 p1MouseDirection;

        public GameInput(GraphicsDevice device)
        {
            if (graphicsDevice == null && device != null)
            {
                graphicsDevice = device;
            }

            update();
        }

        public void update()
        {
            mouse = Mouse.GetState();

            if (graphicsDevice.Viewport.Bounds.Contains(new Point(mouse.X + (graphicsDevice.Viewport.Bounds.X / 2), mouse.Y + (graphicsDevice.Viewport.Bounds.Y / 2))))
            {
                p1MouseDirection.X = mouse.X;
                p1MouseDirection.Y = mouse.Y;

                p1MouseDirection.Normalize();
            }
            else
            {
                p1MouseDirection = Vector2.Zero;
            }

            Console.WriteLine("{0},{1}", p1MouseDirection.X, p1MouseDirection.Y);
        }
    }
}
