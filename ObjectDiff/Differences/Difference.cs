namespace ObjectDiff.Differences
{
    public abstract class Difference
    {
        public IComparableObject Container;
        public string PathToContainer;
        public abstract string GetDescription();

        public Difference(IComparableObject container)
        {
            Container = container;
        }

        protected string FullPath()
        {
            var fullPath = PathToContainer;
            if (fullPath != "")
                fullPath += ", ";

            fullPath += Container.IdentifyingText;

            return fullPath;
        }
    }
}
