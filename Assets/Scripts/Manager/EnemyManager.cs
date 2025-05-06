using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �� ĳ������ ������ ������ ����ϴ� Ŭ�����Դϴ�.
/// ���̺� ������ ���� �����ϰ�, ��� ���� óġ�Ǹ� ���� ���̺긦 �����մϴ�.
/// </summary>
public class EnemyManager : MonoBehaviour
{
    // ���� ���� ���� ���̺� �ڷ�ƾ�� �����ϴ� �����Դϴ�.
    private Coroutine waveRoutine;
    // �ڷ�ƾ�� ����Ƽ���� �����ϴ� ���
    // ������ �ߴ��ϰ� �ٽ� ������ �� �ִ� ���.
    // yield return�� �ڷ�ƾ�� �ߴ��ϰ� ���� ���������� �Ѿ�� �ϴ� ���
    // waitforseconds�� �ڷ�ƾ�� �ߴ��ϰ� ������ �ð���ŭ ����ϴ� ���
    // �� �񵿱��� ���.

    // [SerializeField] �Ӽ��� private ������ ����Ƽ �ν����Ϳ��� ���� �����ϰ� �մϴ�.
    // ������ �� �����յ��� ����Դϴ�.
    [SerializeField] private List<GameObject> enemyPrefabs;

    // ���� ������ �������� ����Դϴ�.
    // Rect�� �簢�� ������ �����ϴ� ����Ƽ ����ü��, x, y, width, height ���� �����ϴ�.
    [SerializeField] List<Rect> spawnAreas;

    // Gizmo ������ �����ϴ� �����Դϴ�. ���İ��� 0.3�� ���������� �ʱ�ȭ�˴ϴ�.
    // Gizmo�� �� �信���� ���̴� ������ �׷����Դϴ�.
    [SerializeField] private Color gizmoColor = new Color(1, 0, 0, 0.3f);

    // ���� Ȱ��ȭ�� ������ ����� �����ϴ� ����Ʈ�Դϴ�.
    private List<EnemyController> activeEnemies = new List<EnemyController>();

    // �� ������ �Ϸ�Ǿ����� ��Ÿ���� �Ҹ��� �����Դϴ�.
    private bool enemySpawnComplite;

    // �� ���� �� �ð� ������ �����ϴ� �������Դϴ�.
    [SerializeField] private float timeBetweenSpawns = 0.2f;  // ���� �� ���� �� ����
    [SerializeField] private float timeBetweenWaves = 1f;     // ���̺� �� ����

    /// <summary>
    /// ���̺� ������ ���� �����ϴ� �ڷ�ƾ �޼����Դϴ�.
    /// </summary>
    /// <param name="waveCount">�� ���̺꿡�� ������ ���� ��</param>
    /// <returns>�ڷ�ƾ ������(IEnumerator)</returns>
    private IEnumerator SpawnWave(int waveCount)
    {
        // �� ���� �Ϸ� �÷��׸� false�� �����մϴ�.
        enemySpawnComplite = false;

        // ���̺� ���� ���� ������ �ð���ŭ ����մϴ�.
        // WaitForSeconds�� ������ �ð� ���� �ڷ�ƾ�� ������ �Ͻ� �����ϴ� ����Ƽ Ŭ�����Դϴ�.
        yield return new WaitForSeconds(timeBetweenWaves);

        // ������ ����ŭ ���� �����մϴ�.
        for (int i = 0; i < waveCount; i++)
        {
            // �� �� ���� ���̿� ������ �ð���ŭ ����մϴ�.
            yield return new WaitForSeconds(timeBetweenSpawns);
            // ������ ��ġ�� ���� �����մϴ�.
            SpawnRandomEnemy();
        }

        // ��� �� ������ �Ϸ�Ǿ����� ǥ���մϴ�.
        enemySpawnComplite = true;
    }

    /// <summary>
    /// ������ ��ġ�� ������ ���� �����ϴ� �޼����Դϴ�.
    /// </summary>
    private void SpawnRandomEnemy()
    {
        // �� �������̳� ���� ������ ������ ��� �޽����� ����ϰ� �����մϴ�.
        if (enemyPrefabs.Count == 0 || spawnAreas.Count == 0)
        {
            // Debug.LogWarning�� �ֿܼ� ��� �޽����� ����ϴ� ����Ƽ �޼����Դϴ�.
            // ��������� ǥ�õǾ� ������ ������ ġ���������� ������ ��Ÿ���ϴ�.
            Debug.LogWarning("Enemy Prefabs �Ǵ� Spawn Areas�� ����ֽ��ϴ�.");
            return;
        }

        // ������ �� �������� �����մϴ�.
        // Random.Range�� ������ ���� ������ ������ ������ ��ȯ�ϴ� ����Ƽ �޼����Դϴ�.
        GameObject randomPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

        // ������ ���� ������ �����մϴ�.
        Rect randomArea = spawnAreas[Random.Range(0, spawnAreas.Count)];

        // ���õ� ���� ���� ������ ������ ��ġ�� ����մϴ�.
        // Random.Range(float, float)�� �ּҰ�(����)�� �ִ밪(����) ������ ������ �ε��Ҽ��� ���� ��ȯ�մϴ�.
        Vector2 randomPosition = new Vector2(
            Random.Range(randomArea.xMin, randomArea.xMax),
            Random.Range(randomArea.yMin, randomArea.yMax));

        // ���� ��ġ�� ���� �����մϴ�.
        // Instantiate�� �������� �������� �����ϴ� ����Ƽ �޼����Դϴ�.
        // ù ��° �Ű������� ������ ������, �� ��°�� ��ġ, �� ��°�� ȸ���Դϴ�.
        // Quaternion.identity�� ȸ���� ������ ��Ÿ���� ����Դϴ�.
        GameObject spqwnEnemy = Instantiate(randomPrefab, new Vector3(randomPosition.x, randomPosition.y), Quaternion.identity);

        // ������ ���� EnemyController ������Ʈ�� ������ �ʱ�ȭ�մϴ�.
        EnemyController enemyController = spqwnEnemy.GetComponent<EnemyController>();
        // Init �޼���� �� �Ŵ����� Ÿ��(�÷��̾�)�� �����մϴ�.
        enemyController.Init(this, gameManager.player.transform);

        // Ȱ��ȭ�� �� ��Ͽ� ���� ������ ���� �߰��մϴ�.
        activeEnemies.Add(enemyController);
    }

