using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class WeaponSystem : MonoSystem
{

    // 무기 List
    List<WeaponComponent> weapons = new List<WeaponComponent>();

    // 마지막으로 생성된 무기의 고유 ID
    static int weaponIDUnique = 0;


    // =================== Data 변경 =================== //

    // 무기 소유자 변경
    public void Acquire(int ID, int ownerID)
    {
        weapons[ID].Acquire(ownerID);
    }
    
    // 무기 버리기 (기획 확인 필요)
    // Throw(Instance ID)

    // 탄약 소모
    public void Shot(int ID, int shotNum = 1)
    {
        FindByID(ID).Shot(shotNum);
    }

    // 탄약 재장전
    public void ReLoad(int ID)
    {
        FindByID(ID).ReLoad();
    }

    // 무기 삭제 (리스트에서 삭제)
    public void Delete(int ID) // ID로 삭제
    {
        int findedIndex = FindListIndexByID(ID);
        if (findedIndex >= 0 && findedIndex < weapons.Count)
        {
            weapons[findedIndex].DeleteBySystem();
            weapons.RemoveAt(findedIndex);
        }
        else
            Debug.Log("삭제 실패: ID가 올바르지 않습니다.");
    }
        
    public void Delete(WeaponComponent weapon) // weapon 정보로 삭제
    {
        if (weapons.Remove(weapon) == false)
        {
            Debug.Log("삭제 실패: 해당 Component가 List에 없습니다.");
            return;
        }
        weapon.DeleteBySystem();
    }


    // =================== 써칭/고유ID 관련/기타 =================== //

    // ID에 해당하는 무기의 리스트 번호 찾기
    public int FindListIndexByID(int ID)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].returnID() == ID)
                return i;
        }
        Debug.Log(ID + " ID 에 해당하는 Weapon 없음");
        return -1;
    }

    // ID에 해당하는 무기 찾기
    public WeaponComponent FindByID(int ID)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].returnID() == ID)
                return weapons[i];
        }
        Debug.Log(ID + " ID 에 해당하는 Weapon 없음");
        return null;
    }

    // Component 고유 ID 생성 및 반환
    int CreateID()
    {
        weaponIDUnique++;
        return weaponIDUnique;
    }

    // 무기 컴포넌트에 고유 ID 부여 및 리스트업
    public void ListUp(WeaponComponent weapon){
        weapon.SetID(CreateID());
        weapons.Add(weapon);
    }

    // Component가 System에 Request를 보낼 때 사용하는 함수
    public void Request(string content){
        // server로 전달하는 내용?

    }

}
