using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using Client;

[CreateAssetMenu(fileName = "GoogleSheet", menuName = "Scriptable Object/GoogleSheet", order = int.MaxValue)]

public class GoogleSheet : ScriptableObject
{
    public string associatedSheet = "";
    public string associatedDataWorksheet = "";
    //public string associatedData2Worksheet = "";

    enum DataCell
    {
        Start,
    }
    enum Data2Cell
    {
       
    }
    /// <summary>
    /// ���� ������ ���� �Լ�
    /// </summary>
    /// <param name="line"> \t ���� Cell�� ������ �� �� </param>
    internal void GetData(string line)
    {
        string[] tmp = line.Split('\t');

        if (tmp[(int)DataCell.Start] != "")
        {
            //TODO


        }

    }
   
}
