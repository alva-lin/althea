namespace Althea.Infrastructure;

public sealed record VoidObject
{
    private static readonly Lazy<VoidObject> Lazy = new(() => new());
    
    private VoidObject() {}

    public static VoidObject Instance => Lazy.Value;
}
