ObjectComparer
-----------

Provide before and after, then ObjectComparer will identify the differences as what is added, what is deleted and what is changed.

Added and removed objects can be identified when the comparing object has ```IEnumerable```. To distinguish "changed" elements from added and deleted elements, your ```IEnumerable``` element type should implement ```IComparableObject```, which requires ```Id``` and ```IdentifyingText```. 

* If the Id is found only in before, it is deleted. 
* If Id is found only in after, it is added. 
* If found both, it may be changed or the same.

Sample usage:
```c#
    private void ShowDifferences(IComparableObject before, IComparableObject after)
    {
  	var comparer = new ObjectComparer();
		var differences = comparer.Compare("", before, after).ToList();
		foreach (var difference in differences)
			Debug.Print(difference.GetDescription() + "\r\n");
    }
```
Sample result:
```
Object 1 deleted Child 3.

Object 1 added Child 4.

Object 1, Child 2 deleted GrandChild 22.

Object 1, Child 2, GrandChild 21 changed.
FieldLongValue: 22 => 23
FieldStringValue: 23 => 23changed
PropertyLongValue: 24 => 25
PropertyStringValue: 25 => 25changed

Object 1 changed.
FieldLongValue: 2 => 3
FieldStringValue: 3 => 3changed
PropertyLongValue: 4 => 5
PropertyStringValue: 5 => 5changed
```

It detects changes in both fields and properties.

To Dos
-----------
* ```Compare()``` method doesn't have to accept ```IComparableObject```. IComparableObject is needed only when it is used as ```IEnumerable``` element.
* When the compared field or property is 
** not an IEnumerable and 
** not defined in AttributeComparer, and 
** it is a complex type, 
ObjectComparer should go deeper and compare attributes automatically, rather than relying on .Equal().
