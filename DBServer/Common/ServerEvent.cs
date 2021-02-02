using System.Collections.Generic;
using Utility;

namespace Server.Common
{
    public class ServerEvent
    {
        public enum EventType
        {
            EVENT_SEND_PACKAGE,
            EVENT_TEXT
        } 

        private static ServerEvent Instance;
        public delegate void EventHandler(ServerPackage serverPackage);
        public Dictionary<EventType, EventHandler> Package;

        public ServerEvent()
        {
            Package = new Dictionary<EventType, EventHandler>();
        }

        public void AddEvent(EventType eventType, EventHandler eventHandler)
        {
            if (Package.ContainsKey(eventType))
            {
                Package[eventType] += eventHandler;
                return;
            }
            Package.Add(eventType, eventHandler);
        }

        public void DeleteEvent(EventType eventType)
        {
            Package.Remove(eventType);
        }

        public static ServerEvent GetInstance
        {
            get
            {
                if (Instance == null)
                    Instance = new ServerEvent();
                return Instance;
            }
        }

        public void SendPackage(EventType eventType, ServerPackage serverPackage)
        {
            if (Package.ContainsKey(eventType))
                Package[eventType](serverPackage);
        }
    }
}
