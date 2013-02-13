/* Magnet Boy Quick GUI */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MagnetBoy
{
    class MBQG
    {
        private static string rowTop = "gui_angledBoxA";
        private static string rowMid = "gui_angledBoxB";
        private static string rowBot = "gui_angledBoxC";

        public static void drawBlackBorderText(SpriteBatch sb, Vector2 position, Color clr, string text, float depth)
        {
            for (int i = -1; i < 2; i++)
            {
                sb.DrawString(Game1.gameFontText, text, position + new Vector2(i, i), (i == 0) ? clr : Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, (i == 0) ? depth: depth + 0.1f);
                sb.DrawString(Game1.gameFontText, text, position + new Vector2(-1 * i, i), (i == 0) ? clr : Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, (i == 0) ? depth: depth + 0.1f);
            }
        }

        public static void drawGUIBox(SpriteBatch sb, Vector2 topLeftCorner, int tilesWide, int tilesHigh, Color clr, float depth)
        {
            for (int i = 0; i < tilesWide; i++)
            {
                for (int j = 0; j < tilesHigh; j++)
                {
                    int col;
                    string row;

                    if (i == 0)
                    {
                        col = 0;
                    }
                    else if (i == tilesWide - 1)
                    {
                        col = 2;
                    }
                    else
                    {
                        col = 1;
                    }

                    if (j == 0)
                    {
                        row = rowTop;
                    }
                    else if (j == tilesHigh - 1)
                    {
                        row = rowBot;
                    }
                    else
                    {
                        row = rowMid;
                    }

                    AnimationFactory.drawAnimationFrame(sb, row, col, topLeftCorner + new Vector2(i * 16, j * 16), clr, depth);
                }
            }
        }
    }
}
