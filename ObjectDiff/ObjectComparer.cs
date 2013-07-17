using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ObjectDiff.Differences;

namespace ObjectDiff
{
    public class ObjectComparer
    {
        public readonly List<IAttributeComparer> AttributeComparers;

        public ObjectComparer()
        {
            AttributeComparers = new List<IAttributeComparer>();
        }

        public IEnumerable<Difference> Compare(string pathToContainer, IComparableObject before, IComparableObject after)
        {
            var differences = new List<Difference>();
            var attributeChanges = new List<AttributeChange>();

            LoopThroughFields(pathToContainer, before, after, differences, attributeChanges);
            LoopThroughProperties(pathToContainer, before, after, differences, attributeChanges);

            if (attributeChanges.Count > 0)
                differences.Add(new ChangedObject(pathToContainer, before, attributeChanges));

            return differences;
        }

        private void LoopThroughProperties(string pathToContainer, IComparableObject before, IComparableObject after, List<Difference> differences, List<AttributeChange> attributeChanges)
        {
            var propertyInfos = before.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var propertyInfo in propertyInfos)
            {
                PropertyInfo info = propertyInfo;
                CompareOne(x => info.GetValue(x, null), pathToContainer, before, after, differences, attributeChanges, propertyInfo.Name);
            }
        }

        private void LoopThroughFields(string pathToContainer, IComparableObject before, IComparableObject after, List<Difference> differences,
                                       List<AttributeChange> attributeChanges)
        {
            var fieldInfos = before.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach (var fieldInfo in fieldInfos)
            {
                FieldInfo info = fieldInfo;
                CompareOne(x => info.GetValue(x), pathToContainer, before, after, differences, attributeChanges, fieldInfo.Name);
            }
        }

        private void CompareOne(Func<IComparableObject, object> getElement, string pathToContainer, IComparableObject before, IComparableObject after, List<Difference> differences, List<AttributeChange> attributeChanges,
                                string name)
        {
            var valueBefore = getElement.Invoke(before);
            var valueAfter = getElement.Invoke(after);

            if (valueBefore is IEnumerable<IComparableObject>)
            {
                differences.AddRange(CheckArray(getElement, pathToContainer, before, after));
                return;
            }

            if (valueBefore is IComparableObject)
            {
                differences.AddRange(Compare(AddToPath(pathToContainer, before.IdentifyingText), valueBefore as IComparableObject, valueAfter as IComparableObject));
                return;
            }

            foreach (var comparer in AttributeComparers.Union(new[] { new DefaultAttributeComparer() }))
                if (comparer.CanHandle(valueBefore))
                {
                    var result = comparer.Compare(name, valueBefore, valueAfter);
                    if (result != null)
                        attributeChanges.Add(result);
                    return;
                }

            throw new Exception("Not handled: " + valueBefore.GetType());
        }

        private string AddToPath(string pathToContainer, string identifyingText)
        {
            if (pathToContainer == "")
                return identifyingText;

            return pathToContainer + ", " + identifyingText;
        }

        private IEnumerable<Difference> CheckArray(Func<IComparableObject, object> getElement, string pathToContainer, IComparableObject before, IComparableObject after)
        {
            var valueBefore = (IEnumerable<IComparableObject>) getElement.Invoke(before);
            var arrayBefore = valueBefore.ToList();

            var valueAfter = getElement.Invoke(after) as IEnumerable<IComparableObject>;
            if (valueAfter == null)
                throw new ArgumentException("after object signature does not match with before object: missing " + valueBefore);
            
            var arrayAfter = valueAfter.ToList();

            var deletedElements = arrayBefore.Where(x => !arrayAfter.Exists(y => x.Id == y.Id));
            var addedElements = arrayAfter.Where(x => !arrayBefore.Exists(y => x.Id == y.Id));

            var differences = new List<Difference>();
            differences.AddRange(deletedElements.Select(x => new DeletedObject(pathToContainer, before, x)));
            differences.AddRange(addedElements.Select(x => new AddedObject(pathToContainer, after, x)));

            foreach (var elementBefore in arrayBefore)
            {
                var elementAfter = arrayAfter.FirstOrDefault(x => x.Id == elementBefore.Id);
                if (elementAfter != null)
                    differences.AddRange(Compare(AddToPath(pathToContainer, before.IdentifyingText), elementBefore, elementAfter));
            }

            return differences;
        }
    }
}
