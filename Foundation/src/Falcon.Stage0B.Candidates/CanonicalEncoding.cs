using System.Buffers.Binary;
using System.Globalization;
using System.Text;

namespace Falcon.Stage0B.Candidates;

public enum FceWireType : byte
{
    Octets = 0x01,
    Text = 0x02,
    UnsignedInteger = 0x03,
    SignedInteger = 0x04,
    Boolean = 0x05,
    Record = 0x06,
    Sequence = 0x07,
    Identifier = 0x08,
    Timestamp = 0x09
}

public sealed record FceField(ushort FieldId, FceWireType WireType, byte[] Value);

public sealed class CanonicalEncodingSupportCandidate : CandidateProviderBase
{
    public CanonicalEncodingSupportCandidate()
        : base("CND-FCE-001")
    {
    }

    public string EncodeTimestamp(DateTimeOffset value) =>
        FalconCanonicalEncoding.Timestamp(value);

    public byte[] EncodeCryptographicDomainContext(
        Guid falconIdentity,
        string environmentIdentity,
        string instanceOrSharingScope,
        string domainId,
        string purposeId,
        string protectionProfileId,
        uint protectionProfileVersion,
        string algorithmId,
        uint keyVersion) =>
        FalconCanonicalEncoding.CryptographicDomainContext(
            falconIdentity,
            environmentIdentity,
            instanceOrSharingScope,
            domainId,
            purposeId,
            protectionProfileId,
            protectionProfileVersion,
            algorithmId,
            keyVersion);
}

public static class FalconCanonicalEncoding
{
    private static readonly UTF8Encoding StrictUtf8 = new(false, true);

    public static string Timestamp(DateTimeOffset value)
    {
        var utc = value.ToUniversalTime();
        var truncatedTicks = utc.Ticks - (utc.Ticks % 10);
        var truncated = new DateTimeOffset(truncatedTicks, TimeSpan.Zero);
        return truncated.ToString("yyyy-MM-dd'T'HH:mm:ss.ffffff'Z'", CultureInfo.InvariantCulture);
    }

    public static bool IsCanonicalTimestamp(string value) =>
        value.Length == 27 &&
        DateTimeOffset.TryParseExact(
            value,
            "yyyy-MM-dd'T'HH:mm:ss.ffffff'Z'",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out _) &&
        !value.Contains(":60.", StringComparison.Ordinal);

    public static string Identifier(Guid value) => value.ToString("D", CultureInfo.InvariantCulture);

    public static bool IsCanonicalIdentifier(string value) =>
        value.Length == 36 &&
        string.Equals(value, value.ToLowerInvariant(), StringComparison.Ordinal) &&
        Guid.TryParseExact(value, "D", out var parsed) &&
        parsed != Guid.Empty &&
        string.Equals(Identifier(parsed), value, StringComparison.Ordinal);

    public static byte[] IdentifierBytes(Guid value)
    {
        Span<byte> runtime = stackalloc byte[16];
        value.TryWriteBytes(runtime);
        return
        [
            runtime[3], runtime[2], runtime[1], runtime[0],
            runtime[5], runtime[4],
            runtime[7], runtime[6],
            runtime[8], runtime[9], runtime[10], runtime[11],
            runtime[12], runtime[13], runtime[14], runtime[15]
        ];
    }

    public static byte[] Text(string value)
    {
        if (!value.IsNormalized(NormalizationForm.FormC))
        {
            throw new CandidateBoundaryException("non_nfc_text");
        }

        if (value.Contains('\0') || value.Contains('\r') || value.Contains('\uFEFF'))
        {
            throw new CandidateBoundaryException("prohibited_text_character");
        }

        return StrictUtf8.GetBytes(value);
    }

    public static byte[] Unsigned(ulong value, int width)
    {
        var bytes = new byte[width];
        switch (width)
        {
            case 1 when value <= byte.MaxValue:
                bytes[0] = (byte)value;
                break;
            case 2 when value <= ushort.MaxValue:
                BinaryPrimitives.WriteUInt16BigEndian(bytes, (ushort)value);
                break;
            case 4 when value <= uint.MaxValue:
                BinaryPrimitives.WriteUInt32BigEndian(bytes, (uint)value);
                break;
            case 8:
                BinaryPrimitives.WriteUInt64BigEndian(bytes, value);
                break;
            default:
                throw new CandidateBoundaryException("invalid_unsigned_width");
        }

        return bytes;
    }

