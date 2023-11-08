using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UIPanelsManager : MonoBehaviour
{
    public List<UIPanel> UIPanels;
    
    private void Start()
    {
        foreach (var uiPanel in UIPanels)
        {
            uiPanel.uiPanelsManager = this;
        }
    }

    public void HideAll()
    {
        
        foreach (var uiPanel in UIPanels)
        {
            uiPanel.Hide();
        }
    }
}