using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public enum CharacterType
{
	CH_03,
	CH_29,
	CH_46,
	MAX
}

public class PlayerComponent : SyncComponent
{
	[SerializeField] private ThirdPersonController controller = null;
	[SerializeField] private Transform cameraHead = null;

	[SerializeField] private GameObject[] characterPrefabs = new GameObject[(int)CharacterType.MAX];
	[SerializeField] private RuntimeAnimatorController[] characterAnimatorControllers = new RuntimeAnimatorController[(int)CharacterType.MAX];
	[SerializeField] private Avatar[] characterAvatars = new Avatar[(int)CharacterType.MAX];
	
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

		if (childSyncComponents != null)
		{
			foreach (var child in childSyncComponents)
			{
				child.Clear();
			}
		}
	}

	public void SetCharacter(CharacterType characterType)
	{
		var characterPrefab = characterPrefabs[(int)characterType];
		var characterAnimator = characterAnimatorControllers[(int)characterType];	
		var characterAvatar = characterAvatars[(int)characterType];

		Instantiate(characterPrefab, transform);
		controller.SetAnimatorController(characterAnimator, characterAvatar);
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
