using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 시스템 내 Serializable한 객체들의 동기화를 담당하는 블랙보드
/// 우선 길어지더라도 클래스를 분리하지말고 모든 컬렉션을 몰아넣기로 합니다.
/// 
/// 이 컬렉션은 서버의 소유입니다.
/// 클라이언트는 이 객체를 소유할 수 없기 때문에,
/// Write 계열 함수엔 모두 Command 어트리뷰트를 달아야 합니다.
/// </summary>
public class BattleNetworkBlackBoard : NetworkBehaviour
{
	private BattleSceneModule.Param currentParam = null;

	// Sync들은 무조건 readonly 처리
	private readonly SyncList<int> testList = new SyncList<int>();
	private readonly SyncDictionary<int, string> testDictionary = new SyncDictionary<int, string>();

	public event Action<IList<int>> onChangedTestList = null;
	public event Action<IDictionary<int, string>> onChangedTestDictionary = null;	

	public void Initialize(BattleSceneModule.Param param)
	{
		currentParam = param;
	}

	/// <summary>
	/// 서버의 시작점, 이 곳에서 초기 값을 세팅합니다.
	/// </summary>
	public override void OnStartServer()
	{
		base.OnStartServer();

		testList.Add(0);
		testList.Add(1);
		testList.Add(2);

		testDictionary.Add(0, "솔휘");
		testDictionary.Add(1, "정훈");
		testDictionary.Add(2, "지수");
	}

	public override void OnStopServer()
	{
		base.OnStopServer();

		testList.Clear();
		testDictionary.Clear();
	}

	/// <summary>
	/// 클라이언트의 시작점, 이 곳에서 값이 변경될 때 받을 콜백을 등록합니다.
	/// </summary>
	public override void OnStartClient()
	{
		testList.Callback += OnUpdateTestList;
		testDictionary.Callback += OnUpdateTestDictionary;
	}

	public override void OnStopClient()
	{
		testList.Callback -= OnUpdateTestList;
		testDictionary.Callback -= OnUpdateTestDictionary;
	}

	private void OnUpdateTestList(SyncList<int>.Operation op, int index, int oldItem, int newItem)
	{
		switch (op)
		{
			case SyncList<int>.Operation.OP_ADD:
				Debug.Log($"[OP_ADD] {index} : {oldItem} -> {newItem}");
				break;
			case SyncList<int>.Operation.OP_INSERT:
				Debug.Log($"[OP_INSERT] {index} : {oldItem} -> {newItem}");
				break;
			case SyncList<int>.Operation.OP_REMOVEAT:
				Debug.Log($"[OP_REMOVEAT] {index}");
				break;
			case SyncList<int>.Operation.OP_SET:
				Debug.Log($"[OP_SET] {index} : {oldItem} -> {newItem}");
				break;
			case SyncList<int>.Operation.OP_CLEAR:
				Debug.Log($"[OP_CLEAR]");
				break;
		}

		// 리스트가 업데이트될 때, 시스템이 이벤트를 받도록 설계하자
		// 오퍼레이션 별로 처리할 필요없음
		onChangedTestList?.Invoke(testList);
	}

	private void OnUpdateTestDictionary(SyncDictionary<int, string>.Operation op, int key, string item)
	{
		switch (op)
		{
			case SyncIDictionary<int, string>.Operation.OP_ADD:
				Debug.Log($"[OP_ADD] {key} : {item}");
				break;
			case SyncIDictionary<int, string>.Operation.OP_SET:
				Debug.Log($"[OP_SET] {key} : {item}");
				break;
			case SyncIDictionary<int, string>.Operation.OP_REMOVE:
				Debug.Log($"[OP_REMOVE] {key} : {item}");
				break;
			case SyncIDictionary<int, string>.Operation.OP_CLEAR:
				Debug.Log($"[OP_CLEAR]");
				break;
		}

		// 딕셔너리가 업데이트될 때, 시스템이 이벤트를 받도록 설계하자
		// 오퍼레이션 별로 처리할 필요없음
		onChangedTestDictionary?.Invoke(testDictionary);
	}

	[Command(requiresAuthority = false)] // Sync Write 계열 함수엔 이걸 달아야 함
	public void AddTestList(int el)
	{
		testList.Add(el);
	}

	[Command(requiresAuthority = false)]
	public void AddTestDictionary(int el)
	{
		testDictionary.Add(el, "테스트 문장");
	}

	public int GetTestList(int index)
	{
		return testList[index];
	}

	public string GetTestDictionary(int key)
	{
		return testDictionary[key];
	}
}
