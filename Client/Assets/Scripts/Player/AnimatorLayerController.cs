using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorLayerController : StateMachineBehaviour
{
	[SerializeField] private int layerIndex = 1;
	[SerializeField] private float layerWeight = 0.0f;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);

		animator.SetLayerWeight(layerIndex, layerWeight);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateExit(animator, stateInfo, layerIndex);
	}
}