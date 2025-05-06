using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// MainScene에서 최고 웨이브 정보를 표시하는 UI 클래스
/// </summary>
public class HighWaveUI : MonoBehaviour
{
    // UI 요소
    [SerializeField] private TextMeshProUGUI highWaveText;

    // 텍스트 형식
    [SerializeField] private string textFormat = "BestWave: {0:00}";

    private void Start()
    {
        // UI 요소 확인
        if (highWaveText == null)
        {
            Debug.LogError("TextMeshProUGUI 컴포넌트가 할당되지 않았습니다!");
            return;
        }

        // WaveDataManager 찾거나 생성
        WaveDataManager waveManager = FindOrCreateWaveManager();

        // 최고 웨이브 정보 표시
        UpdateHighWaveText(waveManager.GetHighestWave());
    }

    /// <summary>
    /// WaveDataManager를 찾거나 없으면 생성합니다.
    /// </summary>
    private WaveDataManager FindOrCreateWaveManager()
    {
        // 기존 WaveDataManager 찾기
        WaveDataManager waveManager = FindObjectOfType<WaveDataManager>();

        // 없으면 새로 생성
        if (waveManager == null)
        {
            GameObject managerObj = new GameObject("WaveDataManager");
            waveManager = managerObj.AddComponent<WaveDataManager>();
        }

        return waveManager;
    }

    /// <summary>
    /// 최고 웨이브 텍스트를 업데이트합니다.
    /// </summary>
    /// <param name="waveCount">표시할 웨이브 수</param>
    public void UpdateHighWaveText(int waveCount)
    {
        // 지정된 형식으로 텍스트 설정
        highWaveText.text = string.Format(textFormat, waveCount);
    }

    /// <summary>
    /// 특정 웨이브로 UI를 업데이트합니다 (외부에서 호출 가능)
    /// </summary>
    /// <param name="waveCount">표시할 웨이브 수</param>
    public void SetHighWave(int waveCount)
    {
        WaveDataManager waveManager = FindOrCreateWaveManager();

        // 웨이브 정보 업데이트 및 저장
        waveManager.UpdateHighestWave(waveCount);

        // UI 업데이트
        UpdateHighWaveText(waveManager.GetHighestWave());
    }
}