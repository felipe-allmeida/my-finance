using MyFinance.CodeAnalysis;

namespace MyFinance.Pluggy.Application.Contracts.V1;

[ApiId]
public readonly partial record struct SyncHistoryId(Guid RawId)
{
    public static string Prefix => "sync_";
}
