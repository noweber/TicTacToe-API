using System;
using System.Collections.Generic;

namespace TicTacToe.Game
{
    /// <summary>
    /// Helper functions for computing various aspects of the game's state such as who the winner is and next moves for an AI.
    /// </summary>
    public static class GameHelper
    {
        /// <summary>
        /// A text string representing the winner state when the current board is inconclusive. This is used in several locations.
        /// </summary>
        public const string InconclusiveState = "inconclusive";

        /// <summary>
        /// Checks both diagonals, all columns, and all rows to determine if any contain all of one player's symbol.
        /// If so, that player is declared the winner.
        /// </summary>
        /// <param name="board">The current board to check whether a winner exists or not.</param>
        /// <returns>
        /// Returns a 2-tuple where
        /// the the first item is a string corresponding the winning player (X, O, tie, or inconclusive) and
        /// the second item contains the indices corresponding to the winning positions found on the game board for said player.
        /// In the event of a tie or inconclusive state, the second part of the 2-tuple will be returned as null.</returns>
        public static Tuple<string, int[]> GetWinner(string[] board)
        {
            // The number of columns within a tic tac toe board (this is used in calculations below):
            const int NumberOfColumns = 3;

            // Convert the game board into a 2D array to make the logic simpler to check who has won:
            string[,] gameBoard = new string[NumberOfColumns, NumberOfColumns]
            {
                { board[0], board[1], board[2] },
                { board[3], board[4], board[5] },
                { board[6], board[7], board[8] },
            };

            // Check for diagonal, row-wise, and column-wise victory conditions:
            // X will be associated with a +1 and O will be associated with -1.
            // The sum of each row, column, and diagonal will be built while looping through the cells.
            // If any row, column, or diagonal reaches 3 or -3, then the respective player has won.
            int down_right_diagonal_sum = 0;
            int down_left_diagonal_sum = 0;
            for (int i = 0; i < NumberOfColumns; i++)
            {
                // These are the sums of the diagonals where X is +1 and O is -1:
                switch (gameBoard[i, i])
                {
                    case "X":
                        down_right_diagonal_sum += 1;
                        break;
                    case "O":
                        down_right_diagonal_sum -= 1;
                        break;
                    default:
                        break;
                }
                switch (gameBoard[i, NumberOfColumns - 1 - i])
                {
                    case "X":
                        down_left_diagonal_sum += 1;
                        break;
                    case "O":
                        down_left_diagonal_sum -= 1;
                        break;
                    default:
                        break;
                }

                // Check if one of the players has won by a diagonal condition:
                // If a diagonal's sum is 3 or -3, then it is a win condition for X or O respectively.
                if (down_right_diagonal_sum == NumberOfColumns)
                {
                    return new Tuple<string, int[]>("X", new int[] { 0, 4, 8 });
                }
                if (down_left_diagonal_sum == NumberOfColumns)
                {
                    return new Tuple<string, int[]>("X", new int[] { 2, 4, 6 });
                }
                if (down_right_diagonal_sum == -NumberOfColumns)
                {
                    return new Tuple<string, int[]>("O", new int[] { 0, 4, 8 });
                }
                if (down_left_diagonal_sum == -NumberOfColumns)
                {
                    return new Tuple<string, int[]>("O", new int[] { 2, 4, 6 });
                }

                // These are the sums of the colums and rows where X is +1 and O is -1:
                int row_sum = 0;
                int column_sum = 0;
                for (int j = 0; j < NumberOfColumns; j++)
                {
                    switch (gameBoard[i, j])
                    {
                        case "X":
                            row_sum += 1;
                            break;
                        case "O":
                            row_sum -= 1;
                            break;
                        default:
                            break;
                    }
                    switch (gameBoard[j, i])
                    {
                        case "X":
                            column_sum += 1;
                            break;
                        case "O":
                            column_sum -= 1;
                            break;
                        default:
                            break;
                    }
                }

                // Check if one of the players has won by a row or column condition:
                // If a row or column's sum is 3 or -3, then it is a win condition for X or O respectively.
                if (row_sum == NumberOfColumns)
                {
                    return new Tuple<string, int[]>("X", new int[] { i * NumberOfColumns, i * NumberOfColumns + 1, i * NumberOfColumns + 2 });
                }
                if (column_sum == NumberOfColumns)
                {
                    return new Tuple<string, int[]>("X", new int[] { i, i + NumberOfColumns, i + NumberOfColumns * 2 });
                }
                if (row_sum == -NumberOfColumns)
                {
                    return new Tuple<string, int[]>("O", new int[] { i * NumberOfColumns, i * NumberOfColumns + 1, i * NumberOfColumns + 2 });
                }
                if (column_sum == -NumberOfColumns)
                {
                    return new Tuple<string, int[]>("O", new int[] { i, i + NumberOfColumns, i + NumberOfColumns * 2 });
                }
            }

            // Check if there are any valid moves and, if not, return "tie" since no winner has been found and there are no open spaces:
            List<int> availableMoves = AvailableMoves(board);
            if (availableMoves.Count == 0)
            {
                return new Tuple<string, int[]>("tie", null); ;
            }

            // Return the "inconclusive" string in case no winner is present.
            return new Tuple<string, int[]>(InconclusiveState, null); ;
        }

