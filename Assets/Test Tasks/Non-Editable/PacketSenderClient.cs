using System;
using TestTask.Editable;
using UnityEngine;

namespace TestTask.NonEditable
{
    public class PacketSenderClient : IPacketSender
    {
        public static Action<byte[]> OnSendToServer;

        public void SendToServer(Packet packet)
        {
            packet.WriteLength();
            ClientManager.Instance.PacketLatencyMock.EnqueueMessage(packet.ToArray());
        }

        private void SendMessageToServer(byte[] message)
        {
            Debug.Log($"Sending message to server:");
            OnSendToServer?.Invoke(message);
        }

        public void Send(byte[] message)
        {
            SendMessageToServer(message);
        }
    }
}
