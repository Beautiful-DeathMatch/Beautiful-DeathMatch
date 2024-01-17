using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TempDB : MonoBehaviour
{

    public static Dictionary<int, string> tempWeaponDB = new Dictionary<int, string>()
    {
        {1,"sys.temp.weapon1"},
        {2,"sys.temp.weapon2"}
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

    public string GetWeaponNameByType(int type)
    {
        return GetStringByKey(tempWeaponDB[type]);
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
