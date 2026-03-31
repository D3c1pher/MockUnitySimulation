using TestTask.NonEditable;
using UnityEngine;

namespace TestTask.Editable
{
    public static class ServerPacketsHandler
    {
        #region Packet Handlers
        public static void LoginRequest(Packet packet)
        {
            var clientLogInResponse = ServerMock.Instance.TryConnectClient(out var clientId);
            SendLoginResponse(clientLogInResponse, clientId);
        }

        #endregion

        #region Packet Senders
        public static void SendLoginResponse(LoginResponse response, int clientId)
        {
            using (Packet packet = new Packet(1))
            {
                packet.Write((int)response);
                packet.Write(clientId);

                ServerMock.Instance.PacketSenderServer.SendToClient(packet);

                if (response == LoginResponse.Success)
                    ServerMock.Instance.ServerMobsManager.SendMonsterToClient();
            }
        }

        public static void SendMonsterSpawn(MonsterData monster)
        {
            using (Packet packet = new Packet(2))
            {
                packet.Write(monster.MonsterId);
                packet.Write((int)monster.MonsterType);
                packet.Write(monster.MonsterMaxHealth);
                packet.Write(monster.MonsterCurrentHealth);

                ServerMock.Instance.PacketSenderServer.SendToClient(packet);
            }
        }
        #endregion
    }
}

public enum LoginResponse
{
    Success = 0,
    Failure = 1,
}
