using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DesignInfo : IComparable<DesignInfo> {

	public string Name;
	public string ImageName;
	public string ModelName;
    public string MainCategory;
	public string[] Category;

	public DesignInfo (string name, string imageName, string modelName, string mainCategory, string category)
	{
		Name = name;
		ImageName = imageName;
		ModelName = modelName;
        MainCategory = mainCategory;
        Category = category.Split(',');
        for(int i = 0; i < Category.Length; i++) {
            Category[i] = Category[i].Replace("[","");
            Category[i] = Category[i].Replace(" ", "");
            Category[i] = Category[i].Replace("\"", "");
            Category[i] = Category[i].Replace("]", "");
        }
	}

    public int CompareTo(DesignInfo comparePart) {
        // A null value means that this object is greater.
        if (comparePart == null)
            return 1;

        else
            return this.Name.CompareTo(comparePart.Name);
    }

    public bool ContainsCategory(string category) {
        foreach(string cat in Category) {
            if(cat == category) {
                return true;
            }
        }
        return false;
    }
}
