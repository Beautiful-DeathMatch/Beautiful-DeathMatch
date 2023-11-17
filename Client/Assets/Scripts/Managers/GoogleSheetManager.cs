using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using System.Text;
using UnityEngine.Networking;

/// <summary>
/// Google Sheet를 읽어오기 위한 Manager
/// </summary>
public class GoogleSheetManager
{
    public GoogleSheet data { get; private set; }
    // Start is called before the first frame update

    public void Init()
    {
        data = Resources.Load<GoogleSheet>("ScriptableObj/GoogleSheet");
    }

    /// <summary>
    /// 구글 스프레드 시트를 받아와 가공하는 함수
    /// </summary>
    /// <param name="GoogleSheetsID"> 구글 스프레드 시트의 ID </param>
    /// <param name="ManufactureData"> 스프레드 시트 가공 함수 input은 한 줄 을 받아야함, Cell은 탭('\t')으로 구분</param>
    /// <param name="WorkSheetsID"> 시트의 WorkSheet 구분 default 는 0 첫 장</param>
    /// <param name="startCell"> 시작 셀 default A2</param>
    /// <param name="endCell"> 끝 셀 default E </param>
    /// <returns></returns>
    public IEnumerator RequestAndSetItemDatas(string GoogleSheetsID, Action<string> ManufactureData, string WorkSheetsID = "0", string endCell = "Z", string startCell = "A2")
    {

        StringBuilder sb = new StringBuilder();
        sb.Append("https://docs.google.com/spreadsheets/d/");
        sb.Append(GoogleSheetsID);
        sb.Append("/export?format=tsv");
        sb.Append("&gid=" + data.associatedDataWorksheet);
        sb.Append("&range=" + startCell + ":" + endCell);

        Debug.Log(sb.ToString());


        using (UnityWebRequest webData = UnityWebRequest.Get(sb.ToString()))
        {

            yield return webData.SendWebRequest();

            string[] dataLines = webData.downloadHandler.text.Split('\n');
            Action[] ManufactureDatas = new Action[dataLines.Length];
            for (int i = 0; i < dataLines.Length; i++)
            {
                ManufactureData(dataLines[i]);
            }

        }

    }


}
