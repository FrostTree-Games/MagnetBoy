using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FuncWorks.XNA.XTiled;

namespace MagnetBoy
{
    class Entity
    {
        // enumeration for magnetic polarity of objects
        public enum Polarity
        {
            Neutral,
            Positive,
            Negative
        }

        protected static List<Entity> globalEntityList = null;

        protected float horizontal_pos = 0.0f;
        protected float vertical_pos = 0.0f;

        public Vector2 Position
        {
            get
            {
                return new Vector2(horizontal_pos, vertical_pos);
            }
        }

        protected float width = 29.5f;
        protected float height = 29.5f;

        protected Polarity pole = Polarity.Neutral;
        protected float magneticMoment = 0.0f;

        public KeyValuePair<Polarity, float> MagneticValue
        {
            get
            {
                return new KeyValuePair<Polarity, float>(pole, magneticMoment);
            }
        }

        protected Vector2 velocity;
        protected Vector2 acceleration;

        protected Boolean solid = false;
        public Boolean IsSolid
        {
            get
            {
                return solid;
            }
        }

        protected bool onTheGround = false;

        protected FrameSheet sheet = null;
        
        public Entity()
        {
            creation();
            horizontal_pos = 0.0f;
            vertical_pos = 0.0f;
        }

        public Entity(float initialx, float initialy)
        {
            creation();

            horizontal_pos = initialx;
            vertical_pos = initialy;
        }

        // must be called in the entity constructor
        protected void creation()
        {
            if (globalEntityList == null)
            {
                globalEntityList = new List<Entity>();
            }

            globalEntityList.Add(this);
        }

        public void death()
        {
            if (globalEntityList != null)
            {
                globalEntityList.Remove(this);
            }
        }

        public virtual void update(GameTime currentTime)
        {
            return;
        }

        public void setSheet(ref FrameSheet fs)
        {
            sheet = fs;
        }

        public virtual void draw(SpriteBatch sb)
        {
            sb.Draw(Game1.globalTestWalrus, new Vector2(horizontal_pos, vertical_pos), Color.White);
        }

        protected Vector2 computeMagneticForce()
        {
            Vector2 acceleration = Vector2.Zero;

            foreach (Entity q2 in globalEntityList)
            {
                if (q2 == this)
                {
                    continue;
                }

                if (pole != Polarity.Neutral && q2.MagneticValue.Key != Polarity.Neutral)
                {
                    double distance = Math.Sqrt(Math.Pow(q2.Position.X - horizontal_pos, 2) + Math.Pow(q2.Position.Y - vertical_pos, 2));
                    double force = (magneticMoment * q2.MagneticValue.Value) / (4 * Math.PI * Math.Pow(distance, 2));
                    double angle = Math.Atan2(q2.Position.X - horizontal_pos, vertical_pos - q2.Position.Y);

                    angle = (angle + (Math.PI / 2)) % (Math.PI * 2);

                    if (pole != q2.MagneticValue.Key)
                    {
                        angle += Math.PI;
                    }

                    Vector2 newForce = new Vector2((float)(force * Math.Cos(angle)) * 100, (float)(force * Math.Sin(angle)) * 100);

                    acceleration += newForce;
                }
            }

            return acceleration;
        }

