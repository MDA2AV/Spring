using System.Runtime.CompilerServices;

namespace Overdrive.HttpProtocol;

internal static unsafe class HeaderParser
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int FindCrlfCrlf(byte* buf, int head, int tail)
    {
        // Construct a Span<byte> view over the raw memory.
        // The caller must guarantee that (tail - head) bytes are valid and readable.
        var span = new ReadOnlySpan<byte>(buf + head, tail - head);

        int idx = span.IndexOf(CrlfCrlf);
        return idx >= 0 ? head + idx : -1;
    }

    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ReadOnlySpan<byte> FindCrlfCrlf(byte* buf, int head, int tail, ref int idx)
    {
        // Construct a Span<byte> view over the raw memory.
        // The caller must guarantee that (tail - head) bytes are valid and readable.
        var span = new ReadOnlySpan<byte>(buf + head, tail - head);

        idx = span.IndexOf(CrlfCrlf);
        if (idx >= 0)
            idx += head;
        else
            idx = -1;

        return span;
    }

    // ===== Common tokens (kept as ReadOnlySpan<byte> for zero-allocation literals) =====

    private static ReadOnlySpan<byte> Crlf => "\r\n"u8;
    private static ReadOnlySpan<byte> CrlfCrlf => "\r\n\r\n"u8;

    // ASCII byte codes (documented for clarity)
    private const byte Space = 0x20;        // ' '
    private const byte Question = 0x3F;     // '?'
    private const byte QuerySeparator = 0x26; // '&'
    private const byte Equal = 0x3D;        // '='
    private const byte Colon = 0x3A;        // ':'
    private const byte SemiColon = 0x3B;    // ';'
}