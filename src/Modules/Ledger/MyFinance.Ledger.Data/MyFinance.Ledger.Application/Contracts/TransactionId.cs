using MyFinance.CodeAnalysis;

namespace MyFinance.Ledger.Application.Contracts;

[ApiId]
public readonly partial record struct TransactionId(Guid RawId)
{
    public static string Prefix => "txn_";
}
