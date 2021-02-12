using Microsoft.AspNetCore.Http;
using Microsoft.Rest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClientSdkLibrary;
using RestClientSdkLibrary.Models;
using System;
using System.Diagnostics;
using System.Text.Json;

namespace FunctionalTestProject
{
    /// <summary>
    /// This a a class of tests to verify the functionality of the Tic Tac Toe web app game.
    /// </summary>
    [TestClass]
    public class FunctionalTests
    {
        // The URL string for local or Azure endpoints.
        // Comment/uncomment each depending on which you would like to use for testing.
        const string EndpointUrlString = "http://localhost:2932/";
        //const string EndpointUrlString = "https://csci-e94-assignment-1-nicholas-weber.azurewebsites.net";

        /// <summary>
        /// The string symbol used to represent the X player on the game board.
        /// This is used to reduce the number of quotation marks written to setup tests.
        /// </summary>
        const string X = "X";

        /// <summary>
        /// The string symbol used to represent the O player on the game board.
        /// This is used to reduce the number of quotation marks written to setup tests.
        /// </summary>
        const string O = "O";

        /// <summary>
        /// The string symbol used to represent an empty cell on the game board.
        /// This is used to reduce the number of quotation marks written to setup tests.
        /// </summary>
        const string _ = "?";

        #region Verify the detection of 'player X winner'

        /// <summary>
        /// This is a functional test to assert the general ExecuteMove response to a victory condition when player X has won is correct.
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveWherePlayerXIsWinner()
        {
            // Arrange the game board so that X wins
            string[] GameBoard = new string[]
            {
                X, X, X,
                _, O, _,
                O, _, _
            };
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest(O, X, GameBoard, 2);

            // Act
            ExecuteMoveResponse results = (ExecuteMoveResponse)client.PostExecuteMove(moveRequest);

            // Assert the response is not null (since this object will be used in subsequent assertions):
            Assert.IsNotNull(results);

            // Assert the player symbols have not changed between the request and the response:
            Assert.AreEqual(moveRequest.AzurePlayerSymbol, results.AzurePlayerSymbol);
            Assert.AreEqual(moveRequest.HumanPlayerSymbol, results.HumanPlayerSymbol);

            // Assert the gameboard has not changed in the response since the game was over (there is a winner):
            for (int i = 0; i < GameBoard.Length; i++)
            {
                Assert.AreEqual(GameBoard[i], results.GameBoard[i]);
            }

            // Assert the symbol of the winning player is correct:
            Assert.AreEqual(X, results.Winner);

            // Assert the win positions in the respone are correct:
            Assert.AreEqual(3, results.WinPositions.Count);
            Assert.AreEqual(0, results.WinPositions[0]);
            Assert.AreEqual(1, results.WinPositions[1]);
            Assert.AreEqual(2, results.WinPositions[2]);

            // Assert the move field is null:
            Assert.IsNull(results.Move);
        }

        #endregion

        #region Verify the detection of 'player O winner'

        /// <summary>
        /// This is a functional test to assert the general ExecuteMove response to a victory condition when player O has won is correct.
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveWherePlayerOIsWinner()
        {
            // Arrange the game board so that O wins.
            string[] GameBoard = new string[]
            {
                X, X, O,
                _, O, _,
                O, _, _
            };
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest(X, O, GameBoard, 2);

            // Act
            ExecuteMoveResponse results = (ExecuteMoveResponse)client.PostExecuteMove(moveRequest);

            // Assert the response is not null (since this object will be used in subsequent assertions):
            Assert.IsNotNull(results);

            // Assert the gameboard has not changed in the response since the game was over (there is a winner):
            for (int i = 0; i < GameBoard.Length; i++)
            {
                Assert.AreEqual(GameBoard[i], results.GameBoard[i]);
            }

            // Assert the player symbols have not changed between the request and the response:
            Assert.AreEqual(moveRequest.AzurePlayerSymbol, results.AzurePlayerSymbol);
            Assert.AreEqual(moveRequest.HumanPlayerSymbol, results.HumanPlayerSymbol);

            // Assert the symbol of the winning player is correct:
            Assert.AreEqual(O, results.Winner);

            // Assert the win positions in the respone are correct:
            Assert.AreEqual(3, results.WinPositions.Count);
            Assert.AreEqual(2, results.WinPositions[0]);
            Assert.AreEqual(4, results.WinPositions[1]);
            Assert.AreEqual(6, results.WinPositions[2]);

            // Assert the move field is null:
            Assert.IsNull(results.Move);
        }

