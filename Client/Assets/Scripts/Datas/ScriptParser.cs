using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptParserAttribute : ResourceAttribute
{
	public ScriptParserAttribute(string path) : base("Datas/Parser/" + path, ResourceType.UnityAsset)
	{

	}
}

public class ScriptAssetInfo
{
	public Type AssetType { get; set; }
	public ScriptParserAttribute Attribute { get; set; }
}

public abstract class ScriptParser : ScriptableObject
{
	// 기본 Parser, List, Dictionary를 만들어 줌
	public abstract void Parser();
	public virtual void RuntimeParser() { }
}