        /// <summary>
        /// Determines which player must have moved first given the current state of the board and which player's symbol is allowed to act next.
        /// This is important because during a minimax search, for example, that symbol will be returned as the next player to move if a board is evaluated with equal Xs and Os present.
        /// </summary>
        /// <param name="board">The board state to evabluate.</param>
        /// <param name="currentMoverPlayerSymbol">The current player who's move it is.</param>
        /// <returns>Returns the symbol of the player who must have made the first move.</returns>
        public static string GetFirstPlayer(string[] board, string currentMoverPlayerSymbol)
        {
            string firstMovingPlayer = currentMoverPlayerSymbol;
            int xCount = 0;
            int oCount = 0;
            for (int i = 0; i < board.Length; i++)
            {
                if (string.Equals(board[i], "X"))
                {
                    xCount += 1;
                }
                else if (string.Equals(board[i], "O"))
                {
                    oCount += 1;
                }
            }
            if (xCount != oCount)
            {
                if(string.Equals("X", currentMoverPlayerSymbol))
                {
                    firstMovingPlayer = "O";
                } else
                {
                    firstMovingPlayer = "X";
                }
            }
            return firstMovingPlayer;
        }

        /// <summary>
        /// A function to determine the next player given the current state of the Tic Tac Toe board.
        /// </summary>
        /// <param name="board">The string array representing the nine cells in a Tic Tac Toe game board.</param>
        /// <param name="tieBreakerSymbol">In the result of a tie between the number of Xs and Os (or an empty board), this symbol will go first.</param>
        /// <returns>Returns X if there more Os than Xs, returns O if there are more Xs than Os, or returns the tieBreakerSymbol input if the number of Xs equals the number of Os.</returns>
        public static string NextPlayer(string[] board, string tieBreakerSymbol)
        {
            int xCount = 0;
            int oCount = 0;
            for (int i = 0; i < board.Length; i++)
            {
                if (string.Equals(board[i], "X"))
                {
                    xCount += 1;
                }
                else if (string.Equals(board[i], "O"))
                {
                    oCount += 1;
                }
            }

            if (xCount > oCount)
            {
                return "O";
            }
            else if (oCount > xCount)
            {
                return "X";
            }
            else
            {
                return tieBreakerSymbol;
            }
        }

        /// <summary>
        /// A method to create the game board which results from making a given move.
        /// </summary>
        /// <param name="board">Some current game board state.</param>
        /// <param name="move">The index of the game board to make a move on.</param>
        /// <param name="player">The symbol representing the player making the move.</param>
        /// <returns>Returns a new game board with the given move applied to it.</returns>
        public static string[] MakeMove(string[] board, int move, string player)
        {
            // Copy the input board to a new array:
            string[] updatedBoard = new List<string>(board).ToArray();

            // Apply the move at the given location for the given player:
            updatedBoard[move] = player;

            return updatedBoard;
        }


        /// <summary>
        /// A method to use minimax search for the best Tic Tac Toe move given the current board state.
        /// </summary>
        /// <param name="board">The current board state.</param>
        /// <param name="tieBreakerSymbol">The player to make a move first if the board is empty of there are an equal number of each player's symbols present.</param>
        /// <returns>Returns the index into the board array for the best move available.</returns>
        public static int SearchBestMoveViaMinimax(string[] board, string tieBreakerSymbol)
        {
            Tuple<int, int?> bestMove = GetBestMove(board, tieBreakerSymbol);
            return bestMove.Item2.Value;
        }

