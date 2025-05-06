using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : BaseUI
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private string mainMapSceneName = "MainScene";  // 메인 맵 씬 이름을 MainScene으로 설정

    public override void Init(UIManager uiManager)
    {
        base.Init(uiManager);
        restartButton.onClick.AddListener(onClickRestartButton);
        exitButton.onClick.AddListener(OnClickExitButton);
    }

    public void onClickRestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnClickExitButton()
    {
        // 게임 종료 대신 메인 씬으로 돌아가기
        SceneManager.LoadScene(mainMapSceneName);
    }

    protected override UIState GetUIstate()
    {
        return UIState.GameOver;
    }
}
