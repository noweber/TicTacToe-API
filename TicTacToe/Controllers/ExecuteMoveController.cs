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
        /// TODO
        /// </summary>
        /// <returns>TODO</returns>
        [HttpPost]
        public IActionResult Post(Move moveRequest)
        {
            return null;
        }
    }
}
