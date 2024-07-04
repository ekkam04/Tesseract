using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using TMPro;

namespace Ekkam
{

    public class Server : MonoBehaviour
    {
        private Socket socket;
        private List<Socket> clients = new List<Socket>();
        private bool acceptingNewClients = true;

        public delegate void ConnectedToServer();

        public event ConnectedToServer connectedToServer;

        public static Server instance;

        void Start()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void StartServer(bool autoConnectCurrentClient = false, string playerName = "Player")
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 3000));
            socket.Listen(10);
            socket.Blocking = false;
            
            if (autoConnectCurrentClient)
            {
                Client.instance.ConnectToServer(playerName);
                Debug.Log("Server started, added current client. Waiting for connections...");
            }
            else
            {
                Debug.Log("Server started, waiting for connections...");
            }
        }

        private void OnDestroy()
        {
            socket.Close();
        }

        void Update()
        {
            if (acceptingNewClients) AcceptNewClients();
            ReceiveDataFromClients();
        }

        private void AcceptNewClients()
        {
            try
            {
                Socket newClient = socket.Accept();
                clients.Add(newClient);
                Debug.Log("New client connected.");
                connectedToServer?.Invoke();
                
                if (clients.Count == 2)
                {
                    acceptingNewClients = false;
                    Debug.Log("All clients connected.");
                    
                    GameManager.instance.waitingUI.SetActive(false);
                    GameManager.instance.gameUI.SetActive(true);
                    Client.instance.player.AllowTapping(true);
                }
            }
            catch
            {
                // No pending connections
            }
        }

        private void ReceiveDataFromClients()
        {
            foreach (Socket client in clients)
            {
                if (client.Available > 0)
                {
                    byte[] buffer = new byte[client.Available];
                    client.Receive(buffer);
                    
                    BasePacket packet = new BasePacket().BaseDeserialize(buffer);
                    switch (packet.type)
                    {
                        case BasePacket.Type.Selector:
                            SelectorPacket selectorPacket = new SelectorPacket().Deserialize(buffer);
                            Debug.Log($"Received selector from {selectorPacket.playerData.name}: {selectorPacket.selectedCell}");
                            BroadcastData(buffer, client);
                            break;
                        // case BasePacket.Type.Position:
                        //     PositionPacket positionPacket = new PositionPacket().Deserialize(buffer);
                        //     Debug.Log($"Received position from {positionPacket.playerData.name}: {positionPacket.position}");
                        //     BroadcastData(buffer, client);
                        //     break;
                        // case BasePacket.Type.Rotation:
                        //     RotationYPacket rotationYPacket = new RotationYPacket().Deserialize(buffer);
                        //     Debug.Log($"Received rotation y from {rotationYPacket.playerData.name}: {rotationYPacket.rotationY}");
                        //     BroadcastData(buffer, client);
                        //     break;
                        // case BasePacket.Type.AnimationState:
                        //     AnimationStatePacket animationStatePacket = new AnimationStatePacket().Deserialize(buffer);
                        //     Debug.Log($"Received animation state from {animationStatePacket.playerData.name}: {animationStatePacket.commandType} {animationStatePacket.parameterName} {animationStatePacket.boolValue} {animationStatePacket.floatValue}");
                        //     BroadcastData(buffer, client);
                        //     break;
                    }
                }
            }
        }

        private void BroadcastData(byte[] data, Socket exceptClient)
        {
            foreach (Socket client in clients)
            {
                if (client != exceptClient)
                {
                    client.Send(data);
                }
            }
        }
    }
}
