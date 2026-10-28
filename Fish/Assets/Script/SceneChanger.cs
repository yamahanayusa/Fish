using UnityEngine;
using UnityEngine.SceneManagement; // シーン遷移に必須

public class SceneChanger : MonoBehaviour
{
    /// <summary>
    /// 指定した名前のシーンをロードする（ボタンから呼び出す用）
    /// </summary>
    public void LoadTargetScene(string sceneName)
    {
        Debug.Log($"【シーン遷移】{sceneName} をロードします");
        SceneManager.LoadScene(sceneName);
    }
}