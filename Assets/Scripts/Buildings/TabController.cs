using System;
using UnityEngine;
using UnityEngine.UI;
public class TabController : MonoBehaviour
{
    private int tabIndex;

    [SerializeField] private GameObject[] tabs;
    [SerializeField] private int defaultTab;

    public int TabIndex
    {
        get => tabIndex;
        set
        {
            tabIndex = value;
            UpdateTabs();
        }
    }

    private void OnEnable()
    {
        if (defaultTab >= 0)
            TabIndex = defaultTab;
    }

    public void SetTabIndex(int index)
    {
        TabIndex = index;
    }

    private void UpdateTabs()
    {
        for (int i = 0; i < tabs.Length; i++)
            tabs[i].SetActive(i == TabIndex);
    }
}
