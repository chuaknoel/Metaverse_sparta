using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ��ü�� �����ϴ� ���� �Ŵ��� Ŭ����
// ���� GameManager.cs�� �ְ� ���̺� ���� ��� �߰�
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerController player { get; private set; } // �÷��̾� ��Ʈ�ѷ� (�б� ���� ������Ƽ)
    private ResourceController _playerResourceController;

    [SerializeField] private int currentWaveIndex = 0; // ���� ���̺� ��ȣ

    private EnemyManager enemyManager; // �� ���� �� �����ϴ� �Ŵ���

    private UIManager uiManager; // UI �Ŵ���

    public static bool isFirstLOading = true; // ù �ε� ����

    // GameManager.cs�� Awake() �޼��� ����
    private void Awake()
    {
        // �̱��� �Ҵ�
        instance = this;

        // �÷��̾� ã�� �ʱ�ȭ
        player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.Init(this);
        }
        else
        {
            Debug.LogError("PlayerController�� ã�� �� �����ϴ�!");
        }

        uiManager = FindObjectOfType<UIManager>();

        // ��� 1: FindObjectOfType ��� (������ EnemyManager ã��)
        enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager != null)
        {
            enemyManager.Init(this);
        }
        else
        {
            Debug.LogError("EnemyManager�� ã�� �� �����ϴ�!");
        }


        _playerResourceController = player.GetComponent<ResourceController>(); // �÷��̾��� ResourceController ������Ʈ ��������
        _playerResourceController.RemoveHealthChangeEvent(uiManager.ChangePlayerHP); // �÷��̾� ü�� UI �ʱ�ȭ
        _playerResourceController.AddHealthCahngeEvent(uiManager.ChangePlayerHP); // �÷��̾� ü�� UI ������Ʈ �̺�Ʈ ���
    }

    private void Start()
    {
        if (isFirstLOading)
        {
            // StartGame() ȣ�� ����
            isFirstLOading = false; // ù �ε� ���� ����
        }

        // UIManager�� �ڵ����� HomeUI�� ǥ���� ���Դϴ�
        // ������ HomeUI���� ���� ��ư�� Ŭ���� ���� ���۵˴ϴ�
    }

    public void StartGame()
    {
        StartNextWave(); // ù ���̺� ����
        uiManager.SetPlayGame(); // ���� UI Ȱ��ȭ
    }

    void StartNextWave()
    {
        currentWaveIndex += 1; // ���̺� �ε��� ����
        // 5���̺긶�� ���̵� ���� (��: 1~4 �� ���� 1, 5~9 �� ���� 2 ...)
        enemyManager.StartWave(1 + currentWaveIndex / 5);

        uiManager.ChangeWave(currentWaveIndex); // UI�� ���̺� ��ȣ ������Ʈ
    }

    // ���̺� ���� �� ���� ���̺� ����
    public void EndOfWave()
    {
        StartNextWave();
    }

    // �÷��̾ �׾��� �� ���� ���� ó��
    public void GameOver()
    {
        // �ְ� ���̺� ��� ����
        SaveHighestWave();

        enemyManager.StopWave(); // �� ���� ����
        uiManager.SetGameOver(); // ���� ���� UI Ȱ��ȭ
    }

    // �ְ� ���̺� ���� �޼��� �߰�
    private void SaveHighestWave()
    {
        // WaveDataManager ã��
        WaveDataManager waveManager = FindObjectOfType<WaveDataManager>();

        // ������ ���� ����
        if (waveManager == null)
        {
            GameObject managerObj = new GameObject("WaveDataManager");
            waveManager = managerObj.AddComponent<WaveDataManager>();
        }

        // �ְ� ���̺� ������Ʈ
        waveManager.UpdateHighestWave(currentWaveIndex);
    }
}