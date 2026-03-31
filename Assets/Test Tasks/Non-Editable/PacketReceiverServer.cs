using TestTask.Editable;
using UnityEngine;

namespace TestTask.NonEditable
{
    public class PacketReceiverServer
    {
        private static Packet _receivedPacket;

        public static void BeginListening()
        {
            Debug.Log("Server has begun listening for connections.");
            PacketSenderClient.OnSendToServer += HandleServerMessage;
            _receivedPacket = new Packet();
        }

        private static void HandleServerMessage(byte[] message)
        {
            Debug.Log($"Server received message.");
            _receivedPacket.Reset(DataHandler(message));
        }

        private static bool DataHandler(byte[] data)
        {
            int packetLength = 0;
            _receivedPacket.SetBytes(data);
            if (_receivedPacket.UnreadLength() >= 4)
            {
                packetLength = _receivedPacket.ReadInt();
                if (packetLength <= 0)
                {
                    return true;
                }
            }
            while (packetLength > 0 && packetLength <= _receivedPacket.UnreadLength())
            {
                byte[] packetBytes = _receivedPacket.ReadBytes(packetLength);
                using (Packet packet = new Packet(packetBytes))
                {
                    int pID = packet.ReadInt();
                    PacketHandlerLookup.OnServerPacketHandlers[pID](packet);
                }

                packetLength = 0;
                if (_receivedPacket.UnreadLength() >= 4)
                {
                    packetLength = _receivedPacket.ReadInt();
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }
            }
            if (packetLength <= 1)
            {
                return true;
            }

            return false;
        }
    }
}
