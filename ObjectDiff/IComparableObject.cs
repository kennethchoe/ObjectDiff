namespace ObjectDiff
{
    public interface IComparableObject
    {
        long Id { get; }
        string IdentifyingText { get; }
    }
}