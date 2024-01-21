using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchBoardCell : MonoBehaviour
{
	private Guid matchId;

	[SerializeField] private Image image;
	[SerializeField] private Toggle toggleButton;
	[SerializeField] private Text matchName;
	[SerializeField] private Text playerCount;

	public event Action<Guid> onClickMatch = null;

	public void OnToggleClicked()
	{
		onClickMatch?.Invoke(toggleButton.isOn ? matchId : Guid.Empty);
		image.color = toggleButton.isOn ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 1f, 1f, 0.2f);
	}

	public Guid GetMatchId() => matchId;

	public void SetMatchInfo(MatchInfo infos)
	{
		matchId = infos.matchId;
		matchName.text = $"Match {infos.matchId.ToString().Substring(0, 8)}";
		playerCount.text = $"{infos.players} / {infos.maxPlayers}";
	}
}
