using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.DataTransferObjects
{
    /// <summary>
    /// TODO
    /// </summary>
    public class ExecuteMoveResponse : Move
    {
        /// <summary>
        /// TODO
        /// </summary>
        public string winner { get; set; }

        /// <summary>
        /// TODO
        /// </summary>
        public string winPositions { get; set; }
    }
}