        public bool hitTest(Entity other)
        {
            if( horizontal_pos > other.horizontal_pos + other.width || horizontal_pos + width < other.horizontal_pos || vertical_pos > other.vertical_pos + other.height || vertical_pos + height < other.vertical_pos)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean checkForSolidObjects(ref Vector2 step)
        {
            Boolean hitY = false;
            Boolean hitX = false;

            foreach (Entity en in globalEntityList)
            {
                if (en == this)
                {
                    continue;
                }

                if (en.IsSolid && hitTest(en))
                {
                    float deltaX = 0.0f;
                    float deltaY = 0.0f;

                    if (en.Position.Y + en.height > vertical_pos && en.Position.Y < vertical_pos)
                    {
                        deltaY += (en.Position.Y + en.height) - vertical_pos;
                    }
                    else if (en.Position.Y < vertical_pos + height && en.Position.Y > vertical_pos)
                    {
                        deltaY += en.Position.Y - (vertical_pos + height);

                        onTheGround = true;
                    }

                    if (en.Position.X + en.width > horizontal_pos && en.Position.X < horizontal_pos)
                    {
                        deltaX += (en.Position.X + en.width) - horizontal_pos;
                    }
                    else if (en.Position.X < horizontal_pos + width && en.Position.X > horizontal_pos)
                    {
                        deltaX += en.Position.X - (horizontal_pos + width);
                    }

                    if (Math.Abs(horizontal_pos - en.Position.X) < Math.Abs(vertical_pos - en.Position.Y))
                    {
                        step.Y += deltaY * 1.1f;

                        hitY = true;
                    }
                    else
                    {
                        step.X += deltaX * 1.1f;

                        hitX = true;
                    }

                    /*
                    if (en.Position.Y + en.height > vertical_pos && en.Position.Y < vertical_pos)
                    {
                        step.Y = Math.Max(en.Position.Y + en.height + 1, vertical_pos);

                        hitY = true;
                    }
                    else if (en.Position.Y < vertical_pos + height && en.Position.Y > vertical_pos)
                    {
                        step.Y = en.Position.Y - height - 1;

                        onTheGround = true;

                        hitY = true;
                    }

                    if (!hitY)
                    {
                        if (en.Position.X + en.width > horizontal_pos && en.Position.X < horizontal_pos)
                        {
                            step.X = Math.Max(en.Position.X + en.width + 1, horizontal_pos);

                            hitX = true;
                        }
                        else if (en.Position.X < horizontal_pos + width && en.Position.X > horizontal_pos)
                        {
                            step.X = en.Position.X - width - 1;

                            hitX = true;
                        }
                    }
                    */
                }

                if (hitY)
                {
                    velocity.Y = 0;
                }
                if (hitX)
                {
                    velocity.X = 0;
                }
            }

            return hitY || hitX;
        }

        // pass a map and a vector stating where you'd like to move to
        // this method checks against nearby walls and prevents you from overdoing boundaries
        // this method modifies step
        public void checkForWalls(Map checkMap, ref Vector2 delta)
        {
            if (this.horizontal_pos < 0 || this.vertical_pos < 0 || this.horizontal_pos + this.width >= checkMap.Width * checkMap.TileWidth || this.vertical_pos + this.height >= checkMap.Height * checkMap.TileHeight)
            {
                return;
            }

            foreach (TileLayer layer in checkMap.TileLayers)
            {
                bool isSolid = false;

                foreach (KeyValuePair<string, Property> p in layer.Properties)
                {
                    if (p.Key.Equals("solid") && p.Value.AsInt32 == 1)
                    {
                        isSolid = true;
                    }
                }

                if (isSolid == true)
                {
                    // horizontal map check
                    if (delta.X - horizontal_pos < -0.0001)
                    {
                        int i;
                        for (i = ((int)delta.X) / checkMap.TileWidth; i >= 0; i--)
                        {
                            if (layer.Tiles[i][((int)vertical_pos) / checkMap.TileHeight] != null || layer.Tiles[i][((int)(vertical_pos + height)) / checkMap.TileHeight] != null)
                            {
                                break;
                            }
                        }

                        if ((i + 1) * checkMap.TileWidth > delta.X)
                        {
                            velocity.X = 0.0f;
                        }

                        delta.X = Math.Max(delta.X, (i + 1) * checkMap.TileWidth);
                    }
                    else if (delta.X - horizontal_pos > 0.0001)
                    {
                        int i;
                        for (i = ((int)(delta.X + width)) / checkMap.TileWidth; i < checkMap.Width; i++)
                        {
                            if (layer.Tiles[i][((int)vertical_pos) / checkMap.TileHeight] != null || layer.Tiles[i][((int)(vertical_pos + height)) / checkMap.TileHeight] != null)
                            {
                                break;
                            }
                        }

                        if ((i) * checkMap.TileWidth < delta.X + width)
                        {
                            velocity.X = 0.0f;
                        }

                        delta.X = Math.Min(delta.X, (i - 1) * checkMap.TileWidth);
                    }

                    // vertical map check
                    if (delta.Y - vertical_pos < -0.0001)
                    {
                        int i;
                        for (i = ((int)delta.Y) / checkMap.TileHeight; i >= 0; i--)
                        {
                            if (layer.Tiles[((int)horizontal_pos) / checkMap.TileWidth][i] != null || layer.Tiles[((int)(horizontal_pos + width)) / checkMap.TileWidth][i] != null)
                            {
                                onTheGround = false;

                                break;
                            }
                        }

                        if ((i + 1) * checkMap.TileHeight > delta.Y)
                        {
                            velocity.Y = 0.0f;
                        }

                        delta.Y = Math.Max(delta.Y, (i + 1) * checkMap.TileHeight);
                    }
                    else if (delta.Y - vertical_pos > 0.0001)
                    {
                        int i;
                        for (i = ((int)(delta.Y + height)) / checkMap.TileHeight; i < checkMap.Height; i++)
                        {
                            if (layer.Tiles[((int)horizontal_pos) / checkMap.TileWidth][i] != null || layer.Tiles[((int)(horizontal_pos + width)) / checkMap.TileWidth][i] != null)
                            {
                                break;
                            }
                        }

                        if ((i - 1) * checkMap.TileHeight < delta.Y)
                        {
                            onTheGround = true;
                        }
                        else
                        {
                            onTheGround = false;
                        }

                        //if ((i + 1) * checkMap.TileHeight < delta.Y)
                        if (Math.Min(delta.Y, (i - 1) * checkMap.TileHeight) < delta.Y)
                        {
                            velocity.Y = 0.0f;
                        }

                        delta.Y = Math.Min(delta.Y, (i - 1) * checkMap.TileHeight);
                    }
                }
            }
        }
    }
}
