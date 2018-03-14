using TicTacToe;

namespace TicTacToeWeb
{
    public class TicTacToeGameSession
    {
        public TicTacToeGame game { get; private set; }
        private TicTacToeAI AI;
        public string ID { get; private set; }
        public GameState GameState { get; private set; }

        public TicTacToeGameSession(string ID)
        {
            game = new TicTacToeGame();
            this.ID = ID;
            GameState = GameState.InProgress;
        }

        public void EnableAI()
        {
            AI = new TicTacToeAI();
            game.AddPlayer("AI");
        }

        public bool IsReady() => game.IsReady();

        public bool IsEmpty() => game.IsEmpty();

        public bool IsSinglePlayer()
        {
            return AI != null;
        }

        public string ActivePlayer => game.ActivePlayer.ToString();

        public string FriendlyGameState()
        {
            switch (GameState)
            {
                case GameState.OWin:
                    return "Player O won!";
                case GameState.XWin:
                    return "Player X won!";
                case GameState.Tie:
                    return "Game tied";
                default:
                    return "Game in Progress";
            }
        }

        public void MakeMove(string playerID, int pos)
        {
            GameState = game.MakeMove(playerID, pos);
        }

        public int MakeAIMove()
        {
            int move = AI.FindMove(game.Board, game.ActivePlayer);
            MakeMove("AI", move);
            return move;
        }

        public PlayerSymbol AddPlayer(string playerID) => game.AddPlayer(playerID);

        public void RemovePlayer(string playerID)
        {
            game.RemovePlayer(playerID);
            if (IsSinglePlayer())
            {
                game.RemovePlayer("AI");
                AI = null;
            }
        }
        public void Reset()
        {
            GameState = GameState.InProgress;
            game.Reset();
        }
    }
}
