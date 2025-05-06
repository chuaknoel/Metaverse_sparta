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
    [SerializeField] private string mainMapSceneName = "MainScene";  // ���� �� �� �̸��� MainScene���� ����

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
        // ���� ���� ��� ���� ������ ���ư���
        SceneManager.LoadScene(mainMapSceneName);
    }

    protected override UIState GetUIstate()
    {
        return UIState.GameOver;
    }
}
