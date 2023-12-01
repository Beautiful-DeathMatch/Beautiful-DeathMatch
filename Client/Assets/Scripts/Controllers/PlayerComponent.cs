using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerComponent : SyncComponent
{
	[SerializeField] private ThirdPersonController controller = null;
	[SerializeField] private Transform cameraHead = null;

	private IEnumerable<SyncComponent> childSyncComponents = null;

	public override void Initialize(int playerId)
	{
		base.Initialize(playerId);

		childSyncComponents = GetComponentsInChildren<SyncComponent>().Where(c => c != this);
		foreach (var child in childSyncComponents)
		{
			child.Initialize(playerId);
		}
	}

	public override void Clear()
	{
		base.Clear();

		foreach (var child in childSyncComponents)
		{
			child.Clear();
		}
	}

	public void SetInput(PlayerInput inputComponent, StarterAssetsInputs inputAsset)
	{
		controller.SetInput(inputComponent, inputAsset);	
	}

	public void SetCamera(Cinemachine.CinemachineVirtualCamera virtualCamera)
	{
		virtualCamera.Follow = cameraHead;
	}

	public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

	public override void OnReceive(IPacket packet)
	{
		
	}
}
