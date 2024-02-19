using StarterAssets;
using System;
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

	[Serializable]
	public class AttachItemDictionary : SerializableDictionary<ENUM_ITEM_TYPE, GameObject> { }
	[SerializeField] private AttachItemDictionary attatchItemDictionary = new();
	private GameObject currentItemObj = null;

	public void SetController(ThirdPersonController controller)
	{
		var animator = controller.GetComponent<Animator>();
		if (animator == null)
			return;

		animator.runtimeAnimatorController = animatorController;
		animator.avatar = avatar;
	}

	public void SetActiveCharacter(bool isActive)
	{
		characterObj.SetActive(isActive);
	}

	public void AttachRightHand(ENUM_ITEM_TYPE itemType)
	{
		if (currentItemObj)
		{
			currentItemObj.SetActive(false);
		}

		// 새 아이템 타입으로 찾아서 삽입
		if (attatchItemDictionary.TryGetValue(itemType, out var item))
		{
			currentItemObj = item;
			currentItemObj.transform.parent = rightHandSocket;
			currentItemObj.transform.localPosition = new Vector3(0, 0, 0);
			currentItemObj.SetActive(true);
		}
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
