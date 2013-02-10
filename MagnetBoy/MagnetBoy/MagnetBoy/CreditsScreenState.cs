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

        public CreditsScreenState(ContentManager newManager)
        {
            manager = newManager;

            IsUpdateable = true;
        }

        protected override void doUpdate(GameTime currentTime)
        {
            throw new NotImplementedException();
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }
    }
}
