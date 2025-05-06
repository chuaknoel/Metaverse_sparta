using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 캐릭터의 동작을 제어하는 클래스입니다.
/// BaseController를 상속받아 기본 캐릭터 기능을 확장하고,
/// 타겟(플레이어)을 따라가고 공격하는 AI 행동을 구현합니다.
/// </summary>
public class EnemyController : BaseController
{
    // EnemyManager는 모든 적 캐릭터를 관리하는 클래스의 참조입니다.
    // 이를 통해 적이 죽었을 때 매니저에 알리거나 다른 관리 기능을 수행합니다.
    private EnemyManager enemyManager;

    // 타겟(일반적으로 플레이어)의 Transform 컴포넌트 참조입니다.
    // Transform은 게임 오브젝트의 위치, 회전, 크기를 제어하는 유니티 기본 컴포넌트입니다.
    private Transform target;

    // [SerializeField] 속성은 private 변수를 유니티 인스펙터에서 편집 가능하게 합니다.
    // 이 변수는 적이 플레이어를 추적하기 시작하는 거리를 설정합니다.
    [SerializeField] private float followRange = 15f;

    /// <summary>
    /// 적 캐릭터를 초기화하는 메서드입니다.
    /// EnemyManager와 타겟(플레이어) 참조를 설정합니다.
    /// EnemyManager에서 적을 생성할 때 호출됩니다.
    /// </summary>
    /// <param name="enemyManager">적 관리자 참조</param>
    /// <param name="target">추적할 대상(보통 플레이어)의 Transform</param>
    public void Init(EnemyManager enemyManager, Transform target)
    {
        this.enemyManager = enemyManager;
        this.target = target;
    }

    /// <summary>
    /// 현재 위치에서 타겟까지의 거리를 계산하는 메서드입니다.
    /// </summary>
    /// <returns>타겟까지의 거리(float)</returns>
    protected float DistancnToTarget()
    {
        // Vector3.Distance는 두 위치 간의 유클리드 거리를 계산합니다.
        // 이 거리는 적이 플레이어를 따라갈지, 공격할지 결정하는 데 사용됩니다.
        return Vector3.Distance(transform.position, target.position);
    }

    /// <summary>
    /// 타겟 방향으로의 정규화된 벡터를 계산하는 메서드입니다.
    /// </summary>
    /// <returns>타겟 방향의 단위 벡터(크기가 1인 벡터)</returns>
    protected Vector2 DirectionTOTarget()
    {
        // normalized는 벡터의 방향은 유지하면서 크기를 1로 만듭니다.
        // 이 벡터는 적이 플레이어를 향해 이동하거나 바라볼 때 사용됩니다.
        return (target.position - transform.position).normalized;
    }

    /// <summary>
    /// 매 프레임마다 적의 행동을 결정하는 메서드입니다.
    /// 타겟과의 거리에 따라 추적, 공격 또는 정지할지 결정합니다.
    /// BaseController의 HandleAction 메서드를 재정의합니다.
    /// </summary>
    protected override void HandleAction()
    {
        // 부모 클래스의 HandleAction 메서드를 먼저 호출합니다.
        // 이는 상속받은 기본 동작을 유지하기 위함입니다.
        base.HandleAction();

        // 무기가 없거나 타겟이 없으면 움직임을 멈춥니다.
        if (weaponHandler == null || target == null)
        {
            // Vector2.zero와 같지 않으면(이동 중이면) 정지시킵니다.
            // Equals 메서드는 두 객체의 내용이 같은지 비교합니다.
            if (!movementDirection.Equals(Vector2.zero))
            {
                movementDirection = Vector2.zero;
            }
            return;
        }

        // 타겟까지의 거리와 방향을 계산합니다.
        float distance = DistancnToTarget();
        Vector2 direction = DirectionTOTarget();

        // 기본적으로 공격 상태를 false로 설정합니다.
        isAttacking = false;

        // 타겟이 추적 범위(followRange) 안에 있는 경우
        if (distance <= followRange)
        {
            // 타겟을 바라봅니다.
            lookDirection = direction;

            // 타겟이 무기의 공격 범위 안에 있는 경우
            if (distance < weaponHandler.AttackRange)
            {
                // 타겟 레이어 마스크를 가져옵니다.
                // 레이어 마스크는 특정 레이어의 오브젝트만 감지하기 위해 사용됩니다.
                int layerMaskTarget = weaponHandler.target;

                // 레이캐스트를 사용하여 타겟과 적 사이에 장애물이 있는지 확인합니다.
                // Physics2D.Raycast는 특정 방향으로 선을 쏘아 충돌하는 오브젝트를 감지합니다.
                // 이 선은 현재 위치에서 타겟 방향으로 발사됩니다.
                RaycastHit2D hit = Physics2D.Raycast(transform.position,
                    direction, weaponHandler.AttackRange * 1.5f,
                    layerMaskTarget | (1 << LayerMask.NameToLayer("Level")));
                // 레이캐스트는 눈에 보이지 않는 선을 쏘는것

                // 레이캐스트가 무언가에 맞았고, 그것이 타겟 레이어인 경우
                // 비트 연산자 |는 OR 연산을 수행하여 두 레이어 마스크를 결합합니다.
                // 비트 연산자 <<는 왼쪽 시프트로, 1을 레이어 번호만큼 이동시켜 해당 레이어의 마스크를 생성합니다.
                if (hit.collider != null && layerMaskTarget == (layerMaskTarget | (1 << hit.collider.gameObject.layer)))
                // 레이캐스트가 맞은 오브젝트의 레이어가 target 레이어와 같을때
                {
                    // 공격 상태를 활성화합니다.
                    isAttacking = true;
                }
                // 공격 범위 안에 있을 때는 이동을 멈춥니다.
                movementDirection = Vector2.zero;
                return;
            }
            // 타겟이 추적 범위 안에 있지만 공격 범위 밖에 있으면, 타겟을 향해 이동합니다.
            movementDirection = direction;
        }
        // 타겟이 추적 범위 밖에 있으면 기본적으로 아무 행동도 하지 않습니다.
        // 이 경우 movementDirection은 변경되지 않습니다.
    }

    /// <summary>
    /// 적 캐릭터가 사망했을 때 호출되는 메서드입니다.
    /// BaseController의 Death 메서드를 재정의하고, 추가로 EnemyManager에 사망 알림을 보냅니다.
    /// </summary>
    public override void Death()
    {
        // 부모 클래스의 Death 메서드를 먼저 호출하여 기본 사망 처리를 수행합니다.
        // 이 메서드는 움직임을 멈추고, 투명도를 낮추고, 컴포넌트를 비활성화합니다.
        base.Death();

        // 적이 사망했을 때 적 매니저에서 제거
        // RemoveEnemyOnDeath 메서드는 EnemyManager에서 이 적 인스턴스를 제거하고,
        // 필요시 새 웨이브 시작과 같은 게임 로직을 처리합니다.
        enemyManager.RemoveEnemyOnDeath(this);
        // 적의 사망 애니메이션 재생
    }
}
