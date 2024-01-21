using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerHelper
{
    public static int GetSortingLayer(GameObject layerObject, int orderIndex)
    {
        int layerNumber = layerObject.layer;
		return layerNumber * 1000 + Mathf.Abs(orderIndex % 1000);
    }
}
