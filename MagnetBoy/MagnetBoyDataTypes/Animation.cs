using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;

namespace MagnetBoyDataTypes
{
    public class Animation
    {
        public string name;

        public string sheetName;

        public Object sheet;

        public int initalX, initalY;
        public int frameWidth, frameHeight;
        public int frameCount;
        public double frameSpeed;
    }
}
