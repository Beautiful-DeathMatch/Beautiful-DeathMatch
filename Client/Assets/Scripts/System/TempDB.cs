using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WeaponDBData
{
    public WeaponDBData(WeaponData.WEAPON_TYPE _weaponType,int _maxMagazine, int _damage, string _nameKey)
    {
        weaponType = _weaponType;
        maxMagazine = _maxMagazine;
        damage = _damage;
        nameKey = _nameKey;
    }
    public WeaponData.WEAPON_TYPE weaponType = WeaponData.WEAPON_TYPE.NONE;          // 무기 타입 (칼/총 등)
    public int maxMagazine = 0;         // 최대 장전 가능 탄창
    public int damage = 0;              // 기본 공격력
    public string nameKey;              // 무기 이름 스트링키
}

public class TempDB : MonoBehaviour
{

    public static Dictionary<int, WeaponDBData> tempWeaponDB = new Dictionary<int, WeaponDBData>()
    {
        {1, new WeaponDBData(WeaponData.WEAPON_TYPE.KNIFE, 1, 50, "sys.temp.weapon1")},
        {2, new WeaponDBData(WeaponData.WEAPON_TYPE.GUN, 5, 100, "sys.temp.weapon2")}
    };
    public static Dictionary<int, string> tempMissionDB = new Dictionary<int, string>()
    {
        {1,"sys.temp.mission1"},
        {2,"sys.temp.mission2"}
    };

    public static Dictionary<string, string> tempStringDB = new Dictionary<string, string>()
    {
        {"sys.temp.weapon1","칼"},
        {"sys.temp.weapon2","권총"},

        {"sys.temp.mission1","미션 1 이름입니다."},
        {"sys.temp.mission2","미션 2 이름입니다."},

        {"sys.temp.interaction.mission1","[F] 를 눌러 미션1 완료"},
        {"sys.temp.interaction.mission2","[F] 를 눌러 미션2 완료"}
    };

    public WeaponDBData GetWeaponDB(int index)
    {
        return tempWeaponDB[index];
    }

    public string GetWeaponNameByIndex(int index)
    {
        return GetStringByKey(tempWeaponDB[index].nameKey);
    }

    public string GetMissionNameByType(int type)
    {
        return GetStringByKey(tempMissionDB[type]);
    }

    public string GetStringByKey(string key)
    {
        return tempStringDB[key];
    }

}
