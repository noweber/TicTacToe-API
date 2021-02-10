using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.DataTransferObjects
{
    /// <summary>
    /// Additional data along which, along with the parent class representing the Azure AI's move, contains the current state of the game whether there is a winner or not, yet.
    /// </summary>
    public class ExecuteMoveResponse : ExecuteMoveRequest
    {
        /// <summary>
        /// Indicates the gameboard status whether there is a winner (the winning player's symbol), a tie, or whether it is inconclusive and the game can continue.
        /// </summary>
        public string winner { get; set; }

        /// <summary>
        /// An array that lists the zero-based position index values of the win or null if no winner is present in the gameBoard array.
        /// </summary>
        public int[] winPositions { get; set; }
    }
}
