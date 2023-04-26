using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class SwitchPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject detTextPanel;
    
    public void SwitchDetailsText()
    {
        if (detTextPanel.activeSelf)
        {
            detTextPanel.SetActive(false);
        }
        else
        {
            detTextPanel.SetActive(true);
        }
    }
}
