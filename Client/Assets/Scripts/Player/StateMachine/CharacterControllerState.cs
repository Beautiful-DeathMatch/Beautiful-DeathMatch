using StarterAssets;
using UnityEngine;
using UnityEngine.Animations;

public class CharacterControllerState : StateMachineBehaviour
{
    private ThirdPersonController controller = null;

    public static void Initialize(Animator ownerAnimator)
    {
		var controller = ownerAnimator.GetComponent<ThirdPersonController>();
		if (controller == null)
			return;

		var states = ownerAnimator.GetBehaviours<CharacterControllerState>();
		foreach (var state in states)
		{
			state.InitializeInternal(controller);
		}
	}

	private void InitializeInternal(ThirdPersonController controller)
	{
		this.controller = controller;
	}

	public override void OnStateEnter(UnityEngine.Animator animator, UnityEngine.AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, animatorStateInfo, layerIndex);
	}

	public sealed override void OnStateUpdate(UnityEngine.Animator animator, UnityEngine.AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
		OnStatePrevUpdate(animator, animatorStateInfo, layerIndex);

		base.OnStateUpdate(animator, animatorStateInfo, layerIndex);

		OnStateLateUpdate(animator, animatorStateInfo, layerIndex);
	}

	public virtual void OnStatePrevUpdate(UnityEngine.Animator animator, UnityEngine.AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
		
	}

	public virtual void OnStateLateUpdate(UnityEngine.Animator animator, UnityEngine.AnimatorStateInfo animatorStateInfo, int layerIndex)
	{

	}

	public override void OnStateExit(UnityEngine.Animator animator, UnityEngine.AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
		base.OnStateExit(animator, animatorStateInfo, layerIndex);
	}

	public sealed override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
	{
		base.OnStateMachineEnter(animator, stateMachinePathHash);
	}

	public sealed override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
	{
		base.OnStateMachineExit(animator, stateMachinePathHash);
	}
}