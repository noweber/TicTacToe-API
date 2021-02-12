using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using TicTacToe.DataTransferObjects;
using TicTacToe.Game;

namespace TicTacToe.Controllers
{
    /// <summary>
    /// An interface for an AI to calculate and return the best move based on the game board presented and update the board with the move made.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class CalculateMoveController : ControllerBase
    {
        /// <summary>
        /// Caclulates the best move given the input Tic Tac Toe board state and player symbol and responds with said move plus any winner data.
        /// </summary>
        /// <param name="moveRequest">The CalculateMoveRequest representing current state for which a move should be calculated.</param>
        /// <returns>Returns a response containing the Azure AI's best move and any relative winner data.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CalculateMoveResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public IActionResult PostCalculateMove(CalculateMoveRequest moveRequest)
        {
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

            // Check if the board state already has a winner, and, if so, return bad request since there is no move to be made and the client should have already known the winner data:
            Tuple<string, int[]> winner = GameHelper.GetWinner(moveRequest.gameBoard);
            if (!string.Equals(winner.Item1, GameHelper.InconclusiveState))
            {
                return BadRequest("The board and player sent for a move to be calculated were already in a terminal board state.");
            }

            // Check which player moved first since, during the minimax search, that symbol will be returned as the next player if a board is evaluated with equal Xs and Os.
            string firstMovingPlayer = GameHelper.GetFirstPlayer(moveRequest.gameBoard, moveRequest.playerSymbol);

            // Calculate the next move to make using Minimax:
            int nextMove = GameHelper.SearchBestMoveViaMinimax(moveRequest.gameBoard, firstMovingPlayer);

            // Check if this move results in a win:
            string[] boardAfterMove = GameHelper.MakeMove(moveRequest.gameBoard, nextMove, moveRequest.playerSymbol);
            Tuple<string, int[]> winnerAfterMove = GameHelper.GetWinner(boardAfterMove);

            // Return the results:
            return new ObjectResult(new CalculateMoveResponse()
            {
                move = nextMove,
                playerSymbol = moveRequest.playerSymbol,
                gameBoard = boardAfterMove,
                winner = winnerAfterMove.Item1,
                winPositions = winnerAfterMove.Item2
            });
        }
    }
}
