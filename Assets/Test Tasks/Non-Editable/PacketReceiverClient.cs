using TestTask.Editable;
using UnityEngine;

namespace TestTask.NonEditable
{
    public class PacketReceiverClient : MonoBehaviour
    {
        private static Packet _receivedPacket;

        public static void BeginListening()
        {
            PacketSenderServer.OnSendToClient += HandleClientMessage;
            _receivedPacket = new Packet();
        }

        private static void HandleClientMessage(byte[] message)
        {
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
                    PacketHandlerLookup.OnClientPacketHandlers[pID](packet);
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