    /// <summary>
    /// �������� �� �信�� ���� ������ �ð�ȭ�ϴ� �޼����Դϴ�.
    /// �� �޼���� ���� ���� �߿��� ȣ����� �ʰ�, �����Ϳ��� ��ü�� ���õǾ��� ���� ȣ��˴ϴ�.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // spawnAreas�� �ʱ�ȭ���� �ʾ����� �޼��带 �����մϴ�.
        if (spawnAreas == null)
        {
            return;
        }

        // Gizmo ������ �����մϴ�.
        Gizmos.color = gizmoColor;

        // �� ���� ������ Gizmo�� �ð�ȭ�մϴ�.
        foreach (var area in spawnAreas)
        {
            // ������ �߽����� ����մϴ�.
            Vector3 centetr = new Vector3(area.x + area.width / 2, area.y + area.height / 2);
            // ������ ũ�⸦ Vector3�� ��ȯ�մϴ�.
            Vector3 size = new Vector3(area.width, area.height);

            // DrawCube�� ������ ��ġ�� ������ ũ���� �Թ�ü�� �׸��� Gizmo �޼����Դϴ�.
            // �̴� �����Ϳ����� ���̸�, ���� �浹 ������ �ƴմϴ�.
            Gizmos.DrawCube(centetr, size);
        }
    }

    /// <summary>
    /// ���� ������� �� ȣ��Ǵ� �޼����Դϴ�.
    /// Ȱ��ȭ�� �� ��Ͽ��� �ش� ���� �����ϰ�, �ʿ�� ���̺� ���� ó���� �մϴ�.
    /// </summary>
    /// <param name="enemy">����� �� ��Ʈ�ѷ�</param>
    public void RemoveEnemyOnDeath(EnemyController enemy)
    {
        // Ȱ��ȭ�� �� ��Ͽ��� ����� ���� �����մϴ�.
        activeEnemies.Remove(enemy);

        // ��� �� ������ �Ϸ�Ǿ��� ���� ���� ������ ���̺� ���� ó���� �մϴ�.
        if (enemySpawnComplite && activeEnemies.Count == 0)
        {
            gameManager.EndOfWave();
        }
    }

    // GameManager ������ �����ϴ� �����Դϴ�.
    GameManager gameManager;

    /// <summary>
    /// EnemyManager�� �ʱ�ȭ�ϴ� �޼����Դϴ�.
    /// GameManager���� ȣ���Ͽ� ������ �����մϴ�.
    /// </summary>
    /// <param name="gameManager">���� �Ŵ��� ����</param>
    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    /// <summary>
    /// �� ���̺긦 �����ϴ� �޼����Դϴ�.
    /// ������ ���� ���� �����ϴ� �ڷ�ƾ�� �����մϴ�.
    /// </summary>
    /// <param name="waveCount">�� ���̺꿡�� ������ ���� ��</param>
    public void StartWave(int waveCount)
    {
        // 0 ������ ��� �� ���� ���� �ٷ� ���̺� ���� ó��
        if (waveCount <= 0)
        {
            gameManager.EndOfWave(); // GameManager�� ���̺� ���� �˸�
            return;
        }

        // �̹� ���� ���� ���̺� �ڷ�ƾ�� ������ �����մϴ�.
        if (waveRoutine != null)
            StopCoroutine(waveRoutine);

        // �� ���̺긦 �����ϴ� �ڷ�ƾ�� �����մϴ�.
        // StartCoroutine�� �ڷ�ƾ�� �����ϴ� ����Ƽ �޼����Դϴ�.
        // ��ȯ�� Coroutine ��ü�� ���߿� �ڷ�ƾ�� ������ �� ����� �� �ֽ��ϴ�.
        waveRoutine = StartCoroutine(SpawnWave(waveCount));
    }

    /// <summary>
    /// ���� ���� ���� ���̺긦 �����ϴ� �޼����Դϴ�.
    /// </summary>
    public void StopWave()
    {
        // ���� ���� ���̺� �ڷ�ƾ�� ������ �����մϴ�.
        if (waveRoutine != null)
        {
            // StopCoroutine�� ���� ���� �ڷ�ƾ�� �����ϴ� ����Ƽ �޼����Դϴ�.
            StopCoroutine(waveRoutine);
            waveRoutine = null;
        }
        // �ʿ�� Ȱ��ȭ�� �� ���� ���� �߰� ����
    }
}