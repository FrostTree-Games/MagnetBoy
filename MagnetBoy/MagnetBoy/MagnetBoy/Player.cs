using System;
using System.Diagnostics;
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
    class Player: Entity
    {
        string currentAnimation = null;
        int currentFrame = 0;
        double lastFrameIncrement = 0;

        private const float knockBackForce = 0.5f;
        private bool isKnockedBack = false;
        private double knockBackStartTime = 0;

        // angle of window for direction magnetic force
        private const double aimWindow = 1.0471975512;
        private bool isPushing = false;
        private double directionAngle = 0;

        private bool spinning;
        private float spin;
        private float spinVelocity;

        private bool playerBlink = false;
        private double playerBlinkTimer = 0.0;
        private string previousAnimation = null;

        private const float magnetForceValue = 1.0f;

        public Player(float initialx, float initialy)
        {
            creation();

            horizontal_pos = initialx;
            vertical_pos = initialy;

            width = 31.5f;
            height = 31.5f;

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;

            acceleration.Y = 0.001f;

            pole = Polarity.Neutral;
            magneticMoment = 0.5f;

            solid = true;

            spinning = false;
            spin = 0.0f;
            spinVelocity = 0.0f;

            currentAnimation = "playerWalkRight";
        }

        public override void update(GameTime currentTime)
        {
            double delta = currentTime.ElapsedGameTime.Milliseconds;
            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.X))
            {
                pole = Polarity.Positive;
            }
            else if (ks.IsKeyDown(Keys.C))
            {
                pole = Polarity.Negative;
            }
            else
            {
                pole = Polarity.Neutral;
            }

            //reset the acceleration vector and recompute it
            acceleration = Vector2.Zero;
            acceleration.Y = 0.001f;

            // compute cursor spread lines
            //aLine = GameInput.P1MouseDirectionNormal * 100;
            //bLine = GameInput.P1MouseDirectionNormal * 120;
            directionAngle = Math.Atan2(GameInput.P1MouseDirectionNormal.Y, GameInput.P1MouseDirectionNormal.X);

            acceleration = acceleration + computeMagneticForce();

            if (isKnockedBack)
            {
                if (currentTime.TotalGameTime.TotalMilliseconds - knockBackStartTime > 500)
                {
                    currentAnimation = previousAnimation;

                    playerBlink = true;
                    playerBlinkTimer = 0;

                    isKnockedBack = false;
                }
            }

            //Player Blink code
            if(playerBlink == true)
            {
                if (playerBlinkTimer > 2000)
                {
                    playerBlinkTimer = 0.0;
                    playerBlink = false;
                }
                else
                {
                    playerBlinkTimer += currentTime.ElapsedGameTime.Milliseconds;
                }
            }
            
            Vector2 keyAcceleration = Vector2.Zero;
            Vector2 step = new Vector2(horizontal_pos, vertical_pos);

            if ((ks.IsKeyDown(Keys.Right) || GameInput.isButtonDown(GameInput.PlayerButton.RightDirection)) && !isKnockedBack)
            {
                currentAnimation = "playerWalkRight";

                if (velocity.X < 0.1f)
                {
                    keyAcceleration.X = 0.001f;
                }
            }
            else if ((ks.IsKeyDown(Keys.Left) || GameInput.isButtonDown(GameInput.PlayerButton.LeftDirection)) && !isKnockedBack)
            {
                currentAnimation = "playerWalkLeft";

                if (velocity.X > -0.1f)
                {
                    keyAcceleration.X = -0.001f;
                }
            }
            else
            {
                if (Math.Abs(velocity.X) > 0.01 && onTheGround)
                {
                    velocity.X = 0.0f;
                }

                if (currentAnimation == "playerWalkRight")
                {
                    currentAnimation = "playerIdleRight";
                }
                else if (currentAnimation == "playerWalkLeft")
                {
                    currentAnimation = "playerIdleLeft";
                }
            }

            /*
            if (ks.IsKeyDown(Keys.Up) || GameInput.isButtonDown(GameInput.PlayerButton.Jump))
            {
                if (onTheGround)
                {
                    velocity.Y = -0.5f;
                }
            }
            */

            if ((GameInput.P1MouseDown == true || GameInput.isButtonDown(GameInput.PlayerButton.Push)) && LevelState.playerStamina > 0.0f)
            {
                LevelState.playerStamina -= LevelState.playerStaminaDepleteRate;
                if (LevelState.playerStamina < 0.0f)
                {
                    LevelState.playerStamina = 0.0f;
                }

                //normally I'd check if LevelState.levelParticlePool is null, but right now I want the thing crashing if this updates outside of a game loop
                double offset = 0.5 - Game1.gameRandom.NextDouble();
                LevelState.levelParticlePool.pushParticle(ParticlePool.ParticleType.BlueSpark, CenterPosition + new Vector2((float)((width / 2) * Math.Cos(directionAngle)), (float)((width / 2) * Math.Sin(directionAngle))), velocity, (float)(directionAngle + (aimWindow * offset)), (float)directionAngle, Color.White);

                isPushing = true;

                double aAngle = directionAngle - (aimWindow / 2);
                double bAngle = directionAngle + (aimWindow / 2);

                foreach (Entity en in globalEntityList)
                {
                    double distance = Math.Sqrt(Math.Pow(en.Position.X - horizontal_pos, 2) + Math.Pow(en.Position.Y - vertical_pos, 2));

                    if (en == this || distance > 128)
                    {
                        continue;
                    }

                    double enAngle = Math.Atan2(en.CenterPosition.Y - vertical_pos, en.CenterPosition.X - horizontal_pos);

                    if ((enAngle > aAngle && enAngle < bAngle) || distance < 32.0f && distance > 8.0f)
                    {
                        bool flip = false;

                        if (en is Enemy)
                        {
                            Enemy em = (Enemy)en;

                            em.PushTime = 500;
                        }

                        if (en is ShieldDude)
                        {
                            Vector2 pushDir = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle));
                            ShieldDude dude = (ShieldDude)en;

                            pushDir.X *= -1;

                            // geometric property of dot product
                            double angleBetweenForceAndShield = Math.Acos(Vector2.Dot(dude.ShieldDir, pushDir) / (dude.ShieldDir.Length() * pushDir.Length()));

                            if (angleBetweenForceAndShield < Math.PI / 8)
                            {
                                flip = true;
                            }
                        }

                        double force = (magneticMoment * en.MagneticValue.Value) / (4 * Math.PI * Math.Pow(distance, 2));
                        double angle = Math.Atan2(en.Position.X - horizontal_pos, vertical_pos - en.Position.Y);

                        Vector2 newForce = new Vector2((float)(force * Math.Cos(angle - (Math.PI / 2))), (float)(force * Math.Sin(angle - (Math.PI / 2))));

                        if (en is Bullet)
                        {
                            ((Bullet)en).Direction = (float)(angle - (Math.PI/2));
                            ((Bullet)en).playerEnact = true;
                        }

                        if (flip == false)
                        {
                            en.velocity += newForce * 10000;
                        }
                        else
                        {
                            velocity += Vector2.Negate(newForce) * 10000;
                        }
                    }
                }

                //wall-pushing
                {
                    foreach (TileLayer layer in LevelState.CurrentLevel.TileLayers)
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
                            Vector2 point = CenterPosition;
                            bool tileFound = false;

                            /*for (int j = 0; j < 3; j++)
                            {
                                point = CenterPosition;
                                if (j == 1)
                                {
                                    if ((-Math.PI < directionAngle && directionAngle < -Math.PI / 2) || (-Math.PI/2 < directionAngle && directionAngle < 0))
                                    {
                                        point.Y = CenterPosition.X - (height/2);
                                    }
                                    else if ((Math.PI > directionAngle && directionAngle > Math.PI / 2) || (Math.PI / 2 > directionAngle && directionAngle > 0))
                                    {
                                        point.Y = CenterPosition.X + (height/2);
                                    }

                                }

                                if (j == 2)
                                {
                                    if ((-Math.PI < directionAngle && directionAngle < -Math.PI / 2) || (Math.PI > directionAngle && directionAngle > Math.PI / 2))
                                    {
                                        point.X = CenterPosition.X - (width / 2);
                                    }
                                    else if ((-Math.PI / 2 < directionAngle && directionAngle < 0) || (Math.PI / 2 > directionAngle && directionAngle > 0))
                                    {
                                        point.X = CenterPosition.X + (width / 2);
                                    }

                                }*/
                                for (int i = 0; ; i++)
                                {
                                    int r = i * 4;
                                    double xPos = r * Math.Cos(directionAngle);
                                    double yPos = r * Math.Sin(directionAngle);
                                    
                                    xPos += CenterPosition.X;
                                    yPos += CenterPosition.Y;

                                    if (xPos < 0 || yPos < 0 || xPos / LevelState.CurrentLevel.TileWidth >= LevelState.CurrentLevel.Width || yPos / LevelState.CurrentLevel.TileHeight >= LevelState.CurrentLevel.Height)
                                    {
                                        break;
                                    }

                                    try
                                    {
                                        if (layer.Tiles[(int)(xPos / LevelState.CurrentLevel.TileWidth)][(int)(yPos / LevelState.CurrentLevel.TileHeight)] != null)
                                        {
                                            tileFound = true;
                                            point.X = (float)xPos;
                                            point.Y = (float)yPos;

                                            break;
                                        }
                                    }
                                    catch (IndexOutOfRangeException)
                                    {
                                        break;
                                    }
                                }
                            //}

                            if (tileFound)
                            {
                                double distance = Math.Sqrt(Math.Pow(point.X - CenterPosition.X, 2) + Math.Pow(point.Y - CenterPosition.Y, 2));
                                double force = (magneticMoment * 1050) / (4 * Math.PI * Math.Pow(distance, 2));
                                double angle = Math.Atan2(point.X - CenterPosition.X, CenterPosition.X - point.Y);

                                Vector2 newForce = new Vector2((float)(force * Math.Cos(directionAngle)), (float)(force * Math.Sin(directionAngle)));

                                velocity += newForce * -1 * magnetForceValue;

                                Vector2.Clamp(velocity, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
                            }
                        }
                    }
                }
            }
            else if (!(GameInput.P1MouseDown == true || GameInput.isButtonDown(GameInput.PlayerButton.Push)))
            {
                isPushing = false;

                LevelState.playerStamina += LevelState.playerStaminaGrowthRate;

                if (LevelState.playerStamina > 100.0f)
                {
                    LevelState.playerStamina = 100.0f;
                }
            }
            else
            {
                isPushing = false;
            }

            //killing enemies
            foreach (Entity en in globalEntityList)
            {
                if (en is Enemy)
                {
                    if (hitTest(en))
                    {
                        if (!onTheGround && velocity.Y > 0.001f && vertical_pos < en.Position.Y)
                        {
                            if (!en.deathAnimation)
                            {
                                velocity.Y *= -1.1f;
                                en.deathAnimation = true;
                            }
                        }
                    }
                }
            }

            Vector2 finalAcceleration = acceleration + keyAcceleration;

            velocity.X += (float)(finalAcceleration.X * delta);
            velocity.Y += (float)(finalAcceleration.Y * delta);

            step.X += (float)((((velocity.X + conveyer.X)) * delta) + (0.5) * (Math.Pow(delta, 2.0)) * finalAcceleration.X);
            step.Y += (float)((((velocity.Y + conveyer.Y)) * delta) + (0.5) * (Math.Pow(delta, 2.0)) * finalAcceleration.Y);

            checkForWalls(LevelState.CurrentLevel, ref step);
            checkForSolidObjects(ref step);

            horizontal_pos = step.X;
            vertical_pos = step.Y;

            if (onTheGround || isKnockedBack)
            {
                spinning = false;

                float targetRotation = 0.0f;

                if (targetRotation > spin + Math.PI)
                {
                    targetRotation -= (float)(2 * Math.PI);
                }
                else if (targetRotation < spin - Math.PI)
                {
                    targetRotation += (float)(2 * Math.PI);
                }

                spin = MathHelper.Lerp(targetRotation, spin, 0.5f);
            }
            else if (!onTheGround && velocity.Y < 0f)
            {
                spinning = true;
                spinVelocity = velocity.X;
            }

            if (spinning)
            {
                spin += spinVelocity;
                spin = MathHelper.WrapAngle(spin);
            }

            // if the last frame time hasn't been set, set it now
            if (lastFrameIncrement == 0)
            {
                lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;
            }

            // update the current frame if needed
            if (currentTime.TotalGameTime.TotalMilliseconds - lastFrameIncrement > AnimationFactory.getAnimationSpeed(currentAnimation))
            {
                lastFrameIncrement = currentTime.TotalGameTime.TotalMilliseconds;

                currentFrame = (currentFrame + 1) % AnimationFactory.getAnimationFrameCount(currentAnimation);
            }
        }

        public override void draw(SpriteBatch sb)
        {
            if (!(playerBlink && ((int)(playerBlinkTimer / 100) % 2 == 0)))
            {
                AnimationFactory.drawAnimationFrame(sb, currentAnimation, currentFrame, Position, new Vector2(32,32), spin, AnimationFactory.DepthLayer1);
            }

            if (isPushing)
            {
                Vector2 dirOffset = Position;
                dirOffset.X -= width * 1.5f;
                dirOffset.Y -= height/2;
            }
        }

        public void knockBack(Vector2 direction, double hitTime)
        {
            if (playerBlink == false)
            {
                if (isKnockedBack)
                {
                    if (velocity.X < 0)
                    {
                        currentAnimation = "playerHurtLeft";
                    }
                    else
                    {
                        currentAnimation = "playerHurtRight";
                    }
                    return;
                }

                LevelState.levelParticlePool.pushParticle(ParticlePool.ParticleType.SweatDrops, new Vector2(horizontal_pos + width / 2, vertical_pos), Vector2.Zero, (float)((7 * (Math.PI / 4)) + (direction.X / 10)), 0.0f, Color.White);
                LevelState.levelParticlePool.pushParticle(ParticlePool.ParticleType.SweatDrops, new Vector2(horizontal_pos + width / 2, vertical_pos), Vector2.Zero, (float)((5.4 * (Math.PI / 4)) + (direction.X / 10)), 0.0f, Color.White);
                LevelState.levelParticlePool.pushParticle(ParticlePool.ParticleType.SweatDrops, new Vector2(horizontal_pos + width / 2, vertical_pos), Vector2.Zero, (float)((5 * (Math.PI / 4)) + (direction.X / 10)), 0.0f, Color.White);

                isKnockedBack = true;
                knockBackStartTime = hitTime;

                onTheGround = false;

                if (direction.X > 0)
                {
                    velocity.X = 0.25f;
                }
                else
                {
                    velocity.X = -0.25f;
                }

                if (direction.Y > 0)
                {
                    velocity.Y = 0.35f;
                }
                else
                {
                    velocity.Y = -0.35f;
                }

                LevelState.currentPlayerHealth = LevelState.currentPlayerHealth - 1;
                previousAnimation = currentAnimation;

                if (velocity.X < 0)
                {
                    currentAnimation = "playerHurtLeft";
                }
                else
                {
                    currentAnimation = "playerHurtRight";
                }
                
            }
        }
    }
}
