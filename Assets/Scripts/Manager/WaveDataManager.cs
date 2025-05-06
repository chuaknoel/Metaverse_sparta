using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �̴ϰ��� ���̺� ������ �����ϰ� �ε��ϴ� �Ŵ��� Ŭ����
/// </summary>
public class WaveDataManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static WaveDataManager Instance { get; private set; }

    // PlayerPrefs Ű �̸�
    private const string HIGH_WAVE_KEY = "HighestWave";

    // ��������� �ְ� ���̺�
    private int highestWave = 0;

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ����� �ְ� ���̺� �ε�
            LoadHighestWave();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ����� �ְ� ���̺긦 �ε��մϴ�.
    /// </summary>
    private void LoadHighestWave()
    {
        // PlayerPrefs���� ����� �� �ε�
        highestWave = PlayerPrefs.GetInt(HIGH_WAVE_KEY, 0);
    }

    /// <summary>
    /// ���� ���̺갡 �ְ� ��Ϻ��� ������ ������Ʈ�մϴ�.
    /// </summary>
    /// <param name="currentWave">���� ���̺�</param>
    /// <returns>�ְ� ����� ���ŵǾ����� ����</returns>
    public bool UpdateHighestWave(int currentWave)
    {
        if (currentWave > highestWave)
        {
            highestWave = currentWave;

            // PlayerPrefs�� ����
            PlayerPrefs.SetInt(HIGH_WAVE_KEY, highestWave);
            PlayerPrefs.Save();

            return true;  // �ְ� ��� ���ŵ�
        }

        return false;  // �ְ� ��� ���ŵ��� ����
    }

    /// <summary>
    /// ���� �ְ� ���̺� ���� ��ȯ�մϴ�.
    /// </summary>
    /// <returns>�ְ� ���̺� ��</returns>
    public int GetHighestWave()
    {
        return highestWave;
    }
}