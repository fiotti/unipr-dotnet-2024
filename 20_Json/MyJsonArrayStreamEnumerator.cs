using System.Buffers;
using System.Text.Json;

public ref struct MyJsonArrayStreamEnumerator
{
    private MyJsonStreamTokenEnumerator _jsonTokens;
    private bool _isInsideArray;

    public MyJsonArrayStreamEnumerator(Stream stream)
    {
        MyJsonStreamTokenEnumerator tokens = new(stream) { DebugPrintToConsole = false };

        _jsonTokens = tokens.GetEnumerator();
    }

    public JsonElement Current { get; private set; }

    public bool MoveNext()
    {
        if (!_isInsideArray)
        {
            if (!_jsonTokens.MoveNext())
                throw new FormatException("Empty stream.");

            if (_jsonTokens.Current.TokenType != JsonTokenType.StartArray)
                throw new FormatException("Not a JSON array.");

            _isInsideArray = true;
        }

        Span<byte> buffer = stackalloc byte[4096];
        int filledBytes = 0;

        bool first = true;
        bool nextItemMayNeedSeparator = false;
        JsonTokenType lastTokenType = JsonTokenType.None;
        while (_jsonTokens.MoveNext())
        {
            int tokenLength = _jsonTokens.Current.HasValueSequence ? (int)_jsonTokens.Current.ValueSequence.Length : _jsonTokens.Current.ValueSpan.Length;
            int minimumBufferLength = filledBytes + tokenLength;

            // HACK: https://github.com/dotnet/runtime/blob/1e8379d98e309724248c0151d09df2530a848e84/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/JsonSerializer.Read.Utf8JsonReader.cs#L408
            bool needsQuotes = _jsonTokens.Current.TokenType is JsonTokenType.String or JsonTokenType.PropertyName;
            if (needsQuotes)
                minimumBufferLength += 2;

            bool needsColon = _jsonTokens.Current.TokenType is JsonTokenType.PropertyName;
            if (needsColon)
                minimumBufferLength++;

            bool needsSeparator = nextItemMayNeedSeparator && _jsonTokens.Current.TokenType is not (JsonTokenType.EndArray or JsonTokenType.EndObject);
            if (needsSeparator)
                minimumBufferLength++;

            if (buffer.Length < minimumBufferLength)
            {
                byte[] newBuffer = new byte[buffer.Length * 2];
                buffer.CopyTo(newBuffer);
                buffer = newBuffer;
            }

            if (needsSeparator)
                buffer[filledBytes++] = (byte)',';

            if (needsQuotes)
                buffer[filledBytes++] = (byte)'"';

            if (_jsonTokens.Current.HasValueSequence)
            {
                _jsonTokens.Current.ValueSequence.CopyTo(buffer.Slice(filledBytes));
            }
            else
            {
                _jsonTokens.Current.ValueSpan.CopyTo(buffer.Slice(filledBytes));
            }

            filledBytes += tokenLength;

            if (needsQuotes)
                buffer[filledBytes++] = (byte)'"';

            if (needsColon)
                buffer[filledBytes++] = (byte)':';

            if (!first && _jsonTokens.Current.CurrentDepth == 1)
            {
                Span<byte> item = buffer.Slice(0, filledBytes);

                Utf8JsonReader reader = new(item);
                Current = JsonElement.ParseValue(ref reader);
                return true;
            }

            first = false;
            nextItemMayNeedSeparator = _jsonTokens.Current.TokenType is not (JsonTokenType.StartArray or JsonTokenType.StartObject or JsonTokenType.PropertyName);
            lastTokenType = _jsonTokens.Current.TokenType;
        }

        if (lastTokenType != JsonTokenType.EndArray)
            throw new FormatException("Unexpected ending.");

        return false;
    }

    public MyJsonArrayStreamEnumerator GetEnumerator() => this;

    public void Dispose()
    {
        _jsonTokens.Dispose();
    }
}
