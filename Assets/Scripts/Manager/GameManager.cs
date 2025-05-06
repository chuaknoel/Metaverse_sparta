using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 게임 전체를 관리하는 메인 매니저 클래스
// 기존 GameManager.cs에 최고 웨이브 저장 기능 추가
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerController player { get; private set; } // 플레이어 컨트롤러 (읽기 전용 프로퍼티)
    private ResourceController _playerResourceController;

    [SerializeField] private int currentWaveIndex = 0; // 현재 웨이브 번호

    private EnemyManager enemyManager; // 적 생성 및 관리하는 매니저

    private UIManager uiManager; // UI 매니저

    public static bool isFirstLOading = true; // 첫 로드 여부

    // GameManager.cs의 Awake() 메서드 수정
    private void Awake()
    {
        // 싱글톤 할당
        instance = this;

        // 플레이어 찾고 초기화
        player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.Init(this);
        }
        else
        {
            Debug.LogError("PlayerController를 찾을 수 없습니다!");
        }

        uiManager = FindObjectOfType<UIManager>();

        // 방법 1: FindObjectOfType 사용 (위에서 EnemyManager 찾기)
        enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager != null)
        {
            enemyManager.Init(this);
        }
        else
        {
            Debug.LogError("EnemyManager를 찾을 수 없습니다!");
        }


        _playerResourceController = player.GetComponent<ResourceController>(); // 플레이어의 ResourceController 컴포넌트 가져오기
        _playerResourceController.RemoveHealthChangeEvent(uiManager.ChangePlayerHP); // 플레이어 체력 UI 초기화
        _playerResourceController.AddHealthCahngeEvent(uiManager.ChangePlayerHP); // 플레이어 체력 UI 업데이트 이벤트 등록
    }

    private void Start()
    {
        if (isFirstLOading)
        {
            // StartGame() 호출 제거
            isFirstLOading = false; // 첫 로드 여부 변경
        }

        // UIManager는 자동으로 HomeUI를 표시할 것입니다
        // 게임은 HomeUI에서 시작 버튼을 클릭할 때만 시작됩니다
    }

    public void StartGame()
    {
        StartNextWave(); // 첫 웨이브 시작
        uiManager.SetPlayGame(); // 게임 UI 활성화
    }

    void StartNextWave()
    {
        currentWaveIndex += 1; // 웨이브 인덱스 증가
        // 5웨이브마다 난이도 증가 (예: 1~4 → 레벨 1, 5~9 → 레벨 2 ...)
        enemyManager.StartWave(1 + currentWaveIndex / 5);

        uiManager.ChangeWave(currentWaveIndex); // UI에 웨이브 번호 업데이트
    }

    // 웨이브 종료 후 다음 웨이브 시작
    public void EndOfWave()
    {
        StartNextWave();
    }

    // 플레이어가 죽었을 때 게임 오버 처리
    public void GameOver()
    {
        // 최고 웨이브 기록 저장
        SaveHighestWave();

        enemyManager.StopWave(); // 적 스폰 중지
        uiManager.SetGameOver(); // 게임 오버 UI 활성화
    }

    // 최고 웨이브 저장 메서드 추가
    private void SaveHighestWave()
    {
        // WaveDataManager 찾기
        WaveDataManager waveManager = FindObjectOfType<WaveDataManager>();

        // 없으면 새로 생성
        if (waveManager == null)
        {
            GameObject managerObj = new GameObject("WaveDataManager");
            waveManager = managerObj.AddComponent<WaveDataManager>();
        }

        // 최고 웨이브 업데이트
        waveManager.UpdateHighestWave(currentWaveIndex);
    }
}