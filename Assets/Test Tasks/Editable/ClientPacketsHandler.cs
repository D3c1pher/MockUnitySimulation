using System;
using System.Collections.Generic;
using TestTask.NonEditable;
using UnityEngine;

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
        }

        public static void MonsterDataReceived(Packet packet)
        {
            int monsterId = packet.ReadInt();
            MonsterNames type = (MonsterNames)packet.ReadInt();
            float maxHp = packet.ReadFloat();
            float currentHp = packet.ReadFloat();

            var monsterData = new MonsterData(monsterId, type, maxHp, currentHp);

            ClientManager.Instance.ClientMobsManager.OnMonsterSpawn(monsterData);
        }

        public static void MonsterHealthUpdateReceived(Packet packet)
        {
            float healthRatio = packet.ReadFloat();

            ClientManager.Instance.ClientMobsManager.OnMonsterHealthUpdate(healthRatio);
        }
        #endregion

        #region Packet Senders
        public static void SendLoginRequest()
        {
            Packet packet = new Packet(1);
            ClientManager.Instance.PacketSenderClient.SendToServer(packet);
        }

        public static void SendDamageRequest(int monsterId, float damage)
        {
            Packet packet = new Packet(2);
            packet.Write(monsterId);
            packet.Write(damage);
            ClientManager.Instance.PacketSenderClient.SendToServer(packet);
        }
        #endregion
    }
}