    public static byte[] Record(
        string schemaId,
        uint schemaVersion,
        IEnumerable<FceField> fields)
    {
        var ordered = fields.OrderBy(field => field.FieldId).ToArray();
        if (ordered.Length == 0 ||
            ordered.Select(field => field.FieldId).Distinct().Count() != ordered.Length ||
            ordered.Any(field => field.FieldId == 0))
        {
            throw new CandidateBoundaryException("invalid_field_set");
        }

        var schema = Text(schemaId);
        using var stream = new MemoryStream();
        stream.Write("FCE1"u8);
        WriteUInt16(stream, 1);
        WriteUInt16(stream, checked((ushort)schema.Length));
        stream.Write(schema);
        WriteUInt32(stream, schemaVersion);
        WriteUInt16(stream, checked((ushort)ordered.Length));

        foreach (var field in ordered)
        {
            WriteUInt16(stream, field.FieldId);
            stream.WriteByte((byte)field.WireType);
            WriteUInt32(stream, checked((uint)field.Value.Length));
            stream.Write(field.Value);
        }

        return stream.ToArray();
    }

    public static byte[] CryptographicDomainContext(
        Guid falconIdentity,
        string environmentIdentity,
        string instanceOrSharingScope,
        string domainId,
        string purposeId,
        string protectionProfileId,
        uint protectionProfileVersion,
        string algorithmId,
        uint keyVersion)
    {
        return Record(
            "falcon/crypto/domain-context",
            1,
            [
                new(1, FceWireType.UnsignedInteger, Unsigned(1, 2)),
                new(2, FceWireType.Identifier, IdentifierBytes(falconIdentity)),
                new(3, FceWireType.Text, Text(environmentIdentity)),
                new(4, FceWireType.Text, Text(instanceOrSharingScope)),
                new(5, FceWireType.Text, Text(domainId)),
                new(6, FceWireType.Text, Text(purposeId)),
                new(8, FceWireType.Text, Text(protectionProfileId)),
                new(9, FceWireType.UnsignedInteger, Unsigned(protectionProfileVersion, 4)),
                new(10, FceWireType.Text, Text(algorithmId)),
                new(11, FceWireType.UnsignedInteger, Unsigned(keyVersion, 4))
            ]);
    }

    public static void ValidateRecord(byte[] bytes)
    {
        if (bytes.Length < 14 || !bytes.AsSpan(0, 4).SequenceEqual("FCE1"u8))
        {
            throw new CandidateBoundaryException("invalid_fce_magic");
        }

        var offset = 4;
        var version = ReadUInt16(bytes, ref offset);
        if (version != 1)
        {
            throw new CandidateBoundaryException("unsupported_fce_version");
        }

        var schemaLength = ReadUInt16(bytes, ref offset);
        EnsureAvailable(bytes, offset, schemaLength);
        _ = StrictUtf8.GetString(bytes, offset, schemaLength);
        offset += schemaLength;
        _ = ReadUInt32(bytes, ref offset);
        var fieldCount = ReadUInt16(bytes, ref offset);
        ushort previous = 0;

        for (var index = 0; index < fieldCount; index++)
        {
            var id = ReadUInt16(bytes, ref offset);
            if (id == 0 || id <= previous)
            {
                throw new CandidateBoundaryException("noncanonical_field_order");
            }

            previous = id;
            EnsureAvailable(bytes, offset, 1);
            var wireType = bytes[offset++];
            if (!Enum.IsDefined(typeof(FceWireType), wireType))
            {
                throw new CandidateBoundaryException("unknown_wire_type");
            }

            var length = ReadUInt32(bytes, ref offset);
            EnsureAvailable(bytes, offset, checked((int)length));
            offset += checked((int)length);
        }

        if (offset != bytes.Length)
        {
            throw new CandidateBoundaryException("trailing_fce_data");
        }
    }

    private static void WriteUInt16(Stream stream, ushort value)
    {
        Span<byte> bytes = stackalloc byte[2];
        BinaryPrimitives.WriteUInt16BigEndian(bytes, value);
        stream.Write(bytes);
    }

    private static void WriteUInt32(Stream stream, uint value)
    {
        Span<byte> bytes = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(bytes, value);
        stream.Write(bytes);
    }

    private static ushort ReadUInt16(byte[] bytes, ref int offset)
    {
        EnsureAvailable(bytes, offset, 2);
        var value = BinaryPrimitives.ReadUInt16BigEndian(bytes.AsSpan(offset, 2));
        offset += 2;
        return value;
    }

    private static uint ReadUInt32(byte[] bytes, ref int offset)
    {
        EnsureAvailable(bytes, offset, 4);
        var value = BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(offset, 4));
        offset += 4;
        return value;
    }

    private static void EnsureAvailable(byte[] bytes, int offset, int length)
    {
        if (length < 0 || offset < 0 || offset > bytes.Length - length)
        {
            throw new CandidateBoundaryException("invalid_fce_length");
        }
    }
}