        /// <summary>
        /// A recursive method for using a Minimax search tree to determine the best move for a player given the current state of the board.
        /// </summary>
        /// <param name="board">The current board state.</param>
        /// <param name="tieBreakerSymbol">The player to make a move first if the board is empty of there are an equal number of each player's symbols present.</param>
        /// <returns>Returns the best move (value, action) tuple for current board and player using a Minimax search tree.</returns>
        private static Tuple<int, int?> GetBestMove(string[] board, string tieBreakerSymbol)
        {
            // If there board is in a terminal state, just returns its current utility so it can be evaluated.
            if (IsBoardTerminal(board))
            {
                return new Tuple<int, int?>(GetMinimaxBoardUtility(board), null);
            }

            // Check which player should currently be able to move based on how many of each symbol is found.
            // Use the current player to set a default best move utilty value such that it will be immediately overridden by first node searched.
            string currentPlayer = NextPlayer(board, tieBreakerSymbol);

            // Set a default starting value for the best move:
            Tuple<int, int?> bestMove;
            if (string.Equals("X", currentPlayer))
            {
                // Use -2 since all utilities will be >= -1 once searched and higher utility is better for "X" player.
                bestMove = new Tuple<int, int?>(-2, null);
            }
            else
            {
                // Use 2 since all utilities will be <= 1 once searched and lower utility is better for "O" player.
                bestMove = new Tuple<int, int?>(2, null);
            }

            // Check every possibly move (no alpha-beta pruning optimization) to determine the best available move:
            List<int> possibleMoves = AvailableMoves(board);
            for (int i = 0; i < possibleMoves.Count; i++)
            {
                // Recursively check the higher possible utilty resulting from this move:
                Tuple<int, int?> result = GetBestMove(MakeMove(board, possibleMoves[i], currentPlayer), tieBreakerSymbol);

                // If the result is better than any previously best known move, store it as the best so far:
                // Note: A higher utility is arbitrarily better for the "X" player according to the utility function.
                // Note: A lower utility is arbitrarily better for the "O" player according to the utility function.
                if ((string.Equals("X", currentPlayer) && result.Item1 > bestMove.Item1) ||
                    (string.Equals("O", currentPlayer) && result.Item1 < bestMove.Item1))
                {
                    bestMove = new Tuple<int, int?>(result.Item1, possibleMoves[i]);
                }
                else if (result.Item1 == bestMove.Item1)
                {
                    // When the utility is tied, randomly decide if the new move should become the best move (since it doesn't matter which move is selected):
                    Random randomNumberGenerator = new Random();
                    if (randomNumberGenerator.NextDouble() < 0.35f)
                    {
                        bestMove = new Tuple<int, int?>(result.Item1, possibleMoves[i]);
                    }
                }
            }

            // Return the best move available:
            return bestMove;
        }

        /// <summary>
        /// A method for checking whether a board is in a terminal state: when there is a winner or there are no moves left
        /// </summary>
        /// <param name="board">The board state to evaluate.</param>
        /// <returns>Returns true if there is a winner for the given board or if there are no moves remaining. Returns false otherwise.</returns>
        private static bool IsBoardTerminal(string[] board)
        {
            // If the board winner is inconclusive then are is no winner yet and there are still moves remaining.
            // Return false in this case since the board is not in a terminal state.
            if (string.Equals(GetWinner(board).Item1, InconclusiveState))
            {
                return false;
            }


            // If there is a winner or it is a tie, then it is not inconclusive.
            // Return true in this case since the board is in a terminal state.
            return true;
        }

        /// <summary>
        /// A utility function for a Minimax search tree to try to make moves based on whether it wants X or O to win.
        /// </summary>
        /// <param name="board">The current board to evaluate the utilty of.</param>
        /// <returns>Returns 1 if X is the winner in the given board, -1 if O is the winner, else returns 0.</returns>
        private static int GetMinimaxBoardUtility(string[] board)
        {
            string winner = GetWinner(board).Item1;
            if (string.Equals("X", winner))
            {
                return 1;
            }
            else if (string.Equals("O", winner))
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// A method to determine the indices of the game board where moves are available.
        /// </summary>
        /// <param name="board">Some current game board to evaluate for open moves.</param>
        /// <returns>Returns a list of indices corresponding to open spaces on the given board.</returns>
        private static List<int> AvailableMoves(string[] board)
        {
            const int NumberOfColumns = 3;
            List<int> availableMoves = new List<int>();
            for (int i = 0; i < NumberOfColumns; i++)
            {
                for (int j = 0; j < NumberOfColumns; j++)
                {
                    int boardIndex = i * NumberOfColumns + j;
                    if (string.Equals(board[boardIndex], "?"))
                    {
                        availableMoves.Add(boardIndex);
                    }
                }
            }
            return availableMoves;
        }
    }
}
