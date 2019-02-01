using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Impl;
using Moqikaka.Util.Json;
using Moqikaka.Util.Log;

namespace TestKfka
{
    class Program
    {
        private static Producer<Null, string> producer;

        static void Main(string[] args)
        {
            Start();
            SendMsg(JsonUtil.Serialize("我是一个有魅力的男人"));
            Console.ReadKey();
        }

        /// <summary>
        /// 启动kafka
        /// </summary>
        public static void Start()
        {
            if (producer != null)
            {
                return;
            }

            var brokerList = "10.1.0.40:9092";
            var configDict = new Dictionary<string, String> { { "bootstrap.servers", brokerList } };
            try
            {
                producer = new Producer<Null, string>(configDict);
            }
            catch (Exception e)
            {
                LogUtil.Write($"KafkaMgrBLL.Start连接kafka报错msg={e.Message + Environment.NewLine + e.StackTrace}", LogType.Error);
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg">消息</param>
        public static void SendMsg(String msg)
        {
            if (producer == null)
            {
                return;
            }

            var topicName = "Person_Test";
            var dr = producer.ProduceAsync(topicName, new Message<Null, string> { Value = msg });
            dr.ContinueWith(task =>
            {
                Console.WriteLine("Producer: " + producer.Name + "\r\nTopic: " + topicName + "\r\nPartition: " +
                                  task.Result.Partition + "\r\nOffset: " + task.Result.Offset);
            });

            producer.Flush(TimeSpan.FromSeconds(10));
        }
    }
}
