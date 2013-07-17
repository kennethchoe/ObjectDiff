namespace ObjectDiff
{
    public class DefaultAttributeComparer : IAttributeComparer
    {
        public bool CanHandle(object valueBefore)
        {
            return true;
        }

        public AttributeChange Compare(string name, object valueBefore, object valueAfter)
        {
            AttributeChange attributeChange = null;

            if ((valueBefore == null && valueAfter != null)
                || (valueBefore != null && !valueBefore.Equals(valueAfter)))
            {
                attributeChange = new AttributeChange(name, valueBefore, valueAfter);
            }

            return attributeChange;
        }
    }
}