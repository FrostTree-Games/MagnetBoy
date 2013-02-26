using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MagnetBoy
{
    /// <summary> 
    /// Class used to for necessary functions that do not exist on the Xbox 360's Compact .NET framework
    /// Stolen from http://xboxforums.create.msdn.com/forums/p/66662/410360.aspx
    /// </summary> 
    public static class XboxListTools
    {
        /// <summary> 
        /// Removes all elements from the List that match the conditions defined by the specified predicate. 
        /// </summary> 
        /// <typeparam name="T">The type of elements held by the List.</typeparam> 
        /// <param name="list">The List to remove the elements from.</param> 
        /// <param name="match">The Predicate delegate that defines the conditions of the elements to remove.</param> 
        public static int RemoveAll<T>(this System.Collections.Generic.List<T> list, Func<T, bool> match)
        {
            int numberRemoved = 0;

            // Loop through every element in the List, in reverse order since we are removing items. 
            for (int i = (list.Count - 1); i >= 0; i--)
            {
                // If the predicate function returns true for this item, remove it from the List. 
                if (match(list[i]))
                {
                    list.RemoveAt(i);
                    numberRemoved++;
                }
            }

            // Return how many items were removed from the List. 
            return numberRemoved;
        }

        /// <summary> 
        /// Returns true if the List contains elements that match the conditions defined by the specified predicate. 
        /// </summary> 
        /// <typeparam name="T">The type of elements held by the List.</typeparam> 
        /// <param name="list">The List to search for a match in.</param> 
        /// <param name="match">The Predicate delegate that defines the conditions of the elements to match against.</param> 
        public static bool Exists<T>(this System.Collections.Generic.List<T> list, Func<T, bool> match)
        {
            // Loop through every element in the List, until a match is found. 
            for (int i = 0; i < list.Count; i++)
            {
                // If the predicate function returns true for this item, return that at least one match was found. 
                if (match(list[i]))
                    return true;
            }

            // Return that no matching elements were found in the List. 
            return false;
        }

        /// <summary> 
        /// Checks if an Entity should be removed from the game.
        /// </summary> 
        /// <param name="en">The Entity in question.</param> 
        public static bool isShouldBeRemoved(Entity en)
        {
            return en.removeFromGame;
        }

        /// <summary> 
        /// Takes an XNA Color and rotates it along the YIQ color space. Rotating will desaturate the color a little.
        /// The algorithm was stolen from:
        /// http://beesbuzz.biz/code/hsv_color_transforms.php
        /// </summary> 
        /// <param name="en">Color you want to modify</param> 
        /// <param name="en">the value in radians to rotate by</param> 
        public static Color rotateHueYIQ(Color original, float rotation)
        {
            float VSU = (float)Math.Cos(rotation);
            float VSW = (float)Math.Sin(rotation);

            Vector3 ret = new Vector3();

            ret.X = (.299f + .701f * VSU + .168f * VSW) * (original.R/255.0f)
                + (.587f - .587f * VSU + .330f * VSW) * (original.G/255.0f)
                + (.114f - .114f * VSU - .497f * VSW) * (original.B/255.0f);
            ret.Y = (.299f - .299f * VSU - .328f * VSW) * (original.R/255.0f)
                + (.587f +.413f *VSU +.035f * VSW) * (original.G/255.0f)
                + (.114f -.114f *VSU +.292f * VSW) * (original.B/255.0f);
            ret.Z = (.299f - .3f * VSU + 1.25f * VSW) * (original.R/255.0f)
                + (.587f - .588f * VSU - 1.05f * VSW) * (original.G/255.0f)
                + (.114f + .886f * VSU - .203f * VSW) * (original.B/255.0f);

            return new Color(ret);
        }
    }
}
