using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.DataTransferObjects
{
    /// <summary>
    /// TODO
    /// </summary>
    public class Move
    {
        /// <summary>
        /// TODO
        /// </summary>
        public int move { get; set; }

        /// <summary>
        /// TODO
        /// </summary>
        public string azurePlayerSymbol { get; set; }

        /// <summary>
        /// TODO
        /// </summary>
        public string humanPlayerSymbol { get; set; }

        /// <summary>
        /// TODO
        /// </summary>
        public string[] gameBoard { get; set; }
    }
}
