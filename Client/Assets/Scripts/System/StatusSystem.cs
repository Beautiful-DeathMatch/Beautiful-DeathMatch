using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusData
{
    public StatusData(int _ownerID=0, int _maxHp=100, int _hp=100, STATE _state=STATE.UNKNOWN, PHASE _phase=PHASE.UNKNOWN)
    {
        ownerID = _ownerID;
        maxHp = _maxHp;    
        hp = _hp;          
        state = _state;    
        phase = _phase;    
    }
    public StatusData(StatusData SD)
    {
        ownerID = SD.ownerID;
        maxHp = SD.maxHp;    
        hp = SD.hp;          
        state = SD.state;    
        phase = SD.phase;    
    }

    public int ownerID;                     // 소유자 ID
    public int maxHp = 100;                 // 최대 체력
    public int hp = 100;                    // 현재 체력
    public STATE state = STATE.UNKNOWN;     // 상태 (생존/사망/부활대기 등)
    public PHASE phase = PHASE.UNKNOWN;     // 도달 구간 (미션완료/헬기탑승 등)

    public enum STATE // 캐릭터의 현재 상태 Enum
    {
        UNKNOWN,
        LIVE,
        DEAD,
        REVIVE,
        GOD
    }
    public enum PHASE // 캐릭터의 현재 진행도 Enum
    {
        UNKNOWN,
        READY,
        START,
        MISSION_COMPLETE,
        HELICOPTER,
        END
    }

}

public class StatusSystem : MonoSystem
{
    Dictionary<int, StatusData> components = new Dictionary<int, StatusData>();
}
