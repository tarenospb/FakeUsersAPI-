using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using FakeUsersAPI.Mappers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using FakeUsersAPI.Models;

namespace FakeUsersAPI.Repositories
{
    public class RabbitClient
    {

        private AppSettingsConnection _conf;
        private IConnection _conn;
        private IModel _channel;
        private UserDataMapper _mapper;
        private CallDapperDb _db;
        public RabbitClient(CallDapperDb db, UserDataMapper mapper, IOptions<AppSettingsConnection> conf) 
        {
            _conf = conf.Value;
            _mapper = mapper;
            _db = db;
        }

        public void Init()
        {
            Subscribe();           
        }

        public void Destruct()
        {
            _channel?.Dispose();
            _conn?.Close();
            _conn?.Dispose();
        }

        public void Subscribe()
        {           
            //получение из rabbit         
            var factory = new ConnectionFactory() { 
                HostName = _conf.RabbitHost, 
                UserName = _conf.RabbitLogin, 
                Password = _conf.RabbitPassword,
                Ssl =
                {
                    ServerName = _conf.RabbitHost,
                    Enabled = false
                }
            };
            _conn = factory.CreateConnection();
            _channel = _conn.CreateModel();
            
            _channel.ExchangeDeclare(
                exchange: "fakeuserex",
                type: "fanout",
                autoDelete: false,
                arguments: null);

            _channel.QueueDeclare(
                queue: "users",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            _channel.QueueBind("users", "fakeuserex", "user");
            
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var str = Encoding.UTF8.GetString(body);
                _mapper.ParseBodyToUserModel(str);
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                
            };

            _channel.BasicConsume(
                queue: "users",
                autoAck: false,
                consumer: consumer);
        }

        public void SendToRabbit(UserModelDB user)
        {
            var factory = new ConnectionFactory() { 
                HostName = _conf.RabbitHost, 
                UserName = _conf.RabbitLogin, 
                Password = _conf.RabbitPassword,
                Ssl =
                {
                    ServerName = _conf.RabbitHost,
                    Enabled = false
                }
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueBind("users", "fakeuserex", "user");
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user));
                channel.BasicPublish(exchange: "fakeuserex",
                                 routingKey: "user",
                                 basicProperties: null,
                                 body: body);

            }
        }




    }
}
