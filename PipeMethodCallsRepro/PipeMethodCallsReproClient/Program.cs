using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipeMethodCallsReproClient
{
    class Program
    {
        private static string DATETIMEFORMAT = "yyyy-MM-dd HH:mm:ss.fff";
        private static List<Task> tasks = new List<Task>();

        static void Main(string[] args)
        {
            var client = new ClientClass();
            Task clpmTask = Task.Run(() => client.ClientPipeMonitor());

            Program p = new Program();
            tasks.Add(p.RunTask(client, "1"));
            tasks.Add(p.RunTask(client, "2"));
            tasks.Add(p.RunTask(client, "3"));
            tasks.Add(p.RunTask(client, "4"));

            Console.ReadKey();
        }

        public async Task RunTask(ClientClass cc, string taskNumber)
        {
            int count = 0;

            while (count < 1000)
            {
                await cc.SendMessage($"{GetCurrentDateTime()} This is a test message {count.ToString()} from the client {taskNumber}.");
                await Task.Delay(2);
                count++;
            }
        }

        public static string GetCurrentDateTime()
        {
            return DateTime.UtcNow.ToString(DATETIMEFORMAT, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