        #endregion

        /// <summary>
        /// This is a functional test to assert "inconclusive" is entered as the winner in a response where the game board is not yet full but no one has 3 in a row.
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveWhereWinnerIsInconclusive()
        {
            // Arrange the game board so there is no winner, but there are still moves to be made.
            string[] GameBoard = new string[]
            {
                X, _, _,
                _, O, _,
                O, _, _
            };
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest(X, O, GameBoard, 4);

            // Act
            ExecuteMoveResponse results = (ExecuteMoveResponse)client.PostExecuteMove(moveRequest);

            // Assert the response is not null (since this object will be used in subsequent assertions):
            Assert.IsNotNull(results);

            // Assert the player symbols have not changed between the request and the response:
            Assert.AreEqual(moveRequest.AzurePlayerSymbol, results.AzurePlayerSymbol);
            Assert.AreEqual(moveRequest.HumanPlayerSymbol, results.HumanPlayerSymbol);

            // Assert the symbol of the winning player is correct:
            Assert.AreEqual("inconclusive", results.Winner);

            // Assert the Azure AI made a move:
            Assert.IsNotNull(results.Move);

            // Assert the gameboard has not changed in the response since the game was over (there is a winner):
            for (int i = 0; i < GameBoard.Length; i++)
            {
                if (i != results.Move)
                {
                    Assert.AreEqual(results.GameBoard[i], GameBoard[i]);
                } else
                {
                    Assert.AreEqual(results.GameBoard[i], results.AzurePlayerSymbol);
                }
            }

            // Assert the win positions in the respone are correct:
            Assert.IsNull(results.WinPositions);
        }

        /// <summary>
        /// This is a functional test to assert Azure will make the first move if given an empty board.
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveAzureMakesMoveWhenBoaardIsEmpty()
        {
            // Arrange the empty game board
            string[] GameBoard = new string[]
            {
                _, _, _,
                _, _, _,
                _, _, _,
            };
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest(X, O, GameBoard);

            // Act
            ExecuteMoveResponse results = (ExecuteMoveResponse)client.PostExecuteMove(moveRequest);

            // Assert the response is not null (since this object will be used in subsequent assertions):
            Assert.IsNotNull(results);

            // Assert the player symbols have not changed between the request and the response:
            Assert.AreEqual(moveRequest.AzurePlayerSymbol, results.AzurePlayerSymbol);
            Assert.AreEqual(moveRequest.HumanPlayerSymbol, results.HumanPlayerSymbol);

            // Assert the symbol of the winning player is correct:
            Assert.AreEqual("inconclusive", results.Winner);

            // Assert the Azure AI made a move:
            Assert.IsNotNull(results.Move);

            // Assert the gameboard has not changed in the response since the game was over (there is a winner):
            for (int i = 0; i < GameBoard.Length; i++)
            {
                if (i != results.Move)
                {
                    Assert.AreEqual(results.GameBoard[i], GameBoard[i]);
                }
                else
                {
                    Assert.AreEqual(results.GameBoard[i], results.AzurePlayerSymbol);
                }
            }

            // Assert the win positions in the respone are correct:
            Assert.IsNull(results.WinPositions);
        }

        /// <summary>
        /// This is a functional test to assert "tie" is entered as the winner in a response where the game board is full but no one has 3 in a row.
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveWhereTheGameIsATie()
        {
            // Arrange the game board so there is no winner and no open cells for available moves.
            string[] GameBoard = new string[]
            {
                X, O, X,
                O, X, O,
                O, X, O
            };
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest(X, O, GameBoard, 1);

            // Act
            ExecuteMoveResponse results = (ExecuteMoveResponse)client.PostExecuteMove(moveRequest);

            // Assert the response is not null (since this object will be used in subsequent assertions):
            Assert.IsNotNull(results);

            // Assert the gameboard has not changed in the response since the game was over (there is a winner):
            for (int i = 0; i < GameBoard.Length; i++)
            {
                Assert.AreEqual(results.GameBoard[i], GameBoard[i]);
            }

            // Assert the symbol of the winning player is correct:
            Assert.AreEqual("tie", results.Winner);

            // Assert the win positions in the respone are correct:
            Assert.IsNull(results.WinPositions);

            // Assert the move field is null:
            Assert.IsNull(results.Move);
        }

