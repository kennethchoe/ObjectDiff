namespace ObjectDiff
{
    public interface IAttributeComparer
    {
        bool CanHandle(object valueBefore);
        AttributeChange Compare(string name, object valueBefore, object valueAfter);
    }
}