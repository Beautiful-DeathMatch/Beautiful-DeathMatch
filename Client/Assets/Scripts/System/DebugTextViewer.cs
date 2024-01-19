using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugTextViewer : MonoBehaviour
{
    [SerializeField]
    StatusComponent statusComponent;
    [SerializeField]
    TextMeshPro textMeshPro;


    void Update()
    {
        textMeshPro.text = statusComponent.LoadData().hp.ToString();
    }
}
