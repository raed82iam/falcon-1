using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Foundation.State;

public sealed class StateOwnershipRegistry
{
    private readonly object _sync = new();
    private readonly Dictionary<string, StateOwnershipDeclaration> _active =
        new(StringComparer.Ordinal);

    public DurableStateClassification Register(StateOwnershipDeclaration? declaration)
    {
        if (!IsValid(declaration))
        {
            return DurableStateClassification.Malformed;
        }

        lock (_sync)
        {
            if (_active.TryGetValue(declaration!.Key, out var existing))
            {
                if (existing == declaration)
                {
                    return DurableStateClassification.Accepted;
                }

                return DurableStateClassification.OwnershipConflict;
            }

            _active.Add(declaration.Key, declaration);
            return DurableStateClassification.Accepted;
        }
    }

    public StateOwnershipDeclaration? Resolve(
        string stateNamespace,
        string subjectId,
        FoundationStateClass stateClass)
    {
        var key = StateCanonicalEncoding.Key(stateNamespace, subjectId, stateClass);
        lock (_sync)
        {
            return _active.TryGetValue(key, out var declaration)
                ? declaration
                : null;
        }
    }

    public ReadOnlyCollection<StateOwnershipDeclaration> Snapshot()
    {
        lock (_sync)
        {
            return Array.AsReadOnly(
                _active.Values
                    .OrderBy(value => value.Key, StringComparer.Ordinal)
                    .ToArray());
        }
    }

    private static bool IsValid(StateOwnershipDeclaration? value)
        => value is not null &&
           !string.IsNullOrWhiteSpace(value.DeclarationId) &&
           !string.IsNullOrWhiteSpace(value.Namespace) &&
           !string.IsNullOrWhiteSpace(value.SubjectId) &&
           !string.IsNullOrWhiteSpace(value.AuthoritativeOwner) &&
           !string.IsNullOrWhiteSpace(value.AuthoritativeSource) &&
           !string.IsNullOrWhiteSpace(value.PersistenceOwner) &&
           !string.IsNullOrWhiteSpace(value.ReadAuthorities) &&
           !string.IsNullOrWhiteSpace(value.WriteAuthority) &&
           !string.IsNullOrWhiteSpace(value.RetentionClassification) &&
           value.DeclarationVersion >= 1 &&
           value.EffectiveTime != default &&
           value.Expiry > value.EffectiveTime;
}
