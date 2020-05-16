using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PenPop.GameObject
{
    class Bubble : GameObject
    {
        public bool isHitCeiling;
        public bool IsDead;
        int movechk = 0;

        public float Speed;
        public float radians;
        public int bubbleposX;
        public int bubbleposY;

        Vector2 ClickPoint;
        public enum BubbleType
        {
            R,
            G,
            B,
            Y,
            SIZE
        }
        public Vector2 Pieces;

        public BubbleType CurrentBubbleType;

        public Bubble(Texture2D texture) : base(texture)
        {
            Pieces = new Vector2();
            isHitCeiling = false;
            IsDead = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (Singleton.Instance.CurrentClick.Position.X < 0 || Singleton.Instance.CurrentClick.Position.X > Singleton.GAMEWIDTH * Singleton.TILESIZE || Singleton.Instance.CurrentClick.Position.Y > Singleton.GAMEHEIGHT * Singleton.TILESIZE || Singleton.Instance.CurrentClick.Position.Y < 0)
            {

            }
            else if (Singleton.Instance.CurrentClick.LeftButton == ButtonState.Pressed && !Singleton.Instance.CurrentClick.Equals(Singleton.Instance.PreviousKey))
            {
                ClickPoint = new Vector2(Singleton.Instance.CurrentClick.Position.X, Singleton.Instance.CurrentClick.Position.Y);
                radians = (float)Math.Atan2(ClickPoint.Y - ((Singleton.GAMEHEIGHT) * Singleton.TILESIZE + (Singleton.TILESIZE / 2)), ClickPoint.X - (Singleton.GAMEWIDTH * Singleton.TILESIZE / 2));
                Velocity.X = (float)Math.Cos(radians) * Speed;
                Velocity.Y = (float)Math.Sin(radians) * Speed;
            }
            if (Singleton.Instance.CurrentClick.LeftButton == ButtonState.Pressed || movechk == 1)
            {
                if (Position.X < (Singleton.GAMEWIDTH - 1) * Singleton.TILESIZE && Position.X > 0)
                {
                    movechk = 1;
                    Position += Velocity * gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
                }
                else if (Position.X >= (Singleton.GAMEWIDTH - 1) * Singleton.TILESIZE)
                {
                    radians = (float)Math.PI - radians;
                    movechk = 1;
                    Velocity.X = (float)Math.Cos(radians) * Speed;
                    Velocity.Y = (float)Math.Sin(radians) * Speed;
                    Position += Velocity * gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
                }
                else if (Position.X <= 0)
                {
                    radians = (float)Math.PI - radians;
                    movechk = 1;
                    Velocity.X = (float)Math.Cos(radians) * Speed;
                    Velocity.Y = (float)Math.Sin(radians) * Speed;
                    Position += Velocity * gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
                }
            }
            if(Position.Y < Singleton.GAMEHEIGHT * Singleton.TILESIZE)
            {
                bubbleposX = -1;
                bubbleposY = -1;
                switch (((int)Position.Y / Singleton.TILESIZE - Singleton.Instance.Ceilingrowpos) % 2)
                {
                    case 1:
                      
                        if ((int)Position.X < Singleton.TILESIZE / 2) bubbleposX = 0;
                        else if ((int)Position.X % Singleton.TILESIZE >= Singleton.TILESIZE / 2)
                        {
                            bubbleposX = (((int)Position.X) / Singleton.TILESIZE) + 1;
                        }
                        else if ((int)Position.X % Singleton.TILESIZE < Singleton.TILESIZE / 2) bubbleposX = (((int)Position.X) / Singleton.TILESIZE);
                        if (bubbleposX == -1) break;
                        bubbleposY = (int)Position.Y / Singleton.TILESIZE + 1 - Singleton.Instance.Ceilingrowpos;
                        if (bubbleposY == 10) break;
                        switch (bubbleposX)
                        {
                            case 0:
                                if (Position.Y + 5 < Singleton.Instance.Ceiling / Singleton.Instance.CeilingSpeed)
                                {
                                    Singleton.Instance.GameBoard[bubbleposX, 0] = (int)CurrentBubbleType;
                                    isHitCeiling = true;
                                }
                                else if (bubbleposY < Singleton.GAMEHEIGHT - 1 - Singleton.Instance.Ceilingrowpos)
                                {
                                    if (Singleton.Instance.GameBoard[bubbleposX + 1, bubbleposY] < 4 || Singleton.Instance.GameBoard[bubbleposX, bubbleposY - 1] < 4)
                                    {
                                        Singleton.Instance.GameBoard[bubbleposX, bubbleposY] = (int)CurrentBubbleType;
                                        isHitCeiling = true;
                                    }
                                }
                                else if (Singleton.Instance.GameBoard[bubbleposX + 1, bubbleposY] < 4 || Singleton.Instance.GameBoard[bubbleposX, bubbleposY - 1] < 4)
                                {
                                    Singleton.Instance.GameBoard[bubbleposX, bubbleposY] = (int)CurrentBubbleType;
                                    isHitCeiling = true;
                                }
                                break;
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                                if (Position.Y + 5 < Singleton.Instance.Ceiling / Singleton.Instance.CeilingSpeed)
                                {
                                    Singleton.Instance.GameBoard[bubbleposX, 0] = (int)CurrentBubbleType;
                                    isHitCeiling = true;
                                }
                                else if (bubbleposY < Singleton.GAMEHEIGHT - 1 - Singleton.Instance.Ceilingrowpos)
                                {
                                    if (Singleton.Instance.GameBoard[bubbleposX - 1, bubbleposY] < 4 || Singleton.Instance.GameBoard[bubbleposX + 1, bubbleposY] < 4 || Singleton.Instance.GameBoard[bubbleposX - 1, bubbleposY - 1] < 4 || Singleton.Instance.GameBoard[bubbleposX, bubbleposY - 1] < 4 /*|| Singleton.Instance.GameBoard[bubbleposX, bubbleposY + 1] < 4 || Singleton.Instance.GameBoard[bubbleposX + 1, bubbleposY + 1] < 4*/)
                                    {
                                        Singleton.Instance.GameBoard[bubbleposX, bubbleposY] = (int)CurrentBubbleType;
                                        isHitCeiling = true;
                                    }
                                }
                                else if (Singleton.Instance.GameBoard[bubbleposX - 1, bubbleposY] < 4 || Singleton.Instance.GameBoard[bubbleposX + 1, bubbleposY] < 4 || Singleton.Instance.GameBoard[bubbleposX - 1, bubbleposY - 1] < 4 || Singleton.Instance.GameBoard[bubbleposX, bubbleposY - 1] < 4)
                                {
                                    Singleton.Instance.GameBoard[bubbleposX, bubbleposY] = (int)CurrentBubbleType;
                                    isHitCeiling = true;
                                }
                                break;

                            case 7:

                                if (Position.Y + 5 < Singleton.Instance.Ceiling / Singleton.Instance.CeilingSpeed)
                                {
                                    Singleton.Instance.GameBoard[bubbleposX, 0] = (int)CurrentBubbleType;
                                    isHitCeiling = true;
                                }
                                else if (bubbleposY < Singleton.GAMEHEIGHT - 1 - Singleton.Instance.Ceilingrowpos)
                                {
                                    if (Singleton.Instance.GameBoard[bubbleposX - 1, bubbleposY] < 4 || Singleton.Instance.GameBoard[bubbleposX - 1, bubbleposY - 1] < 4)
                                    {
                                        Singleton.Instance.GameBoard[bubbleposX, bubbleposY] = (int)CurrentBubbleType;
                                        isHitCeiling = true;
                                    }
                                }
                                else if (Singleton.Instance.GameBoard[bubbleposX - 1, bubbleposY] < 4 || Singleton.Instance.GameBoard[bubbleposX - 1, bubbleposY - 1] < 4)
                                {
                                    Singleton.Instance.GameBoard[bubbleposX, bubbleposY] = (int)CurrentBubbleType;
                                    isHitCeiling = true;
                                }
                                break;
                        }

                        break;


                    case 0:
                        bubbleposX = -1;
                        if ((int)Position.X < Singleton.TILESIZE) bubbleposX = 0;
                        else if ((((int)Position.X - Singleton.TILESIZE / 2) / Singleton.TILESIZE) % Singleton.TILESIZE >= Singleton.TILESIZE / 2)
                        {
                            bubbleposX = (((int)Position.X) / Singleton.TILESIZE) + 1;
                        }
                        else if ((((int)Position.X - Singleton.TILESIZE / 2) / Singleton.TILESIZE) % Singleton.TILESIZE < Singleton.TILESIZE / 2) bubbleposX = (((int)Position.X) / Singleton.TILESIZE);
                        if (bubbleposX == -1) break;
                        bubbleposY = (int)Position.Y / Singleton.TILESIZE + 1 - Singleton.Instance.Ceilingrowpos;
                        switch (bubbleposX)
                        {
                            case 0:
                                if (Position.Y + 5 < Singleton.Instance.Ceiling / Singleton.Instance.CeilingSpeed)
                                {
                                    Singleton.Instance.GameBoard[bubbleposX, 0] = (int)CurrentBubbleType;
                                    isHitCeiling = true;
                                }
                                else if (bubbleposY < Singleton.GAMEHEIGHT - 1 - Singleton.Instance.Ceilingrowpos)
                                {
                                    if (Singleton.Instance.GameBoard[bubbleposX + 1, bubbleposY] < 4 || Singleton.Instance.GameBoard[bubbleposX, bubbleposY - 1] < 4 || Singleton.Instance.GameBoard[bubbleposX + 1, bubbleposY - 1] < 4 /*|| Singleton.Instance.GameBoard[bubbleposX, bubbleposY] < 4 || Singleton.Instance.GameBoard[bubbleposX + 1, bubbleposY + 1] < 4*/)
                                    {
                                        Singleton.Instance.GameBoard[bubbleposX, bubbleposY] = (int)CurrentBubbleType;
                                        isHitCeiling = true;
                                    }
                                }
                                else if (Singleton.Instance.GameBoard[bubbleposX + 1, bubbleposY] < 4 || Singleton.Instance.GameBoard[bubbleposX, bubbleposY - 1] < 4 || Singleton.Instance.GameBoard[bubbleposX + 1, bubbleposY - 1] < 4)
                                {
                                    Singleton.Instance.GameBoard[bubbleposX, bubbleposY] = (int)CurrentBubbleType;
                                    isHitCeiling = true;
                                }
                                break;
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                                if (Position.Y + 5 < Singleton.Instance.Ceiling / Singleton.Instance.CeilingSpeed)
                                {
                                    Singleton.Instance.GameBoard[bubbleposX, 0] = (int)CurrentBubbleType;
                                    isHitCeiling = true;
                                }
                                else if (bubbleposY < Singleton.GAMEHEIGHT - 1 - Singleton.Instance.Ceilingrowpos)
                                {
                                    if (Singleton.Instance.GameBoard[bubbleposX - 1, bubbleposY] < 4 || Singleton.Instance.GameBoard[bubbleposX + 1, bubbleposY] < 4 || Singleton.Instance.GameBoard[bubbleposX, bubbleposY - 1] < 4 || Singleton.Instance.GameBoard[bubbleposX + 1, bubbleposY - 1] < 4 /*|| Singleton.Instance.GameBoard[bubbleposX, bubbleposY + 1] < 4 || Singleton.Instance.GameBoard[bubbleposX + 1, bubbleposY + 1] < 4*/)
                                    {
                                        Singleton.Instance.GameBoard[bubbleposX, bubbleposY] = (int)CurrentBubbleType;
                                        isHitCeiling = true;
                                    }
                                }
                                else if (Singleton.Instance.GameBoard[bubbleposX - 1, bubbleposY] < 4 || Singleton.Instance.GameBoard[bubbleposX + 1, bubbleposY] < 4 || Singleton.Instance.GameBoard[bubbleposX, bubbleposY - 1] < 4 || Singleton.Instance.GameBoard[bubbleposX + 1, bubbleposY - 1] < 4)
                                {
                                    Singleton.Instance.GameBoard[bubbleposX, bubbleposY] = (int)CurrentBubbleType;
                                    isHitCeiling = true;
                                }
                                break;
                        }

                        break;
                }
            }

                base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            switch (CurrentBubbleType)
            {
                case BubbleType.R:
                    spriteBatch.Draw(_texture, Pieces + Position, null, Color.Red);
                    break;
                case BubbleType.G:
                    spriteBatch.Draw(_texture, Pieces + Position, null, Color.Green);
                    break;
                case BubbleType.B:
                    spriteBatch.Draw(_texture, Pieces + Position, null, Color.Blue);
                    break;
                case BubbleType.Y:
                    spriteBatch.Draw(_texture, Pieces + Position, null, Color.Yellow);
                    break;
            }
            base.Draw(spriteBatch);
        }

        public override void Reset()
        {
            CurrentBubbleType = (BubbleType)(Singleton.Instance.Random.Next((int)BubbleType.SIZE));
            Pieces = new Vector2(0, 0);
            Speed = 1000;
            base.Reset();
        }


    }
}
