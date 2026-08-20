using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Foundation.Contracts;

namespace Foundation.Guardian;

public sealed record GuardianRestrictionPersistenceSnapshot(
    string FormatVersion,
    DateTimeOffset CapturedAt,
    GuardianProtectiveDecision Decision,
    GuardianProtectiveRestriction Restriction,
    string SnapshotIdentity);

public sealed record GuardianRestartReconstructionResult(
    bool Success,
    string Reason,
    bool ContainmentFenceRequired,
    bool TrustedOperationPermitted,
    GuardianProtectiveDecision? Decision,
    GuardianProtectiveRestriction? Restriction,
    RestrictionRecord? ContractRecord)
{
    public static GuardianRestartReconstructionResult FailClosed(string reason)
        => new(false, reason, true, false, null, null, null);
}

public static class GuardianRestrictionPersistence
{
    public const string FormatVersion = "FALCON_STAGE8_WP06_RESTRICTION_SNAPSHOT_V1";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static GuardianRestrictionPersistenceSnapshot CreateSnapshot(
        GuardianProtectiveDecision decision,
        GuardianProtectiveRestriction restriction,
        DateTimeOffset capturedAt)
    {
        if (capturedAt == default)
            throw new ArgumentException("Snapshot capture time is required.", nameof(capturedAt));

        var validation = GuardianProtectiveRestrictionRuntime.Validate(restriction, decision);
        if (!validation.Success)
            throw new ArgumentException("Invalid restriction snapshot input: " + validation.Reason, nameof(restriction));

        var snapshotIdentity = ComputeSnapshotIdentity(decision, restriction, capturedAt);
        return new GuardianRestrictionPersistenceSnapshot(
            FormatVersion,
            capturedAt,
            decision,
            restriction,
            snapshotIdentity);
    }

    public static byte[] Serialize(GuardianRestrictionPersistenceSnapshot snapshot)
    {
        ValidateSnapshotOrThrow(snapshot);
        return JsonSerializer.SerializeToUtf8Bytes(snapshot, JsonOptions);
    }

    public static GuardianRestrictionPersistenceSnapshot Deserialize(byte[] payload)
    {
        if (payload is null || payload.Length == 0)
            throw new InvalidDataException("MISSING_PERSISTED_RESTRICTION_SNAPSHOT");

        GuardianRestrictionPersistenceSnapshot? snapshot;
        try
        {
            snapshot = JsonSerializer.Deserialize<GuardianRestrictionPersistenceSnapshot>(payload, JsonOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException("INVALID_PERSISTED_RESTRICTION_SNAPSHOT", ex);
        }

        ValidateSnapshotOrThrow(snapshot);
        return snapshot!;
    }

    public static void SaveAtomic(string path, GuardianRestrictionPersistenceSnapshot snapshot)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Persistence path is required.", nameof(path));

        var fullPath = Path.GetFullPath(path);
        var directory = Path.GetDirectoryName(fullPath);
        if (string.IsNullOrWhiteSpace(directory))
            throw new ArgumentException("Persistence path must have a directory.", nameof(path));

        Directory.CreateDirectory(directory);

        var payload = Serialize(snapshot);
        var temporaryPath = fullPath + ".tmp";

        try
        {
            using (var stream = new FileStream(
                temporaryPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                4096,
                FileOptions.WriteThrough))
            {
                stream.Write(payload, 0, payload.Length);
                stream.Flush(true);
            }

            File.Move(temporaryPath, fullPath, true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);
        }
    }

