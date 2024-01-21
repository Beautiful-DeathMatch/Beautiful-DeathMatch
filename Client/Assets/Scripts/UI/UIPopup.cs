using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIParam
{

}


/// <summary>
/// 코드를 통해 유기적으로 열고 닫을 수 있는 UI는 Popup을 상속받는다.
/// </summary>

public class UIPopup : UIWindow
{
    public bool IsActive
    {
        get;
        private set;
    }

    public bool TryOpen(UIParam param = null)
    {
        if (IsActive)
            return false;

        gameObject.SetActive(true);
        IsActive = true;

        OnOpen(param);
        return true;
    }

    public bool TryClose()
    {
        if (!IsActive)
            return false;

        gameObject.SetActive(false);
        IsActive = false;

        OnClose();
        return true;
    }

    protected virtual void OnOpen(UIParam param = null)
    {

    }

    protected virtual void OnClose()
    {

    }

    public void OnClickClose()
    {
        uiSystem.ClosePopup(GetType());
    }
}

