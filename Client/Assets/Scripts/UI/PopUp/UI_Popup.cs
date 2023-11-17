using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_Popup : UI_Base
{
    /// <summary> 정렬 설정 </summary>
    public override void Init()
    {
        GameManager.UI.SetCanvas(gameObject, true);
    }

    /// <summary> pop up 닫기 </summary>
    public virtual void ClosePopUpUI()
    {
        GameManager.UI.ClosePopUpUI(this);
    }
    /// <summary> pop up 다시 열 때마다 실행</summary>
    public virtual void ReOpenPopUpUI(){}
}

