using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using TicTacToe.DataTransferObjects;
using TicTacToe.Game;

namespace TicTacToe.Controllers
{
    /// <summary>
    /// An interface for executing moves within a REST-based Tic Tac Toe game from which an AI opponent will respond.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class ExecuteMoveController : ControllerBase
    {
        /// <summary>
        /// Executes the Tic Tac Toe move provided in the request body and determines the next move by Azure AI if the game is not over.
        /// </summary>
        /// <param name="moveRequest">The ExecuteMoveRequest representing the last move made within the game. Azure will act upon this move, if valid, and return the result.</param>
        /// <returns>Returns a response containing the Azure AI's next move if the game is not over else the winner and positions associated with their win.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ExecuteMoveResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public IActionResult PostExecuteMove(ExecuteMoveRequest moveRequest)
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
            if (!string.Equals(GameHelper.NextPlayer(moveRequest.gameBoard, moveRequest.azurePlayerSymbol), moveRequest.azurePlayerSymbol))
            {
                return BadRequest("The game board is invalid because the Azure player is not the next player to move and therefore cannot execute a valid move from the current board state.");
            }

            // Check if the board has a winner and, if not, make a move:
            Tuple<string, int[]> winner = GameHelper.GetWinner(moveRequest.gameBoard);
            if (!string.Equals(winner.Item1, GameHelper.InconclusiveState))
            {
                return new ObjectResult(new ExecuteMoveResponse()
                {
                    move = null,
                    azurePlayerSymbol = moveRequest.azurePlayerSymbol,
                    humanPlayerSymbol = moveRequest.humanPlayerSymbol,
                    gameBoard = moveRequest.gameBoard,
                    winner = winner.Item1,
                    winPositions = winner.Item2
                });
            }
            else
            {
                // Need to determine which player moved first else the Minimax tree search might expect Azure can move twice in a row (which it can't):
                // This would happen if Azure was behind by 1 move, makes a hypothtetical move in its search tree, and then the method to get next player can't tell.
                // Determining which player moved first sets the tie breaker correctly when both players have equal number of turns on Minimax tree node board.
                // This can be done correctly right here because we know it is Azure's turn and what their symbol is.
                string firstMovingPlayer = GameHelper.GetFirstPlayer(moveRequest.gameBoard, moveRequest.azurePlayerSymbol);

                // Select a move to make:
                int nextMove = GameHelper.SearchBestMoveViaMinimax(moveRequest.gameBoard, firstMovingPlayer);

                // Check if this move results in a win:
                string[] boardAfterMove = GameHelper.MakeMove(moveRequest.gameBoard, nextMove, moveRequest.azurePlayerSymbol);
                Tuple<string, int[]> winnerAfterMove = GameHelper.GetWinner(boardAfterMove);

                // Return the results:
                return new ObjectResult(new ExecuteMoveResponse()
                {
                    move = nextMove,
                    azurePlayerSymbol = moveRequest.azurePlayerSymbol,
                    humanPlayerSymbol = moveRequest.humanPlayerSymbol,
                    gameBoard = boardAfterMove,
                    winner = winnerAfterMove.Item1,
                    winPositions = winnerAfterMove.Item2
                });
            }
        }
    }
}
