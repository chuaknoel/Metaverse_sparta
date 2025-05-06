using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeUI : BaseUI
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private string mainSceneName = "MainScene";  // 메인 씬 이름 추가

    public override void Init(UIManager uiManager)
    {
        base.Init(uiManager);
        startButton.onClick.AddListener(onClickStartButton);
        exitButton.onClick.AddListener(onClickExitButton);
    }

    public void onClickStartButton()
    {
        GameManager.instance.StartGame();
    }

    public void onClickExitButton()
    {
        // 게임 종료 대신 메인 씬으로 전환
        SceneManager.LoadScene(mainSceneName);

        // 원래 코드는 아래와 같았습니다:
        // Application.Quit();
    }


    protected override UIState GetUIstate()
    {
        return UIState.Home;
    }

}
