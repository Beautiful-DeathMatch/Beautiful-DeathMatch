using System;
using System.Collections.Generic;


/// <summary>
/// !주의! 수동으로 조작하지 마시오. .Helper.cs에 편의성 함수를 추가하시오.
/// </summary>
[Serializable]
[ScriptParserAttribute("MissionTable.asset")]
public partial class MissionTable : ScriptParser
{
	public override void Parser()
	{
		missionDataDictionary.Clear();
		foreach(var value in missionDataList)
		{
			missionDataDictionary.Add(value.key, value);
		}
	}

	[Serializable]
	public class MissionData
	{
		public ENUM_MISSION_TYPE key;
		public int maxProgression;
		public string nameKey;
		public string contentKey;
	}

	public List<MissionData> missionDataList = new List<MissionData>();
	[System.Serializable]
	public class MissionDataDictionary : SerializableDictionary<ENUM_MISSION_TYPE, MissionData> {}
	public MissionDataDictionary missionDataDictionary = new MissionDataDictionary();


}
