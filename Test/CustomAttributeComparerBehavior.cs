using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using ObjectDiff;
using ObjectDiff.Differences;

namespace Test
{
    [TestFixture]
    class CustomAttributeComparerBehavior
    {
        [Test]
        public void TestDiff()
        {
            var before = CreateHierarchicalObject();

            var after = CreateHierarchicalObject();

            MakeSomeChanges(after);

            var comparer = new ObjectComparer();
            comparer.AttributeComparers.Add(new SampleComplexTypeAttributeComparer());

            var differences = comparer.Compare("", before, after).ToList();
            foreach (var difference in differences)
                Debug.Print(difference.GetDescription() + "\r\n");

            var expectedDifferences = GetExpectedDifferences(before, after);

            CollectionAssert.AreEquivalent(
                expectedDifferences.Select(x => x.GetDescription()),
                differences.Select(x => x.GetDescription()));
        }

        private void MakeSomeChanges(SampleComplexClass after)
        {
            ChangeValues(after);
            after.Children.Add(CreateObject(3));
            ChangeValues(after.Children[0]);
        }

        private IEnumerable<Difference> GetExpectedDifferences(SampleComplexClass before, SampleComplexClass after)
        {
            var expectedDifferences = new List<Difference>();
            expectedDifferences.Add(new ChangedObjectForSampleComplexClass("", before, after));
            expectedDifferences.Add(new AddedObject("", after, after.Children[1]));
            expectedDifferences.Add(new ChangedObjectForSampleComplexClass("sample complex class 1", before.Children[0],
                                                                         after.Children[0]));
            return expectedDifferences;
        }

        internal class ChangedObjectForSampleComplexClass : ChangedObject
        {
            public ChangedObjectForSampleComplexClass(string pathToContainer, SampleComplexClass before, SampleComplexClass after)
                : base(pathToContainer, before,
                       new[]
                       {
                           new AttributeChange("TextField", before.TextField.Value, after.TextField.Value),
                           new AttributeChange("TextProperty", before.TextProperty.Value, after.TextProperty.Value)
                       })
            {
            }
        }

        private SampleComplexClass CreateHierarchicalObject()
        {
            var obj = CreateObject(1);
            obj.Children.Add(CreateObject(2));
            return obj;
        }

        private void ChangeValues(SampleComplexClass target)
        {
            target.TextField.Value += "changed";
            target.TextProperty.Value += "changed";
        }

        private SampleComplexClass CreateObject(int seed)
        {
            var obj = new SampleComplexClass();
            obj.Id = seed;
            obj.TextField.Value = (seed + 1).ToString(CultureInfo.InvariantCulture);
            obj.TextProperty.Value = (seed + 1).ToString(CultureInfo.InvariantCulture);

            return obj;
        }
    }

    internal class SampleComplexClass : IComparableObject
    {
        public SampleComplexType TextField;
        public SampleComplexType TextProperty { get; set; }

        public List<SampleComplexClass> Children;

        public SampleComplexClass()
        {
            Children = new List<SampleComplexClass>();
            TextField = new SampleComplexType();
            TextProperty = new SampleComplexType();
        }

        public long Id { get; set; }
        public string IdentifyingText { get { return "sample complex class " + Id.ToString(CultureInfo.InvariantCulture); }
        }
    }
}