        /// <summary>
        /// This is a functional test to assert a BadRequest is returned when both players are said to use the X symbol.
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveBothPlayersCannotBeX()
        {
            // Arrange the game board so that no moves have been made
            string[] GameBoard = new string[]
            {
                _, _, _,
                _, _, _,
                _, _, _
            };

            // Arrange an execute move request where both players have the same symbol as X
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest(X, X, GameBoard);

            // Act
            string results = (string)client.PostExecuteMove(moveRequest);

            // Assert the response object is not null to prove it was a BadRequest:
            Assert.IsNotNull(results);

            // Assert the BadRequest description string is correct:
            Assert.AreEqual("The Azure and Human players must use different symbols. Only X and O are permissable.", results);
        }

        /// <summary>
        /// This is a functional test to assert a BadRequest is returned when both players are said to use the O symbol.
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveBothPlayersCannotBeO()
        {
            // Arrange 
            string[] GameBoard = new string[]
            {
                _, _, _,
                _, _, _,
                _, _, _
            };

            // Arrange an execute move request where both players have the same symbol as O
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest(O, O, GameBoard, 1);

            // Act
            string results = (string)client.PostExecuteMove(moveRequest);

            // Assert the response object is not null to prove it was a BadRequest:
            Assert.IsNotNull(results);

            // Assert the BadRequest description string is correct:
            Assert.AreEqual("The Azure and Human players must use different symbols. Only X and O are permissable.", results);
        }

        #region Verify proper handling of invalid data

        /// <summary>
        /// This is a functional test to assert a BadRequest is returned when the human player tries to use an invalid symbol.
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveReturnsBadRequestWhenHumanPlayerSymbolIsInvalid()
        {
            // Arrange 
            string[] GameBoard = new string[]
            {
                _, _, _,
                _, _, _,
                _, _, _
            };

            // Arrange an execute move request where a player used an invalid symbol "Y"
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest(X, "Y", GameBoard);

            // Act
            string response = (string)client.PostExecuteMove(moveRequest);

            // Assert the response object is not null to prove it was a BadRequest:
            Assert.IsNotNull(response);

            // Assert the BadRequest description string is correct:
            Assert.IsTrue(response.Contains("The field humanPlayerSymbol must match the regular expression 'X|O'"));
        }

        // <summary>
        /// This is a functional test to assert a BadRequest is returned when the Azure player is assigned an invalid symbol.
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveReturnsBadRequestWhenAzurePlayerSymbolIsInvalid()
        {
            // Arrange 
            string[] GameBoard = new string[]
            {
                _, _, _,
                _, _, _,
                _, _, _
            };

            // Arrange an execute move request where a player used an invalid symbol "Y"
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest("Y", X, GameBoard);

            // Act
            string response = (string)client.PostExecuteMove(moveRequest);

            // Assert the response object is not null to prove it was a BadRequest:
            Assert.IsNotNull(response);

            // Assert the BadRequest description string is correct:
            Assert.IsTrue(response.Contains("The field azurePlayerSymbol must match the regular expression 'X|O'"));
        }

        /// <summary>
        /// This is a functional test to assert a BadRequest is returned when both players are said to use the O symbol.
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveReturnsBadRequestWhenInvalidCharacterIsInTheGameBoard()
        {
            // Arrange a game board which contains an invalid symbol
            string[] GameBoard = new string[]
            {
                X, _, "Y",
                _, O, _,
                _, _, _
            };
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest(X, O, GameBoard, 4);

            // Act
            string response = (string)client.PostExecuteMove(moveRequest);

            // Assert the response object is not null to prove it was a BadRequest:
            Assert.IsNotNull(response);

            // Assert the BadRequest description string is correct:
            Assert.IsTrue(response.Contains("The game board contains an invalid character. Only X, O, and ? are permissable."));
        }

        /// <summary>
        /// This is a functional test to assert a BadRequest is returned when the game board contains an invalid number of moves by one player -> three Xs and only 1 O
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveReturnsBadRequestWhenTheGameBoardContainsInvalidNumberOfMovesByAPlayer()
        {
            // Arrange a game board which contains an invalid symbol
            string[] GameBoard = new string[]
            {
                X, X, X,
                _, O, _,
                _, _, _
            };
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest(O, X, GameBoard, 4);

            // Act
            string response = (string)client.PostExecuteMove(moveRequest);

            // Assert the response object is not null to prove it was a BadRequest:
            Assert.IsNotNull(response);

            // Assert the BadRequest description string is correct:
            Assert.IsTrue(response.Contains("The game board contains an illegal move. There must be an equal number of X and O moves within the board +/- 1 for the most recent move."));
        }

