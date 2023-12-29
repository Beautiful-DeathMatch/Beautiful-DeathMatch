using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterViewAsset
{
	[SerializeField] private GameObject characterObj;
	[SerializeField] private RuntimeAnimatorController animatorController;
	[SerializeField] private Avatar avatar;

	[SerializeField] private Transform leftHandSocket;
	[SerializeField] private Transform rightHandSocket;

	private GameObject currentOwnerObj = null;
	private GameObject currentHandObj = null;

	public void SetController(ThirdPersonController controller)
	{
		controller.SetAnimatorController(animatorController, avatar);
	}

	public void SetActiveCharacter(bool isActive)
	{
		characterObj.SetActive(isActive);
	}

	public void AttachRightHand(GameObject prefab)
	{
		if (currentOwnerObj == null)
			return;

		if (currentHandObj != null)
			MonoBehaviour.Destroy(currentHandObj);

		currentHandObj = MonoBehaviour.Instantiate(prefab, rightHandSocket.transform);
	}

	public void AttachLeftHand(GameObject prefab)
	{
		if (currentOwnerObj == null)
			return;

		if (currentHandObj != null)
			MonoBehaviour.Destroy(currentHandObj);

		currentHandObj = MonoBehaviour.Instantiate(prefab, leftHandSocket.transform);
	}
}


public class CharacterViewComponent : MonoBehaviour
{
	[SerializeField] private CharacterViewAsset[] viewAssets = new CharacterViewAsset[(int)CharacterType.MAX];

	private CharacterType characterType = CharacterType.MAX;

	public void SetCharacter(CharacterType characterType, ThirdPersonController controller)
	{
		if (characterType == CharacterType.MAX)
			return;

		this.characterType = characterType;

		for (int i = 0; i < (int)CharacterType.MAX; i++)
		{
			bool isActive = (CharacterType)i == characterType;
			if (isActive)
			{
				viewAssets[i].SetController(controller);
			}

			viewAssets[i].SetActiveCharacter(isActive);
		}
	}

	// 여기 나중에 들 것 타입 추가되면, WeaponType 넘기는 쪽으로 수정할 것
	public bool TryAttachRightHand(GameObject prefab)
	{
		if (characterType == CharacterType.MAX)
			return false;

		viewAssets[(int)characterType].AttachRightHand(prefab);
		return true;
	}

	public bool TryAttachLeftHand(GameObject prefab)
	{
		if (characterType == CharacterType.MAX)
			return false;

		viewAssets[(int)characterType].AttachLeftHand(prefab);
		return true;
	}
}
