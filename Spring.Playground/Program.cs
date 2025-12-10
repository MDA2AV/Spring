using System.Runtime.CompilerServices;
using Overdrive.Engine;

// dotnet publish -f net9.0 -c Release /p:PublishAot=true /p:OptimizationPreference=Speed

namespace Overdrive;

[SkipLocalsInit]
internal static class Program
{
    internal static async Task Main()
    {
        var builder = OverdriveEngine
            .CreateBuilder()
            //.SetWorkersSolver(() => Environment.ProcessorCount / 2)
            .SetWorkersSolver(() => 16)
            .SetBacklog(16 * 1024)
            .SetPort(8080)
            .SetRecvBufferSize(32 * 1024);
        
        var engine = builder.Build();
        _ = Task.Run(() =>engine.Run());

        await Task.Delay(1000);
        
        while (true)
        {
            var conn = await engine.AcceptAsync();
            Console.WriteLine($"Connection: {conn.Fd}");

            _ = HandleAsync(conn);
        }
    }
    
    internal static async ValueTask HandleAsync(Connection connection)
    {
        Console.WriteLine("start");
        try
        {
            while (true)
            {
                // Read request
                await connection.ReadAsync();
                //connection.Tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
                connection.Tcs = new(); // reset tcs
                
                // Flush response
                unsafe
                {
                    var okPtr = OverdriveEngine.OK_PTR;
                    var okLen = OverdriveEngine.OK_LEN;
                    
                    if (!OverdriveEngine.TryQueueSend(connection, okPtr, okLen))
                    {
                        Console.WriteLine("Failed to queue send");
                    }
                        
                    /*connection.OutPtr  = okPtr;
                    connection.OutHead = 0;
                    connection.OutTail = okLen;
                    connection.Sending = true;
                    
                    //DIOGO HERE, CAN WE SEND HERE TO AVOID THE CHANNEL? THE CHANNEL IS INCREASING LATENCY TOO MUCH??
                    OverdriveEngine.SubmitSend(
                        OverdriveEngine.s_Workers[connection.WorkerIndex].PRing,
                        connection.Fd,
                        connection.OutPtr,
                        connection.OutHead,
                        connection.OutTail);*/
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        Console.WriteLine("end");
    }
}