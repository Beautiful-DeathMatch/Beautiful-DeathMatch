using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayerCell : MonoBehaviour
{
	public Text playerName;

	public void SetPlayerInfo(PlayerInfo info)
	{
		playerName.text = $"Player {info.playerId}";
		playerName.color = info.ready ? Color.green : Color.red;
	}
}