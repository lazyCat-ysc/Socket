using Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Server.Core
{
    class SocketHandler
    {
        private static SocketHandler Instance;

        public SocketHandler()
        {
            ServerEvent.GetInstance.AddEvent(ServerEvent.EventType.EVENT_SEND_PACKAGE, HandlerServerPackage);
        }

        public static SocketHandler GetInstance
        {
            get
            {
                if (Instance == null)
                    Instance = new SocketHandler();
                return Instance;
            }
        }

        public void HandlerServerPackage(ServerPackage serverPackage)
        {
            int i = 0;
        }

        ~SocketHandler()
        {
            ServerEvent.GetInstance.DeleteEvent(ServerEvent.EventType.EVENT_SEND_PACKAGE);
        }
    }
}
