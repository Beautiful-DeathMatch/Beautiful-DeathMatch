using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleMainWindow : UIMainWindow
{
    [SerializeField] private ItemSystem itemSystem;
    [SerializeField] private MissionSystem missionSystem;
    [SerializeField] private StatusSystem statusSystem;
    private PlayerComponent myPlayerComponent = null;
	private PlayerItemComponent myPlayerItemComponent = null;
	private PlayerAttackComponent myPlayerAttackComponent = null;
	private PlayerMissionComponent myPlayerMissionComponent = null;
	private PlayerInteractionComponent myPlayerInteractionComponent = null;
	

    [SerializeField] private Image[] itemImages = null;
    [SerializeField] private TextMeshProUGUI[] currentItemTexts = null;
    [SerializeField] private TextMeshProUGUI aimText = null;
    [SerializeField] private TextMeshProUGUI interactionTimeText = null;
    [SerializeField] private TextMeshProUGUI healthText = null;
    [SerializeField] private TextMeshProUGUI stateText = null;
    [SerializeField] private TextMeshProUGUI missionText = null;
    [SerializeField] private TextMeshProUGUI[] debug_missionText = null;

    [SerializeField] private ItemTable itemTable;
    [SerializeField] private MissionTable missionTable;
    [SerializeField] private StringTable stringTable;

    private int myPlayerId = -1;
    private List<int> playerIdList = new(); // debug

    private readonly StringBuilder stringBuilder = new();
    private float currentInteractionTime;
    private float maxInteractionTime;
    private float remainedInteractionTime;
    
    public override void OnEnter(SceneModuleParam param)
    {
		if (param is BattleSceneModule.Param battleParam)
        {
            myPlayerId = battleParam.myPlayerId;
            foreach (var playerInfo in battleParam.playerInfoList) // debug
            {
                playerIdList.Add(playerInfo.playerId);
            }

            foreach(var player in FindObjectsOfType<PlayerComponent>())
            {
                if (player.playerId == myPlayerId)
                {
                    myPlayerComponent = player;
                    myPlayerItemComponent = player.transform.GetComponent<PlayerItemComponent>();
                    myPlayerAttackComponent = player.transform.GetComponent<PlayerAttackComponent>();
                    myPlayerMissionComponent = player.transform.GetComponent<PlayerMissionComponent>();
                    myPlayerInteractionComponent = player.transform.GetComponent<PlayerInteractionComponent>();
                    break;
                }
            }
        }
	}

    public void PrintPlayerItemSlot()
    {
        var playerItemSprites = itemSystem.GetPlayerItemSprites(myPlayerId).ToArray();

        if (itemImages.Length != playerItemSprites.Length)
        {
            Debug.LogError("실제 아이템 슬롯과 UI에 표기되는 슬롯의 수가 다릅니다.");
            return;
        }

        for (int i = 0; i < playerItemSprites.Length; i++)
        {
            if(playerItemSprites[i] == null)
            {
                itemImages[i].sprite = null;
                itemImages[i].color = new Color(0, 0, 0, 0);
            }
            else
            {
                itemImages[i].sprite = playerItemSprites[i];
                itemImages[i].color = new Color(0, 0, 0, 0.5f);
            }
        }
    }

    public void PrintCurrentItemData()
    {
        itemImages[myPlayerItemComponent.currentItemSlotIndex].color = new Color(0, 0, 0, 1f);
        var data = itemSystem.GetPlayerItemData(myPlayerId, myPlayerItemComponent.currentItemSlotIndex);
        currentItemTexts[0].text = stringTable.GetStringByKey(data.tableData.nameKey);
        currentItemTexts[1].text = data.currentUsableCount.ToString();
        currentItemTexts[2].text = data.tableData.maxUsableCount.ToString();
    }

    public void PrintAimText()
    {
        var interactionObject = myPlayerInteractionComponent.GetCurrentInteractableObject();
        stringBuilder.Clear();

        if (interactionObject != null)
        {
            if (interactionObject is FieldItemComponent fieldItem)
            {
                DynamicItemData itemData = itemSystem.GetItemData(fieldItem.GetItemId());
                string itemName = stringTable.GetStringByKey(itemData.tableData.nameKey);
                stringBuilder.AppendFormat(stringTable.GetStringByKey("sys.hud.interaction.getItem"), itemName, itemData.currentUsableCount);
            }
            else if (interactionObject is BoxComponent box)
            {
                stringBuilder.AppendFormat(stringTable.GetStringByKey("sys.hud.interaction.openBox"));
            }
            else if (interactionObject is MissionComponent mission)
            {
                MissionTable.MissionData missionData = missionTable.GetMissionDataByType(mission.missionType);
                string missionName = stringTable.GetStringByKey(missionData.nameKey);
                if(missionSystem.IsMissionCompleted(myPlayerId,mission.missionType))
                    stringBuilder.AppendFormat(stringTable.GetStringByKey("sys.hud.interaction.completedMission"), missionName);
                else
                    stringBuilder.AppendFormat(stringTable.GetStringByKey("sys.hud.interaction.mission"), missionName);
            }
        }
        aimText.text = stringBuilder.ToString();
    }

    public void PrintPlayerInteractionTime()
    {
        currentInteractionTime = myPlayerInteractionComponent.currentInteractionTime;
        maxInteractionTime = myPlayerInteractionComponent.objectMaxInteractionTime;
        if (currentInteractionTime == 0f)
            interactionTimeText.text = "";
        else
        {
            remainedInteractionTime = maxInteractionTime - currentInteractionTime;
            interactionTimeText.text = $"{(int)remainedInteractionTime}.{(int)(remainedInteractionTime * 10) % 10}";
        }
    }

    public void PrintStatus()
    {
        healthText.text = statusSystem.GetHealth(myPlayerId).ToString();
        stateText.text = statusSystem.GetState(myPlayerId).ToString();
    }

    public void PrintMission(int? index = null)
    {
        stringBuilder.Clear();
        PlayerMissionSlot playerMissionSlot = null;
        if(index == null)
            playerMissionSlot = missionSystem.GetPlayerMissionSlot(myPlayerId);
        else
            playerMissionSlot = missionSystem.GetPlayerMissionSlot(playerIdList[index.Value]);

        if (playerMissionSlot == null)
            Debug.Log("NULL???");

        foreach(int id in playerMissionSlot.missionIds)
        {
            DynamicMissionData mission = missionSystem.GetMissionData(id);
            string missionName = stringTable.GetStringByKey(mission.tableData.nameKey);
            stringBuilder.Append(missionName);
            if(mission.currentProgression == mission.tableData.maxProgression)
                stringBuilder.Append("(완료)");
            stringBuilder.Append("\n");
        }

        if (index == null)
            missionText.text = stringBuilder.ToString();
        else
        {
            stringBuilder.Append(playerIdList[index.Value].ToString());
            debug_missionText[index.Value].text = stringBuilder.ToString();
        }
    }

	public override void OnUpdate(int deltaFrameCount, float deltaTime)
    {
        base.OnUpdate(deltaFrameCount, deltaTime);

        PrintPlayerItemSlot();
        PrintCurrentItemData();
        PrintAimText();
        PrintPlayerInteractionTime();
        PrintStatus();
        PrintMission();
        for (int i = 0; i < playerIdList.Count; i++)
        {
            PrintMission(i);
        }
    }
}
