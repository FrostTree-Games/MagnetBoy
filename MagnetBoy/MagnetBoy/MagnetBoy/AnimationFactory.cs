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

        public static Single DepthLayer3 { get { return 0.75f; } }
        public static Single DepthLayer2 { get { return 0.5f; } }
        public static Single DepthLayer1 { get { return 0.25f; } }
        public static Single DepthLayer0 { get { return 0.0f; } }

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

        public static double getAnimationFrameWidth(string animation)
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

            return am.frameWidth;
        }

        public static double getAnimationFrameHeight(string animation)
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

            return am.frameHeight;
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

        public static void drawAnimationFrame(SpriteBatch sb, string animation, int frame, Vector2 position, Single depth)
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

            if (rf.X < 0)
            {
                rf.X = 0;
            }

            //sb.Draw((Texture2D)am.sheet, position, rf, Color.White);
            sb.Draw((Texture2D)am.sheet, position, rf, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
        }

        public static void drawAnimationFrame(SpriteBatch sb, string animation, int frame, Vector2 position, Color tint, Single depth)
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

            if (rf.X < 0)
            {
                rf.X = 0;
            }

            //sb.Draw((Texture2D)am.sheet, position, rf, tint);
            sb.Draw((Texture2D)am.sheet, position, rf, tint, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
        }

        public static void drawAnimationFrame(SpriteBatch sb, string animation, int frame, Vector2 position, Vector2 scale, Color tint, Single depth)
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

            if (rf.X < 0)
            {
                rf.X = 0;
            }

            sb.Draw((Texture2D)am.sheet, position, rf, tint, 0.0f, Vector2.Zero, scale, SpriteEffects.None, depth);
        }

        public static void drawAnimationFrame(SpriteBatch sb, string animation, int frame, Vector2 position, Vector2 widthHeight, float rotation, Single depth)
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

            if (rf.X < 0)
            {
                rf.X = 0;
            }

            //sb.Draw((Texture2D)am.sheet, position, rf, Color.White);
            sb.Draw((Texture2D)am.sheet, new Rectangle((int)(position.X + widthHeight.X / 2), (int)(position.Y + widthHeight.Y / 2), (int)widthHeight.X, (int)widthHeight.Y), rf, Color.White, rotation, new Vector2(widthHeight.X / 2, widthHeight.Y / 2), SpriteEffects.None, depth);
        }

        public static void drawAnimationFrame(SpriteBatch sb, string animation, int frame, Vector2 position, Vector2 widthHeight, float rotation, Color clr, Single depth)
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

            if (rf.X < 0)
            {
                rf.X = 0;
            }

            //sb.Draw((Texture2D)am.sheet, position, rf, Color.White);
            sb.Draw((Texture2D)am.sheet, new Rectangle((int)(position.X + widthHeight.X / 2), (int)(position.Y + widthHeight.Y / 2), (int)widthHeight.X, (int)widthHeight.Y), rf, clr, rotation, new Vector2(widthHeight.X / 2, widthHeight.Y / 2), SpriteEffects.None, depth);
        }
    }
}
