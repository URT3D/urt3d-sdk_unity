using System.Collections.Generic;
using Miniscript;
using UnityEngine;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Partial UrtScriptBridge class containing networking and multiplayer functions
    /// </summary>
    public partial class UrtScriptBridge
    {
        /// <summary>
        /// Register functions for networking and multiplayer
        /// </summary>
        private void RegisterNetworkingFunctions()
        {
            // Connect to server
            var connectToServer = Intrinsic.Create("connectToServer");
            connectToServer.AddParam("serverAddress");
            connectToServer.AddParam("port", 0);
            connectToServer.AddParam("username", "");
            connectToServer.AddParam("password", "");
            connectToServer.code = (context, _) => {
                var serverAddress = context.GetVar("serverAddress").ToString();
                var port = (int)context.GetVar("port").DoubleValue();
                var username = context.GetVar("username").ToString();
                var password = context.GetVar("password").ToString();

                Debug.Log($"[MiniScript] Connecting to server {serverAddress}:{port}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(connectToServer);

            // Disconnect from server
            var disconnectFromServer = Intrinsic.Create("disconnectFromServer");
            disconnectFromServer.code = (_, _) => {
                Debug.Log("[MiniScript] Disconnecting from server");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(disconnectFromServer);

            // Send data to server
            var sendToServer = Intrinsic.Create("sendToServer");
            sendToServer.AddParam("eventName");
            sendToServer.AddParam("data");
            sendToServer.code = (context, _) => {
                var eventName = context.GetVar("eventName").ToString();
                var data = context.GetVar("data");

                Debug.Log($"[MiniScript] Sending event {eventName} to server with data {data}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(sendToServer);

            // Get connection state
            var getConnectionState = Intrinsic.Create("getConnectionState");
            getConnectionState.code = (_, _) => {
                Debug.Log("[MiniScript] Getting connection state");

                // Return placeholder connection state
                var connectionState = new ValMap
                {
                    ["connected"] = ValNumber.zero,
                    ["ping"] = new ValNumber(0),
                    ["serverAddress"] = new ValString("")
                };

                return new Intrinsic.Result(connectionState);
            };
            _registeredIntrinsics.Add(getConnectionState);

            // Get online players
            var getOnlinePlayers = Intrinsic.Create("getOnlinePlayers");
            getOnlinePlayers.code = (_, _) => {
                Debug.Log("[MiniScript] Getting online players");

                // Return placeholder player list using array constructor
                var player1 = new ValMap
                {
                    ["id"] = new ValString("player1"),
                    ["name"] = new ValString("Player 1")
                };

                var player2 = new ValMap
                {
                    ["id"] = new ValString("player2"),
                    ["name"] = new ValString("Player 2")
                };

                var players = new List<Value> { player1, player2 };
                var playerList = new ValList(players);

                return new Intrinsic.Result(playerList);
            };
            _registeredIntrinsics.Add(getOnlinePlayers);

            // Send message to player
            var sendToPlayer = Intrinsic.Create("sendToPlayer");
            sendToPlayer.AddParam("playerId");
            sendToPlayer.AddParam("eventName");
            sendToPlayer.AddParam("data");
            sendToPlayer.code = (context, _) => {
                var playerId = context.GetVar("playerId").ToString();
                var eventName = context.GetVar("eventName").ToString();
                var data = context.GetVar("data");

                Debug.Log($"[MiniScript] Sending event {eventName} to player {playerId} with data {data}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(sendToPlayer);
        }
    }
}
