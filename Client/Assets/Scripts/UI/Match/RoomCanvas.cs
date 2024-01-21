using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class RoomCanvas : MonoBehaviour
{
	public GameObject playerList;
	public GameObject playerPrefab;
	public GameObject cancelButton;
	public GameObject leaveButton;
	public Button startButton;

	public bool owner;

	public void RefreshRoomPlayers(PlayerInfo[] playerInfos)
	{
		foreach (Transform child in playerList.transform)
			Destroy(child.gameObject);

		startButton.interactable = false;
		bool everyoneReady = true;

		foreach (PlayerInfo playerInfo in playerInfos)
		{
			GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
			newPlayer.transform.SetParent(playerList.transform, false);
			newPlayer.GetComponent<RoomPlayerCell>().SetPlayerInfo(playerInfo);

			if (!playerInfo.ready)
				everyoneReady = false;
		}

		startButton.interactable = everyoneReady && owner && (playerInfos.Length > 1);
	}

	public void SetOwner(bool owner)
	{
		this.owner = owner;

		cancelButton.SetActive(owner);
		leaveButton.SetActive(!owner);
	}
}