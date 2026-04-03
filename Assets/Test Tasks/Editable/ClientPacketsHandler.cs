using System.Collections.Generic;
using TestTask.NonEditable;
using UnityEngine;
using Color = UnityEngine.Color;

namespace TestTask.Editable
{
    public static class ClientPacketsHandler
    {
        #region Packet Handlers
        public static void LoginDataReceived(Packet packet)
        {
            int responseCode = packet.ReadInt();
            int clientId = packet.ReadInt();

            ClientManager.Instance.SetClientLogInStatus(responseCode, clientId);

            if (responseCode == (int)LoginResponse.Success)
                SendMonsterRequest();
        }

        public static void MonsterDataReceived(Packet packet)
        {
            int monsterId = packet.ReadInt();
            MonsterNames type = (MonsterNames)packet.ReadInt();
            float maxHp = packet.ReadFloat();
            float currentHp = packet.ReadFloat();

            var monsterData = new MonsterData(monsterId, type, maxHp, currentHp);

            ClientManager.Instance.ClientMobsManager.SpawnMonster(monsterData);
        }

        public static void MonsterHealthUpdateReceived(Packet packet)
        {
            int monsterId = packet.ReadInt();
            float healthRatio = packet.ReadFloat();

            ClientManager.Instance.ClientMobsManager.UpdateMonsterHealth(monsterId, healthRatio);
        }

        public static void ColorsDataReceived(Packet packet)
        {
            int count = packet.ReadInt();
            var colors = new List<Color>();

            for (int i = 0; i < count; i++)
            {
                float r = packet.ReadFloat();
                float g = packet.ReadFloat();
                float b = packet.ReadFloat();
                float a = packet.ReadFloat();
                colors.Add(new Color(r, g, b, a));
            }

            ClientManager.Instance.ClientColorManager.ReceiveColors(colors);
        }
        #endregion

        #region Packet Senders
        public static void SendLoginRequest()
        {
            Packet packet = new Packet(1);
            ClientManager.Instance.PacketSenderClient.SendToServer(packet);
        }

        public static void SendMonsterRequest()
        {
            Packet packet = new Packet(2);
            ClientManager.Instance.PacketSenderClient.SendToServer(packet);
        }

        public static void SendDamageRequest(int monsterId, float damage)
        {
            Packet packet = new Packet(3);
            packet.Write(monsterId);
            packet.Write(damage);
            ClientManager.Instance.PacketSenderClient.SendToServer(packet);
        }

        public static void SendColorRequest()
        {
            Packet packet = new Packet(4);
            ClientManager.Instance.PacketSenderClient.SendToServer(packet);
        }
        #endregion
    }
}
