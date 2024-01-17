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

	[SerializeField] private CharacterViewComponent characterViewComponent = null;

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

	public override void OnReceive(IPacket packet)
	{
		
	}

	// =============JH============= //

	// 해당 Player 가 가지고 있는 Weapon Component, Mission Component 리스트
	// 다른 Component, System, UI 에서 참조할 수 있음
	[SerializeField]
	public List<WeaponComponent> weapons { get; private set; } = new();
	[SerializeField]
	public List<MissionComponent> missions { get; private set; } = new();

	public void WeaponAdd(WeaponComponent weaponComponent)
	{
		weapons.Add(weaponComponent);
	}

	public void WeaponDelete(WeaponComponent weaponComponent)
	{
		weapons.Remove(weaponComponent);
	}

	public void MissionAdd(MissionComponent missionComponent)
	{
		missions.Add(missionComponent);
	}

	public void MissionDelete(MissionComponent missionComponent)
	{
		missions.Remove(missionComponent);
	}

	public bool IsMissionTypeInProgress(int missionType)
	{
		foreach(MissionComponent mission in  missions)
		{
			if (mission.LoadData().missionType == missionType)
				return mission.IsMissionInProgress();
		}
		return false;
	}

}
