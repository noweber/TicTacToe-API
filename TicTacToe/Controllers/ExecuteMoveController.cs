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
    /// A REST controller to provide an interface for executing moves within the Tic Tac Toe game.
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
            if (string.Equals(moveRequest.azurePlayerSymbol, moveRequest.humanPlayerSymbol))
            {
                return BadRequest("The Azure and Human players must use different symbols. Only X and O are permissable.");
            }

            // Check that the game board is valid:
            int countOfXMoves = 0;
            int countOfOMoves = 0;
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
                else if (!string.Equals(moveRequest.gameBoard[i], "?"))
                {
                    return BadRequest("The game board contains an invalid character. Only X, O, and ? are permissable.");
                }
            }

            // Check that the number of X and O moves are valid:
            if(Math.Abs(countOfXMoves - countOfOMoves) > 1)
            {
                return BadRequest("The game board contains an illegal move. There must be an equal number of X and O moves within the board +/- 1 for the most recent move.");
            }

            // Check that the location of the move on the gameboard is equal to the human's symbol:
            if (!string.Equals(moveRequest.humanPlayerSymbol, moveRequest.gameBoard[moveRequest.move]))
            {
                return BadRequest("The move does not match the game board state. The symbol on the game board at the move's index must match the human player's symbol.");
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
    }
}
