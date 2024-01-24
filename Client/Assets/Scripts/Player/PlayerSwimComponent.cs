using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwimComponent : MonoBehaviour
{
	[SerializeField] private ThirdPersonController controller = null;

	private bool isInWater = false;

	private void OnEnable()
	{
		controller.IsInWater += IsInWater;
	}

	private void OnDisable()
	{
		controller.IsInWater -= IsInWater;
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("Water"))
		{
			isInWater = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
		{
			isInWater = false;
		}
	}

	private bool IsInWater()
	{
		return isInWater;
	}
}
