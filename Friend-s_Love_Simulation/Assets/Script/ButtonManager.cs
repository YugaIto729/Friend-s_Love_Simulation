using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public bool Click = false;
    public ItemData_Ins itemdata;
    public ItemPanel itemPanel;

    private void Start()
    {
        itemPanel = new ItemPanel();
    }

    public void MenuClick()
    {
        Click = true;
    }

    public void BackClick()
    {
        Click = false;
    }

    public void UseClick()
    {
        itemdata.Use();
    }

    public void DeleteClick()
    {
        itemPanel.Remove();
    }
}
