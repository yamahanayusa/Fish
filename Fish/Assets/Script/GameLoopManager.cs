using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoopManager : MonoBehaviour
{
    private void Update()
    {
        // 仮のクリア条件：キーボードの「C」キーを押したらクリア画面へ
        if (Input.GetKeyDown(KeyCode.C))
        {
            GameClear();
        }
    }

    public void GameClear()
    {
        Debug.Log("【ゲームクリア】ClearSceneへ遷移します！");
        SceneManager.LoadScene("Clear"); // 作成したクリアシーン名に合わせる
    }
}