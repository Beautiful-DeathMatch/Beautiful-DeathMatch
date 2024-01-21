using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[PopupAttribute("TestPopup.prefab")]
public class TestPopup : UIPopup
{
	protected override void OnOpen(UIParam param = null)
	{
		base.OnOpen(param);
	}

	protected override void OnClose()
	{
		base.OnClose();
	}
}
