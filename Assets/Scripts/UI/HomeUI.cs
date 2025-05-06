using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeUI : BaseUI
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private string mainSceneName = "MainScene";  // ���� �� �̸� �߰�

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
        // ���� ���� ��� ���� ������ ��ȯ
        SceneManager.LoadScene(mainSceneName);

        // ���� �ڵ�� �Ʒ��� ���ҽ��ϴ�:
        // Application.Quit();
    }


    protected override UIState GetUIstate()
    {
        return UIState.Home;
    }

}
