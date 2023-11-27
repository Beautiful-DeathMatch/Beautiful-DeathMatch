using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkScene : GameScene
{
    public void TryConnect()
	{
		NetworkManager.Instance.TryConnect();
	}

	// Update is called once per frame
	protected override void Update()	
    {
		NetworkManager.Instance.OnUpdateInstance();
	}
}
