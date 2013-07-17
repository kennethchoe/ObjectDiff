using ObjectDiff;

namespace Test
{
    class SampleComplexTypeAttributeComparer : IAttributeComparer
    {
        public bool CanHandle(object valueBefore)
        {
            return (valueBefore is SampleComplexType);
        }

        public AttributeChange Compare(string name, object valueBefore, object valueAfter)
        {
            AttributeChange attributeChange = null;

            var fieldBefore = (SampleComplexType) valueBefore;
            var fieldAfter = (SampleComplexType) valueAfter;
            if ((fieldBefore.Value == null && fieldAfter.Value != null)
                || (fieldBefore.Value != null && !fieldBefore.Value.Equals(fieldAfter.Value)))
                attributeChange = new AttributeChange(name, fieldBefore.Value, fieldAfter.Value);

            return attributeChange;
        }
    }
}
