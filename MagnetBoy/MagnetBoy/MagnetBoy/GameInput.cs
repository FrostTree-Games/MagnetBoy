using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;

/* This class is to be a public abstraction for keyboard/mouse input.
 * It is to follow a singleton pattern.
 */

namespace MagnetBoy
{
    class GameInput
    {
        public enum PlayerButton
        {
            UpDirection,
            DownDirection,
            LeftDirection,
            RightDirection,
            Push,
            Jump,
            Confirm,
            Cancel,
            StartButton,
            BackButton,
            AnyButton
        }

        private static GraphicsDevice graphicsDevice = null;

        private static MouseState mouse;
        private static Vector2 p1MouseDirection;
        private static Vector2[] analogDirections = null;

        private static GamePadState[] padStates = null;
        private static int[] padPrevTick = null;
        private static KeyboardState kbdState;

        private static int mostRecentPad = 0;
        private static bool lockMostRecentPad = false;
        public static bool LockMostRecentPad { get { return lockMostRecentPad; } set { lockMostRecentPad = value; } }

        public static Vector2 P1MouseDirection
        {
            get
            {
                return analogDirections[mostRecentPad];
            }
        }

        public static Vector2 P1MouseDirectionNormal
        {
            get
            {
                if (analogDirections[mostRecentPad].Length() > 0)
                {
                    return Vector2.Normalize(analogDirections[mostRecentPad]);
                }
                else
                {
                    return Vector2.Zero;
                }
            }
        }

        public GameInput(GraphicsDevice device)
        {
            if (padStates == null)
            {
                padStates = new GamePadState[4];
                padPrevTick = new int[4];
                analogDirections = new Vector2[4];
            }

            if (graphicsDevice == null && device != null)
            {
                graphicsDevice = device;
            }
        }

        public static Boolean P1MouseDown
        {
            get
            {
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool isButtonDown(PlayerButton button)
        {
            for (int i = 0; i < padStates.Length; i++)
            {
                if (lockMostRecentPad && i != mostRecentPad)
                {
                    continue;
                }

                switch (button)
                {
                    case PlayerButton.UpDirection:
                        if (padStates[i].DPad.Up == ButtonState.Pressed || padStates[i].ThumbSticks.Left.Y > 0.01 || kbdState.IsKeyDown(Keys.Up))
                        {
                            return true;
                        }
                        break;
                    case PlayerButton.DownDirection:
                        if (padStates[i].DPad.Down == ButtonState.Pressed || padStates[i].ThumbSticks.Left.Y < -0.01 || kbdState.IsKeyDown(Keys.Down))
                        {
                            return true;
                        }
                        break;
                    case PlayerButton.LeftDirection:
                        if (padStates[i].DPad.Left == ButtonState.Pressed || padStates[i].ThumbSticks.Left.X < -0.01 || kbdState.IsKeyDown(Keys.Left))
                        {
                            return true;
                        }
                        break;
                    case PlayerButton.RightDirection:
                        if (padStates[i].DPad.Right == ButtonState.Pressed || padStates[i].ThumbSticks.Left.X > 0.01 || kbdState.IsKeyDown(Keys.Right))
                        {
                            return true;
                        }
                        break;
                    case PlayerButton.Push:
                        if (padStates[i].ThumbSticks.Right.Length() > 0.01f)
                        {
                            return true;
                        }
                        break;
                    case PlayerButton.Jump:
                        if (padStates[i].Buttons.RightShoulder == ButtonState.Pressed)
                        {
                            return true;
                        }
                        break;
                    case PlayerButton.Confirm:
                        if (padStates[i].Buttons.A == ButtonState.Pressed || kbdState.IsKeyDown(Keys.Enter))
                        {
                            return true;
                        }
                        break;
                    case PlayerButton.Cancel:
                        if (padStates[i].Buttons.B == ButtonState.Pressed || kbdState.IsKeyDown(Keys.Back))
                        {
                            return true;
                        }
                        break;
                    case PlayerButton.StartButton:
                        if (padStates[i].Buttons.Start == ButtonState.Pressed || kbdState.IsKeyDown(Keys.Escape))
                        {
                            return true;
                        }
                        break;
                    case PlayerButton.AnyButton:
                        if (padStates[i].Buttons.A == ButtonState.Pressed || padStates[i].Buttons.B == ButtonState.Pressed || padStates[i].Buttons.X == ButtonState.Pressed || padStates[i].Buttons.Y == ButtonState.Pressed || padStates[i].Buttons.Back == ButtonState.Pressed || padStates[i].Buttons.Start == ButtonState.Pressed || padStates[i].Buttons.RightShoulder == ButtonState.Pressed || padStates[i].Buttons.LeftShoulder == ButtonState.Pressed || padStates[i].Buttons.RightStick == ButtonState.Pressed || padStates[i].Buttons.LeftStick == ButtonState.Pressed)
                        {
                            return true;
                        }
                        break;
                    default:
                        return false;
                }
            }

            return false;
        }

        public void update()
        {
            padStates[0] = GamePad.GetState(PlayerIndex.One);
            padStates[1] = GamePad.GetState(PlayerIndex.Two);
            padStates[2] = GamePad.GetState(PlayerIndex.Three);
            padStates[3] = GamePad.GetState(PlayerIndex.Four);

            for (int i = 0; i < padStates.Length; i++)
            {
                if (padStates[i].IsConnected && !lockMostRecentPad)
                {
                    if (padPrevTick[i] != padStates[i].PacketNumber)
                    {
                        mostRecentPad = i;
                    }

                    padPrevTick[i] = padStates[i].PacketNumber;
                }
            }

#if WINDOWS
            mouse = Mouse.GetState();
            kbdState = Keyboard.GetState();
#endif

            /*
#if WINDOWS
            if (graphicsDevice.Viewport.Bounds.Contains(new Point(mouse.X, mouse.Y)))
            {
                analogDirections[mostRecentPad].X = mouse.X - (graphicsDevice.Viewport.Bounds.Width / 2);
                analogDirections[mostRecentPad].Y = mouse.Y - (graphicsDevice.Viewport.Bounds.Height / 2);
            }
            else
#endif
                */

            for (int i = 0; i < padStates.Length; i++)
            {
                if (padStates[i].IsConnected)
                {
                    analogDirections[i] = padStates[i].ThumbSticks.Right;
                    analogDirections[i].Y *= -1;

                    if (analogDirections[i].Y == 0 && analogDirections[i].X != 0)
                    {
                        analogDirections[i].Y = 0.0001f;
                    }

                    if (float.IsNaN(analogDirections[i].X) || float.IsNaN(analogDirections[i].Y))
                    {
                        analogDirections[i] = Vector2.Zero;
                    }
                }
                else
                {
                    analogDirections[i] = Vector2.Zero;
                }
            }
        }

        public static PlayerIndex LockedPlayerIndex
        {
            get
            {
                if (!lockMostRecentPad)
                {
                    throw new Exception("Game pads are not locked");
                }
                else
                {
                    switch (mostRecentPad)
                    {
                        case 0:
                            return PlayerIndex.One;
                        case 1:
                            return PlayerIndex.Two;
                        case 2:
                            return PlayerIndex.Three;
                        case 3:
                            return PlayerIndex.Four;
                        default:
                            return PlayerIndex.One;
                    }
                }
            }
        }
    }
}
