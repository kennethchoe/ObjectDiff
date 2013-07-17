namespace ObjectDiff.Differences
{
    public class DeletedObject : Difference
    {
        public IComparableObject Object;

        public DeletedObject(string pathToContainer, IComparableObject container, IComparableObject deletedObject) 
            : base(container)
        {
            PathToContainer = pathToContainer;
            Object = deletedObject;
        }

        public override string GetDescription()
        {
            return string.Format("{0} deleted {1}.", FullPath(), Object.IdentifyingText);
        }
    }
}