using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class DropdownSorter : MonoBehaviour {

    public Dropdown dropdownMenu;
    private List<string> categories;

    public void setCatagories(List<string> categories) {
        this.categories = categories;
        categories.Sort();
        dropdownMenu.AddOptions(categories);
        updateObjectDropdown();
        updateLandscapeDropdown();
    }

	public void updateObjectDropdown () {
        int index = dropdownMenu.value;
        Dropdown.OptionData item = dropdownMenu.options[index];
        UIManager.Instance.SetObjectData(DataManager.objectInfoList, "Elements", item.text);
    }

    public void updateLandscapeDropdown() {
        int index = dropdownMenu.value;
        Dropdown.OptionData item = dropdownMenu.options[index];
        UIManager.Instance.SetLandscapeData(DataManager.objectInfoList, "Landscape", item.text);
    }
}
