﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace RestClientSdkLibrary.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;

    /// <summary>
    /// Represents the response based on the Azure AI's calculations of what
    /// the best move to make for a given player is given the input data in
    /// the request.
    /// </summary>
    public partial class CalculateMoveResponse
    {
        /// <summary>
        /// Initializes a new instance of the CalculateMoveResponse class.
        /// </summary>
        public CalculateMoveResponse() { }

        /// <summary>
        /// Initializes a new instance of the CalculateMoveResponse class.
        /// </summary>
        public CalculateMoveResponse(string playerSymbol, IList<string> gameBoard, int? move = default(int?), string winner = default(string), IList<int?> winPositions = default(IList<int?>))
        {
            PlayerSymbol = playerSymbol;
            GameBoard = gameBoard;
            Move = move;
            Winner = winner;
            WinPositions = winPositions;
        }

        /// <summary>
        /// The symbol of the player to calculate a move for.
        /// </summary>
        [JsonProperty(PropertyName = "playerSymbol")]
        public string PlayerSymbol { get; set; }

        /// <summary>
        /// An array of X, O, and ? symbols representing the current state of
        /// the game board where ? is an open space and the other symbols
        /// correspond to players.
        /// </summary>
        [JsonProperty(PropertyName = "gameBoard")]
        public IList<string> GameBoard { get; set; }

        /// <summary>
        /// Indicates the position on the board the last actor to update state
        /// chooses.
        /// This is a Nullable type because, if it were not, the value would
        /// default to 0 if it was not present in the request body.
        /// The move field is not required if and only if the game board is
        /// empty (contains only ? symbols)
        /// </summary>
        [JsonProperty(PropertyName = "move")]
        public int? Move { get; set; }

        /// <summary>
        /// Indicates the gameboard status whether there is a winner (the
        /// winning player's symbol), a tie, or whether it is inconclusive
        /// and the game can continue.
        /// </summary>
        [JsonProperty(PropertyName = "winner")]
        public string Winner { get; set; }

        /// <summary>
        /// An array that lists the zero-based position index values of the
        /// win or null if no winner is present in the gameBoard array.
        /// </summary>
        [JsonProperty(PropertyName = "winPositions")]
        public IList<int?> WinPositions { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (PlayerSymbol == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "PlayerSymbol");
            }
            if (GameBoard == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "GameBoard");
            }
            if (this.PlayerSymbol != null)
            {
                if (this.PlayerSymbol.Length > 1)
                {
                    throw new ValidationException(ValidationRules.MaxLength, "PlayerSymbol", 1);
                }
                if (this.PlayerSymbol.Length < 0)
                {
                    throw new ValidationException(ValidationRules.MinLength, "PlayerSymbol", 0);
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(this.PlayerSymbol, "X|O"))
                {
                    throw new ValidationException(ValidationRules.Pattern, "PlayerSymbol", "X|O");
                }
            }
            if (this.GameBoard != null)
            {
                if (this.GameBoard.Count > 9)
                {
                    throw new ValidationException(ValidationRules.MaxItems, "GameBoard", 9);
                }
                if (this.GameBoard.Count < 9)
                {
                    throw new ValidationException(ValidationRules.MinItems, "GameBoard", 9);
                }
            }
            if (this.Move > 8)
            {
                throw new ValidationException(ValidationRules.InclusiveMaximum, "Move", 8);
            }
            if (this.Move < 0)
            {
                throw new ValidationException(ValidationRules.InclusiveMinimum, "Move", 0);
            }
            if (this.Winner != null)
            {
                if (this.Winner.Length > 1)
                {
                    throw new ValidationException(ValidationRules.MaxLength, "Winner", 1);
                }
                if (this.Winner.Length < 0)
                {
                    throw new ValidationException(ValidationRules.MinLength, "Winner", 0);
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(this.Winner, "X|O"))
                {
                    throw new ValidationException(ValidationRules.Pattern, "Winner", "X|O");
                }
            }
            if (this.WinPositions != null)
            {
                if (this.WinPositions.Count > 3)
                {
                    throw new ValidationException(ValidationRules.MaxItems, "WinPositions", 3);
                }
                if (this.WinPositions.Count < 3)
                {
                    throw new ValidationException(ValidationRules.MinItems, "WinPositions", 3);
                }
            }
        }
    }
}
