namespace Overdrive.Engine;

public sealed unsafe partial class OverdriveEngine
{
    private const int c_bufferRingGID = 1;
    
    private const string c_ip = "0.0.0.0";
    
    private static int s_ringEntries =  8 * 1024;
    private static int s_recvBufferSize  = 32 * 1024;
    private static int s_bufferRingEntries   = 16 * 1024;     // power-of-two
    private static int s_backlog      = 65535;
    private static int s_batchCQES   = 4096;
    private static int s_nWorkers;
    
    private static ushort s_port = 8080;
    
    private static Func<int>? s_calculateNumberWorkers;
    
    // Default request handler (overridden via builder). Writes a minimal plaintext response.
    private static Func<Connection, ValueTask> _sRequestHandler = DefaultRequestHandler;

    private static ValueTask DefaultRequestHandler(Connection connection)
    {
        // TODO: this  v
        /*connection.WriteBuffer.WriteUnmanaged("HTTP/1.1 200 OK\r\n"u8 +
                                              "Server: W\r\n"u8 +
                                              "Content-Type: text/plain\r\n"u8 +
                                              "Content-Length: 28\r\n\r\n"u8 +
                                              "Request handler was not set!"u8 );*/

        return ValueTask.CompletedTask;
    }
    
    public static OverdriveBuilder CreateBuilder() => new OverdriveBuilder();
    
    public sealed class OverdriveBuilder
    {
        private readonly OverdriveEngine _engine;
        
        public OverdriveBuilder() => _engine = new OverdriveEngine();

        public OverdriveEngine Build()
        {
            s_nWorkers = s_calculateNumberWorkers?.Invoke() ?? Environment.ProcessorCount / 2;

            return _engine;
        }

        public OverdriveBuilder SetBacklog(int backlog)
        {
            s_backlog = backlog;
            
            return this;
        }
        
        public OverdriveBuilder SetPort(ushort port)
        {
            s_port = port;
            
            return this;
        }
        
        public OverdriveBuilder SetRingEntries(int ringEntries)
        {
            s_ringEntries = ringEntries;
            
            return this;
        }
        
        public OverdriveBuilder SetBufferRingEntries(int bufferRingEntries)
        {
            s_bufferRingEntries = bufferRingEntries;
            
            return this;
        }
        
        public OverdriveBuilder SetBatchCQES(int batchCQES)
        {
            s_batchCQES = batchCQES;
            
            return this;
        }
        
        public OverdriveBuilder SetRecvBufferSize(int recvBufferSize)
        {
            s_recvBufferSize = recvBufferSize;
            
            return this;
        }
        
        public OverdriveBuilder SetWorkersSolver(Func<int>? calculateNumberWorkers)
        {
            s_calculateNumberWorkers = calculateNumberWorkers;
            
            return this;
        }
        
    }
}