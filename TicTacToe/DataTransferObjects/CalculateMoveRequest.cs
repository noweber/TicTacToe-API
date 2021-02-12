using System.ComponentModel.DataAnnotations;

namespace TicTacToe.DataTransferObjects
{
    /// <summary>
    /// Represents the current state within the Tic Tac Toe game board for the Azure AI to calculate a next more for.
    /// </summary>
    public class CalculateMoveRequest
    {
        /// <summary>
        /// The symbol of the player to calculate a move for.
        /// </summary>
        [Required]
        [StringLength(1)]
        [RegularExpression("X|O")]
        public string playerSymbol { get; set; }

        /// <summary>
        /// An array of X, O, and ? symbols representing the current state of the game board where ? is an open space and the other symbols correspond to players.
        /// </summary>
        [Required]
        [MinLength(9)]
        [MaxLength(9)]
        public string[] gameBoard { get; set; }
    }
}
