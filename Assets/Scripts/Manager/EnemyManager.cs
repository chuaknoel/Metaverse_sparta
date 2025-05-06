using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 캐릭터의 생성과 관리를 담당하는 클래스입니다.
/// 웨이브 단위로 적을 생성하고, 모든 적이 처치되면 다음 웨이브를 시작합니다.
/// </summary>
public class EnemyManager : MonoBehaviour
{
    // 현재 실행 중인 웨이브 코루틴을 참조하는 변수입니다.
    private Coroutine waveRoutine;
    // 코루틴은 유니티에서 제공하는 기능
    // 실행을 중단하고 다시 시작할 수 있는 기능.
    // yield return은 코루틴을 중단하고 다음 프레임으로 넘어가게 하는 기능
    // waitforseconds는 코루틴을 중단하고 지정한 시간만큼 대기하는 기능
    // 등 비동기적 기능.

    // [SerializeField] 속성은 private 변수를 유니티 인스펙터에서 수정 가능하게 합니다.
    // 생성할 적 프리팹들의 목록입니다.
    [SerializeField] private List<GameObject> enemyPrefabs;

    // 적을 생성할 영역들의 목록입니다.
    // Rect는 사각형 영역을 정의하는 유니티 구조체로, x, y, width, height 값을 가집니다.
    [SerializeField] List<Rect> spawnAreas;

    // Gizmo 색상을 설정하는 변수입니다. 알파값이 0.3인 빨간색으로 초기화됩니다.
    // Gizmo는 씬 뷰에서만 보이는 디버깅용 그래픽입니다.
    [SerializeField] private Color gizmoColor = new Color(1, 0, 0, 0.3f);

    // 현재 활성화된 적들의 목록을 저장하는 리스트입니다.
    private List<EnemyController> activeEnemies = new List<EnemyController>();

    // 적 생성이 완료되었는지 나타내는 불리언 변수입니다.
    private bool enemySpawnComplite;

    // 적 생성 간 시간 간격을 설정하는 변수들입니다.
    [SerializeField] private float timeBetweenSpawns = 0.2f;  // 개별 적 생성 간 간격
    [SerializeField] private float timeBetweenWaves = 1f;     // 웨이브 간 간격

    /// <summary>
    /// 웨이브 단위로 적을 생성하는 코루틴 메서드입니다.
    /// </summary>
    /// <param name="waveCount">이 웨이브에서 생성할 적의 수</param>
    /// <returns>코루틴 열거자(IEnumerator)</returns>
    private IEnumerator SpawnWave(int waveCount)
    {
        // 적 생성 완료 플래그를 false로 설정합니다.
        enemySpawnComplite = false;

        // 웨이브 시작 전에 지정된 시간만큼 대기합니다.
        // WaitForSeconds는 지정된 시간 동안 코루틴의 실행을 일시 중지하는 유니티 클래스입니다.
        yield return new WaitForSeconds(timeBetweenWaves);

        // 지정된 수만큼 적을 생성합니다.
        for (int i = 0; i < waveCount; i++)
        {
            // 각 적 생성 사이에 지정된 시간만큼 대기합니다.
            yield return new WaitForSeconds(timeBetweenSpawns);
            // 랜덤한 위치에 적을 생성합니다.
            SpawnRandomEnemy();
        }

        // 모든 적 생성이 완료되었음을 표시합니다.
        enemySpawnComplite = true;
    }

    /// <summary>
    /// 랜덤한 위치에 랜덤한 적을 생성하는 메서드입니다.
    /// </summary>
    private void SpawnRandomEnemy()
    {
        // 적 프리팹이나 생성 영역이 없으면 경고 메시지를 출력하고 종료합니다.
        if (enemyPrefabs.Count == 0 || spawnAreas.Count == 0)
        {
            // Debug.LogWarning은 콘솔에 경고 메시지를 출력하는 유니티 메서드입니다.
            // 노란색으로 표시되어 문제가 있지만 치명적이지는 않음을 나타냅니다.
            Debug.LogWarning("Enemy Prefabs 또는 Spawn Areas가 비어있습니다.");
            return;
        }

        // 랜덤한 적 프리팹을 선택합니다.
        // Random.Range은 지정된 범위 내에서 랜덤한 정수를 반환하는 유니티 메서드입니다.
        GameObject randomPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

        // 랜덤한 생성 영역을 선택합니다.
        Rect randomArea = spawnAreas[Random.Range(0, spawnAreas.Count)];

        // 선택된 생성 영역 내에서 랜덤한 위치를 계산합니다.
        // Random.Range(float, float)는 최소값(포함)과 최대값(포함) 사이의 랜덤한 부동소수점 값을 반환합니다.
        Vector2 randomPosition = new Vector2(
            Random.Range(randomArea.xMin, randomArea.xMax),
            Random.Range(randomArea.yMin, randomArea.yMax));

        // 계산된 위치에 적을 생성합니다.
        // Instantiate는 프리팹의 복제본을 생성하는 유니티 메서드입니다.
        // 첫 번째 매개변수는 복제할 프리팹, 두 번째는 위치, 세 번째는 회전입니다.
        // Quaternion.identity는 회전이 없음을 나타내는 상수입니다.
        GameObject spqwnEnemy = Instantiate(randomPrefab, new Vector3(randomPosition.x, randomPosition.y), Quaternion.identity);

        // 생성된 적의 EnemyController 컴포넌트를 가져와 초기화합니다.
        EnemyController enemyController = spqwnEnemy.GetComponent<EnemyController>();
        // Init 메서드로 적 매니저와 타겟(플레이어)을 설정합니다.
        enemyController.Init(this, gameManager.player.transform);

        // 활성화된 적 목록에 새로 생성된 적을 추가합니다.
        activeEnemies.Add(enemyController);
    }

