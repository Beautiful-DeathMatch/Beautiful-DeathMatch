using Mirror;
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


[RequireComponent(typeof(PlayerInteractionComponent))]
[RequireComponent(typeof(PlayerItemComponent))]
[RequireComponent(typeof(PlayerMissionComponent))]
[RequireComponent(typeof(PlayerAttackComponent))]
[RequireComponent(typeof(PlayerSwimComponent))]
[RequireComponent(typeof(PlayerStatusComponent))]
[RequireComponent(typeof(NetworkAnimator))]
public class PlayerComponent : NetworkBehaviour
{
	[SerializeField] private ThirdPersonController controller = null;
	[SerializeField] private Transform cameraHead = null;

	[SerializeField] private CharacterViewComponent characterViewComponent = null;

	[SerializeField] private PlayerItemComponent playerItemComponent = null;
	[SerializeField] private PlayerAttackComponent playerAttackComponent = null;
	[SerializeField] private PlayerMissionComponent playerMissionComponent = null;
	[SerializeField] private PlayerInteractionComponent playerInteractionComponent = null;

	[SerializeField] private NetworkAnimator networkAnimator = null;

	public int playerId { get; private set; }

	public void Initialize(int playerId)
	{
		this.playerId = playerId;

		playerItemComponent.SetPlayerId(playerId);
		playerAttackComponent.SetPlayerId(playerId);
		playerMissionComponent.SetPlayerId(playerId);
		playerInteractionComponent.SetPlayerId(playerId);
	}

	public void SetCharacter(CharacterType characterType)
	{
		characterViewComponent.SetCharacter(characterType, controller);
		networkAnimator.Initialize();
	}

	public void SetInput(PlayerInputAsset inputAsset)
	{
		controller.SetInput(inputAsset);	
	}

	public void SetCamera(Cinemachine.CinemachineVirtualCamera virtualCamera)
	{
		virtualCamera.Follow = cameraHead;
	}

	public void SetPosition(Vector3 pos)
    {
		transform.position = pos;
    }
}
