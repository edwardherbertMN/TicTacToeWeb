The code included is a Web based conversion of a Tic Tac Toe program I wrote in college. It has the ability to either play competitively against other players by creating and joining a game room or to play against an AI computer player. Each game room also comes with a chat box that can be used to talk to the other player or receive game updates. The game and chat are designed to run in a real time single page app.



It uses ASP.Net Core and SignalR to deliver the game page. A SignalR Hub pipes messages between clients communicating via Javascript and a C# backend which handles game logic on the server side. The entire app runs in memory without a database, using a cache to hold individual game instances which timeout after 10 minutes of inactivity or when the last player leaves. This approach was due to the shortness of the game not being seen as justifying the overheads of a database. The AI is also coded in C# and is a rule based scripted AI.

Included are two versions of the code, a full visual studio solution containing the entire project and an alternative selection of code without the project boilerplate. 
Files:
Index.cshtml - Base HTML layout
site.js - Client side code to handle piping messages to server and reacting to responses
TicTacToeAI.cs and TicTacToeGame.cs - The backend game code and AI
TicTacToeGameSession.cs - Wrapper class
TicTacToeGameHub.cs - Handles piping messages and changes to game state between clients and game

The game is currently hosted at http://edwardherbertmn.azurewebsites.net/

