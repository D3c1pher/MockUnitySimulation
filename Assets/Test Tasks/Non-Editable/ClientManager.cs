using System;
using TestTask.Editable;
using UnityEngine;

namespace TestTask.NonEditable
{
    public class ClientManager : MonoBehaviour
    {
        public static ClientManager Instance { get; private set; }
        [field: SerializeField] public int ClientId { get; private set; }

        [field: SerializeField] public ClientMobsManager ClientMobsManager { get; private set; }
        [field: SerializeField] public ClientColors ClientColorManager { get; private set; }

        [field: SerializeField] public PacketLatencyMock PacketLatencyMock { get; private set; }
        [field: SerializeField] public PacketSenderClient PacketSenderClient { get; private set; }

        public Action<int, int> ClientLogInStatusChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        void Start()
        {
            PacketSenderClient = new PacketSenderClient();
            PacketLatencyMock.Initialize(PacketSenderClient);

            PacketReceiverClient.BeginListening();
        }

        public void LogIn()
        {
            ClientPacketsHandler.SendLoginRequest();
        }

        public void SetClientLogInStatus(int status, int clientId)
        {
            ClientId = clientId;

            ClientLogInStatusChanged?.Invoke(status, clientId);

            if (status == 0)
            {
                Debug.Log("Client logged in successfully with Client ID: " + ClientId);
            }
            else
            {
                Debug.Log("Client login failed.");
            }
        }
    }
}
