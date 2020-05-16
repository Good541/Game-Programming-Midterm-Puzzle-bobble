using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenPop
{
    class Singleton
    {
        public const int TILESIZE = 50;

        public const int GAMEWIDTH = 8;
        public const int GAMEHEIGHT = 10;

        public const int SCREENWIDTH = GAMEWIDTH;
        public const int SCREENHEIGHT = GAMEHEIGHT + 2;
        public float Ceiling;
        public int Ceilingrowpos;
        public int CeilingSpeed = 20;
        public float CurrentCursorRadians = (float)Math.PI / 2;
        public float PreviousCursorRadians;
        public int CountDeleteBubble;
        public int bubbleposX = -1;
        public int bubbleposY = -1;


        public float Score;
        public bool questComplete;
        public int samebubble;

        public KeyboardState PreviousKey, CurrentKey;
        public MouseState CurrentClick;

        public int[,] GameBoard;

        public Random Random = new Random();

        public enum GameState
        {
            GamePlaying,
            GamePaused,
            GameEnded,
            GameClear
        }

        public enum QuestState
        {
            PopUp,
            NotPopUp,
            SIZE
        }

        public enum QuestList
        {
            BurstAnyBubble,
            BurstMoreThreeBubble,
            SIZE
        }
        public enum QuestReward
        {
            CeilingStopNextTurn,
            GetExtraScore,
            SIZE
        }

        public GameState CurrentGameState;
        public QuestState CurrentQuestState;
        public QuestReward CurrentQuestReward;
        public QuestList CurrentQuestList;


        private static Singleton instance;

        private Singleton()
        {
        }

        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Singleton();
                }
                return instance;
            }
        }
    }
}
