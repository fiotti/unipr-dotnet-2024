using System.Buffers;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

public ref struct MyJsonStreamTokenEnumerator
{
    // Per ulteriori informazioni:
    // https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/use-utf8jsonreader

    private readonly Stream? _stream;

    // Buffer, numero di byte nel buffer, e reader.
    private byte[]? _buffer;
    private int _bufferedBytesCount;
    private bool _streamReadToCompletion;
    private Utf8JsonReader _reader;

    public MyJsonStreamTokenEnumerator(Stream stream)
    {
        _stream = stream;
    }

    public bool DebugPrintToConsole { get; init; }

    public Utf8JsonReader Current => _reader;

    public bool MoveNext()
    {
        if (_stream == null)
            throw new InvalidOperationException("Enumerator not initialized.");

        // Alla prima iterazione richiede un buffer.
        if (_buffer == null)
        {
            // Esempio: buffer con dimensione iniziale di (almeno) 8 byte solo a scopo dimostrativo,
            // generalmente i buffer hanno dimensione iniziale di 4KB.
            _buffer = ArrayPool<byte>.Shared.Rent(8);

            // Fa una prima lettura dallo stream.
            _bufferedBytesCount = _stream.Read(_buffer);

            if (DebugPrintToConsole)
            {
                Console.WriteLine($"[DEBUG] Contenuto del buffer: '{Encoding.UTF8.GetString(_buffer).Replace('\r', ' ').Replace('\n', ' ')}'");
            }

            // Costruisce un Utf8JsonReader sui dati letti alla prima lettura.
            _reader = new(_buffer.AsSpan(0, _bufferedBytesCount), isFinalBlock: false, state: default);
        }

        if (!_streamReadToCompletion)
        {
            // Prova a leggere il prossimo token, se non ce la fa espande il buffer finché non riesce a farcelo stare.
            while (!_reader.Read() && !_streamReadToCompletion)
            {
                _streamReadToCompletion = TryReadNextJsonToken() == false;
            }

            if (DebugPrintToConsole)
            {
                string tokenValue = _reader.HasValueSequence ? Encoding.UTF8.GetString(_reader.ValueSequence) : Encoding.UTF8.GetString(_reader.ValueSpan);
                Console.WriteLine($"[DEBUG] Token: '{tokenValue}' ({_reader.TokenType})");
            }
        }

        return !_streamReadToCompletion;
    }

    private bool TryReadNextJsonToken()
    {
        Debug.Assert(_buffer != null);
        Debug.Assert(_stream != null);

        int bufferOffset;
        int bytesRead = 0;
        if (_reader.BytesConsumed < _bufferedBytesCount)
        {
            ReadOnlySpan<byte> leftover = _buffer.AsSpan((int)_reader.BytesConsumed);

            if (leftover.Length == _buffer.Length)
            {
                ArrayPool<byte>.Shared.Return(_buffer);

                _buffer = ArrayPool<byte>.Shared.Rent(_buffer.Length * 2);

                if (DebugPrintToConsole)
                {
                    Console.WriteLine($"[DEBUG] Nuova dimensione del buffer: {_buffer.Length}");
                }
            }

            leftover.CopyTo(_buffer);

            bufferOffset = leftover.Length;

            int fillFrom = bufferOffset;
            while (fillFrom < _buffer.Length)
            {
                int lastBytesRead = _stream.Read(_buffer.AsSpan(fillFrom));
                bytesRead += lastBytesRead;
                fillFrom += lastBytesRead;

                if (lastBytesRead == 0)
                    break;
            }
        }
        else
        {
            bufferOffset = 0;
            bytesRead = _stream.Read(_buffer);
        }

        // Reader e stream esauriti? Return false per segnalarlo al chiamante.
        if (_reader.BytesConsumed == _bufferedBytesCount && bytesRead == 0)
            return false;

        ReadOnlySpan<byte> data = _buffer.AsSpan(0, bufferOffset + bytesRead);
        _bufferedBytesCount = data.Length;

        if (DebugPrintToConsole)
        {
            Console.WriteLine($"[DEBUG] Contenuto del buffer: '{Encoding.UTF8.GetString(data).Replace('\r', ' ').Replace('\n', ' ')}'");
        }

        _reader = new Utf8JsonReader(data, isFinalBlock: bytesRead == 0, _reader.CurrentState);

        return true;
    }

    public MyJsonStreamTokenEnumerator GetEnumerator() => this;

    public void Dispose()
    {
        if (_buffer != null)
        {
            ArrayPool<byte>.Shared.Return(_buffer);
        }

        _buffer = null;
        _bufferedBytesCount = 0;
        _streamReadToCompletion = false;
        _reader = default;
    }
}
