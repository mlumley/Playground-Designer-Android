using UnityEngine;
using System.Collections;
using System;

public class DesignInfo : IComparable<DesignInfo> {

	public string Name;
	public string ImageName;
	public string ModelName;
    public string MainCategory;
	public string Category;

	public DesignInfo (string name, string imageName, string modelName, string mainCategory, string category)
	{
		Name = name;
		ImageName = imageName;
		ModelName = modelName;
        MainCategory = mainCategory;
        Category = category;
	}

    public int CompareTo(DesignInfo comparePart) {
        // A null value means that this object is greater.
        if (comparePart == null)
            return 1;

        else
            return this.Name.CompareTo(comparePart.Name);
    }
}
