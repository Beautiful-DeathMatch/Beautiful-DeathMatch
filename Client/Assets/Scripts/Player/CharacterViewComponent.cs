using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterViewAsset
{
	[SerializeField] private GameObject characterObj;
	[SerializeField] private Animator animator;

	[SerializeField] private Transform muzzle;

	[Serializable]
	public class AttachItemDictionary : SerializableDictionary<ENUM_ITEM_TYPE, GameObject> { }
	[SerializeField] private AttachItemDictionary attatchItemDictionary = new();
	private GameObject currentItemObj = null;

	public void SetActiveCharacter(bool isActive)
	{
		characterObj.SetActive(isActive);
	}

	public Animator GetAnimator()
	{
		return animator;
	}

	public Transform GetMuzzle()
	{
		return muzzle;
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
			currentItemObj.SetActive(true);
		}
	}
}


public class CharacterViewComponent : MonoBehaviour
{
	[SerializeField] private CharacterViewAsset[] viewAssets = new CharacterViewAsset[(int)CharacterType.MAX];

	private CharacterType characterType = CharacterType.MAX;

	public void SetCharacter(CharacterType characterType)
	{
		if (characterType == CharacterType.MAX)
			return;

		this.characterType = characterType;

		for (int i = 0; i < (int)CharacterType.MAX; i++)
		{
			bool isActive = (CharacterType)i == characterType;
			viewAssets[i].SetActiveCharacter(isActive);
		}
	}

	public Animator GetCharacterAnimator(CharacterType characterType)
	{
		int i = (int)characterType;
		return viewAssets[i].GetAnimator();
	}

	public Transform GetMuzzle(CharacterType characterType)
	{
		int i = (int)characterType;
		return viewAssets[i].GetMuzzle();
	}

	public bool TryAttachRightHand(ENUM_ITEM_TYPE itemType)
	{
		if (characterType == CharacterType.MAX)
			return false;

		viewAssets[(int)characterType].AttachRightHand(itemType);
		return true;
	}
}
