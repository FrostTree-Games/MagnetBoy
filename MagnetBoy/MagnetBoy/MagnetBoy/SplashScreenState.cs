using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class SplashScreenState : IState
    {
        private ContentManager contentManager = null;
        private GameInput gameInput;

        private bool calledLoadAssets;

        private bool flashOffText;

        public SplashScreenState(ContentManager newManager)
        {
            contentManager = newManager;

            IsUpdateable = true;

            calledLoadAssets = false;
            flashOffText = false;

            gameInput = new GameInput(Game1.graphics.GraphicsDevice);
        }

        protected override void doUpdate(GameTime currentTime)
        {
            if (!Game1.AssetsLoaded)
            {
                if (!calledLoadAssets)
                {
                    new Thread(Game1.loadGameAssets).Start();

                    calledLoadAssets = true;
                }

                flashOffText = currentTime.TotalGameTime.Milliseconds % 500 < 250;
            }
            else
            {
                GameScreenManager.switchScreens(GameScreenManager.GameScreenType.Menu, "TitleScreenMenu");
            }
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            Game1.graphics.GraphicsDevice.Clear(Color.Black);

            if (flashOffText)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(Game1.gameFontText, "Loading...", new Vector2(460, 380), Color.White);
                spriteBatch.End();
            }
        }
    }
}
