using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class BattleMainWindow : UIMainWindow
{
    public TextMeshProUGUI currentWeaponText;
    public TextMeshProUGUI currentWeaponInfoText;
    public TextMeshProUGUI weaponList;
    public TextMeshProUGUI itemListText;
    public TextMeshProUGUI missionText;
    public TextMeshProUGUI interactionText;

    public PlayerComponent playerComponent = null;
    public TempDB tempDB;
    StringBuilder tempStringBuilder = new StringBuilder();

    public override void OnEnter(SceneModuleParam param)
    {
		playerComponent = FindObjectOfType<PlayerComponent>();
	}

    private void PrintCurrentActiveText()
    {
        tempStringBuilder.Clear();
        if (playerComponent.currentActiveIndex < 2)
        {
            WeaponData weaponData = playerComponent.weapons[playerComponent.currentActiveIndex].LoadData();
            currentWeaponText.text = tempDB.GetWeaponNameByIndex(weaponData.weaponIndex); // 무기 이름 출력

            tempStringBuilder.AppendFormat("[{0}", weaponData.ownerID);
            tempStringBuilder.AppendFormat("{0}] ", weaponData.weaponType);
            tempStringBuilder.AppendFormat("{0} ", weaponData.currentMagazine);
            tempStringBuilder.AppendFormat("/ {0} ", weaponData.maxMagazine);
            tempStringBuilder.AppendFormat("/ {0} ", weaponData.remainedMagazine);
        }
        else
        {
            ItemData itemData = playerComponent.items[playerComponent.currentActiveIndex-2].LoadData();
            currentWeaponText.text = tempDB.GetItemNameByIndex(itemData.itemIndex); // 아이템 이름 출력
            tempStringBuilder.AppendFormat("[{0}]", itemData.ownerID);
            tempStringBuilder.AppendFormat("{0} ", itemData.currentMagazine);
        }
        currentWeaponInfoText.text = tempStringBuilder.ToString(); // 무기 정보 출력
    }

	private void PrintWeaponListText()
    {
        tempStringBuilder.Clear();
        for (int i = 0; i < playerComponent.weapons.Count; i++)
        {
            WeaponComponent weaponComponent = playerComponent.weapons[i];
            WeaponData weaponData = weaponComponent.LoadData();
            tempStringBuilder.AppendFormat("{0} : ", i+1);
            tempStringBuilder.AppendFormat("{0} ", tempDB.GetWeaponNameByIndex(weaponData.weaponIndex));
            tempStringBuilder.AppendFormat("{0} ", weaponData.currentMagazine);
            tempStringBuilder.AppendFormat("/ {0} ", weaponData.maxMagazine);
            tempStringBuilder.AppendFormat("/ {0} ", weaponData.remainedMagazine);
            if (playerComponent.currentActiveIndex < 2 && playerComponent.weapons[playerComponent.currentActiveIndex] == weaponComponent)
                tempStringBuilder.Append(" (선택 중)");
            tempStringBuilder.Append("\n");   
        }
            weaponList.text = tempStringBuilder.ToString(); // 무기 정보 출력
    }

	private void PrintItemListText()
    {
        tempStringBuilder.Clear();
        for (int i = 0; i < playerComponent.items.Count; i++)
        {
            ItemComponent itemComponent = playerComponent.items[i];
            ItemData itemData = itemComponent.LoadData();
            tempStringBuilder.AppendFormat("{0} : ", i+3);
            tempStringBuilder.AppendFormat("{0} ", tempDB.GetItemNameByIndex(itemData.itemIndex));
            tempStringBuilder.AppendFormat("{0} ", itemData.currentMagazine);
            if (playerComponent.currentActiveIndex >= 2 && playerComponent.items[playerComponent.currentActiveIndex-2] == itemComponent)
                tempStringBuilder.Append(" (선택 중)");
            tempStringBuilder.Append("\n");   
        }
            itemListText.text = tempStringBuilder.ToString(); // 아이템 정보 출력
    }

	private void PrintMissionText()
    {
        tempStringBuilder.Clear();
        foreach (MissionComponent missionComponent in playerComponent.missions)
        {
            MissionData missionData = missionComponent.LoadData();
            tempStringBuilder.AppendFormat("{0}", tempDB.GetMissionNameByType(missionData.missionType));
            if (missionData.progression >= missionData.maxProgression)
                tempStringBuilder.Append(" (완료)");
            tempStringBuilder.Append("\n");   
        }
        missionText.text = tempStringBuilder.ToString(); // 미션 정보 출력        
    }

	private void PrintInteractionText()
    {
        tempStringBuilder.Clear();
        interactionText.text = "";

        if (playerComponent.currentInteraction != null)
        {
			if (playerComponent.currentInteraction.interactionType == InteractionData.INTERACTION_TYPE.MISSION)
            {
				if (playerComponent.IsMissionTypeInProgress(playerComponent.currentInteraction.subType))
				{
					tempStringBuilder.AppendFormat("sys.temp.interaction.mission{0}", playerComponent.currentInteraction.subType);
					interactionText.text = tempDB.GetStringByKey(tempStringBuilder.ToString()); // 인터랙션 정보 출력
				}
			}
		}
    }

	public override void OnUpdate(int deltaFrameCount, float deltaTime)
    {
        base.OnUpdate(deltaFrameCount, deltaTime);

		PrintCurrentActiveText();
		PrintWeaponListText();
		PrintItemListText();
		PrintMissionText();
		PrintInteractionText();
	}
}
