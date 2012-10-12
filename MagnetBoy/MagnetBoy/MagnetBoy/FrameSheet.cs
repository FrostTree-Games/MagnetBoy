using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace MagnetBoy
{
    class FrameSheet
    {
        class Animation
        {
            //public string name;

            public int initalX, initalY;
            public int frameWidth, frameHeight;
            public int frameCount;
            public double frameSpeed;
        }

        // might not need this
        /*class AnimationComparer : IEqualityComparer<Animation>
        {
            public bool Equals(Animation a1, Animation a2)
            {
                if (a1.name == a2.name)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public int GetHashCode(Animation an)
            {
                return an.name.GetHashCode();
            }
        }*/

        Texture2D imageData = null;

        Dictionary<string, Animation> anims = null;

        // for now, this is a test constructor, which will be replaced with some XNA content loader later
        // this constructor is written to be made with the actor3.png file
        public FrameSheet(ref Texture2D sheet)
        {
            imageData = sheet;

            anims = new Dictionary<string, Animation>();

            Animation a = new Animation();
            a.initalX = 288;
            a.initalY = 64;
            a.frameWidth = 32;
            a.frameHeight = 32;
            a.frameCount = 3;
            a.frameSpeed = 100;
            anims.Add("walkRight", a);

            a = new Animation();
            a.initalX = 288;
            a.initalY = 32;
            a.frameWidth = 32;
            a.frameHeight = 32;
            a.frameCount = 3;
            a.frameSpeed = 100;
            anims.Add("walkLeft", a);

            a = new Animation();
            a.initalX = 320;
            a.initalY = 64;
            a.frameWidth = 32;
            a.frameHeight = 32;
            a.frameCount = 1;
            a.frameSpeed = 100;
            anims.Add("standingRight", a);

            a = new Animation();
            a.initalX = 320;
            a.initalY = 32;
            a.frameWidth = 32;
            a.frameHeight = 32;
            a.frameCount = 1;
            a.frameSpeed = 100;
            anims.Add("standingLeft", a);
        }

        // stolen from http://stackoverflow.com/questions/8331494/crop-texture2d-spritesheet
        private static Texture2D Crop(Texture2D image, Rectangle source)
        {
            var graphics = image.GraphicsDevice;
            var ret = new RenderTarget2D(graphics, source.Width, source.Height);
            var sb = new SpriteBatch(graphics);

            graphics.SetRenderTarget(ret); // draw to image
            graphics.Clear(new Color(0, 0, 0, 0));

            sb.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            sb.Draw(image, Vector2.Zero, source, Color.White);
            sb.End();

            graphics.SetRenderTarget(null); // set back to main window

            return (Texture2D)ret;
        }

        public double getAnimationSpeed(string animation)
        {
            Animation am = null;

            if (animation == null || imageData == null)
            {
                return 0;
            }

            try
            {
                am = anims[animation];
            }
            catch (KeyNotFoundException)
            {
                return 0;
            }

            return am.frameSpeed;
        }

        public int getAnimationFrameCount(string animation)
        {
            Animation am = null;

            if (animation == null || imageData == null)
            {
                return 1;
            }

            try
            {
                am = anims[animation];
            }
            catch (KeyNotFoundException)
            {
                return 1;
            }

            return am.frameCount;
        }

        public void drawAnimationFrame(SpriteBatch sb, string animation, int frame, Vector2 position)
        {
            Animation am = null;

            if (frame < 0 || animation == null || imageData == null)
            {
                return;
            }

            if (!anims.ContainsKey(animation))
            {
                sb.Draw(Game1.globalTestWalrus, position, Color.White);
                return;
            }
            else
            {
                am = anims[animation];
            }

            int actualFrame = frame % am.frameCount;

            Rectangle rf = new Rectangle(am.initalX + (actualFrame * am.frameWidth), am.initalY, am.frameWidth, am.frameHeight);

            sb.Draw(imageData, position, rf, Color.White);
        }
    }
}
