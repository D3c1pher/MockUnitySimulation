using TestTask.Editable;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TestTask.NonEditable
{
    public class ServerMock : MonoBehaviour
    {
        public static ServerMock Instance { get; private set; }
        [field: SerializeField] public bool IsClientConnected { get; private set; }

        [field: SerializeField] public ServerMobsManager ServerMobsManager { get; private set; }
        [field: SerializeField] public ServerColors ServerColors { get; private set; }
        [field: SerializeField] public PacketLatencyMock PacketLatencyMock { get; private set; }
        [field: SerializeField] public PacketSenderServer PacketSenderServer { get; private set; }


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            PacketSenderServer = new PacketSenderServer();
            PacketLatencyMock.Initialize(PacketSenderServer);
            PacketReceiverServer.BeginListening();

            ServerColors = new ServerColors();
            ServerMobsManager = new ServerMobsManager();
        }

        public LoginResponse TryConnectClient(out int clientId)
        {
            if (!IsClientConnected)
            {
                IsClientConnected = true;
                clientId = Random.Range(1, 1000);
                return LoginResponse.Success;
            }
            clientId = -1;
            return LoginResponse.Failure;
        }
    }
}
