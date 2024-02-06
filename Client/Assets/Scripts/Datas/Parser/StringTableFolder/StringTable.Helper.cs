using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public enum LANGUAGE
{
    KR,
    EN
}

public partial class StringTable
{
    public LANGUAGE language { get; private set; } = LANGUAGE.KR;

    public void ChangeLanguage(LANGUAGE newLanguage)
    {
        language = newLanguage;
    }
    
    public string GetStringByKey(string key)
	{
		if (stringKeyDictionary.TryGetValue(key, out var data))
		{
            switch (language)
            {
                case LANGUAGE.KR: return data.KR;
                case LANGUAGE.EN: return data.EN;

                default: return data.KR;
            }
		}
		else
			return "GetStringError";
	}
    

}
