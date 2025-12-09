using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Overdrive.Engine;

[SkipLocalsInit]
public sealed unsafe class Connection
{
    public int Fd;
    public int WorkerIndex;
    public bool Sending;
    
    // In buffer
    public byte* InPtr;
    public int InLength;
        
    // Out buffer
    public nuint OutHead, OutTail;
    public byte* OutPtr;

    public TaskCompletionSource<bool> Tcs =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    
    public Channel<OverdriveEngine.SendRequest> ReadQueue { get; } =
        Channel.CreateUnbounded<OverdriveEngine.SendRequest>(new UnboundedChannelOptions
        {
            SingleReader = true,  // worker thread
            SingleWriter = true,  // connection thread
        });

    public Connection(int fd)
    {
        Fd = fd; 
    }

    public Connection()
    {
    }
    
    public bool IsTcsCompleted() => Tcs.Task.IsCompleted;
    
    public Task ReadAsync() => Tcs.Task;

    public void Clear()
    {
        Sending = false;
        OutPtr = null;
        OutHead = 0;
        OutTail = 0;
    }

    public Connection SetFd(int fd)
    {
        Fd = fd;
            
        return this;
    }
    
    public Connection SetWorkerIndex(int workerIndex)
    {
        WorkerIndex = workerIndex;
            
        return this;
    }
}