using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.Sqlite;
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
		// JH // Player 생성 시 현재 플레이어의 ID를 InGameSystem의 리스트에 등록 (디버깅용)
		FindObjectOfType<InGameSystem>().playerIdList.Add(playerId);
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
	[SerializeField] // For Debug
	public int currentWeaponIndex /*{ get; private set; } */= 0;
	[SerializeField] // For Debug
	public InteractionData currentInteraction;

	public RaycastHit hit;
	Transform cam;
	float interactionMaxDistance = 8f;
	float gunMaxDistance = 100f;
	float knifeMaxDistance = 10f;
	LayerMask interactionLayerMask; // 인터랙션 레이 무시용 레이어 마스크
	LayerMask shotLayerMask; // 공격 레이 무시용 레이어 마스크

	// 각 리스트 항목 추가 및 삭제 (외부 접근용)
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

	// 미션과 상호작용 성공 시 해당 미션 완료 처리
	public void MissionInteraction(int missionType)
	{
		if(GetMissionTypeInProgress(missionType, out MissionComponent missionComponent))
		{
			missionComponent.Complete();
		}
	}

	// 특정 미션 타입이 현재 진행중인지 확인 및 가져오는 함수
	public bool IsMissionTypeInProgress(int missionType)
	{
		foreach(MissionComponent mission in missions)
		{
			if (mission.LoadData().missionType == missionType)
			{
				return mission.IsMissionInProgress();
			}
		}
		return false;
	}
	public bool GetMissionTypeInProgress(int missionType, out MissionComponent missionComponent)
	{
		foreach(MissionComponent mission in missions)
		{
			if (mission.LoadData().missionType == missionType)
			{
				missionComponent = mission;
				return mission.IsMissionInProgress();
			}
		}
		missionComponent = null;
		return false;
	}

	// 들고 있는 무기를 변경
	public void WeaponChange(int weaponIndex)
	{
		currentWeaponIndex = weaponIndex;
	}
	public WeaponComponent GetCurrentWeapon()
	{
		return weapons[currentWeaponIndex];
	}

	public void InteractionCheck()
	{
		currentInteraction = null;
		if(Physics.Raycast(cam.position, cam.forward, out hit, interactionMaxDistance, interactionLayerMask)) // 충돌 감지 시
		{
			Debug.Log(hit.transform);
			if (hit.transform.GetComponent<InteractionComponent>() != null) // InteractionComponent 감지 시
			{
				currentInteraction = hit.transform.GetComponent<InteractionComponent>().LoadData();
				if (currentInteraction.interactionType == InteractionData.INTERACTION_TYPE.MISSION)  // InteractionComponent의 타입이 미션일 시
				{
					if (Input.GetKeyDown(KeyCode.F))    // F키 입력 시
					{
						MissionInteraction(currentInteraction.subType);
					}
				}
			}
		}
	}

	public void ShotCheck()
	{
		if (weapons[currentWeaponIndex].LoadData().weaponType == WeaponData.WEAPON_TYPE.GUN)
		{
			Debug.DrawRay(cam.position, cam.forward * gunMaxDistance, Color.red, 1f);
			if (Physics.Raycast(cam.position, cam.forward, out hit, gunMaxDistance, shotLayerMask)) // 충돌 감지 시
			{
				Debug.Log("Shot : " + hit.transform);
				if (hit.transform.GetComponent<StatusComponent>() != null)
				{
					hit.transform.GetComponent<StatusComponent>().Hit(weapons[currentWeaponIndex].LoadData().damage);
				}
			}
		}
		else if (weapons[currentWeaponIndex].LoadData().weaponType == WeaponData.WEAPON_TYPE.KNIFE)
		{
			Debug.DrawRay(cam.position, cam.forward * knifeMaxDistance, Color.red, 1f);
			if (Physics.Raycast(cam.position, cam.forward, out hit, knifeMaxDistance, shotLayerMask)) // 충돌 감지 시
			{
				Debug.Log("Shot : " + hit.transform);
				if (hit.transform.GetComponent<StatusComponent>() != null)
				{
					hit.transform.GetComponent<StatusComponent>().Hit(weapons[currentWeaponIndex].LoadData().damage);
				}
			}
		}
	}

	void Start()
	{
		interactionLayerMask = ~LayerMask.GetMask("Player");
		shotLayerMask = ~LayerMask.GetMask("Player") & ~LayerMask.GetMask("Interaction");
		cam = Camera.main.transform;
	}

	// 디버깅용 Input Update 문, 실제로는 내가 조종하는 플레이어일 경우에만 가능한 행동들
	void Update() 
	{
		if(Input.GetKeyDown(KeyCode.Alpha1)){
			WeaponChange(0);
        }
		if(Input.GetKeyDown(KeyCode.Alpha2)){
			WeaponChange(1);
        }
		if(Input.GetMouseButtonDown(0)){
			if (weapons.Count > 0)
			{
				if (weapons[currentWeaponIndex].LoadData().currentMagazine > 0)
				{
					weapons[currentWeaponIndex].Shot();
					ShotCheck();
				}
			}
        }
		if(Input.GetKeyDown(KeyCode.R)){
			if(weapons.Count>0)
				weapons[currentWeaponIndex].Reload();
        }


		Debug.DrawRay(cam.position, cam.forward * interactionMaxDistance, Color.blue, 0.3f);
		InteractionCheck();
	}

}
