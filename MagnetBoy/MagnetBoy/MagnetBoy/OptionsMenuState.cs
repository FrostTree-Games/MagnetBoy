using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class OptionsMenuState : IState
    {
        private ContentManager contentManager = null;
        private GameInput gameInput;

        /*
        private bool downPressed = false;
        private bool upPressed = false;
        private bool leftPressed = false;
        private bool rightPressed = false;
        private bool confirmPressed = false; */
        private bool cancelPressed = false; 

        public OptionsMenuState(ContentManager newManager)
        {
            IsUpdateable = true;

            contentManager = newManager;
            gameInput = new GameInput(Game1.graphics.GraphicsDevice);
        }

        protected override void doUpdate(GameTime currentTime)
        {
            gameInput.update();

            if (GameInput.isButtonDown(GameInput.PlayerButton.Cancel))
            {
                cancelPressed = true;
            }
            else if (cancelPressed == true)
            {
                GameScreenManager.switchScreens(GameScreenManager.GameScreenType.Menu, "TitleScreenMenu_fromOptions");
                cancelPressed = false;

                AudioFactory.playSFX("sfx/menu");
            }

            if (GameInput.isButtonDown(GameInput.PlayerButton.Push))
            {
                if (GameInput.P1MouseDirectionNormal.X != float.NaN)
                {
                    TitleScreenMenuState.checkerBoardSlide.X += GameInput.P1MouseDirectionNormal.X;
                }

                if (GameInput.P1MouseDirectionNormal.Y != float.NaN)
                {
                    TitleScreenMenuState.checkerBoardSlide.Y += GameInput.P1MouseDirectionNormal.Y;
                }
            }

            Game1.grayCheckerBoard.Parameters["slideX"].SetValue(TitleScreenMenuState.checkerBoardSlide.X);
            Game1.grayCheckerBoard.Parameters["slideY"].SetValue(TitleScreenMenuState.checkerBoardSlide.Y);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            Game1.graphics.GraphicsDevice.Clear(Color.Lerp(Color.DarkGray, Color.White, 0.4f));

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, Game1.grayCheckerBoard, Matrix.CreateScale(720, 480, 0));
            spriteBatch.Draw(Game1.globalBlackPixel, Vector2.Zero, Color.White);
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw(Game1.globalTestWalrus, new Vector2(100, 100), Color.White);
            spriteBatch.End();
        }
    }
}
