using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Ekkam
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        
        public TMP_InputField nameInput;
        public TMP_InputField ipInput;
        public Button connectButton;
        public Button createButton;
        
        public GameObject mainMenuVCam;
        public GameObject mainMenuUI;
        public GameObject lobbyUI;
        public GameObject waitingUI;
        public GameObject gameUI;

        private void Start()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
            }
            
            connectButton.onClick.AddListener(Connect);
            createButton.onClick.AddListener(StartAndConnect);
            
            mainMenuVCam.SetActive(true);
            mainMenuUI.SetActive(true);
            lobbyUI.SetActive(false);
            gameUI.SetActive(false);
        }
        
        void Connect()
        {
            Client.instance.playerData = new PlayerData(Guid.NewGuid().ToString(), nameInput.text);
            Client.instance.ConnectToServer(nameInput.text, ipInput.text);
            Client.instance.player.Initialize(2);
            
            mainMenuVCam.SetActive(false);
            lobbyUI.SetActive(false);
            gameUI.SetActive(true);
        }
        
        void StartAndConnect()
        {
            Server.instance.StartServer(true, nameInput.text);
            Client.instance.player.Initialize(1);
            
            mainMenuVCam.SetActive(false);
            lobbyUI.SetActive(false);
            waitingUI.SetActive(true);
        }
        
        public void ShowLobby()
        {
            mainMenuUI.SetActive(false);
            lobbyUI.SetActive(true);
            gameUI.SetActive(false);
        }
    }
}