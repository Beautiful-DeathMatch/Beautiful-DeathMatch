using System;
using System.Collections.Generic;


/// <summary>
/// !주의! 수동으로 조작하지 마시오. .Helper.cs에 편의성 함수를 추가하시오.
/// </summary>
[Serializable]
[ScriptParserAttribute("StringTable.asset")]
public partial class StringTable : ScriptParser
{
	public override void Parser()
	{
		stringKeyDictionary.Clear();
		foreach(var value in stringKeyList)
		{
			stringKeyDictionary.Add(value.key, value);
		}
	}

	[Serializable]
	public class StringKey
	{
		public string key;
		public string KR;
		public string EN;
	}

	public List<StringKey> stringKeyList = new List<StringKey>();
	[System.Serializable]
	public class StringKeyDictionary : SerializableDictionary<string, StringKey> {}
	public StringKeyDictionary stringKeyDictionary = new StringKeyDictionary();


}
