﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using TicTacToe.DataTransferObjects;

namespace TicTacToe.Controllers
{
    /// <summary>
    /// An interface for executing moves within the Tic Tac Toe game given some current state of a game (provided in a request).
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class ExecuteMoveController : ControllerBase
    {
        /// <summary>
        /// Executes the Tic Tac Toe move provided in the request body and determines the next move by Azure AI if the game is not over.
        /// </summary>
        /// <returns>Returns a response containing the Azure AI's next move if the game is not over else the winner and positions associated with their win.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ExecuteMoveResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public IActionResult Post(ExecuteMoveRequest moveRequest)
        {
            // Check that the Azure and Human player symbols are different:
            // If the symbols in the request body are the same, return Bad Request.
            if (string.Equals(moveRequest.azurePlayerSymbol, moveRequest.humanPlayerSymbol))
            {
                return BadRequest("The Azure and Human players must use different symbols. Only X and O are permissable.");
            }

            // Check that the game board is valid:
            // If there are invalid symbols in the game board, return Bad Request.
            // This also counts the number of Xs and Os present in the game board for further checks.
            int countOfXMoves = 0;
            int countOfOMoves = 0;
            int countOfQuestionMarks = 0;
            for (int i = 0; i < moveRequest.gameBoard.Length; i++)
            {
                if (string.Equals(moveRequest.gameBoard[i], "X"))
                {
                    countOfXMoves += 1;
                }
                else if (string.Equals(moveRequest.gameBoard[i], "O"))
                {
                    countOfOMoves += 1;
                }
                else if (string.Equals(moveRequest.gameBoard[i], "?"))
                {
                    countOfQuestionMarks += 1;
                }
                else
                {
                    return BadRequest("The game board contains an invalid character. Only X, O, and ? are permissable.");
                }
            }

            // Check that the number of X and O moves are valid:
            // If there is +/- 1 of one symbol relative to the other, return Bad Request.
            if (Math.Abs(countOfXMoves - countOfOMoves) > 1)
            {
                return BadRequest("The game board contains an illegal move. There must be an equal number of X and O moves within the board +/- 1 for the most recent move.");
            }

            // Check that if a move was not present, then the board must contain only ? symbols:
            if (!moveRequest.move.HasValue && (countOfXMoves > 0 || countOfOMoves > 0 || countOfQuestionMarks != 9))
            {
                return BadRequest("The move field does not contain a value, but the board is not empty.");
            }

            // Check that the location of the move on the gameboard is equal to the human's symbol:
            if (moveRequest.move.HasValue && !string.Equals(moveRequest.humanPlayerSymbol, moveRequest.gameBoard[moveRequest.move.Value]))
            {
                return BadRequest("The move does not match the game board state. The symbol on the game board at the move's index must match the human player's symbol.");
            }

            // Check that Azure is the next actor to make a turn given the current state of the board:
            if (!string.Equals(NextPlayer(moveRequest.gameBoard, moveRequest.azurePlayerSymbol), moveRequest.azurePlayerSymbol))
            {
                return BadRequest("The game board is invalid because the Azure player is not the next player to move and therefore cannot execute a valid move from the current board state.");
            }

            Tuple<string, int[]> winner = GetWinner(moveRequest.gameBoard);
            return new ObjectResult(new ExecuteMoveResponse()
            {
                move = -1,
                azurePlayerSymbol = moveRequest.azurePlayerSymbol,
                humanPlayerSymbol = moveRequest.humanPlayerSymbol,
                gameBoard = moveRequest.gameBoard,
                winner = winner.Item1,
                winPositions = winner.Item2
            });
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="board">TODO</param>
        /// <returns>TODO</returns>
        private static List<int> AvailableMoves(string[] board)
        {
            const int NumberOfColumns = 3;
            List<int> availableMoves = new List<int>();
            for(int i = 0; i < NumberOfColumns; i++)
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

        /// <summary>
        /// A function to determine the next player given the current state of the Tic Tac Toe board.
        /// </summary>
        /// <param name="board">The string array representing the nine cells in a Tic Tac Toe game board.</param>
        /// <param name="tieBreakerSymbol">In the result of a tie between the number of Xs and Os, this symbol will go first.</param>
        /// <returns>Returns X if there more Os than Xs, returns O if there are more Xs than Os, or returns the tieBreakerSymbol input if the number of Xs equals the number of Os.</returns>
        private static string NextPlayer(string[] board, string tieBreakerSymbol)
        {
            int xCount = 0;
            int OCount = 0;
            for (int i = 0; i < board.Length; i++)
            {
                if (string.Equals(board[i], "X"))
                {
                    xCount += 1;
                }
                else if (string.Equals(board[i], "O"))
                {
                    OCount += 1;
                }
            }

            if (xCount > OCount)
            {
                return "O";
            }
            else if (OCount > xCount)
            {
                return "X";
            }
            else
            {
                return tieBreakerSymbol;
            }
        }

        /// <summary>
        /// Checks both diagonals, all columns, and all rows to determine if any contain all of one player's symbol.
        /// If so, that player is declared the winner.
        /// </summary>
        /// <param name="board">TODO</param>
        /// <param name="tieBreakerSymbol">TODO</param>
        /// <returns>
        /// Returns a 2-tuple where
        /// the the first item is a string corresponding the winning player (X, O, tie, or inconclusive) and
        /// the second item contains the indices corresponding to the winning positions found on the game board for said player.
        /// In the event of a tie or inconclusive state, the second part of the 2-tuple will be returned as null.</returns>
        private static Tuple<string, int[]> GetWinner(string[] board)
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
            for(int i = 0; i < NumberOfColumns; i++)
            {
                // These are the sums of the diagonals where X is +1 and O is -1:
                switch (gameBoard[i,i])
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
                for(int j = 0; j < NumberOfColumns; j++)
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
            if(availableMoves.Count == 0)
            {
                return new Tuple<string, int[]>("tie", null); ;
            }

            // Return the "inconclusive" string in case no winner is present.
            return new Tuple<string, int[]>("inconclusive", null); ;
        }
    }
}
