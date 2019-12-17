using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DocStore.Worker.Services
{
    class QueueService : IHostedService
    {
        private readonly ILogger<QueueService> _logger;
        private readonly ConnectionFactory _factory;

        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string _queueName;

        public QueueService(ILogger<QueueService> logger, IConfiguration config)
        {
            _logger = logger;

            _factory = new ConnectionFactory();

            try
            {
                var queueConfig = config.GetSection("RabbitMQ");
                var _factory = new ConnectionFactory()
                {
                    HostName = queueConfig.GetValue<string>("HostName"),
                    UserName = queueConfig.GetValue<string>("Username"),
                    Password = queueConfig.GetValue<string>("Password"),
                    Port = queueConfig.GetValue<int>("Port")
                };
                this.connection = _factory.CreateConnection();
                this.channel = connection.CreateModel();
                _queueName = queueConfig.GetValue<string>("queueName");
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to start Queue Service - Error: {0}", ex.Message);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Register();
            return Task.CompletedTask;
        }

        public void Register()
        {
            _logger.LogDebug("QueueService - Connecting to Queue {0}", _queueName);
            channel.QueueDeclare(_queueName, false, false, false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var result = Process(message);
                if (result)
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };
            channel.BasicConsume(_queueName, false, consumer);
            _logger.LogDebug("QueueService - Connected to Queue {0}", _queueName);
        }

        public bool Process(string msg)
        {
            //TODO: Implement message processing
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.connection.Close();
            return Task.CompletedTask;
        }
    }
}
