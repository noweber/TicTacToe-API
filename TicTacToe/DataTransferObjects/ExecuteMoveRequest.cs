using System.ComponentModel.DataAnnotations;

namespace TicTacToe.DataTransferObjects
{
    /// <summary>
    /// Represents the last move data made by an actor (human player or Azure AI player) within the Tic Tac Toe game and the updated board.
    /// </summary>
    public class ExecuteMoveRequest
    {
        /// <summary>
        /// Indicates the position on the board the last actor to update state chooses.
        /// </summary>
        [Required]
        [Range(0, 8)]
        public int move { get; set; }

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
