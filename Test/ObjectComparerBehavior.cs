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
    class ObjectComparerBehavior
    {
        [Test]
        public void TestDiff()
        {
            var before = CreateHierarchicalObject();

            var after = CreateHierarchicalObject();

            MakeSomeChanges(after);

            var comparer = new ObjectComparer();
            var differences = comparer.Compare("", before, after).ToList();
            foreach (var difference in differences)
                Debug.Print(difference.GetDescription() + "\r\n");

            var expectedDifferences = GetExpectedDifferences(before, after);

            CollectionAssert.AreEquivalent(
                expectedDifferences.Select(x => x.GetDescription()),
                differences.Select(x => x.GetDescription()));
        }

        private void MakeSomeChanges(SampleClass after)
        {
            ChangeValues(after);
            after.Children.RemoveAt(1);
            after.Children.Add(CreateObject(4));
            after.Children[0].Children.RemoveAt(1);
            ChangeValues(after.Children[0].Children[0]);
        }

        private IEnumerable<Difference> GetExpectedDifferences(SampleClass before, SampleClass after)
        {
            var expectedDifferences = new List<Difference>
                                          {
                                              new ChangedObjectForSampleClass("", before, after),
                                              new DeletedObject("", before, before.Children[1]),
                                              new AddedObject("", after, after.Children[1]),
                                              new DeletedObject("1", before.Children[0], before.Children[0].Children[1]),
                                              new ChangedObjectForSampleClass("1, 2", before.Children[0].Children[0],
                                                                              after.Children[0].Children[0])
                                          };
            return expectedDifferences;
        }

        private SampleClass CreateHierarchicalObject()
        {
            var obj = CreateObject(1);
            obj.Children.Add(CreateObject(2));
            obj.Children.Add(CreateObject(3));
            obj.Children[0].Children.Add(CreateObject(21));
            obj.Children[0].Children.Add(CreateObject(22));
            return obj;
        }

        private void ChangeValues(SampleClass target)
        {
            target.FieldLongValue += 1;
            target.PropertyLongValue += 1;
            target.FieldStringValue += "changed";
            target.PropertyStringValue += "changed";
        }

        private SampleClass CreateObject(int seed)
        {
            var obj = new SampleClass();
            obj.Id = seed;
            obj.IdentifyingText = seed.ToString(CultureInfo.InvariantCulture);

            obj.FieldLongValue = seed + 1;
            obj.FieldStringValue = (seed + 2).ToString(CultureInfo.InvariantCulture);
            obj.PropertyLongValue = seed + 3;
            obj.PropertyStringValue = (seed + 4).ToString(CultureInfo.InvariantCulture);

            return obj;
        }
    }

    internal class ChangedObjectForSampleClass : ChangedObject
    {
        public ChangedObjectForSampleClass(string pathToContainer, SampleClass before, SampleClass after)
            : base(pathToContainer, before,
                   new[]
                       {
                           new AttributeChange("FieldLongValue", before.FieldLongValue, after.FieldLongValue),
                           new AttributeChange("FieldStringValue", before.FieldStringValue, after.FieldStringValue),
                           new AttributeChange("PropertyLongValue", before.PropertyLongValue, after.PropertyLongValue),
                           new AttributeChange("PropertyStringValue", before.PropertyStringValue,
                                               after.PropertyStringValue)
                       })
        {
        }
    }

    internal class SampleClass : IComparableObject
    {
        public long Id { get; set; }
        public string IdentifyingText { get; set; }
        public long FieldLongValue;
        public string FieldStringValue;
        public long PropertyLongValue { get; set; }
        public string PropertyStringValue { get; set; }

        public List<SampleClass> Children;

        public SampleClass()
        {
            Children = new List<SampleClass>();
        }
    }
}
