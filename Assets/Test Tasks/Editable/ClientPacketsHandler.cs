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
        #endregion

        #region Packet Senders
        public static void SendLoginRequest()
        {
            Packet packet = new Packet(1);
            ClientManager.Instance.PacketSenderClient.SendToServer(packet);
        }
        #endregion
    }
}
