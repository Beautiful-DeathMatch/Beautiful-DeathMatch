using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData
{
    public ItemData(int _ownerID, int _itemIndex, int _currentMagazine)
    {
        ownerID = _ownerID;
        itemIndex = _itemIndex;
        currentMagazine = _currentMagazine;
    }
    public ItemData(ItemData itemData)
    {
        ownerID = itemData.ownerID;
        itemIndex = itemData.itemIndex;
        currentMagazine = itemData.currentMagazine;
    }
    public int ownerID = 0;                               // 소유자 ID
    public int itemIndex = 0;                             // 아이템 Index
    public int currentMagazine = 0;                       // 보유 갯수
}

public class ItemSubSystem : MonoSubSystem
{
    // 아이템 List
    Dictionary<int, ItemData> items = new Dictionary<int, ItemData>();


    // 마지막으로 생성된 아이템의 고유 ID, 1 부터 시작
    [SerializeField] // For Debug
    int itemIDUnique = 0;

    // Item이 달릴 오브젝트 프리팹
    [SerializeField]
    ItemComponent itemPrefeb;

    [SerializeField]
    TempDB tempDB;

    

    // =================== 생성/삭제 =================== //

    // Component 고유 ID 생성 및 반환
    int CreateID()
    {
        itemIDUnique++;
        return itemIDUnique;
    }

    // item 생성
    public void Create(int ownerID, int itemIndex)
    {
        int newID = CreateID();
        ItemDBData itemDB = tempDB.GetItemDB(itemIndex); // DB Load
        items.Add(newID, new ItemData(ownerID, itemIndex, itemDB.initialMagazine));
        // ItemComponent 생성하여 유저에게 할당
        Transform ownerTransform = FindObjectOfType<SpawnSystem>().GetPlayerComponent(ownerID).transform;
        ItemComponent item = Instantiate(itemPrefeb, ownerTransform);
        item.SetID(newID);
        item.AddToPlayerComponent(ownerTransform);
        
    }
    public void TryCreate(int ownerID, int itemIndex)
    {
        Create(ownerID, itemIndex);
    }

    // item 제거
    public void Delete(int ID)
    {
        items.Remove(ID);
    }
    public void TryDelete(int ID)
    {
        Delete(ID);
    }


    // =================== 외부에서의 Data 확인용 =================== //

    // 존재 여부 확인
    public bool IsContainsKey(int ID)
    {
        return items.ContainsKey(ID);
    }

    // Data 확인
    public ItemData LoadData(int ID)
    {
        if (items.ContainsKey(ID))
        {
            // return items[ID];
            return new ItemData(items[ID]); // 보안을 위해?
        }
        else
        {
            Debug.Log("Data 조회 실패!");
            return null;
        }
    }

    // =================== 기능 =================== //

    // 아이템 소유자 변경
    public void Acquire(int ID, int ownerID)
    {
        items[ID].ownerID = ownerID;
    }
    public void TryAcquire(int ID, int ownerID)
    {
        Acquire(ID, ownerID);
    }

    public void UseItem(int ID)
    {
        ItemData item = items[ID];
        if (item.itemIndex == 0)
        {
            // 아이템 0 의 기능
        }
        else if (item.itemIndex == 1)
        {
            // 아이템 1 의 기능
        }
        if (item.currentMagazine == 0)
            TryDelete(ID);
    }
    public void TryUseItem(int ID)
    {
        UseItem(ID);
    }

    // 아이템 소모
    public void Shot(int ID)
    {
        ItemData item = items[ID];
        if (item.currentMagazine > 0)
            item.currentMagazine -= 1;
        TryUseItem(ID);
    }
    public void TryShot(int ID)
    {
        Shot(ID);
    }

}