        /// <summary>
        /// This is a functional test to assert a BadRequest is returned when the game board contains fewer than 9 elements in its array
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveReturnsBadRequestWhenTheGameBoardContainsTooFewElements()
        {
            // Arrange a game board which contains too few elements (less than 9)
            string[] GameBoard = new string[]
            {
                _, _, _,
                _, _, _
            };
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest(O, X, GameBoard);

            // Act
            string response = (string)client.PostExecuteMove(moveRequest);

            // Assert the response object is not null to prove it was a BadRequest:
            Assert.IsNotNull(response);

            // Assert the BadRequest description string is correct:
            Assert.IsTrue(response.Contains("The field gameBoard must be a string or array type with a minimum length of '9'."));
        }

        /// <summary>
        /// This is a functional test to assert a BadRequest is returned when the game board contains more than 9 elements in its array
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveReturnsBadRequestWhenTheGameBoardContainsTooManyElements()
        {
            // Arrange a game board which contains too many elements (more than 9)
            string[] GameBoard = new string[]
            {
                _, _, _,
                _, _, _,
                _, _, _,
                _, _, _
            };
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest(O, X, GameBoard);

            // Act
            string response = (string)client.PostExecuteMove(moveRequest);

            // Assert the response object is not null to prove it was a BadRequest:
            Assert.IsNotNull(response);

            // Assert the BadRequest description string is correct:
            Assert.IsTrue(response.Contains("The field gameBoard must be a string or array type with a maximum length of '9'."));
        }

        /// <summary>
        /// This is a functional test to assert a BadRequest is returned when the move made by the player in the request is actually the other player's symbol on the game board sent.
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveReturnsBadRequestWhenPlayerMoveIsNotTheirSymbol()
        {
            // Arrange a game board with both player's symbols
            string[] GameBoard = new string[]
            {
                X, _, _,
                O, O, _,
                X, _, _
            };

            // Arrange a request where the 'move' field corresponds to the other player's symbol on the game board.
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest(O, X, GameBoard, 3);

            // Act
            string response = (string)client.PostExecuteMove(moveRequest);

            // Assert the response object is not null to prove it was a BadRequest:
            Assert.IsNotNull(response);

            // Assert the BadRequest description string is correct:
            Assert.IsTrue(response.Contains("The move does not match the game board state. The symbol on the game board at the move's index must match the human player's symbol."));
        }

        /// <summary>
        /// This is a functional test to assert a BadRequest is returned when the move made by the player in the request's move field is not reflect in the game board sent.
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveReturnsBadRequestWhenPlayerMoveIsNotPresentInGameBoard()
        {
            // Arrange a game board with both player's symbols
            string[] GameBoard = new string[]
            {
                X, _, _,
                _, O, _,
                X, _, _
            };

            // Arrange a request where the 'move' field corresponds to a "?" symbol on the game board.
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest(X, O, GameBoard, 3);

            // Act
            string response = (string)client.PostExecuteMove(moveRequest);

            // Assert the response object is not null to prove it was a BadRequest:
            Assert.IsNotNull(response);

            // Assert the BadRequest description string is correct:
            Assert.IsTrue(response.Contains("The move does not match the game board state. The symbol on the game board at the move's index must match the human player's symbol."));
        }

        /// <summary>
        /// This is a functional test to assert a BadRequest is returned when the game board is not empty, but the player did not send their next move in the move field.
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveReturnsBadRequestWhenPlayerMoveIsNotPresentInRequestButGameBoardIsNotEmpty()
        {
            // Arrange a non-empty game board
            string[] GameBoard = new string[]
            {
                X, _, _,
                _, O, _,
                X, _, _
            };

            // Arrange a request with no 'move' value
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest(X, O, GameBoard);

            // Act
            string response = (string)client.PostExecuteMove(moveRequest);

            // Assert the response object is not null to prove it was a BadRequest:
            Assert.IsNotNull(response);

            // Assert the BadRequest description string is correct:
            Assert.IsTrue(response.Contains("The move field does not contain a value, but the board is not empty."));
        }

