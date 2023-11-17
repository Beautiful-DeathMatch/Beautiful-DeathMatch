using UnityEngine;

public class GameScene : MonoBehaviour
{
    private void Start()
    {
        GameManager.UI.ShowSceneUI<UI_GameScene>();
    }
}
