using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PenPop.GameObject;
using System;

namespace PenPop
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Mainscene : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D _rectTexture;
        Texture2D ceiling;
        Texture2D background;
        Texture2D floor;
        Texture2D cursor;
        SpriteFont _font;
        Texture2D snowball;
        Texture2D pen;
        Texture2D pen1;
        Texture2D questpen;


        Vector2 MousePos;

        Bubble _currentBubble, _nextBubble;
        Random rand = new Random();
        public Mainscene()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = Singleton.SCREENWIDTH * Singleton.TILESIZE;
            graphics.PreferredBackBufferHeight = Singleton.SCREENHEIGHT * Singleton.TILESIZE;
            graphics.ApplyChanges();
            IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _rectTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            // TODO: use this.Content to load your game content here
            Color[] data = new Color[1 * 1];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
            _rectTexture.SetData(data);
            cursor = this.Content.Load<Texture2D>("Guildeline");
            background = this.Content.Load<Texture2D>("BGG");
            pen = this.Content.Load<Texture2D>("pen");
            pen1 = this.Content.Load<Texture2D>("pen1");
            _font = Content.Load<SpriteFont>("GameFont");
            ceiling = this.Content.Load<Texture2D>("ice pathtopwall");
            snowball = this.Content.Load<Texture2D>("snowball");
            floor = this.Content.Load<Texture2D>("floor");
            questpen = this.Content.Load<Texture2D>("penguinQ");

            Reset();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Singleton.Instance.CurrentKey = Keyboard.GetState();
            Singleton.Instance.CurrentClick = Mouse.GetState();
            switch (Singleton.Instance.CurrentGameState)
            {
                case Singleton.GameState.GamePlaying:
                    MousePos = new Vector2(Singleton.Instance.CurrentClick.Position.X, Singleton.Instance.CurrentClick.Position.Y);
                    // handle the pause
                    if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.Escape) && !Singleton.Instance.CurrentKey.Equals(Singleton.Instance.PreviousKey))
                        Singleton.Instance.CurrentGameState = Singleton.GameState.GamePaused;
                    // TODO: Add your update logic here
                    _currentBubble.Update(gameTime);
                    Singleton.Instance.PreviousCursorRadians = Singleton.Instance.CurrentCursorRadians;
                    if(Singleton.Instance.CurrentClick.Position.X >= 0 && Singleton.Instance.CurrentClick.Position.X <= Singleton.GAMEWIDTH * Singleton.TILESIZE && Singleton.Instance.CurrentClick.Position.Y <= Singleton.GAMEHEIGHT * Singleton.TILESIZE && Singleton.Instance.CurrentClick.Position.Y >= 0)
                        Singleton.Instance.CurrentCursorRadians = (float)Math.Atan2(MousePos.Y - ((Singleton.GAMEHEIGHT) * Singleton.TILESIZE + (Singleton.TILESIZE / 2)), MousePos.X - (Singleton.GAMEWIDTH * Singleton.TILESIZE / 2));
                    if (Singleton.Instance.questComplete == false)
                    {
                        Singleton.Instance.Ceiling += 100 * gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
                        if (Singleton.Instance.Ceiling > 1000 * (Singleton.Instance.Ceilingrowpos + 1))
                        {
                            Singleton.Instance.Ceilingrowpos++;
                        }
                    }

                    if (_currentBubble.isHitCeiling)
                    {
                        int[,] TempGameBoard = new int[Singleton.GAMEWIDTH, Singleton.GAMEHEIGHT];
                        for (int i = 0; i < Singleton.GAMEHEIGHT; i++)
                        {
                            for (int j = 0; j < Singleton.GAMEWIDTH; j++)
                            {
                                TempGameBoard[j, i] = Singleton.Instance.GameBoard[j, i];
                            }
                        }
                        Singleton.Instance.samebubble = 1;
                        ChkDeleteLeftBubble(_currentBubble.bubbleposX, _currentBubble.bubbleposY);
                        ChkDeleteRightBubble(_currentBubble.bubbleposX, _currentBubble.bubbleposY);
                        ChkDeleteLowerBubble(_currentBubble.bubbleposX, _currentBubble.bubbleposY);
                        ChkDeleteUpperBubble(_currentBubble.bubbleposX, _currentBubble.bubbleposY);
                        if (Singleton.Instance.samebubble > 2)
                        {
                            Singleton.Instance.samebubble = 1;
                            DeleteLeftBubble(_currentBubble.bubbleposX, _currentBubble.bubbleposY);
                            DeleteRightBubble(_currentBubble.bubbleposX, _currentBubble.bubbleposY);
                            DeleteLowerBubble(_currentBubble.bubbleposX, _currentBubble.bubbleposY);
                            DeleteUpperBubble(_currentBubble.bubbleposX, _currentBubble.bubbleposY);
                            Singleton.Instance.GameBoard[_currentBubble.bubbleposX, _currentBubble.bubbleposY] = 4;
                            Singleton.Instance.CountDeleteBubble = 0;
                            ChkFallfromUpperBubble();
                        }
                        Singleton.Instance.CountDeleteBubble = 0;
                        for (int i = 0; i < Singleton.GAMEHEIGHT; i++)
                        {
                            for (int j = 0; j < Singleton.GAMEWIDTH; j++)
                            {
                                if (TempGameBoard[j, i] != Singleton.Instance.GameBoard[j, i] && Singleton.Instance.GameBoard[j, i] > 3)
                                {
                                    Singleton.Instance.CountDeleteBubble++;
                                    
                                }
                            }
                        }
                        if (Singleton.Instance.CurrentQuestReward == Singleton.QuestReward.CeilingStopNextTurn)
                        {
                            if (Singleton.Instance.questComplete == true)
                            {
                                Singleton.Instance.questComplete = false;
                                Singleton.Instance.CurrentQuestState = Singleton.QuestState.NotPopUp;
                                Singleton.Instance.CurrentQuestList = Singleton.QuestList.SIZE;
                                Singleton.Instance.CurrentQuestReward = Singleton.QuestReward.SIZE;
                            }
                            else if (Singleton.Instance.CountDeleteBubble <= 2)
                            {
                                Singleton.Instance.questComplete = false;
                                Singleton.Instance.CurrentQuestState = Singleton.QuestState.NotPopUp;
                                Singleton.Instance.CurrentQuestList = Singleton.QuestList.SIZE;
                                Singleton.Instance.CurrentQuestReward = Singleton.QuestReward.SIZE;
                            }
                            else if (Singleton.Instance.CurrentQuestReward == Singleton.QuestReward.CeilingStopNextTurn && Singleton.Instance.CurrentQuestState == Singleton.QuestState.PopUp && Singleton.Instance.CountDeleteBubble > 2 && Singleton.Instance.CurrentQuestList == Singleton.QuestList.BurstAnyBubble)
                            {
                                Singleton.Instance.questComplete = true;
                            }
                            else if (Singleton.Instance.CurrentQuestReward == Singleton.QuestReward.CeilingStopNextTurn && Singleton.Instance.CurrentQuestState == Singleton.QuestState.PopUp && Singleton.Instance.CountDeleteBubble > 3 && Singleton.Instance.CurrentQuestList == Singleton.QuestList.BurstMoreThreeBubble)
                            {
                                Singleton.Instance.questComplete = true;

                            }
                        }
                        else if (Singleton.Instance.CurrentQuestReward == Singleton.QuestReward.GetExtraScore)
                        {
                            if (Singleton.Instance.CurrentQuestReward == Singleton.QuestReward.GetExtraScore && Singleton.Instance.CurrentQuestState != Singleton.QuestState.NotPopUp && Singleton.Instance.CountDeleteBubble > 2 && Singleton.Instance.CurrentQuestList == Singleton.QuestList.BurstAnyBubble)
                            {
                                Singleton.Instance.Score += 500;
                                Singleton.Instance.CurrentQuestState = Singleton.QuestState.NotPopUp;
                                Singleton.Instance.CurrentQuestList = Singleton.QuestList.SIZE;
                                Singleton.Instance.CurrentQuestReward = Singleton.QuestReward.SIZE;
                            }
                            else if (Singleton.Instance.CurrentQuestReward == Singleton.QuestReward.GetExtraScore && Singleton.Instance.CurrentQuestState != Singleton.QuestState.NotPopUp && Singleton.Instance.CountDeleteBubble > 3 && Singleton.Instance.CurrentQuestList == Singleton.QuestList.BurstMoreThreeBubble)
                            {
                                Singleton.Instance.Score += 500 * (1 + ((float)Singleton.Instance.CountDeleteBubble / 10));
                                Singleton.Instance.CurrentQuestState = Singleton.QuestState.NotPopUp;
                                Singleton.Instance.CurrentQuestList = Singleton.QuestList.SIZE;
                                Singleton.Instance.CurrentQuestReward = Singleton.QuestReward.SIZE;
                            }
                            else
                            {
                                Singleton.Instance.questComplete = false;
                                Singleton.Instance.CurrentQuestState = Singleton.QuestState.NotPopUp;
                                Singleton.Instance.CurrentQuestList = Singleton.QuestList.SIZE;
                                Singleton.Instance.CurrentQuestReward = Singleton.QuestReward.SIZE;
                            }
                        }


                        if (Singleton.Instance.CurrentQuestState == Singleton.QuestState.NotPopUp)
                        {
                            int randomGetQuest = rand.Next(4);
                            if (randomGetQuest == 0 && Singleton.Instance.CurrentQuestState != Singleton.QuestState.PopUp)
                            {
                                Singleton.Instance.CurrentQuestState = Singleton.QuestState.PopUp;
                            }
                            if (Singleton.Instance.CurrentQuestState == Singleton.QuestState.PopUp)
                            {
                                int randomQuestList = rand.Next((int)Singleton.QuestList.SIZE);
                                int randomQuestReward = rand.Next((int)Singleton.QuestReward.SIZE);
                                switch (randomQuestList)
                                {
                                    case 0:
                                        switch (randomQuestReward)
                                        {
                                            case 0:
                                                Singleton.Instance.CurrentQuestReward = Singleton.QuestReward.CeilingStopNextTurn;
                                                Singleton.Instance.questComplete = false;
                                                break;
                                            case 1:
                                                Singleton.Instance.CurrentQuestReward = Singleton.QuestReward.GetExtraScore;
                                                Singleton.Instance.questComplete = false;
                                                break;
                                        }
                                        //}
                                        Singleton.Instance.CurrentQuestList = Singleton.QuestList.BurstAnyBubble;
                                        break;
                                    case 1:
                                        switch (randomQuestReward)
                                        {
                                            case 0:
                                                Singleton.Instance.CurrentQuestReward = Singleton.QuestReward.CeilingStopNextTurn;
                                                Singleton.Instance.questComplete = false;
                                                break;
                                            case 1:
                                                Singleton.Instance.CurrentQuestReward = Singleton.QuestReward.GetExtraScore;
                                                Singleton.Instance.questComplete = false;
                                                break;
                                        }
                                        Singleton.Instance.CurrentQuestList = Singleton.QuestList.BurstMoreThreeBubble;
                                        break;
                                }
                            }
                        }

                        Singleton.Instance.Score += ((Singleton.Instance.CountDeleteBubble * 100) * (1 + ((float)Singleton.Instance.CountDeleteBubble / 10)));
                        bool gameclear = true;
                        for (int i = 0; i < Singleton.GAMEHEIGHT; i++)
                        {
                            for (int j = 0; j < Singleton.GAMEWIDTH; j++)
                            {
                                if (Singleton.Instance.GameBoard[j, i] < 4) 
                                {
                                    gameclear = false;
                                }
                            }
                            if (i % 2 == 1) Singleton.Instance.GameBoard[Singleton.GAMEWIDTH - 1, i] = 4;

                        }
                        if (gameclear) Singleton.Instance.CurrentGameState = Singleton.GameState.GameClear;
                        _currentBubble = _nextBubble;
                        _currentBubble.Position = new Vector2(Singleton.GAMEWIDTH * Singleton.TILESIZE / 2 - Singleton.TILESIZE / 2, Singleton.GAMEHEIGHT * Singleton.TILESIZE);

                        _nextBubble = new Bubble(snowball)
                        {
                            Position = new Vector2(Singleton.GAMEWIDTH * Singleton.TILESIZE / 2 - Singleton.TILESIZE * 3, (Singleton.SCREENHEIGHT - 2) * Singleton.TILESIZE)
                        };
                        _nextBubble.Reset();
                    }
                    for (int i = 0; i < Singleton.GAMEWIDTH; i++)
                    {
                        if (Singleton.Instance.GameBoard[i, Singleton.GAMEHEIGHT - Singleton.Instance.Ceilingrowpos - 1] < 4)
                            Singleton.Instance.CurrentGameState = Singleton.GameState.GameEnded;
                    }

                    break;

                    case Singleton.GameState.GamePaused:
                    // unpause
                        if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.Escape) && !Singleton.Instance.CurrentKey.Equals(Singleton.Instance.PreviousKey))
                            Singleton.Instance.CurrentGameState = Singleton.GameState.GamePlaying;
                        break;

                    case Singleton.GameState.GameEnded:
                    case Singleton.GameState.GameClear:
                    // handle the input
                    if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.Space) && !Singleton.Instance.CurrentKey.Equals(Singleton.Instance.PreviousKey))
                        {
                            Singleton.Instance.Ceiling = 0;
                            Singleton.Instance.Ceilingrowpos = 0;
                            Singleton.Instance.CurrentGameState = Singleton.GameState.GamePlaying;
                            Reset();
                        }
                        break;


            }
            Singleton.Instance.PreviousKey = Singleton.Instance.CurrentKey;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            // TODO: Add your drawing code here
            
            spriteBatch.Draw(background, new Vector2(0,0));
            spriteBatch.Draw(ceiling, new Vector2(0, -500+Singleton.Instance.Ceiling/Singleton.Instance.CeilingSpeed));
            spriteBatch.Draw(floor, new Vector2(0, 500));
            spriteBatch.Draw(pen, new Vector2(225, 500));
            spriteBatch.Draw(pen1, new Vector2(0,500));




            spriteBatch.Draw(cursor, new Vector2(Singleton.GAMEWIDTH * Singleton.TILESIZE / 2, Singleton.GAMEHEIGHT * Singleton.TILESIZE + Singleton.TILESIZE / 2), null, null, new Vector2(cursor.Width / 2, cursor.Height), Singleton.Instance.CurrentCursorRadians + -(float)Math.PI * 3/2, null, Color.White, SpriteEffects.None, 0);//add...................

           
            _currentBubble.Draw(spriteBatch);
            _nextBubble.Draw(spriteBatch);
            if (Singleton.Instance.CurrentQuestState == Singleton.QuestState.PopUp && Singleton.Instance.questComplete == false)
            {
                spriteBatch.Draw(_rectTexture, new Vector2(0, 549), null, Color.CornflowerBlue, 0f, Vector2.Zero, Singleton.SCREENWIDTH * Singleton.TILESIZE, SpriteEffects.None, 0);
                spriteBatch.Draw(questpen, new Vector2(0, 525));
            }


            for (int i = 0; i < Singleton.GAMEWIDTH; i++)
            {
                for (int j = 0; j < Singleton.GAMEHEIGHT; j++)
                {
                    if (j % 2 == 0) {
                        switch (Singleton.Instance.GameBoard[i, j]) 
                        {
                            case 0:
                                spriteBatch.Draw(snowball, new Vector2(i * Singleton.TILESIZE, j * Singleton.TILESIZE + Singleton.Instance.Ceiling / Singleton.Instance.CeilingSpeed), null, Color.Red);
                                break;
                            case 1:
                                spriteBatch.Draw(snowball, new Vector2(i * Singleton.TILESIZE, j * Singleton.TILESIZE + Singleton.Instance.Ceiling / Singleton.Instance.CeilingSpeed), null, Color.Green);
                                break;
                            case 2:
                                spriteBatch.Draw(snowball, new Vector2(i * Singleton.TILESIZE, j * Singleton.TILESIZE + Singleton.Instance.Ceiling / Singleton.Instance.CeilingSpeed), null, Color.Blue);
                                break;
                            case 3:
                                spriteBatch.Draw(snowball, new Vector2(i * Singleton.TILESIZE, j * Singleton.TILESIZE + Singleton.Instance.Ceiling / Singleton.Instance.CeilingSpeed), null, Color.Yellow);
                                break;
                        } 
                    }
                    else if (j % 2 == 1)
                    {
                        switch (Singleton.Instance.GameBoard[i, j])
                        {
                            case 0:
                                spriteBatch.Draw(snowball, new Vector2(i * Singleton.TILESIZE+ Singleton.TILESIZE/2, j * Singleton.TILESIZE + Singleton.Instance.Ceiling / Singleton.Instance.CeilingSpeed), null, Color.Red);
                                break;
                            case 1:
                                spriteBatch.Draw(snowball, new Vector2(i * Singleton.TILESIZE + Singleton.TILESIZE / 2, j * Singleton.TILESIZE + Singleton.Instance.Ceiling / Singleton.Instance.CeilingSpeed), null, Color.Green);
                                break;
                            case 2:
                                spriteBatch.Draw(snowball, new Vector2(i * Singleton.TILESIZE + Singleton.TILESIZE / 2, j * Singleton.TILESIZE + Singleton.Instance.Ceiling / Singleton.Instance.CeilingSpeed), null, Color.Blue);
                                break;
                            case 3:
                                spriteBatch.Draw(snowball, new Vector2(i * Singleton.TILESIZE + Singleton.TILESIZE / 2, j * Singleton.TILESIZE + Singleton.Instance.Ceiling / Singleton.Instance.CeilingSpeed), null, Color.Yellow);
                                break;
                        }
                    }

                }
            }
            Vector2 fontSize;
            fontSize = _font.MeasureString("Score");
            spriteBatch.DrawString(_font, "Score: " + (int)Singleton.Instance.Score, new Vector2((Singleton.SCREENWIDTH * Singleton.TILESIZE - fontSize.X - Singleton.TILESIZE * 3/2), ((Singleton.SCREENHEIGHT-1) * Singleton.TILESIZE - fontSize.Y)), Color.White);

            if (Singleton.Instance.CurrentQuestState == Singleton.QuestState.PopUp && Singleton.Instance.questComplete == false)
            {                
                switch ((int)Singleton.Instance.CurrentQuestList)
                {
                    case 0:
                        switch ((int)Singleton.Instance.CurrentQuestReward)
                        {
                            case 0:
                                spriteBatch.DrawString(_font, "Quest: Burst any bubble", new Vector2((3 * Singleton.TILESIZE - fontSize.X), ((Singleton.SCREENHEIGHT) * Singleton.TILESIZE - fontSize.Y) - Singleton.TILESIZE / 2), Color.Black);
                                spriteBatch.DrawString(_font, "Reward: Ceiling stop next turn", new Vector2((3 * Singleton.TILESIZE - fontSize.X), ((Singleton.SCREENHEIGHT) * Singleton.TILESIZE) - Singleton.TILESIZE / 2), Color.Black);
                                break;
                            case 1:
                                spriteBatch.DrawString(_font, "Quest: Burst any bubble", new Vector2((3 * Singleton.TILESIZE - fontSize.X), ((Singleton.SCREENHEIGHT) * Singleton.TILESIZE - fontSize.Y) - Singleton.TILESIZE / 2), Color.Black);
                                spriteBatch.DrawString(_font, "Reward: Get extra score", new Vector2((3 * Singleton.TILESIZE - fontSize.X), ((Singleton.SCREENHEIGHT) * Singleton.TILESIZE) - Singleton.TILESIZE / 2), Color.Black);
                                break;
                        }
                        break;
                    case 1:
                        //spriteBatch.DrawString(_font, "Reward: Burst more than 3 bubble", new Vector2((3 * Singleton.TILESIZE - fontSize.X), ((Singleton.SCREENHEIGHT) * Singleton.TILESIZE - fontSize.Y) - Singleton.TILESIZE / 2), Color.Black);
                        switch ((int)Singleton.Instance.CurrentQuestReward)
                        {
                            case 0:
                                spriteBatch.DrawString(_font, "Quest: Burst more than 3 bubble", new Vector2((3 * Singleton.TILESIZE - fontSize.X), ((Singleton.SCREENHEIGHT) * Singleton.TILESIZE - fontSize.Y) - Singleton.TILESIZE / 2), Color.Black);
                                spriteBatch.DrawString(_font, "Reward: Ceiling stop next turn", new Vector2((3 * Singleton.TILESIZE - fontSize.X), ((Singleton.SCREENHEIGHT) * Singleton.TILESIZE) - Singleton.TILESIZE / 2), Color.Black);
                                break;
                            case 1:
                                spriteBatch.DrawString(_font, "Quest: Burst more than 3 bubble", new Vector2((3 * Singleton.TILESIZE - fontSize.X), ((Singleton.SCREENHEIGHT) * Singleton.TILESIZE - fontSize.Y) - Singleton.TILESIZE / 2), Color.Black);
                                spriteBatch.DrawString(_font, "Reward: Get extra score", new Vector2((3 * Singleton.TILESIZE - fontSize.X), ((Singleton.SCREENHEIGHT) * Singleton.TILESIZE) - Singleton.TILESIZE / 2), Color.Black);
                                break;
                        }
                        break;
                }
            }

            if (Singleton.Instance.CurrentGameState == Singleton.GameState.GamePaused)
            {
                fontSize = _font.MeasureString("Paused");
                spriteBatch.DrawString(_font, "Paused", new Vector2((Singleton.GAMEWIDTH * Singleton.TILESIZE - fontSize.X) / 2, (Singleton.GAMEHEIGHT * Singleton.TILESIZE - fontSize.Y) / 2), Color.SpringGreen);
            }
            else if (Singleton.Instance.CurrentGameState == Singleton.GameState.GameEnded)
            {
                fontSize = _font.MeasureString("Game over");
                spriteBatch.DrawString(_font, "Game over", new Vector2((Singleton.GAMEWIDTH * Singleton.TILESIZE - fontSize.X) / 2, (Singleton.GAMEHEIGHT * Singleton.TILESIZE - fontSize.Y) / 2), Color.Red);
                fontSize = _font.MeasureString("Press space to continue..");
                spriteBatch.DrawString(_font, "Press space to continue..", new Vector2((Singleton.GAMEWIDTH * Singleton.TILESIZE - fontSize.X) / 2, (Singleton.GAMEHEIGHT * Singleton.TILESIZE - fontSize.Y) / 2 + Singleton.TILESIZE), Color.Red);
            }
            else if (Singleton.Instance.CurrentGameState == Singleton.GameState.GameClear)
            {
                fontSize = _font.MeasureString("You win");
                spriteBatch.DrawString(_font, "You win", new Vector2((Singleton.GAMEWIDTH * Singleton.TILESIZE - fontSize.X) / 2, (Singleton.GAMEHEIGHT * Singleton.TILESIZE - fontSize.Y) / 2), Color.SpringGreen);
                fontSize = _font.MeasureString("Press space to continue.."); 
                spriteBatch.DrawString(_font, "Press space to continue..", new Vector2((Singleton.GAMEWIDTH * Singleton.TILESIZE - fontSize.X) / 2, (Singleton.GAMEHEIGHT * Singleton.TILESIZE - fontSize.Y) / 2 + Singleton.TILESIZE), Color.SpringGreen);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected void Reset()
        {
            Singleton.Instance.GameBoard = new int[Singleton.GAMEWIDTH, Singleton.GAMEHEIGHT];
            for (int i = 0; i < Singleton.GAMEHEIGHT; i++)
            {
                for (int j = 0; j < Singleton.GAMEWIDTH; j++)
                {
                    if (i < 3) Singleton.Instance.GameBoard[j, i] = rand.Next(4);
                    else if (i >= 3) Singleton.Instance.GameBoard[j, i] = 4;
                }
                if (i % 2 == 1) Singleton.Instance.GameBoard[Singleton.GAMEWIDTH - 1, i] = 4;

            }
            _currentBubble = new Bubble(snowball)
            {
                Position = new Vector2(Singleton.GAMEWIDTH * Singleton.TILESIZE / 2 - Singleton.TILESIZE / 2, Singleton.GAMEHEIGHT * Singleton.TILESIZE)
            };
            _currentBubble.Reset();
            _nextBubble = new Bubble(snowball)
            {
                Position = new Vector2(Singleton.GAMEWIDTH * Singleton.TILESIZE / 2 - Singleton.TILESIZE * 3, (Singleton.SCREENHEIGHT - 2) * Singleton.TILESIZE)
            };
            _nextBubble.Reset();

            Singleton.Instance.Score = 0;
            Singleton.Instance.CurrentGameState = Singleton.GameState.GamePlaying;
            Singleton.Instance.CurrentQuestState = Singleton.QuestState.NotPopUp;
            Singleton.Instance.CurrentQuestList = Singleton.QuestList.SIZE;
            Singleton.Instance.CurrentQuestReward = Singleton.QuestReward.SIZE;
        }

        protected void DeleteLeftBubble(int x, int y)
        {
            if (x <= 0)
            {
            }
            else
            {
                
                    if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x - 1, y])
                    {
                        Singleton.Instance.GameBoard[x, y] = 4;
                        Singleton.Instance.GameBoard[x - 1, y] = 4;
                        x-=1;
                        DeleteLeftBubble(x, y);
                        DeleteLowerBubble(x, y);
                        DeleteUpperBubble(x, y);
                    }
            }
        }
        protected void DeleteRightBubble(int x, int y)
        {
            if ((x >= Singleton.GAMEWIDTH - 2 && y % 2 == 1)||(x >= Singleton.GAMEWIDTH - 1 && y % 2 == 0))
            {
                Singleton.Instance.GameBoard[x, y] = 4;
            }
            else
            {
                if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x + 1, y])
                {
                    Singleton.Instance.GameBoard[x, y] = 4;
                    Singleton.Instance.GameBoard[x + 1, y] = 4;
                    x += 1;
                    DeleteRightBubble(x, y);
                    DeleteLowerBubble(x, y);
                    DeleteUpperBubble(x, y);
                    x += 1;
                }
            }
        }

        protected void DeleteUpperBubble(int x, int y)
        {
            if (y <= 0 || (x > Singleton.GAMEWIDTH - 2 && y % 2 == 1) || (x > Singleton.GAMEWIDTH - 1 && y % 2 == 0))
            {
                Singleton.Instance.GameBoard[x, y] = 4;
            }
            else if (y % 2 == 0)
            {
                switch (x)
                {
                    case 0:
                        if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x, y - 1])
                        {
                            Singleton.Instance.GameBoard[x, y] = 4;
                            Singleton.Instance.GameBoard[x, y - 1] = 4;
                            y -= 1;
                            DeleteUpperBubble(x, y);
                            DeleteRightBubble(x, y);
                            DeleteLeftBubble(x, y);
                            y += 1;
                        }
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        
                            if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x - 1, y - 1])
                            {
                                Singleton.Instance.GameBoard[x, y] = 4;
                                Singleton.Instance.GameBoard[x - 1, y - 1] = 4;
                                x -= 1;
                                y -= 1;
                                DeleteUpperBubble(x, y);
                                DeleteRightBubble(x, y);
                                DeleteLeftBubble(x, y);
                                x += 1;
                                y += 1;
                            }

                        if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x, y - 1])
                        {
                            Singleton.Instance.GameBoard[x, y] = 4;
                            Singleton.Instance.GameBoard[x, y - 1] = 4;
                            y -= 1;
                            DeleteUpperBubble(x, y);
                            DeleteRightBubble(x, y);
                            DeleteLeftBubble(x, y);
                            y += 1;
                        }
                        break;
                    case 7:
                        
                            if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x - 1, y - 1])
                            {
                            Singleton.Instance.GameBoard[x, y] = 4;
                                Singleton.Instance.GameBoard[x - 1, y - 1] = 4;
                                x -= 1;
                                y -= 1;
                                DeleteUpperBubble(x, y);
                                DeleteRightBubble(x, y);
                                DeleteLeftBubble(x, y);
                                x += 1;
                                y += 1;
                            }
                        break;
                }
            }
            else if (y % 2 == 1)
            {
                
                if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x, y - 1])
                {
                    Singleton.Instance.GameBoard[x, y] = 4;
                    Singleton.Instance.GameBoard[x, y - 1] = 4;
                            
                    y -= 1;
                    DeleteUpperBubble(x, y);
                    DeleteRightBubble(x, y);
                    DeleteLeftBubble(x, y);
                    y += 1;
                }
                
                if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x + 1, y - 1])
                {
                    Singleton.Instance.GameBoard[x, y] = 4;
                    Singleton.Instance.GameBoard[x + 1, y - 1] = 4;
                    x += 1;
                    y -= 1;
                    DeleteUpperBubble(x, y);
                    DeleteRightBubble(x, y);
                    DeleteLeftBubble(x, y);
                }
            }
        }
        
        protected void DeleteLowerBubble(int x, int y)
        {
            if (y >= 9|| (x > Singleton.GAMEWIDTH - 2 && y % 2 == 1) || (x > Singleton.GAMEWIDTH - 1 && y % 2 == 0))
            {
                Singleton.Instance.GameBoard[x, y] = 4;
            }
            else if (y % 2 == 0)
            {
                switch (x)
                {
                    case 0:
                        if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x, y + 1])
                        {
                            Singleton.Instance.GameBoard[x, y] = 4;
                            Singleton.Instance.GameBoard[x, y + 1] = 4;
                            y += 1;
                            DeleteLowerBubble(x, y);
                            DeleteRightBubble(x, y);
                            DeleteLeftBubble(x, y);
                            y -= 1;
                        }
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        
                            if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x - 1, y + 1])
                            {
                                Singleton.Instance.GameBoard[x, y] = 4;
                                Singleton.Instance.GameBoard[x - 1, y + 1] = 4;
                                x -= 1;
                                y += 1;
                                DeleteLowerBubble(x, y);
                                DeleteRightBubble(x, y);
                                DeleteLeftBubble(x, y);
                                x += 1;
                                y -= 1;
                            }

                        if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x, y + 1])
                        {
                            Singleton.Instance.GameBoard[x, y] = 4;
                            Singleton.Instance.GameBoard[x, y + 1] = 4;
                            y += 1;
                            DeleteLowerBubble(x, y);
                            DeleteRightBubble(x, y);
                            DeleteLeftBubble(x, y);
                            y -= 1;
                        }
                        break;
                    case 7:
                        
                            if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x - 1, y + 1])
                            {
                            Singleton.Instance.GameBoard[x, y] = 4;
                                Singleton.Instance.GameBoard[x - 1, y + 1] = 4;
                                x -= 1;
                                y += 1;
                                DeleteLowerBubble(x, y);
                                DeleteRightBubble(x, y);
                                DeleteLeftBubble(x, y);
                                x += 1;
                                y -= 1;
                            }
                        break;
                }
            }
            else if (y % 2 == 1)
            {

                if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x, y + 1])
                {
                    Singleton.Instance.GameBoard[x, y] = 4;
                    Singleton.Instance.GameBoard[x, y + 1] = 4;

                    y += 1;
                    DeleteLowerBubble(x, y);
                    DeleteRightBubble(x, y);
                    DeleteLeftBubble(x, y);
                    y -= 1;
                }
                
                    if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x + 1, y + 1])
                    {
                    Singleton.Instance.GameBoard[x, y] = 4;
                        Singleton.Instance.GameBoard[x + 1, y + 1] = 4;
                        x += 1;
                        y += 1;
                        DeleteLowerBubble(x, y);
                        DeleteRightBubble(x, y);
                        DeleteLeftBubble(x, y);
                    }
            }
        }
        protected void ChkFallfromUpperBubble()
        {
            for (int i = 0; i < Singleton.GAMEHEIGHT; i++)
            {
                for (int j = 0; j < Singleton.GAMEWIDTH; j++)
                {
                    switch (i % 2)
                    {
                        case 0:
                            if (i <= 0) break;
                            else
                            {
                                switch (j)
                                {
                                    case 0:
                                        if (Singleton.Instance.GameBoard[j, i - 1] == 4 &&  Singleton.Instance.GameBoard[j + 1, i] == 4)
                                        {
                                            Singleton.Instance.GameBoard[j, i] = 4;
                                            
                                        }
                                        break;
                                    case 1:
                                    case 2:
                                    case 3:
                                    case 4:
                                    case 5:
                                    case 6:
                                        if (Singleton.Instance.GameBoard[j - 1, i - 1] == 4 && Singleton.Instance.GameBoard[j, i - 1] == 4  && Singleton.Instance.GameBoard[j, i - 1] == 4)
                                        {
                                            Singleton.Instance.GameBoard[j, i] = 4;
                                        }
                                        break;
                                    case 7:
                                        if (Singleton.Instance.GameBoard[j - 1, i - 1] == 4 && Singleton.Instance.GameBoard[j - 1, i] == 4)
                                        {
                                            Singleton.Instance.GameBoard[j, i] = 4;
                                        }
                                        break;
                                }
                            }
                            break;
                        case 1:
                            if (i >= Singleton.GAMEHEIGHT - 1) break;
                            else
                            {
                                switch (j)
                                {
                                    case 0:
                                        if (Singleton.Instance.GameBoard[j, i - 1] == 4 && Singleton.Instance.GameBoard[j + 1, i - 1] == 4)
                                        {
                                            Singleton.Instance.GameBoard[j, i] = 4;
                                        }
                                            break;
                                    case 1:
                                    case 2:
                                    case 3:
                                    case 4:
                                    case 5:
                                        if (Singleton.Instance.GameBoard[j, i - 1] == 4 && Singleton.Instance.GameBoard[j + 1, i - 1] == 4 && Singleton.Instance.GameBoard[j - 1, i] == 4 && Singleton.Instance.GameBoard[j + 1, i] == 4)
                                        {
                                            Singleton.Instance.GameBoard[j, i] = 4;
                                        }
                                            break;
                                    case 6:
                                        if (Singleton.Instance.GameBoard[j, i - 1] == 4 && Singleton.Instance.GameBoard[j + 1, i - 1] == 4 && Singleton.Instance.GameBoard[j - 1, i] == 4)
                                        {
                                            Singleton.Instance.GameBoard[j, i] = 4;
                                        }
                                            break;
                                }        
                            }
                            break;
                    }
                }

            }
        }

        protected void ChkDeleteLeftBubble(int x, int y)
        {
            if (x <= 0 || y < 0 || y > Singleton.GAMEHEIGHT - 1)
            {
            }
            else
            {
                
                    if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x - 1, y])
                    {
                        if (Singleton.Instance.samebubble > 2) return;
                        Singleton.Instance.samebubble += 1;
                        x -=1;
                        ChkDeleteLeftBubble(x, y);
                        ChkDeleteLowerBubble(x, y);
                        ChkDeleteUpperBubble(x, y);
                    }
            }
        }
        protected void ChkDeleteRightBubble(int x, int y)
        {
            if ((x >= Singleton.GAMEWIDTH - 2 && y % 2 == 1)||(x >= Singleton.GAMEWIDTH - 1 && y % 2 == 0) || y < 0 || y > Singleton.GAMEHEIGHT - 1)
            {
            }
            else
            {
                if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x + 1, y])
                {
                    if (Singleton.Instance.samebubble > 2) return;
                    Singleton.Instance.samebubble += 1;

                    x += 1;
                    ChkDeleteRightBubble(x, y);
                    ChkDeleteLowerBubble(x, y);
                    ChkDeleteUpperBubble(x, y);
                    x += 1;
                }
            }
        }

        protected void ChkDeleteUpperBubble(int x, int y)
        {
            if (y <= 0 || (x > Singleton.GAMEWIDTH - 2 && y % 2 == 1) || (x > Singleton.GAMEWIDTH - 1 && y % 2 == 0))
            {

            }
            else if (y % 2 == 0)
            {
                switch (x)
                {
                    case 0:
                        if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x, y - 1])
                        {
                            if (Singleton.Instance.samebubble > 2) return;
                            Singleton.Instance.samebubble += 1;
                            y -= 1;
                            ChkDeleteUpperBubble(x, y);
                            ChkDeleteRightBubble(x, y);
                            ChkDeleteLeftBubble(x, y);
                            y += 1;
                        }
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                            if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x - 1, y - 1])
                            {
                            if (Singleton.Instance.samebubble > 2) return;
                                Singleton.Instance.samebubble += 1;
                                x -= 1;
                                y -= 1;
                                ChkDeleteUpperBubble(x, y);
                                ChkDeleteRightBubble(x, y);
                                ChkDeleteLeftBubble(x, y);
                                x += 1;
                                y += 1;
                            }

                        if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x, y - 1])
                        {
                            if (Singleton.Instance.samebubble > 2) return;
                            Singleton.Instance.samebubble += 1;
                            y -= 1;
                            ChkDeleteUpperBubble(x, y);
                            ChkDeleteRightBubble(x, y);
                            ChkDeleteLeftBubble(x, y);
                            y += 1;
                        }
                        break;
                    case 7:
                        
                            if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x - 1, y - 1])
                            {
                                if (Singleton.Instance.samebubble > 2) return;
                                Singleton.Instance.samebubble += 1;
                                x -= 1;
                                y -= 1;
                                ChkDeleteUpperBubble(x, y);
                                ChkDeleteRightBubble(x, y);
                                ChkDeleteLeftBubble(x, y);
                                x += 1;
                                y += 1;
                            }
                        break;
                }
            }
            else if (y % 2 == 1)
            {
                
                if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x, y - 1])
                {
                    if (Singleton.Instance.samebubble > 2) return;
                    Singleton.Instance.samebubble += 1;

                    y -= 1;
                    ChkDeleteUpperBubble(x, y);
                    ChkDeleteRightBubble(x, y);
                    ChkDeleteLeftBubble(x, y);
                    y += 1;
                }
                
                if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x + 1, y - 1])
                {
                    if (Singleton.Instance.samebubble > 2) return;
                    Singleton.Instance.samebubble += 1;
                    x += 1;
                    y -= 1;
                    ChkDeleteUpperBubble(x, y);
                    ChkDeleteRightBubble(x, y);
                    ChkDeleteLeftBubble(x, y);
                }
            }
        }
        
        protected void ChkDeleteLowerBubble(int x, int y)
        {
            if (y >= Singleton.GAMEHEIGHT - 1 || (x > Singleton.GAMEWIDTH - 2 && y % 2 == 1) || (x > Singleton.GAMEWIDTH - 1 && y % 2 == 0))
            {

            }
            else if (y % 2 == 0)
            {
                switch (x)
                {
                    case 0:
                        if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x, y + 1])
                        {
                            if (Singleton.Instance.samebubble > 2) return;
                            Singleton.Instance.samebubble += 1;
                            y += 1;
                            ChkDeleteLowerBubble(x, y);
                            ChkDeleteRightBubble(x, y);
                            ChkDeleteLeftBubble(x, y);
                            y -= 1;
                        }
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        
                            if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x - 1, y + 1])
                            {
                                if (Singleton.Instance.samebubble > 2) return;
                                Singleton.Instance.samebubble += 1;
                                x -= 1;
                                y += 1;
                                ChkDeleteLowerBubble(x, y);
                                ChkDeleteRightBubble(x, y);
                                ChkDeleteLeftBubble(x, y);
                                x += 1;
                                y -= 1;
                            }

                        if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x, y + 1])
                        {
                            if (Singleton.Instance.samebubble > 2) return;
                            Singleton.Instance.samebubble += 1;
                            y += 1;
                            ChkDeleteLowerBubble(x, y);
                            ChkDeleteRightBubble(x, y);
                            ChkDeleteLeftBubble(x, y);
                            y -= 1;
                        }
                        break;
                    case 7:
                        
                            if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x - 1, y + 1])
                            {
                                if (Singleton.Instance.samebubble > 2) return;
                                Singleton.Instance.samebubble += 1;
                                x += 1;
                                y += 1;
                                ChkDeleteLowerBubble(x, y);
                                ChkDeleteRightBubble(x, y);
                                ChkDeleteLeftBubble(x, y);
                                x -= 1;
                                y -= 1;
                            }
                        break;
                        
                }
            }
            
            else if (y % 2 == 1)
            {
                if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x, y + 1])
                {
                    if (Singleton.Instance.samebubble > 2) return;
                    Singleton.Instance.samebubble += 1;

                    y += 1;
                    ChkDeleteLowerBubble(x, y);
                    ChkDeleteRightBubble(x, y);
                    ChkDeleteLeftBubble(x, y);
                    y -= 1;
                }

                if ((int)_currentBubble.CurrentBubbleType == Singleton.Instance.GameBoard[x + 1, y + 1])
                {
                    if (Singleton.Instance.samebubble > 2) return;
                    Singleton.Instance.samebubble += 1;
                    x += 1;
                    y += 1;
                    ChkDeleteLowerBubble(x, y);
                    ChkDeleteRightBubble(x, y);
                    ChkDeleteLeftBubble(x, y);
                }
            }
        }

    }
    
}