        /// <summary>
        /// This is a functional test to assert a BadRequest is returned when Azure should not be the next player to move given how many moves each player has made -- human has fewer symbols on the board, but has also just moved.
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveReturnsBadRequestWhenAzureHasMadeMoreMovesThanThePlayerInTheGameBoardSent()
        {
            // Arrange a game board where the human player has fewer moves made than Azure
            string[] GameBoard = new string[]
            {
                X, O, X,
                _, O, _,
                X, _, _
            };

            // Arrange a request where the human has just moved, but according to the game board it should still be the human's move
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest(X, O, GameBoard, 4);

            // Act
            string response = (string)client.PostExecuteMove(moveRequest);

            // Assert the response object is not null to prove it was a BadRequest:
            Assert.IsNotNull(response);

            // Assert the BadRequest description string is correct:
            Assert.IsTrue(response.Contains("The game board is invalid because the Azure player is not the next player to move and therefore cannot execute a valid move from the current board state."));
        }

        #endregion

        /// <summary>
        /// This is a functional test to assert a BadRequest is returned when Azure should not be the next player to move given how many moves each player has made -- human has fewer symbols on the board, but has also just moved.
        /// </summary>
        [TestMethod]
        public void TestPostCalculateMoveCanPlayTicTacToeAgainstItself()
        {
            // Arrange an empty starting game board
            string[] gameBoard = new string[]
            {
                _, _, _,
                _, _, _,
                _, _, _
            };

            // Arrange a random starting symbol for the player:
            string playerSymbol = "X";
            Random randomNumberGenerator = new Random();
            if(randomNumberGenerator.NextDouble() < 0.5f)
            {
                playerSymbol = "O";
            }

            // Arrange a REST client object to send requests to the server:
            RestClientSdkLibraryClient client = this.GetRestSdkClient();

            // Act - loop until the game is over due to a winner or tie
            bool gameIsOver = false;
            while(!gameIsOver)
            {
                // Swap the player symbol so that the next player gets to make a turn:
                if(string.Equals("X", playerSymbol))
                {
                    playerSymbol = "O";
                } else
                {
                    playerSymbol = "X";
                }

                // Generate a request given current state for this player:
                CalculateMoveRequest moveRequest = new CalculateMoveRequest(playerSymbol, gameBoard);
                Debug.Write("Request: ");
                Debug.WriteLine(JsonSerializer.Serialize(moveRequest));

                // Send the calculate move request:
                CalculateMoveResponse response = (CalculateMoveResponse)client.PostCalculateMove(moveRequest);
                Debug.Write("Response: ");
                Debug.WriteLine(JsonSerializer.Serialize(response));
                Assert.AreEqual(playerSymbol, response.PlayerSymbol);

                // Assert that every cell of the response board is valid:
                for(int i = 0; i < response.GameBoard.Count; i++)
                {
                    Assert.IsTrue(string.Equals("X", response.GameBoard[i]) || string.Equals("O", response.GameBoard[i]) || string.Equals("?", response.GameBoard[i]));
                }

                // Update and display the local board state:
                if(response.Move != null)
                {
                    gameBoard[response.Move.Value] = playerSymbol;
                }
                Debug.WriteLine("Board: ");
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (string.Equals(gameBoard[i * 3 + j], "?"))
                        {
                            Debug.Write(i * 3 + j + " ");
                        }
                        else
                        {
                            Debug.Write(gameBoard[i * 3 + j] + " ");
                        }
                    }
                    Debug.WriteLine(string.Empty);
                }

                // Check for a winner:
                if(response.Winner != null)
                {
                    // Assert that the winner field value is valid:
                    Assert.IsTrue(string.Equals("X", response.Winner) || string.Equals("O", response.Winner) || string.Equals("tie", response.Winner) || string.Equals("inconclusive", response.Winner));

                    if (!string.Equals("inconclusive", response.Winner))
                    {
                        Debug.WriteLine("The winner is: " + response.Winner);
                        gameIsOver = true;

                        // Assert the win positions are present when there is a winner:
                        if(!string.Equals("tie", response.Winner))
                        {
                            Assert.IsNotNull(response.WinPositions);
                            Assert.AreEqual(3, response.WinPositions.Count);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This is a helper function used by all of the functional tests to instnatiate the REST client object.
        /// </summary>
        /// <returns>TODO</returns>
        private RestClientSdkLibraryClient GetRestSdkClient()
        {
            ServiceClientCredentials serviceClientCredentials = new TokenCredentials("FakeTokenValue");
            return new RestClientSdkLibraryClient(new Uri(EndpointUrlString), serviceClientCredentials);
        }
    }
}
