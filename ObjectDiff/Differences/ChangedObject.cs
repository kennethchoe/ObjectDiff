using System.Collections.Generic;
using System.Text;

namespace ObjectDiff.Differences
{
    public class ChangedObject : Difference
    {
        public IEnumerable<AttributeChange> Changes;

        public ChangedObject(IComparableObject container)
            : base(container)
        {
        }

        public ChangedObject(string pathToContainer, IComparableObject container, IEnumerable<AttributeChange> attributeChanges) : this(container)
        {
            PathToContainer = pathToContainer;
            Changes = attributeChanges;
        }

        public override string GetDescription()
        {
            var changeDetails = new StringBuilder();

            foreach (var change in Changes)
            {
                var beforeString = (change.Before ?? "<null>").ToString();
                if (beforeString == "") beforeString = "<empty>";

                var afterString = (change.After ?? "<null>").ToString();
                if (afterString == "") afterString = "<empty>";

                changeDetails.Append(string.Format("\r\n{0}: {1} => {2}", change.Name, beforeString, afterString));
            }

            return string.Format("{0} changed.\r\n{1}", FullPath(), changeDetails.ToString().Substring(2));
        }
    }
}