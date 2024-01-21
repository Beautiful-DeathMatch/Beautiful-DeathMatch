
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MatchBoardCanvas : UIMainWindow, IMatchSessionSubscriber
{
	private Dictionary<Guid, MatchInfo> openMatches = new Dictionary<Guid, MatchInfo>();

	internal Guid localJoinedMatchId = Guid.Empty;
	internal Guid selectedMatchId = Guid.Empty;

	[SerializeField] private Transform matchBoardRoot;
	[SerializeField] private MatchBoardCell matchCellPrefab;

	[SerializeField] private Button createButton;
	[SerializeField] private Button joinButton;

	[SerializeField] private GameObject lobbyView;
	[SerializeField] private GameObject roomView;

	[SerializeField] private RoomCanvas roomGUI;
	[SerializeField] private ToggleGroup toggleGroup;

	private void InitializeData()
	{
		openMatches.Clear();
		localJoinedMatchId = Guid.Empty;
	}

	private void ResetCanvas()
	{
		InitializeData();

		lobbyView.SetActive(false);
		roomView.SetActive(false);
		gameObject.SetActive(false);
	}

	private void OnSelectedMatch(Guid matchId)
	{
		if (matchId == Guid.Empty)
		{
			selectedMatchId = Guid.Empty;
			joinButton.interactable = false;
		}
		else
		{
			if (!openMatches.Keys.Contains(matchId))
			{
				joinButton.interactable = false;
				return;
			}

			selectedMatchId = matchId;

			MatchInfo info = openMatches[matchId];
			joinButton.interactable = info.players < info.maxPlayers;
		}
	}

	public void RequestCreateMatch()
	{
		MatchSessionManager.Instance.RequestCreateMatch();
	}

	public void RequestJoinMatch()
	{
		MatchSessionManager.Instance.RequestJoinMatch(selectedMatchId);
	}

	public void RequestLeaveMatch()
	{
		MatchSessionManager.Instance.RequestLeaveMatch();
	}

	public void RequestCancelMatch()
	{
		MatchSessionManager.Instance.RequestCancelMatch();
	}

	public void RequestReadyChange()
	{
		MatchSessionManager.Instance.RequestReadyChange();
	}

	public void RequestStartMatch()
	{
		MatchSessionManager.Instance.RequestStartMatch();
	}

	public void OnStartClient()
	{
		InitializeData();
		ShowLobbyView();

		createButton.gameObject.SetActive(true);
		joinButton.gameObject.SetActive(true);
	}

	public void OnClientDisconnect()
	{
		InitializeData();
	}

	public void OnStopClient()
	{
		ResetCanvas();
	}

	private void ShowLobbyView()
	{
		lobbyView.SetActive(true);
		roomView.SetActive(false);

		foreach (Transform child in matchBoardRoot)
		{
			if (child.gameObject.GetComponent<MatchBoardCell>().GetMatchId() == selectedMatchId)
			{
				Toggle toggle = child.gameObject.GetComponent<Toggle>();
				toggle.isOn = true;
			}
		}	
	}

	private void ShowRoomView()
	{
		lobbyView.SetActive(false);
		roomView.SetActive(true);
	}

	private void RefreshMatchList(Dictionary<Guid, MatchInfo> openMatches)
	{
		foreach (Transform child in matchBoardRoot.transform)
		{
			Destroy(child.gameObject);
		}

		joinButton.interactable = false;

		foreach (MatchInfo matchInfo in openMatches.Values)
		{
			MatchBoardCell newMatch = Instantiate(matchCellPrefab, Vector3.zero, Quaternion.identity);
			
			newMatch.transform.SetParent(matchBoardRoot.transform, false);
			newMatch.SetMatchInfo(matchInfo);
			newMatch.onClickMatch += OnSelectedMatch;

			Toggle toggle = newMatch.GetComponent<Toggle>();
			toggle.group = toggleGroup;

			if (matchInfo.matchId == selectedMatchId)
			{
				toggle.isOn = true;
			}
		}
	}

	public void OnChangedMatchList(Dictionary<Guid, MatchInfo> openMatches)
	{
		this.openMatches = openMatches;
		RefreshMatchList(openMatches);
	}

	public void OnChangedMyMatch(bool isOwner, Guid matchId)
	{
		localJoinedMatchId = matchId;
		roomGUI.SetOwner(isOwner);
	}

	public void OnChangedPlayerInfos(PlayerInfo[] playerInfos)
	{
		roomGUI.RefreshRoomPlayers(playerInfos);
	}

	public void OnMatchStarted()
	{
		lobbyView.SetActive(false);
		roomView.SetActive(false);
	}

	public void OnClientMatchMessage(ClientMatchOperation operationType)
	{
		switch (operationType)
		{
			case ClientMatchOperation.Created:
			case ClientMatchOperation.Joined:
				ShowRoomView();
				break;

			case ClientMatchOperation.Cancelled:
			case ClientMatchOperation.Departed:
				ShowLobbyView();
				break;

		}
	}
}
