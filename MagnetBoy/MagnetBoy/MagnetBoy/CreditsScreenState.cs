using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    public 

    class CreditsScreenState : IState
    {
        private ContentManager manager = null;

        private double creditsTimePassed;
        private const double creditsDurationTime = 30000;

        private const float topEmptySpace = 500f;
        private const float bottomEmptySpace = 300f;

        public CreditsScreenState(ContentManager newManager)
        {
            manager = newManager;

            IsUpdateable = true;

            creditsTimePassed = 0;

            AudioFactory.stopSong();
            AudioFactory.playSong("songs/introTheme");
        }

        protected override void doUpdate(GameTime currentTime)
        {
            creditsTimePassed += currentTime.ElapsedGameTime.Milliseconds;

            if (creditsTimePassed > creditsDurationTime)
            {
                AudioFactory.stopSong();
                GameScreenManager.switchScreens(GameScreenManager.GameScreenType.SplashScreen, null);
            }
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            Game1.graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.Draw(Game1.globalCreditsList, new Vector2(360 - Game1.globalCreditsList.Bounds.Width/4 , -1 * ((Game1.globalCreditsList.Bounds.Height/2) + topEmptySpace + bottomEmptySpace) * (float)(creditsTimePassed/creditsDurationTime) + topEmptySpace), null, Color.White, 0.0f, Vector2.Zero, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
