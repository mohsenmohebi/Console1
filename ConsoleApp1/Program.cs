using Microsoft.Extensions.Configuration;
using System;
using System.Text;
using System.IO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            var storage = configuration.GetConnectionString("Storage");
            Console.WriteLine(storage);
            Console.WriteLine("Hello World!");

            var endPoinQueue = configuration.GetConnectionString("QueueEndpoint");
            if (endPoinQueue == "ask")
            {
                Console.Write("Enter QueueEndPoint:");
                endPoinQueue = Console.ReadLine();
            }
            Console.WriteLine(endPoinQueue);
            var factory = new ConnectionFactory();
            var queue = "IWantATesla.Messages.TeslaOrder, IWantATesla.Messages_my subscription id";


            factory.Uri = new Uri(endPoinQueue);
            using (var conneciton = factory.CreateConnection())
            {
                using (var channel = conneciton.CreateModel())
                {
                    channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {

                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("[X] Received {0}", message);
                    };

                    channel.BasicConsume(queue: queue,
                               autoAck: true,
                               consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();

                }
            }


        }
    }
}
