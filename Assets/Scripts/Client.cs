using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using TMPro;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

namespace Ekkam
{
    public class Client : MonoBehaviour
    {
        public static Client instance;

        private Socket socket;

        public PlayerData playerData;
        public Player player;

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

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        
        void OnDestroy()
        {
            socket.Close();
        }

        void Update()
        {
            ReceiveDataFromServer();
        }
        
        public void ConnectToServer(string playerName, string ipAddress = "127.0.0.1")
        {
            playerData = new PlayerData(Guid.NewGuid().ToString(), playerName);
            socket.Connect(new IPEndPoint(IPAddress.Parse(ipAddress), 3000));
            socket.Blocking = false;
            Debug.Log("Connected to server.");
        }

        private void SendDataToServer(BasePacket packet)
        {
            byte[] data = packet.Serialize();
            socket.Send(data);
        }

        private void ReceiveDataFromServer()
        {
            if (socket.Available > 0)
            {
                byte[] buffer = new byte[socket.Available];
                socket.Receive(buffer);

                BasePacket packet = new BasePacket().BaseDeserialize(buffer);
                switch (packet.type)
                {
                    case BasePacket.Type.Selector:
                        SelectorPacket selectorPacket = new SelectorPacket().Deserialize(buffer);
                        Debug.Log($"Received selector: {selectorPacket.selectedCell} from {selectorPacket.playerData.name}");
                        if (playerData.id != selectorPacket.playerData.id)
                        {
                            Debug.Log("Mirroring selector");
                            player.opponent.DropPiece(selectorPacket.selectedCell, false);
                            player.AllowTapping(true);
                        }
                        break;
                }
            }
        }
        
        public void SendSelector(Vector2Int selectedCell)
        {
            SelectorPacket selectorPacket = new SelectorPacket(BasePacket.Type.Selector, playerData, selectedCell);
            SendDataToServer(selectorPacket);
        }
    }
}