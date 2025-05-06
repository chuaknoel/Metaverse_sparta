using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// MainScene���� �ְ� ���̺� ������ ǥ���ϴ� UI Ŭ����
/// </summary>
public class HighWaveUI : MonoBehaviour
{
    // UI ���
    [SerializeField] private TextMeshProUGUI highWaveText;

    // �ؽ�Ʈ ����
    [SerializeField] private string textFormat = "BestWave: {0:00}";

    private void Start()
    {
        // UI ��� Ȯ��
        if (highWaveText == null)
        {
            Debug.LogError("TextMeshProUGUI ������Ʈ�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        // WaveDataManager ã�ų� ����
        WaveDataManager waveManager = FindOrCreateWaveManager();

        // �ְ� ���̺� ���� ǥ��
        UpdateHighWaveText(waveManager.GetHighestWave());
    }

    /// <summary>
    /// WaveDataManager�� ã�ų� ������ �����մϴ�.
    /// </summary>
    private WaveDataManager FindOrCreateWaveManager()
    {
        // ���� WaveDataManager ã��
        WaveDataManager waveManager = FindObjectOfType<WaveDataManager>();

        // ������ ���� ����
        if (waveManager == null)
        {
            GameObject managerObj = new GameObject("WaveDataManager");
            waveManager = managerObj.AddComponent<WaveDataManager>();
        }

        return waveManager;
    }

    /// <summary>
    /// �ְ� ���̺� �ؽ�Ʈ�� ������Ʈ�մϴ�.
    /// </summary>
    /// <param name="waveCount">ǥ���� ���̺� ��</param>
    public void UpdateHighWaveText(int waveCount)
    {
        // ������ �������� �ؽ�Ʈ ����
        highWaveText.text = string.Format(textFormat, waveCount);
    }

    /// <summary>
    /// Ư�� ���̺�� UI�� ������Ʈ�մϴ� (�ܺο��� ȣ�� ����)
    /// </summary>
    /// <param name="waveCount">ǥ���� ���̺� ��</param>
    public void SetHighWave(int waveCount)
    {
        WaveDataManager waveManager = FindOrCreateWaveManager();

        // ���̺� ���� ������Ʈ �� ����
        waveManager.UpdateHighestWave(waveCount);

        // UI ������Ʈ
        UpdateHighWaveText(waveManager.GetHighestWave());
    }
}