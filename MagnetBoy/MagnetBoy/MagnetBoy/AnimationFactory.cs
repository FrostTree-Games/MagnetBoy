using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MagnetBoyDataTypes;

namespace MagnetBoy
{
    class AnimationFactory
    {
        private static ContentManager manager = null;

        private static Dictionary<string, Texture2D> sheetLib = null;
        private static Dictionary<string, Animation> animLib = null;

        public AnimationFactory(ContentManager newManager)
        {
            if (newManager != null)
            {
                manager = newManager;
            }

            if (sheetLib == null)
            {
                sheetLib = new Dictionary<string, Texture2D>();
            }

            if (animLib == null)
            {
                animLib = new Dictionary<string, MagnetBoyDataTypes.Animation>();
            }
        }

        public void pushSheet(string sheetContentName)
        {
            if (manager == null)
            {
                return;
            }

            Texture2D newSheet = manager.Load<Texture2D>(sheetContentName);

            sheetLib.Add(sheetContentName, newSheet);
        }

        public void pushAnimation(string animationContentName)
        {
            if (manager == null)
            {
                return;
            }

            Animation[] newAnims = manager.Load<Animation[]>(animationContentName);

            foreach (Animation a in newAnims)
            {
                Texture2D findSheet = null;

                if (sheetLib.TryGetValue(a.sheetName, out findSheet))
                {
                    a.sheet = findSheet;

                    animLib.Add(a.name, a);
                }
            }
        }

        public static double getAnimationSpeed(string animation)
        {
            if (sheetLib == null || animLib == null)
            {
                return -3;
            }

            Animation am = null;

            if (animation == null)
            {
                return -1;
            }

            try
            {
                am = animLib[animation];
            }
            catch (KeyNotFoundException)
            {
                return -2;
            }

            return am.frameSpeed;
        }

        public static int getAnimationFrameCount(string animation)
        {
            if (sheetLib == null || animLib == null)
            {
                return -3;
            }

            Animation am = null;

            if (animation == null)
            {
                return -1;
            }

            try
            {
                am = animLib[animation];
            }
            catch (KeyNotFoundException)
            {
                return -2;
            }

            return am.frameCount;
        }

        public static void drawAnimationFrame(SpriteBatch sb, string animation, int frame, Vector2 position)
        {
            if (manager == null || sheetLib == null || animLib == null)
            {
                return;
            }

            Animation am = null;

            if (frame < 0 || animation == null)
            {
                return;
            }

            if (!animLib.ContainsKey(animation))
            {
                sb.Draw(Game1.globalTestWalrus, position, Color.White);
                return;
            }
            else
            {
                am = animLib[animation];
            }

            int actualFrame = frame % am.frameCount;

            Rectangle rf = new Rectangle(am.initalX + (actualFrame * am.frameWidth), am.initalY, am.frameWidth, am.frameHeight);

            sb.Draw((Texture2D)am.sheet, position, rf, Color.White);
        }
    }
}
