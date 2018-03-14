using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe
{
    public enum PlayerSymbol
    {
        Empty,
        X,
        O
    }

    public enum GameState
    {
        InProgress,
        XWin,
        OWin,
        Tie
    }
    
    public class TicTacToeGame
    {
        public PlayerSymbol ActivePlayer { get; private set; }
        private Dictionary<string, PlayerSymbol> players;
        private PlayerSymbol[] board;
        public PlayerSymbol[] Board => board.Clone() as PlayerSymbol[];

        public TicTacToeGame()
        {
            players = new Dictionary<string, PlayerSymbol>();
            Reset();
        }

        public bool IsEmpty() => players.Count == 0;

        public bool IsReady() => players.Count == 2;

        public void Reset()
        {
            board = new PlayerSymbol[9];
            ActivePlayer = PlayerSymbol.X;
        }

        public PlayerSymbol AddPlayer(string playerID)
        {
            if (players.ContainsKey(playerID))
                throw new ArgumentException("Player is already in Game");
            if (players.Count >= 2)
                throw new ArgumentException("Game is already full");

            var playerSymbol = players.Values.Contains(PlayerSymbol.X) ? PlayerSymbol.O : PlayerSymbol.X;
            players.Add(playerID, playerSymbol);
            return playerSymbol;
        }

        public void RemovePlayer(string playerID) => players.Remove(playerID);

        public GameState MakeMove(string playerID, int position)
        {
            if (position < 0 || position > 8)
                throw new ArgumentException("Not a valid move");
            if (board[position] != PlayerSymbol.Empty)
                throw new ArgumentException("Spot has already been played");
            if (!players.ContainsKey(playerID) || players[playerID] != ActivePlayer)
                throw new ArgumentException("Not the active player");

            board[position] = players[playerID];
            ActivePlayer = ActivePlayer == PlayerSymbol.X ? PlayerSymbol.O : PlayerSymbol.X;
            return CurrentGameState();
        }
        
        private GameState CurrentGameState()
        {
            var WinningRows = new List<(int, int, int)> {(0,1,2), (3,4,5), (6,7,8),
                                                         (0,3,6), (1,4,7), (2,5,8),
                                                         (0,4,8), (2,4,6)};
            foreach (var row in WinningRows)
            {
                if (board[row.Item1] == PlayerSymbol.X && board[row.Item2] == PlayerSymbol.X && board[row.Item3] == PlayerSymbol.X)
                    return GameState.XWin;
                if (board[row.Item1] == PlayerSymbol.O && board[row.Item2] == PlayerSymbol.O && board[row.Item3] == PlayerSymbol.O)
                    return GameState.OWin;
            }
            if (!board.Contains(PlayerSymbol.Empty))
                return GameState.Tie;
            return GameState.InProgress;
        }
    }
}
