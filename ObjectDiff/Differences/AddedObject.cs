namespace ObjectDiff.Differences
{
    public class AddedObject: Difference
    {
        public IComparableObject Object;

        public AddedObject(string pathToContainer, IComparableObject container, IComparableObject addedObject)
            : base(container)
        {
            PathToContainer = pathToContainer;
            Object = addedObject;
        }

        public override string GetDescription()
        {
            return string.Format("{0} added {1}.", FullPath(), Object.IdentifyingText);
        }
    }
}