using System.Reflection;

namespace MyFinance.Ledger.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
