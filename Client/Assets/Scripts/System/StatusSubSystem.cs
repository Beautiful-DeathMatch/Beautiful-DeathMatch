using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusSubSystem : MonoSubSystem
{

    // Status 리스트
    [SerializeField]
    List<StatusComponent> statuses = new List<StatusComponent>();

    // 마지막으로 생성된 고유 ID
    static int statusIDUnique = 0;

    // =================== Data 변경 =================== //

    // Status 소유자 변경
    public void Acquire(int ID, int ownerID, bool bySystem = false)
    {
        FindByID(ID).Acquire(ownerID); 
    }

    // 피격
    public void Hit(int ID, int amount)
    {
        if (FindByID(ID).Hit(amount) <=0)
            FindByID(ID).Dead(); 
    }

    // 힐
    public void Heal(int ID, int amount)
    {
        FindByID(ID).Heal(amount); 
    }

    // 게임 준비
    public void GameReady()
    {
        foreach (StatusComponent status in statuses)
        {
            status.GameReady();
        }
    }

    //게임 시작
    public void GameStart()
    {
        foreach (StatusComponent status in statuses)
        {
            status.GameStart();
        }
    }

    // =================== 써칭/고유ID 관련/기타 =================== //

    // ID에 해당하는 리스트 Index 찾기
    public int FindListIndexByID(int ID)
    {
        for (int i = 0; i < statuses.Count; i++)
        {
            if (statuses[i].returnID() == ID)
                return i;
        }
        Debug.Log(ID + " ID 에 해당하는 status 없음");
        return -1;
    }

    // ID에 해당하는 Component 찾기
    public StatusComponent FindByID(int ID)
    {
        for (int i = 0; i < statuses.Count; i++)
        {
            if (statuses[i].returnID() == ID)
                return statuses[i];
        }
        Debug.Log(ID + " ID 에 해당하는 status 없음");
        return null;
    }

    // Component 고유 ID 생성 및 반환
    int CreateID()
    {
        statusIDUnique++;
        return statusIDUnique;
    }

    // Component에 고유 ID 부여 및 리스트업
    public void ListUp(StatusComponent status){
        status.SetID(CreateID());
        statuses.Add(status);
        Debug.Log(statusIDUnique + " index 로 리스트업 완료");
    }

    // Component가 System에 Request를 보낼 때 사용하는 함수
    public void Request(string content){
        // server로 전달하는 내용?

    }    

}
