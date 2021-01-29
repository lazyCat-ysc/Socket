using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Common
{
    class ServerRabbitMQ
    {
        private IConnection rabbitMq;

        public ServerRabbitMQ()
        {
            Start();
        }

        public IConnection GetConnection(string hostName, string userName, string password)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = hostName;
            connectionFactory.UserName = userName;
            connectionFactory.Password = password;
            return connectionFactory.CreateConnection();
        }

        public void Start()
        {
            rabbitMq = GetConnection("172.16.9.151", "407551879", "64450252");
            Send("ww", "ysc");
            Send("ww", "ttt");
            Receive("ww");
        }

        public void Send(string queue, string data)
        {
            if(rabbitMq != null)
            {
                using (IModel channel = rabbitMq.CreateModel())
                {
                    channel.QueueDeclare(queue, false, false, true, null);
                    channel.BasicPublish(string.Empty, queue, null, Encoding.UTF8.GetBytes(data));
                }
            }
                
        }

        public void Receive(string queue)
        {
            if (rabbitMq != null)
            {
                using (IModel channel = rabbitMq.CreateModel())
                {
                    uint y = channel.MessageCount(queue);
                   
                    channel.QueueDeclare(queue, false, false, true, null);
                    var consumer = new EventingBasicConsumer(channel);
                    
                    BasicGetResult result = channel.BasicGet(queue, true);
                    if (result != null)
                    {
                        string data = Encoding.UTF8.GetString(result.Body.ToArray());
                        Console.WriteLine(data);
                    }
                }
            }
        }
    }
}
