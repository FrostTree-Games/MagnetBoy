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

        public static Vector2 P1MouseDirection
        {
            get
            {
                return p1MouseDirection;
            }
        }

        public static Vector2 P1MouseDirectionNormal
        {
            get
            {
                return Vector2.Normalize(p1MouseDirection);
            }
        }

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

            if (graphicsDevice.Viewport.Bounds.Contains(new Point(mouse.X, mouse.Y)))
            {
                p1MouseDirection.X = mouse.X - (graphicsDevice.Viewport.Bounds.Width / 2);
                p1MouseDirection.Y = mouse.Y - (graphicsDevice.Viewport.Bounds.Height / 2);
            }
            else
            {
                p1MouseDirection = Vector2.Zero;
            }

            Console.WriteLine("{0}", p1MouseDirection);
        }
    }
}
