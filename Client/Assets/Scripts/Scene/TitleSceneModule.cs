using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneModule : SceneModule
{
	protected override void OnGUI()
	{
		if (GUI.Button(new Rect(0, 0, 300, 100), "매치 채널로 이동"))
		{
			SceneModuleSystemManager.Instance.TryEnterSceneModule(SceneType.Match);
		}
	}

}