    public static GuardianRestartReconstructionResult ReconstructAfterRestart(
        string path,
        DateTimeOffset restartTime)
    {
        if (restartTime == default)
            return GuardianRestartReconstructionResult.FailClosed("INVALID_RESTART_TIME");
        if (string.IsNullOrWhiteSpace(path))
            return GuardianRestartReconstructionResult.FailClosed("MISSING_PERSISTENCE_PATH");

        string fullPath;
        try
        {
            fullPath = Path.GetFullPath(path);
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return GuardianRestartReconstructionResult.FailClosed("INVALID_PERSISTENCE_PATH:" + ex.Message);
        }

        if (!File.Exists(fullPath))
            return GuardianRestartReconstructionResult.FailClosed("PERSISTED_RESTRICTION_SNAPSHOT_UNAVAILABLE");

        GuardianRestrictionPersistenceSnapshot snapshot;
        try
        {
            snapshot = Deserialize(File.ReadAllBytes(fullPath));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or ArgumentException or NotSupportedException)
        {
            return GuardianRestartReconstructionResult.FailClosed("PERSISTED_RESTRICTION_SNAPSHOT_UNTRUSTED:" + ex.Message);
        }

        var evaluation = GuardianProtectiveRestrictionRuntime.EvaluateAt(
            snapshot.Restriction,
            snapshot.Decision,
            restartTime);

        if (!evaluation.Success || !evaluation.RemainsEnforced)
            return GuardianRestartReconstructionResult.FailClosed("RESTART_RESTRICTION_RECONSTRUCTION_FAILED:" + evaluation.Reason);

        RestrictionRecord contractRecord;
        try
        {
            contractRecord = GuardianRestrictionContractPublisher.Publish(snapshot.Restriction, snapshot.Decision);
        }
        catch (ArgumentException ex)
        {
            return GuardianRestartReconstructionResult.FailClosed("RESTART_CONTRACT_PUBLICATION_FAILED:" + ex.Message);
        }

        return new GuardianRestartReconstructionResult(
            true,
            evaluation.Status == GuardianRestrictionStatus.ReviewRequired
                ? "RECONSTRUCTED_REVIEW_REQUIRED_RESTRICTION_REMAINS_ENFORCED"
                : "RECONSTRUCTED_ACTIVE_RESTRICTION_REMAINS_ENFORCED",
            true,
            false,
            snapshot.Decision,
            snapshot.Restriction,
            contractRecord);
    }

    private static void ValidateSnapshotOrThrow(GuardianRestrictionPersistenceSnapshot? snapshot)
    {
        if (snapshot is null)
            throw new InvalidDataException("MISSING_PERSISTED_RESTRICTION_SNAPSHOT");
        if (!string.Equals(snapshot.FormatVersion, FormatVersion, StringComparison.Ordinal))
            throw new InvalidDataException("UNSUPPORTED_RESTRICTION_SNAPSHOT_FORMAT");
        if (snapshot.CapturedAt == default)
            throw new InvalidDataException("INVALID_RESTRICTION_SNAPSHOT_TIME");
        if (snapshot.Decision is null)
            throw new InvalidDataException("MISSING_RESTRICTION_SNAPSHOT_DECISION");
        if (snapshot.Restriction is null)
            throw new InvalidDataException("MISSING_RESTRICTION_SNAPSHOT_RESTRICTION");

        var decisionValidation = GuardianProtectiveDecisionValidator.Validate(snapshot.Decision);
        if (!decisionValidation.Success)
            throw new InvalidDataException("INVALID_RESTRICTION_SNAPSHOT_DECISION:" + decisionValidation.Reason);

        var restrictionValidation = GuardianProtectiveRestrictionRuntime.Validate(snapshot.Restriction, snapshot.Decision);
        if (!restrictionValidation.Success)
            throw new InvalidDataException("INVALID_RESTRICTION_SNAPSHOT_RESTRICTION:" + restrictionValidation.Reason);

        var expectedIdentity = ComputeSnapshotIdentity(snapshot.Decision, snapshot.Restriction, snapshot.CapturedAt);
        if (!string.Equals(snapshot.SnapshotIdentity, expectedIdentity, StringComparison.Ordinal))
            throw new InvalidDataException("RESTRICTION_SNAPSHOT_IDENTITY_MISMATCH");
    }

    private static string ComputeSnapshotIdentity(
        GuardianProtectiveDecision decision,
        GuardianProtectiveRestriction restriction,
        DateTimeOffset capturedAt)
    {
        var canonical = string.Join("\n", new[]
        {
            FormatVersion,
            capturedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            decision.Identity,
            restriction.Identity,
            restriction.PersistAcrossRestart ? "1" : "0",
            restriction.SubjectSelfReleaseForbidden ? "1" : "0"
        });

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }
}
