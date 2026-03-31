using System;
using TestTask.Editable;
using UnityEngine;

namespace TestTask.NonEditable
{
    public class PacketSenderServer : IPacketSender
    {
        public static Action<byte[]> OnSendToClient;

        public void SendToClient(Packet packet)
        {
            if (!ServerMock.Instance.IsClientConnected)
                return;

            packet.WriteLength();
            ServerMock.Instance.PacketLatencyMock.EnqueueMessage(packet.ToArray());
            //SendMessageToClient(packet.ToArray());
        }

        private void SendMessageToClient(byte[] message)
        {
            OnSendToClient?.Invoke(message);
        }

        public void Send(byte[] message)
        {
            SendMessageToClient(message);
        }
    }

    public interface IPacketSender
    { 
        void Send(byte[] message);
    }

}
