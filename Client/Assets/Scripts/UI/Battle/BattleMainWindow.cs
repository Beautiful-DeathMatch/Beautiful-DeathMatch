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

    [SerializeField] private ItemTable itemTable;

    private int myPlayerId = -1;

    private readonly StringBuilder stringBuilder = new();
    private float currentInteractionTime;
    private float maxInteractionTime;
    private float remainedInteractionTime;
    
    public override void OnEnter(SceneModuleParam param)
    {
		if (param is BattleSceneModule.Param battleParam)
        {
            myPlayerId = battleParam.myPlayerId;

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
        currentItemTexts[0].text = itemTable.LoadStringByKey(data.tableData.nameKey);
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
                stringBuilder.AppendFormat("F를 눌러 아이템 획득: {0} ({1})", itemTable.LoadStringByKey(itemData.tableData.nameKey), itemData.currentUsableCount);
            }
            else if (interactionObject is BoxComponent box)
            {
                stringBuilder.AppendFormat("F를 눌러 상자 열기");
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

	public override void OnUpdate(int deltaFrameCount, float deltaTime)
    {
        base.OnUpdate(deltaFrameCount, deltaTime);

        PrintPlayerItemSlot();
        PrintCurrentItemData();
        PrintAimText();
        PrintPlayerInteractionTime();
    }
}