    /// <summary>
    /// 에디터의 씬 뷰에서 스폰 영역을 시각화하는 메서드입니다.
    /// 이 메서드는 게임 실행 중에는 호출되지 않고, 에디터에서 객체가 선택되었을 때만 호출됩니다.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // spawnAreas가 초기화되지 않았으면 메서드를 종료합니다.
        if (spawnAreas == null)
        {
            return;
        }

        // Gizmo 색상을 설정합니다.
        Gizmos.color = gizmoColor;

        // 각 생성 영역을 Gizmo로 시각화합니다.
        foreach (var area in spawnAreas)
        {
            // 영역의 중심점을 계산합니다.
            Vector3 centetr = new Vector3(area.x + area.width / 2, area.y + area.height / 2);
            // 영역의 크기를 Vector3로 변환합니다.
            Vector3 size = new Vector3(area.width, area.height);

            // DrawCube는 지정된 위치에 지정된 크기의 입방체를 그리는 Gizmo 메서드입니다.
            // 이는 에디터에서만 보이며, 실제 충돌 영역은 아닙니다.
            Gizmos.DrawCube(centetr, size);
        }
    }

    /// <summary>
    /// 적이 사망했을 때 호출되는 메서드입니다.
    /// 활성화된 적 목록에서 해당 적을 제거하고, 필요시 웨이브 종료 처리를 합니다.
    /// </summary>
    /// <param name="enemy">사망한 적 컨트롤러</param>
    public void RemoveEnemyOnDeath(EnemyController enemy)
    {
        // 활성화된 적 목록에서 사망한 적을 제거합니다.
        activeEnemies.Remove(enemy);

        // 모든 적 생성이 완료되었고 남은 적이 없으면 웨이브 종료 처리를 합니다.
        if (enemySpawnComplite && activeEnemies.Count == 0)
        {
            gameManager.EndOfWave();
        }
    }

    // GameManager 참조를 저장하는 변수입니다.
    GameManager gameManager;

    /// <summary>
    /// EnemyManager를 초기화하는 메서드입니다.
    /// GameManager에서 호출하여 참조를 설정합니다.
    /// </summary>
    /// <param name="gameManager">게임 매니저 참조</param>
    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    /// <summary>
    /// 새 웨이브를 시작하는 메서드입니다.
    /// 지정된 수의 적을 생성하는 코루틴을 시작합니다.
    /// </summary>
    /// <param name="waveCount">이 웨이브에서 생성할 적의 수</param>
    public void StartWave(int waveCount)
    {
        // 0 이하인 경우 적 생성 없이 바로 웨이브 종료 처리
        if (waveCount <= 0)
        {
            gameManager.EndOfWave(); // GameManager에 웨이브 종료 알림
            return;
        }

        // 이미 실행 중인 웨이브 코루틴이 있으면 중지합니다.
        if (waveRoutine != null)
            StopCoroutine(waveRoutine);

        // 새 웨이브를 시작하는 코루틴을 실행합니다.
        // StartCoroutine은 코루틴을 시작하는 유니티 메서드입니다.
        // 반환된 Coroutine 객체는 나중에 코루틴을 중지할 때 사용할 수 있습니다.
        waveRoutine = StartCoroutine(SpawnWave(waveCount));
    }

    /// <summary>
    /// 현재 진행 중인 웨이브를 중지하는 메서드입니다.
    /// </summary>
    public void StopWave()
    {
        // 실행 중인 웨이브 코루틴이 있으면 중지합니다.
        if (waveRoutine != null)
        {
            // StopCoroutine은 실행 중인 코루틴을 중지하는 유니티 메서드입니다.
            StopCoroutine(waveRoutine);
            waveRoutine = null;
        }
        // 필요시 활성화된 적 제거 등의 추가 로직
    }
}