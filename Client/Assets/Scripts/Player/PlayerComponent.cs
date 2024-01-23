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
[RequireComponent(typeof(PlayerStatusComponent))]
[RequireComponent(typeof(PlayerShotComponent))]
[RequireComponent(typeof(PlayerSlashComponent))]
[RequireComponent(typeof(PlayerSwimComponent))]
public class PlayerComponent : MonoBehaviour
{
	[SerializeField] private ThirdPersonController controller = null;
	[SerializeField] private Transform cameraHead = null;

	[SerializeField] private CharacterViewComponent characterViewComponent = null;

	public int playerId { get; private set; }

	public void Initialize(int playerId)
	{
		this.playerId = playerId;
	}

	public void SetCharacter(CharacterType characterType)
	{
		characterViewComponent.SetCharacter(characterType, controller);
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
}
