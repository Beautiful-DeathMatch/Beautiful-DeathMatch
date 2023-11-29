using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkScene : GameScene
{
    public void TryConnect()
	{
		SessionManager.Instance.TryConnect();
	}
	protected override void Update()	
    {
		SessionManager.Instance.OnUpdateInstance();
	}
}
