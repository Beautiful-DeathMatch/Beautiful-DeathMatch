using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

// 임시 디버깅을 위한 UI System
public class UISystem : MonoBehaviour
{
    public GameObject canvas;
    public TextMeshProUGUI weapon;
    public TextMeshProUGUI weaponContent;
    public TextMeshProUGUI mission;
    public TextMeshProUGUI interaction;
    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera playerCamera = null;
    public PlayerComponent playerComponent = null;
    public TempDB tempDB;
    StringBuilder stringBuilder = new StringBuilder();

    void PrintWeaponText()
    {
        stringBuilder.Clear();
        WeaponData weaponData = playerComponent.weapons[playerComponent.currentWeaponIndex].LoadData();
        weapon.text = tempDB.GetWeaponNameByType(weaponData.weaponType); // 무기 이름 출력
        stringBuilder.Clear();
        stringBuilder.AppendFormat("[{0}", weaponData.ownerID);
        stringBuilder.AppendFormat("{0}] ", weaponData.weaponType);
        stringBuilder.AppendFormat("{0} ", weaponData.currentMagazine);
        stringBuilder.AppendFormat("/ {0} ", weaponData.maxMagazine);
        stringBuilder.AppendFormat("/ {0} ", weaponData.remainedMagazine);
        weaponContent.text = stringBuilder.ToString(); // 무기 정보 출력
    }

    void PrintMissionText()
    {
        stringBuilder.Clear();
        foreach (MissionComponent missionComponent in playerComponent.missions)
        {
            MissionData missionData = missionComponent.LoadData();
            stringBuilder.AppendFormat("{0}", tempDB.GetMissionNameByType(missionData.missionType));
            if (missionData.progression >= missionData.maxProgression)
                stringBuilder.Append(" (완료)");
            stringBuilder.Append("\n");   
        }
        mission.text = stringBuilder.ToString(); // 미션 정보 출력        
    }

    void PrintInteractionText()
    {
        stringBuilder.Clear();
        interaction.text = "";
        if (playerComponent.currentInteraction != null)
            if (playerComponent.currentInteraction.interactionType == InteractionData.INTERACTION_TYPE.MISSION)
                if (playerComponent.IsMissionTypeInProgress(playerComponent.currentInteraction.subType))
                {
                    stringBuilder.AppendFormat("sys.temp.interaction.mission{0}", playerComponent.currentInteraction.subType);
                    interaction.text = tempDB.GetStringByKey(stringBuilder.ToString()); // 인터랙션 정보 출력
                }       
    }

    void Update()
    {
        if(canvas.activeSelf)
        {
            if (playerComponent == null)
                playerComponent = playerCamera.Follow.parent.GetComponent<PlayerComponent>();
            else
            {
                PrintWeaponText();
                PrintMissionText();
                PrintInteractionText();
            }
        }
    }
}
