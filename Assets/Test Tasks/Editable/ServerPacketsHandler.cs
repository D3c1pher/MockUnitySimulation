using System.Collections.Generic;
using System.Linq;
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

        public static void MonsterRequest(Packet packet)
        {
            ServerMock.Instance.ServerMobsManager.SendMonsterToClient();
        }

        public static void DamageRequest(Packet packet)
        {
            int monsterId = packet.ReadInt();
            float damage = packet.ReadFloat();

            var monsterData = ServerMock.Instance.ServerMobsManager.MonsterData;

            if (monsterData == null || monsterData.MonsterId != monsterId)
                return;

            Debug.Log($"Monster {monsterData.MonsterName} (ID={monsterId}) took {damage} damage. HP: {monsterData.MonsterCurrentHealth}/{monsterData.MonsterMaxHealth} -> {monsterData.MonsterCurrentHealth - damage}/{monsterData.MonsterMaxHealth}");

            monsterData.TakeDamage(damage);

            if (monsterData.MonsterCurrentHealth <= 0)
                Debug.Log($"Monster {monsterData.MonsterName} (ID={monsterId}) has died.");
        }

        public static void ColorRequest(Packet packet)
        {
            var colors = ServerMock.Instance.ServerColors.GetServerColors().ToList();

            Debug.Log($"Server sending {colors.Count} colors to client.");

            SendColors(colors);
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

        public static void SendMonsterHealthUpdate(int monsterId, float healthRatio)
        {
            using (Packet packet = new Packet(3))
            {
                packet.Write(monsterId);
                packet.Write(healthRatio);
                ServerMock.Instance.PacketSenderServer.SendToClient(packet);
            }
        }

        public static void SendColors(List<Color> colors)
        {
            using (Packet packet = new Packet(4))
            {
                packet.Write(colors.Count);

                foreach (var color in colors)
                {
                    packet.Write(color.r);
                    packet.Write(color.g);
                    packet.Write(color.b);
                    packet.Write(color.a);
                }

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
