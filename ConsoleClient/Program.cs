using Microsoft.Rest;
using RestClientSdkLibrary;
using RestClientSdkLibrary.Models;
using System;

namespace ConsoleClient
{
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

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to CSCI E-94 Assignment 1's:");
            Console.WriteLine("Tic Tac Toe (vs. Minimax AI)");
            Console.WriteLine("Author: Nicholas Weber.");
            Console.WriteLine();

            bool isPlaying = true;
            ResetPlayerSymbols();
            string[] board = GetNewBoard();
            while (isPlaying)
            {
                Console.WriteLine("You are " + playerSymbol);
                Console.WriteLine("Please enter the number of the cell you would like to move to:");
                DisplayBoard(board);
                Console.Write("Your Move: ");
                string input = Console.ReadLine();
                Console.Clear();
                if (Int32.TryParse(input, out int move))
                {
                    if(move >= 0 && move <= 8)
                    {
                        // Validate the move:
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
                            ExecuteMoveResponse response = (ExecuteMoveResponse)client.Post(request);

                            // Update local board state:
                            if (response.Move != null)
                            {
                                board[response.Move.Value] = azureSymbol;
                                Console.WriteLine("Azure responded by moving to: " + response.Move);
                                Console.WriteLine();
                            } else
                            {
                                // Check for winner:
                                if(response.Winner != null && !string.Equals(response.Winner, "inconclusive"))
                                {
                                    if(string.Equals(response.Winner, playerSymbol))
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

                                    Console.WriteLine("(press any button to reset)");
                                    Console.ReadLine();
                                    Console.Clear();
                                    board = GetNewBoard();
                                    ResetPlayerSymbols();
                                }
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

        static string[] GetNewBoard()
        {
            return new string[] { "?", "?", "?", "?", "?", "?", "?", "?", "?" };
        }

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
