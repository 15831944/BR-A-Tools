﻿using System;
using BRPLUSA.Core.Services;
using BRPLUSA.Revit.Core.Interfaces;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;

namespace BRPLUSA.Revit.Services.Web
{
    public class SocketService : ISocketProvider
    {
        private IO.Options Options { get; set; }
        private Socket Socket { get; set; }
        public string RevitId { get; private set; }
        public string ClientUri { get; private set; }
        public string ServerUri { get; private set; }

        public SocketService(string url, bool productionMode)
        {
            Initialize(url, productionMode);
        }

        private void Initialize(string clientUrl, bool inProduction = true)
        {
            var production = "https://cmd-center-api.herokuapp.com/";
            var debug = "http://localhost:4422/";

            RevitId = Guid.NewGuid().ToString();
            //ServerUri = debug;
            //ClientUri = $"{clientUrl}?revitappid={RevitId}&debug=true";

            ServerUri = inProduction ? production : debug;
            ClientUri = inProduction
                ? $"{clientUrl}?revitappid={RevitId}"
                : $"{clientUrl}?revitappid={RevitId}&debug=true";

            Options = new IO.Options
            {
                IgnoreServerCertificateValidation = true,
                AutoConnect = true,
            };

            Socket = IO.Socket(ServerUri, Options);

            SetSockets();
        }

        private void SetSockets()
        {
            Socket.On(Socket.EVENT_CONNECT, HandleConnection);

            Socket.On(Socket.EVENT_CONNECT_ERROR, (err) =>
            {
                LoggingService.LogError($"There was an error connecting back to the server: {err}", null);
                //HandleConnection();
            });

            Socket.On(Socket.EVENT_DISCONNECT, (info) =>
            {
                LoggingService.LogInfo($"Disconnected from socket server");
                LoggingService.LogInfo($"More information: ${info}");
            });

            Socket.On(Socket.EVENT_RECONNECTING, (info) =>
            {
                LoggingService.LogInfo($"Trying to reconnect to the socket server");
                LoggingService.LogInfo($"More information: ${info}");
            });

            Socket.On(Socket.EVENT_CONNECT_TIMEOUT, () => { LoggingService.LogInfo("Connection timed out"); });

            Socket.On(Socket.EVENT_ERROR, (err) =>
            {
                LoggingService.LogInfo("An error occurred on the Revit client connection side");
                LoggingService.LogInfo($"Failure: {err}");
            });
        }

        private void HandleConnection()
        {
            var id = new
            {
                revit = RevitId,
                socket = Socket.Io().EngineSocket.Id,
            };

            LoggingService.LogInfo($"Connected to socket server for Revit document: {id.revit}");
            LoggingService.LogInfo($"Using socket: {id.socket}");

            Socket.Emit("REVIT_CONNECTION_START", id);
        }

        public void AddSocketEvent(string eventName, Action callback)
        {
            LoggingService.LogInfo($"Attempting to add socket event called: {eventName}");
            Socket.On(eventName, callback);
            LoggingService.LogInfo($"Added socket event called: {eventName}");
        }

        public void Dispose()
        {
            Socket.Disconnect();
        }
    }
}
