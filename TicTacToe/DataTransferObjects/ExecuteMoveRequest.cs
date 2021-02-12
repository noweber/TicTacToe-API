using System.ComponentModel.DataAnnotations;

namespace TicTacToe.DataTransferObjects
{
    /// <summary>
    /// Represents the last move data made by an actor within the Tic Tac Toe game from which the AI can update the game state and respond with its next move.
    /// </summary>
    public class ExecuteMoveRequest
    {
        /// <summary>
        /// Indicates the position on the board the last actor to update state chooses.
        /// This is a Nullable type because, if it were not, the value would default to 0 if it was not present in the request body.
        /// The move field is not required if and only if the game board is empty (contains only ? symbols)
        /// </summary>
        [Range(0, 8)]
        public System.Nullable<int> move { get; set; }

        /// <summary>
        /// The letter X or the letter O indicating the symbol used by Azure.
        /// </summary>
        [Required]
        [StringLength(1)]
        [RegularExpression("X|O")]
        public string azurePlayerSymbol { get; set; }

        /// <summary>
        /// The letter X or the letter O indicating the symbol used by the human.
        /// </summary>
        [Required]
        [StringLength(1)]
        [RegularExpression("X|O")]
        public string humanPlayerSymbol { get; set; }

        /// <summary>
        /// An array of X, O, and ? symbols representing the current state of the game board where ? is an open space and the other symbols correspond to players.
        /// </summary>
        [Required]
        [MinLength(9)]
        [MaxLength(9)]
        public string[] gameBoard { get; set; }
    }
}
