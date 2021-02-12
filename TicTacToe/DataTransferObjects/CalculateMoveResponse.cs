using System;
using System.ComponentModel.DataAnnotations;

namespace TicTacToe.DataTransferObjects
{
    /// <summary>
    /// Represents the response based on the Azure AI's calculations of what the best move to make for a given player is given the input data in the request.
    /// </summary>
    public class CalculateMoveResponse : CalculateMoveRequest
    {
        /// <summary>
        /// Indicates the position on the board the last actor to update state chooses.
        /// This is a Nullable type because, if it were not, the value would default to 0 if it was not present in the request body.
        /// The move field is not required if and only if the game board is empty (contains only ? symbols)
        /// </summary>
        [Range(0, 8)]
        public System.Nullable<int> move { get; set; }

        /// <summary>
        /// Indicates the gameboard status whether there is a winner (the winning player's symbol), a tie, or whether it is inconclusive and the game can continue.
        /// </summary>
        [StringLength(1)]
        [RegularExpression("X|O")]
        public string winner { get; set; }

        /// <summary>
        /// An array that lists the zero-based position index values of the win or null if no winner is present in the gameBoard array.
        /// </summary>
        [MinLength(3)]
        [MaxLength(3)]
        public int[] winPositions { get; set; }
    }
}
