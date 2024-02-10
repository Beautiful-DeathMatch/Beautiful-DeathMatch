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

	[SerializeField] private Transform rightHandSocket;

	private GameObject currentItemObj = null;

	public void SetController(ThirdPersonController controller)
	{
		controller.SetAnimatorController(animatorController, avatar);
	}

	public void SetActiveCharacter(bool isActive)
	{
		characterObj.SetActive(isActive);
	}

	public void AttachRightHand(ENUM_ITEM_TYPE itemType)
	{
		if (currentItemObj != null)
		{
            currentItemObj.SetActive(false);
        }

        // 새 아이템 타입으로 찾아서 삽입
        // currentItemObj.SetActive(true);
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

	public bool TryAttachRightHand(ENUM_ITEM_TYPE itemType)
	{
		if (characterType == CharacterType.MAX)
			return false;

		viewAssets[(int)characterType].AttachRightHand(itemType);
		return true;
	}
}
