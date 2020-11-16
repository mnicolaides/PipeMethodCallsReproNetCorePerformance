using System;
using System.Threading;
using System.Threading.Tasks;
using Common;
using PipeMethodCalls;

namespace PipeMethodCallsReproClient
{
    public class ClientClass
    {
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
        private PipeClientWithCallback<IServerMethods, IClientMethods> ClientPipe = null;
        // This monitors the client pipe in case it's disconnected.
        // It's started in Program.Main()
        public async Task ClientPipeMonitor()
        {
            while (true)
            {
                if (ClientPipe == null ||
                    //ClientPipe.RawPipeStream.IsConnected == false ||
                    ClientPipe?.State != PipeState.Connected)
                {
                    // Now reconnect
                    Console.WriteLine("Connecting...");
                    ClientPipe?.Dispose();
                    ClientPipe = null;
                    ClientPipe = new PipeClientWithCallback<IServerMethods, IClientMethods>("Pipename", () => new PipeClientImpl());
                    await ClientPipe.ConnectAsync();
                    Console.WriteLine("Connected.");
                }
                await Task.Run(() => Thread.Sleep(100));
            }
        }

        // This is used by some code in Process 1
        public async Task SendMessage(string message)
        {
            try
            {
                if (ClientPipe != null &&
                    //ClientPipe.RawPipeStream.IsConnected == true &&
                    ClientPipe.State == PipeState.Connected)
                {
                    await semaphoreSlim.WaitAsync();
                    await ClientPipe.InvokeAsync(cmd => cmd.WriteMessage(message));
                    semaphoreSlim.Release();
                }
                else
                {
                    Console.WriteLine($"Pipe state is:[{ClientPipe?.State}]");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Message: [{message}]] | Exception: [{e.Message}] | Stack: [{e.StackTrace}] | InnerEx: [{e.InnerException}]");
                ClientPipe.Dispose();
                ClientPipe = null;
            }
        }
    }

    public class PipeClientImpl : IClientMethods
    {
    }
}
