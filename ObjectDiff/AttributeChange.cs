namespace ObjectDiff
{
    public class AttributeChange
    {
        public string Name;
        public object Before;
        public object After;

        public AttributeChange(string name, object before, object after)
        {
            Before = before;
            After = after;
            Name = name;
        }
    }
}