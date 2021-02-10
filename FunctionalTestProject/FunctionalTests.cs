using Microsoft.Rest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClientSdkLibrary;
using RestClientSdkLibrary.Models;
using System;

namespace FunctionalTestProject
{
    /// <summary>
    /// TODO
    /// </summary>
    [TestClass]
    public class FunctionalTests
    {
        // The URL string for local or Azure endpoints.
        // Comment/uncomment each depending on which you would like to use for testing.
        const string EndpointUrlString = "http://localhost:2932/";
        //const string EndpointUrlString = "TODO: Azure Endpoint";

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

        /// <summary>
        /// TODO
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveWherePlayerXIsWinner()
        {
            // Arrange 
            string[] GameBoard = new string[]
            {
                X, X, X,
                _, O, _,
                O, _, _
            };
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest(O, X, GameBoard, 2);

            // Act
            ExecuteMoveResponse results = (ExecuteMoveResponse)client.Post(moveRequest);

            // Assert the response is not null (since this object will be used in subsequent assertions):
            Assert.IsNotNull(results);

            // Assert the gameboard has not changed in the response since the game was over (there is a winner):
            for (int i = 0; i < GameBoard.Length; i++)
            {
                Assert.AreEqual(GameBoard[i], results.GameBoard[i]);
            }

            // Assert the symbol of the winning player is correct:
            Assert.AreEqual(X, results.Winner);
        }

        /// <summary>
        /// TODO
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveWherePlayerOIsWinner()
        {
            // Arrange 
            string[] GameBoard = new string[]
            {
                X, X, O,
                _, O, _,
                O, _, _
            };
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest(X, O, GameBoard, 2);

            // Act
            ExecuteMoveResponse results = (ExecuteMoveResponse)client.Post(moveRequest);

            // Assert the response is not null (since this object will be used in subsequent assertions):
            Assert.IsNotNull(results);

            // Assert the gameboard has not changed in the response since the game was over (there is a winner):
            for(int i = 0; i < GameBoard.Length; i++)
            {
                Assert.AreEqual(GameBoard[i], results.GameBoard[i]);
            }

            // Assert the symbol of the winning player is correct:
            Assert.AreEqual(O, results.Winner);
        }

        /// <summary>
        /// TODO
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveWhereWinnerIsInconclusive()
        {
            // Arrange 
            string[] GameBoard = new string[]
            {
                X, X, _,
                _, O, _,
                O, _, _
            };
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest(X, O, GameBoard, 4);

            // Act
            ExecuteMoveResponse results = (ExecuteMoveResponse)client.Post(moveRequest);

            // Assert the response is not null (since this object will be used in subsequent assertions):
            Assert.IsNotNull(results);

            // Assert the gameboard has not changed in the response since the game was over (there is a winner):
            for (int i = 0; i < GameBoard.Length; i++)
            {
                Assert.AreEqual(results.GameBoard[i], GameBoard[i]);
            }

            // Assert the symbol of the winning player is correct:
            Assert.AreEqual("inconclusive", results.Winner);
        }

        /// <summary>
        /// TODO
        /// </summary>
        [TestMethod]
        public void TestPostExecuteMoveWhereTheGameIsATie()
        {
            // Arrange 
            string[] GameBoard = new string[]
            {
                X, O, X,
                O, X, O,
                O, X, O
            };
            RestClientSdkLibraryClient client = this.GetRestSdkClient();
            ExecuteMoveRequest moveRequest = new ExecuteMoveRequest(X, O, GameBoard, 1);

            // Act
            ExecuteMoveResponse results = (ExecuteMoveResponse)client.Post(moveRequest);

            // Assert the response is not null (since this object will be used in subsequent assertions):
            Assert.IsNotNull(results);

            // Assert the gameboard has not changed in the response since the game was over (there is a winner):
            for (int i = 0; i < GameBoard.Length; i++)
            {
                Assert.AreEqual(results.GameBoard[i], GameBoard[i]);
            }

            // Assert the symbol of the winning player is correct:
            Assert.AreEqual("tie", results.Winner);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns>TODO</returns>
        private RestClientSdkLibraryClient GetRestSdkClient()
        {
            ServiceClientCredentials serviceClientCredentials = new TokenCredentials("FakeTokenValue");
            return new RestClientSdkLibraryClient(new Uri(EndpointUrlString), serviceClientCredentials);
        }
    }
}
