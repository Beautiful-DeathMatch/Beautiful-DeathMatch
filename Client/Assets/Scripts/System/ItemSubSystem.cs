using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData
{
    public ItemData(int _ownerID=0, int _itemIndex=0, int _currentMagazine=1)
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

public class ItemSubSystem : MonoSystem
{
    // 아이템 List
    Dictionary<int, ItemData> components = new Dictionary<int, ItemData>();


    // 마지막으로 생성된 아이템의 고유 ID, 1 부터 시작
    [SerializeField] // For Debug
    int componentIDUnique = 0;

    // // Item이 달릴 오브젝트 프리팹
    // [SerializeField]
    // ItemComponent itemPrefeb;

    [SerializeField]
    TempDB tempDB;

    // ====================== 공통 ==========================//

    // =================== 생성/삭제 =================== //

    // Component 고유 ID 생성 및 반환
    int CreateID()
    {
        componentIDUnique++;
        return componentIDUnique;
    }

    // Component가 자기 자신의 등록을 요청했을 경우
    public void Register(FieldItemComponent component, int ownerID, ItemData data = null)
    {
        if (data == null)
            TryCreateData(component, new ItemData(ownerID));
        else
            TryCreateData(component, data);
    }
    public void TryRegister(FieldItemComponent component, int ownerID, ItemData data = null)
    {
        Register(component, ownerID, data);
    }

    // Component Data 생성
    public void CreateData(FieldItemComponent component, ItemData data)
    {
        int newID = CreateID();
        components.Add(newID, new ItemData(data));
        component.SetID(newID);
    }
    public void TryCreateData(FieldItemComponent component, ItemData data)
    {
        CreateData(component, data);
    }

    // Component 제거
    public void Delete(int ID)
    {
        components.Remove(ID);
    }
    public void TryDelete(int ID)
    {
        Delete(ID);
    }

    // component 소유자 변경
    public void Acquire(int ID, int ownerID)
    {
        components[ID].ownerID = ownerID;
    }
    public void TryAcquire(int ID, int ownerID)
    {
        Acquire(ID, ownerID);
    }

    // =================== 외부에서의 Data 확인용 =================== //

    // 존재 여부 확인
    public bool IsContainsKey(int ID)
    {
        return components.ContainsKey(ID);
    }

    // Data 확인
    public ItemData LoadData(int ID)
    {
        if (components.ContainsKey(ID))
        {
            return new ItemData(components[ID]); // 보안을 위해?
        }
        else
        {
            Debug.Log("Data 조회 실패!");
            return null;
        }
    }

    // ownerID로 Data 리스트 찾기
    public List<ItemData> FindByOwnerID(int ownerID)
    {
        List<ItemData> returnComponents = new();
        foreach (ItemData component in components.Values)
        {
            if (component.ownerID == ownerID)
                returnComponents.Add(component);
        }
        return returnComponents;
    }

    // ====================== 공통 끝 ==========================//

    // =================== 기능 =================== //


    public void UseItem(int ID)
    {
        ItemData item = components[ID];
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
        ItemData item = components[ID];
        if (item.currentMagazine > 0)
            item.currentMagazine -= 1;
        TryUseItem(ID);
    }
    public void TryShot(int ID)
    {
        Shot(ID);
    }

}
