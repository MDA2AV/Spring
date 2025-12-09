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
            .SetWorkersSolver(() => 32)
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
        Console.WriteLine("Handling connection..");
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
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}