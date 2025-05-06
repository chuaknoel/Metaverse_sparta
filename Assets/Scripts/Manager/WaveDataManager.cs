using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 미니게임 웨이브 정보를 저장하고 로드하는 매니저 클래스
/// </summary>
public class WaveDataManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static WaveDataManager Instance { get; private set; }

    // PlayerPrefs 키 이름
    private const string HIGH_WAVE_KEY = "HighestWave";

    // 현재까지의 최고 웨이브
    private int highestWave = 0;

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 저장된 최고 웨이브 로드
            LoadHighestWave();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 저장된 최고 웨이브를 로드합니다.
    /// </summary>
    private void LoadHighestWave()
    {
        // PlayerPrefs에서 저장된 값 로드
        highestWave = PlayerPrefs.GetInt(HIGH_WAVE_KEY, 0);
    }

    /// <summary>
    /// 현재 웨이브가 최고 기록보다 높으면 업데이트합니다.
    /// </summary>
    /// <param name="currentWave">현재 웨이브</param>
    /// <returns>최고 기록이 갱신되었는지 여부</returns>
    public bool UpdateHighestWave(int currentWave)
    {
        if (currentWave > highestWave)
        {
            highestWave = currentWave;

            // PlayerPrefs에 저장
            PlayerPrefs.SetInt(HIGH_WAVE_KEY, highestWave);
            PlayerPrefs.Save();

            return true;  // 최고 기록 갱신됨
        }

        return false;  // 최고 기록 갱신되지 않음
    }

    /// <summary>
    /// 현재 최고 웨이브 수를 반환합니다.
    /// </summary>
    /// <returns>최고 웨이브 수</returns>
    public int GetHighestWave()
    {
        return highestWave;
    }
}