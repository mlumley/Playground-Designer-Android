using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Controls what dropdown menus display
/// </summary>
public class DropdownSorter : MonoBehaviour {

    public Dropdown dropdownMenu;
    public GameObject panels;

    /// <summary>
    /// Set the list of categories for a dropdown menu
    /// </summary>
    /// <param name="categories">Categories to be set</param>
    public void setCatagories(List<string> categories) {
        categories.Sort();
        dropdownMenu.AddOptions(categories);
        updateObjectDropdown();
        updateLandscapeDropdown();
    }

    /// <summary>
    /// Update the currently selected category and display that categories models
    /// </summary>
	public void updateObjectDropdown () {
        int index = dropdownMenu.value;
        Dropdown.OptionData item = dropdownMenu.options[index];
        UIManager.Instance.SetObjectData(DataManager.objectInfoList, "Elements", item.text);
        if(panels.GetComponent<ModelPanelController>().IsHid)
            panels.GetComponent<ModelPanelController>().ShowHidePanel(0);
    }

    /// <summary>
    /// Update the currently selected category and display that categories models
    /// </summary>
    public void updateLandscapeDropdown() {
        int index = dropdownMenu.value;
        Dropdown.OptionData item = dropdownMenu.options[index];
        UIManager.Instance.SetLandscapeData(DataManager.objectInfoList, "Landscape", item.text);
        if (panels.GetComponent<ModelPanelController>().IsHid)
            panels.GetComponent<ModelPanelController>().ShowHidePanel(0);
    }
}
