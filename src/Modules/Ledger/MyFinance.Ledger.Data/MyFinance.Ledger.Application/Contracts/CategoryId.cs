using MyFinance.CodeAnalysis;

namespace MyFinance.Ledger.Application.Contracts;

[ApiId]
public readonly partial record struct CategoryId(Guid RawId)
{
    public static string Prefix => "cat_";
}
