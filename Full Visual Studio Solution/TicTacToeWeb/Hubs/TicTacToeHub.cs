using TicTacToe;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicTacToeWeb;

/*
    Games are managed by piping user input to a backend server side dll and then piping updates back to players
    Game Sessions are stored in memory in a cache and retrieved by user generated roomID
    They expire after 10 minutes of inactivity
    Due to the short nature of games, it seemed more reasonable to avoid database overheads but risk data loss

    Game rooms are limited to 2 active players

    Whilst the room could be provided by the user when leaving it cannot be assumed that they will disconnect gracefully
    In that case only a ConnectionID is provided and so this is mapped to roomID in a static dictionary
 */

namespace TicTacToeWeb.Hubs
{
    public class TicTacToeHub : Hub
    {
        private IMemoryCache gameCache;
        private static Dictionary<string, string> playerRoomMap = new Dictionary<string, string>();

        public TicTacToeHub(IMemoryCache gameCache)
        {
            this.gameCache = gameCache;
        }

        public void BroadcastMessage(string message, string roomID) => Clients.Group(roomID).InvokeAsync("DisplayMessage", message);
        public void SendMessage(string message, string playerID) => Clients.Client(playerID).InvokeAsync("DisplayMessage", message);

        public void PlayComputer()
        {
            var roomID = Context.ConnectionId;
            var game = GameSession(roomID);
            JoinRoom(roomID);
            game.EnableAI();
            Clients.Group(roomID).InvokeAsync("ResetGame");
            Clients.Group(roomID).InvokeAsync("SetRoomID", roomID);
        }

        public void JoinRoom(string roomID)
        {
            var game = GameSession(roomID);
            var playerID = Context.ConnectionId;
            try
            {
                var playerSymbol = game.AddPlayer(playerID);
                
                Clients.Client(playerID).InvokeAsync("SetPlayer", playerSymbol.ToString());
                Clients.Client(playerID).InvokeAsync("ToggleGameMode");

                Groups.AddAsync(playerID, roomID);
                playerRoomMap.Add(playerID, roomID);
                if (game.IsReady())
                    Clients.Group(roomID).InvokeAsync("ResetGame");
            }
            catch (ArgumentException e)
            {
                SendMessage(e.Message, playerID);
            }
        }

        public void LeaveRoom()
        {
            var playerID = Context.ConnectionId;
            var roomID = CurrentRoom(playerID);
            if (roomID == null)
                return;
            var game = GameSession(roomID);
            game.RemovePlayer(playerID);
            game.Reset();

            Groups.RemoveAsync(playerID, roomID);
            Clients.Client(playerID).InvokeAsync("ToggleGameMode");
            UnmapPlayer(playerID);
            BroadcastMessage("Player left", roomID);
            if (game.IsEmpty())
                gameCache.Remove(roomID);
        }

        public void MakeMove(string roomID, int pos)
        {
            var game = GameSession(roomID);
            var playerID = Context.ConnectionId;
            try
            {
                var playerSymbol = game.ActivePlayer;
                game.MakeMove(playerID, pos);
                Clients.Group(game.ID).InvokeAsync("UpdateTile", pos, playerSymbol);
                if (game.GameState != GameState.InProgress)
                {
                    NotifyGameOver(game);
                    return;
                }

                if (!game.IsSinglePlayer())
                    return;

                playerSymbol = game.ActivePlayer;
                int AIMove = game.MakeAIMove();
                Clients.Group(game.ID).InvokeAsync("UpdateTile", AIMove, playerSymbol);
                if (game.GameState != GameState.InProgress)
                {
                    NotifyGameOver(game);
                    return;
                }
            }
            catch (ArgumentException e)
            {
                SendMessage(e.Message, playerID);
            }
        }

        private void NotifyGameOver(TicTacToeGameSession game)
        {
            BroadcastMessage(game.FriendlyGameState(), game.ID);
            Clients.Group(game.ID).InvokeAsync("ResetGame");
            game.Reset();
        }
        
        public override Task OnDisconnectedAsync(Exception exception)
        {
            LeaveRoom();
            return base.OnDisconnectedAsync(exception);
        }

        private TicTacToeGameSession GameSession(string roomID)
        {
            return gameCache.GetOrCreate(roomID, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                var game = new TicTacToeGameSession(roomID);
                return game;
            });
        }

        /*
            Whilst the room could be provided by the user when leaving it cannot be assumed that they will disconnect gracefully
            In that case only a ConnectionID is provided and so this is mapped to roomID in a static dictionary
        */
        private static string CurrentRoom(string userID)
        {
            if (!playerRoomMap.ContainsKey(userID))
                return null;
            return playerRoomMap[userID];
        }

        private static void UnmapPlayer(string userID)
        {
            playerRoomMap.Remove(userID);
        }

    }
}
