using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSystem : MonoBehaviour
{
	private void Update()	
    {
		SessionManager.Instance.OnUpdateInstance();
	}
}
