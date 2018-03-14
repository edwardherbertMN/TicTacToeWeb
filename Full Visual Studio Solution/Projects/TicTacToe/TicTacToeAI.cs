using System;
using System.Collections.Generic;

namespace TicTacToe
{
    public class TicTacToeAI
    {
        public int FindMove(PlayerSymbol[] board, PlayerSymbol AI)
        {
            var opponent = AI == PlayerSymbol.X ? PlayerSymbol.O : PlayerSymbol.X;
            int move = FindTwoInARow(board, AI);
            if (move < 0)
                move = FindTwoInARow(board, opponent);
            if (move < 0)
                move = FindFork(board, AI);
            if (move < 0)
                move = FindFork(board, opponent);
            if (move < 0)
                move = CheckCenter(board);
            if (move < 0)
                move = FindOppositeCorner(board, AI, opponent);
            if (move < 0)
                move = FindEmpty(board);
            return move;
        }

        private int FindTwoInARow(PlayerSymbol[] board, PlayerSymbol player)
        {
            var WinningRows = new List<(int, int, int)> {(0,1,2), (3,4,5), (6,7,8),
                                                         (0,3,6), (1,4,7), (2,5,8),
                                                         (0,4,8), (2,4,6)};
            foreach (var row in WinningRows)
            {
                if (board[row.Item1] == player && board[row.Item2] == player && board[row.Item3] == PlayerSymbol.Empty)
                    return row.Item3;
                if (board[row.Item1] == player && board[row.Item3] == player && board[row.Item2] == PlayerSymbol.Empty)
                    return row.Item2;
                if (board[row.Item2] == player && board[row.Item3] == player && board[row.Item1] == PlayerSymbol.Empty)
                    return row.Item1;
            }
            return -1;
        }

        private int NumTwoInARow(PlayerSymbol[] board, PlayerSymbol player)
        {
            var WinningRows = new List<(int, int, int)> {(0,1,2), (3,4,5), (6,7,8),
                                                         (0,3,6), (1,4,7), (2,5,8),
                                                         (0,4,8), (2,4,6)};
            int result = 0;
            foreach (var row in WinningRows)
            {
                if (board[row.Item1] == player && board[row.Item2] == player && board[row.Item3] == PlayerSymbol.Empty)
                    ++result;
                if (board[row.Item1] == player && board[row.Item3] == player && board[row.Item2] == PlayerSymbol.Empty)
                    ++result;
                if (board[row.Item2] == player && board[row.Item3] == player && board[row.Item1] == PlayerSymbol.Empty)
                    ++result;
            }
            return result;
        }

        private int FindFork(PlayerSymbol[] board, PlayerSymbol player)
        {
            for(int i = 0; i < 8; ++i)
            {
                if (board[i] != PlayerSymbol.Empty)
                    continue;
                board[i] = player;
                if (NumTwoInARow(board, player) > 1)
                    return i;
                board[i] = PlayerSymbol.Empty;
            }
            return -1;
        }

        private int CheckCenter(PlayerSymbol[] board)
        {
            return board[4] == PlayerSymbol.Empty ? 4 : -1;
        }

        private int FindOppositeCorner(PlayerSymbol[] board, PlayerSymbol player, PlayerSymbol opponent)
        {
            if (board[0] == opponent && board[8] == PlayerSymbol.Empty)
                return 8;
            if (board[2] == opponent && board[6] == PlayerSymbol.Empty)
                return 6;
            if (board[6] == opponent && board[2] == PlayerSymbol.Empty)
                return 2;
            if (board[8] == opponent && board[0] == PlayerSymbol.Empty)
                return 0;
            return -1;
        }

        private int FindEmpty(PlayerSymbol[] board)
        {
            for(int i = 0; i < 8; ++i)
            {
                if (board[i] == PlayerSymbol.Empty)
                    return i;
            }
            return -1;
        }
    }
}
