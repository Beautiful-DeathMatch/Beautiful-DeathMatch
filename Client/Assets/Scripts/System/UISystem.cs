using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

// 임시 디버깅을 위한 UI System
public class UISystem : MonoBehaviour
{
    public GameObject canvas;
    public TextMeshProUGUI current;
    public TextMeshProUGUI currentContent;
    public TextMeshProUGUI weaponList;
    public TextMeshProUGUI itemList;
    public TextMeshProUGUI mission;
    public TextMeshProUGUI interaction;
    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera playerCamera = null;
    public PlayerComponent playerComponent = null;
    public TempDB tempDB;
    StringBuilder stringBuilder = new StringBuilder();

    void PrintCurrentActiveText()
    {
        stringBuilder.Clear();
        if (playerComponent.currentActiveIndex < 2)
        {
            WeaponData weaponData = playerComponent.weapons[playerComponent.currentActiveIndex].LoadData();
            current.text = tempDB.GetWeaponNameByIndex(weaponData.weaponIndex); // 무기 이름 출력
            stringBuilder.AppendFormat("[{0}", weaponData.ownerID);
            stringBuilder.AppendFormat("{0}] ", weaponData.weaponType);
            stringBuilder.AppendFormat("{0} ", weaponData.currentMagazine);
            stringBuilder.AppendFormat("/ {0} ", weaponData.maxMagazine);
            stringBuilder.AppendFormat("/ {0} ", weaponData.remainedMagazine);
        }
        else
        {
            ItemData itemData = playerComponent.items[playerComponent.currentActiveIndex-2].LoadData();
            current.text = tempDB.GetItemNameByIndex(itemData.itemIndex); // 아이템 이름 출력
            stringBuilder.AppendFormat("[{0}]", itemData.ownerID);
            stringBuilder.AppendFormat("{0} ", itemData.currentMagazine);
        }
        currentContent.text = stringBuilder.ToString(); // 무기 정보 출력
    }

    void PrintWeaponListText()
    {
        stringBuilder.Clear();
        for (int i = 0; i < playerComponent.weapons.Count; i++)
        {
            WeaponComponent weaponComponent = playerComponent.weapons[i];
            WeaponData weaponData = weaponComponent.LoadData();
            stringBuilder.AppendFormat("{0} : ", i+1);
            stringBuilder.AppendFormat("{0} ", tempDB.GetWeaponNameByIndex(weaponData.weaponIndex));
            stringBuilder.AppendFormat("{0} ", weaponData.currentMagazine);
            stringBuilder.AppendFormat("/ {0} ", weaponData.maxMagazine);
            stringBuilder.AppendFormat("/ {0} ", weaponData.remainedMagazine);
            if (playerComponent.currentActiveIndex < 2 && playerComponent.weapons[playerComponent.currentActiveIndex] == weaponComponent)
                stringBuilder.Append(" (선택 중)");
            stringBuilder.Append("\n");   
        }
            weaponList.text = stringBuilder.ToString(); // 무기 정보 출력
    }

    void PrintItemListText()
    {
        stringBuilder.Clear();
        for (int i = 0; i < playerComponent.items.Count; i++)
        {
            ItemComponent itemComponent = playerComponent.items[i];
            ItemData itemData = itemComponent.LoadData();
            stringBuilder.AppendFormat("{0} : ", i+3);
            stringBuilder.AppendFormat("{0} ", tempDB.GetItemNameByIndex(itemData.itemIndex));
            stringBuilder.AppendFormat("{0} ", itemData.currentMagazine);
            if (playerComponent.currentActiveIndex >= 2 && playerComponent.items[playerComponent.currentActiveIndex-2] == itemComponent)
                stringBuilder.Append(" (선택 중)");
            stringBuilder.Append("\n");   
        }
            itemList.text = stringBuilder.ToString(); // 아이템 정보 출력
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

    void Awake()
    {
        canvas.SetActive(false);
    }

    void Update()
    {
        if(canvas.activeSelf)
        {
            if (playerComponent == null)
                playerComponent = playerCamera.Follow.parent.GetComponent<PlayerComponent>();
            else
            {
                PrintCurrentActiveText();
                PrintWeaponListText();
                PrintItemListText();
                PrintMissionText();
                PrintInteractionText();
            }
        }
    }
}
