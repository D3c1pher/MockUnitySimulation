using System.Collections.Generic;
using TestTask.NonEditable;

namespace TestTask.Editable
{
    public static class PacketHandlerLookup
    {
        public delegate void PacketHandler(Packet packet);
        public static Dictionary<int, PacketHandler> OnClientPacketHandlers = new Dictionary<int, PacketHandler>()
        {
            {1, ClientPacketsHandler.LoginDataReceived},
        };

        public static Dictionary<int, PacketHandler> OnServerPacketHandlers = new Dictionary<int, PacketHandler>()
        {
            {1, ServerPacketsHandler.LoginRequest},
        };
    }
}
