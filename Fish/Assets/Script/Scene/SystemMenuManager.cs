using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemMenuManager : MonoBehaviour
{
    [Header("【共通】オプションパネル設定")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject gamePlayPanel;
    [SerializeField] private GameObject graphicPanel;
    [SerializeField] private GameObject languagePanel;

    [Header("【タイトル用】UI設定")]
    [SerializeField] private GameObject titleText;
    [SerializeField] private GameObject buttonGroup; // Start, Option, Exit 等

    [Header("【メインゲーム用】ポーズ設定")]
    [SerializeField] private bool canPause = false; // ★チェックを入れるとEscキーでポーズ可能になる
    [SerializeField] private GameObject pauseBackgroundPanel; // 暗転用
    [SerializeField] private GameObject pauseMenuUI; // ポーズのメインメニュー
    [SerializeField] private GameObject quitConfirmMenuUI; // 終了確認（ワンクッション）

    private bool isPaused = false;

    private void Start()
    {
        if (canPause)
        {
            // メインシーンの場合は、開始時にポーズを解除してUIを隠す
            ResumeGame();
        }
        else
        {
            // タイトルシーンの場合は、オプション画面の中身を初期化
            HideAllOptionPanels();
            if (optionsPanel != null) optionsPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // canPauseがTrue（メインゲーム中）のみ、Escキーでポーズ切り替え
        if (canPause && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    // ==========================================
    // 共通：オプション開閉・タブ切り替え処理
    // ==========================================
    public void OpenOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(true);
        HideAllOptionPanels();

        // タイトル画面のUIがあれば隠す
        if (titleText != null) titleText.SetActive(false);
        if (buttonGroup != null) buttonGroup.SetActive(false);

        // メイン画面のポーズUIがあれば隠す
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
    }

    public void CloseOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);

        // タイトル画面のUIがあれば再表示
        if (titleText != null) titleText.SetActive(true);
        if (buttonGroup != null) buttonGroup.SetActive(true);

        // メイン画面のポーズUIがあれば再表示
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
    }

    public void OnClickAudioTab() { HideAllOptionPanels(); if (audioPanel != null) audioPanel.SetActive(true); }
    public void OnClickGamePlayTab() { HideAllOptionPanels(); if (gamePlayPanel != null) gamePlayPanel.SetActive(true); }
    public void OnClickGraphicTab() { HideAllOptionPanels(); if (graphicPanel != null) graphicPanel.SetActive(true); }
    public void OnClickLanguageTab() { HideAllOptionPanels(); if (languagePanel != null) languagePanel.SetActive(true); }

    private void HideAllOptionPanels()
    {
        if (audioPanel != null) audioPanel.SetActive(false);
        if (gamePlayPanel != null) gamePlayPanel.SetActive(false);
        if (graphicPanel != null) graphicPanel.SetActive(false);
        if (languagePanel != null) languagePanel.SetActive(false);
    }

    // ==========================================
    // メインゲーム用：ポーズ処理
    // ==========================================
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // ゲーム時間を止める

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (pauseBackgroundPanel != null) pauseBackgroundPanel.SetActive(true);

        // オプションや終了確認が開いていたらリセットして、メインメニューを出す
        if (quitConfirmMenuUI != null) quitConfirmMenuUI.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // ゲーム時間を動かす

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (pauseBackgroundPanel != null) pauseBackgroundPanel.SetActive(false);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (quitConfirmMenuUI != null) quitConfirmMenuUI.SetActive(false);
    }

    public void OpenQuitConfirm()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (quitConfirmMenuUI != null) quitConfirmMenuUI.SetActive(true);
    }

    public void CancelQuit()
    {
        if (quitConfirmMenuUI != null) quitConfirmMenuUI.SetActive(false);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
    }

    // ==========================================
    // 共通：シーン遷移・終了処理
    // ==========================================
    public void ReturnToTitle()
    {
        Time.timeScale = 1f; // 時間を戻してから遷移する
        SceneManager.LoadScene("Title");
    }

    public void QuitGame()
    {
        Debug.Log("ゲームを終了します");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}