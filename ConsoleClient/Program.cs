using Microsoft.Rest;
using RestClientSdkLibrary;
using RestClientSdkLibrary.Models;
using System;

namespace ConsoleClient
{
    /// <summary>
    /// The program class itself of a console application used to play Tic Tac Toe with an AI via REST.
    /// </summary>
    class Program
    {
        // The URL string for local or Azure endpoints.
        // Comment/uncomment each depending on which you would like to use for playing.
        const string EndpointUrlString = "http://localhost:2932/";
        //const string EndpointUrlString = "https://csci-e94-assignment-1-nicholas-weber.azurewebsites.net";
        static RestClientSdkLibraryClient client = new RestClientSdkLibraryClient(new Uri(EndpointUrlString), new TokenCredentials("FakeTokenValue"));
        static Random randomNumberGenerator = new Random();
        const int NumberOfColumns = 3;
        static string playerSymbol = "X";
        static string azureSymbol = "O";

        /// <summary>
        /// The main function which starts the game loop of the console client to play Tic Tac Toe with a REST-based AI.
        /// </summary>
        /// <param name="args">The command line arguments (not used by this app)</param>
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to CSCI E-94 Assignment 1's:");
            Console.WriteLine("Tic Tac Toe (vs. Minimax AI)");
            Console.WriteLine("Author: Nicholas Weber.");
            Console.WriteLine();

            // Setup the main game loop where the play can view the current game state and intput their moves:
            bool isPlaying = true;
            ResetPlayerSymbols();
            string[] board = GetNewBoard();
            while (isPlaying)
            {
                // Print out the current game state for the play in the console:
                Console.WriteLine("You are " + playerSymbol);
                Console.WriteLine("Please enter the number of the cell you would like to move to:");
                DisplayBoard(board);

                // Take the player's next move:
                Console.Write("Your Move: ");
                string input = Console.ReadLine();

                // Validate the move input:
                Console.Clear();
                if (Int32.TryParse(input, out int move))
                {
                    // See if their move value falls within a normal board:
                    if(move >= 0 && move <= 8)
                    {
                        // See if their move input is an open cell of the game board:
                        if(!string.Equals(board[move], "?"))
                        {
                            Console.WriteLine("You may only make a move to an empty cell. Please try again.");
                            Console.WriteLine();
                        } else
                        {
                            // Send the executemove request to the API:
                            board[move] = playerSymbol;
                            ExecuteMoveRequest request = new ExecuteMoveRequest(azureSymbol, playerSymbol, board, move);
                            Console.WriteLine("You moved to: " + move);
                            Console.WriteLine("Azure is thinking...");
                            ExecuteMoveResponse response = (ExecuteMoveResponse)client.PostExecuteMove(request);

                            // Update local board state:
                            if (response.Move != null)
                            {
                                board[response.Move.Value] = azureSymbol;
                                Console.WriteLine("Azure responded by moving to: " + response.Move);
                                Console.WriteLine();
                            }

                            // Check for winner and update the console:
                            if(response.Winner != null && !string.Equals(response.Winner, "inconclusive"))
                            {
                                Console.WriteLine("You are " + playerSymbol);
                                DisplayBoard(board);
                                if (string.Equals(response.Winner, playerSymbol))
                                {
                                    Console.WriteLine("You win! Congratulations!");
                                }
                                else if (string.Equals(response.Winner, azureSymbol))
                                {

                                    Console.WriteLine("Azure wins! Better luck next time!");
                                }
                                else if (string.Equals(response.Winner, "tie"))
                                {
                                    Console.WriteLine("It's a tie! (this is a tough AI)");
                                }

                                // Reset the game so they can play again:
                                Console.WriteLine("(press any button to reset)");
                                Console.ReadLine();
                                Console.Clear();
                                board = GetNewBoard();
                                ResetPlayerSymbols();
                            }
                        }
                    } else
                    {
                        Console.WriteLine("Your move must be between 0 and 8. Please try again.");
                        Console.WriteLine();
                    }
                } else
                {
                    Console.WriteLine("Your move must be an integer. Please try again.");
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Resets the player and azure symbols randomly as either X or O (each will be opposite).
        /// </summary>
        static void ResetPlayerSymbols()
        {
            if (randomNumberGenerator.NextDouble() < 0.5)
            {
                playerSymbol = "X";
                azureSymbol = "O";
            } else
            {
                playerSymbol = "O";
                azureSymbol = "X";
            }
        }

        /// <summary>
        /// Creates a string array representing the nine cells of a Tic Tac Toe board where all cells begin as empty cells represented with a ? string.
        /// </summary>
        /// <returns>Returns a new array of nine ? strings.</returns>
        static string[] GetNewBoard()
        {
            return new string[] { "?", "?", "?", "?", "?", "?", "?", "?", "?" };
        }

        /// <summary>
        /// A helper function for printing the current board to the console.
        /// </summary>
        /// <param name="board">The board to be printed to console.</param>
        static void DisplayBoard(string[] board)
        {

            Console.WriteLine("Current Board (numbers are empty spaces):");
            for (int i = 0; i < NumberOfColumns; i++)
            {
                for (int j = 0; j < NumberOfColumns; j++)
                {
                    if (string.Equals(board[i * NumberOfColumns + j], "?"))
                    {
                        Console.Write(i * NumberOfColumns + j + " ");
                    }
                    else
                    {
                        Console.Write(board[i * NumberOfColumns + j] + " ");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
