using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            if (!string.Equals(this.NextPlayer(moveRequest.gameBoard, moveRequest.azurePlayerSymbol), moveRequest.azurePlayerSymbol))
            {
                return BadRequest("The game board is invalid because the Azure player is not the next player to move and therefore cannot execute a valid move from the current board state.");
            }

            return new ObjectResult(new ExecuteMoveResponse()
            {
                move = -1,
                azurePlayerSymbol = moveRequest.azurePlayerSymbol,
                humanPlayerSymbol = moveRequest.humanPlayerSymbol,
                gameBoard = null,
                winner = null,
                winPositions = null
            });
        }

        /// <summary>
        /// A function to determine the next player given the current state of the Tic Tac Toe board.
        /// </summary>
        /// <param name="board">The string array representing the nine cells in a Tic Tac Toe game board.</param>
        /// <param name="tieBreakerSymbol">In the result of a tie between the number of Xs and Os, this symbol will go first.</param>
        /// <returns>Returns X if there more Os than Xs, returns O if there are more Xs than Os, or returns the tieBreakerSymbol input if the number of Xs equals the number of Os.</returns>
        private string NextPlayer(string[] board, string tieBreakerSymbol)
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
    }
}
