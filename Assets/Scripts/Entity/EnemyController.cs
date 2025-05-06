using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �� ĳ������ ������ �����ϴ� Ŭ�����Դϴ�.
/// BaseController�� ��ӹ޾� �⺻ ĳ���� ����� Ȯ���ϰ�,
/// Ÿ��(�÷��̾�)�� ���󰡰� �����ϴ� AI �ൿ�� �����մϴ�.
/// </summary>
public class EnemyController : BaseController
{
    // EnemyManager�� ��� �� ĳ���͸� �����ϴ� Ŭ������ �����Դϴ�.
    // �̸� ���� ���� �׾��� �� �Ŵ����� �˸��ų� �ٸ� ���� ����� �����մϴ�.
    private EnemyManager enemyManager;

    // Ÿ��(�Ϲ������� �÷��̾�)�� Transform ������Ʈ �����Դϴ�.
    // Transform�� ���� ������Ʈ�� ��ġ, ȸ��, ũ�⸦ �����ϴ� ����Ƽ �⺻ ������Ʈ�Դϴ�.
    private Transform target;

    // [SerializeField] �Ӽ��� private ������ ����Ƽ �ν����Ϳ��� ���� �����ϰ� �մϴ�.
    // �� ������ ���� �÷��̾ �����ϱ� �����ϴ� �Ÿ��� �����մϴ�.
    [SerializeField] private float followRange = 15f;

    /// <summary>
    /// �� ĳ���͸� �ʱ�ȭ�ϴ� �޼����Դϴ�.
    /// EnemyManager�� Ÿ��(�÷��̾�) ������ �����մϴ�.
    /// EnemyManager���� ���� ������ �� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="enemyManager">�� ������ ����</param>
    /// <param name="target">������ ���(���� �÷��̾�)�� Transform</param>
    public void Init(EnemyManager enemyManager, Transform target)
    {
        this.enemyManager = enemyManager;
        this.target = target;
    }

    /// <summary>
    /// ���� ��ġ���� Ÿ�ٱ����� �Ÿ��� ����ϴ� �޼����Դϴ�.
    /// </summary>
    /// <returns>Ÿ�ٱ����� �Ÿ�(float)</returns>
    protected float DistancnToTarget()
    {
        // Vector3.Distance�� �� ��ġ ���� ��Ŭ���� �Ÿ��� ����մϴ�.
        // �� �Ÿ��� ���� �÷��̾ ������, �������� �����ϴ� �� ���˴ϴ�.
        return Vector3.Distance(transform.position, target.position);
    }

    /// <summary>
    /// Ÿ�� ���������� ����ȭ�� ���͸� ����ϴ� �޼����Դϴ�.
    /// </summary>
    /// <returns>Ÿ�� ������ ���� ����(ũ�Ⱑ 1�� ����)</returns>
    protected Vector2 DirectionTOTarget()
    {
        // normalized�� ������ ������ �����ϸ鼭 ũ�⸦ 1�� ����ϴ�.
        // �� ���ʹ� ���� �÷��̾ ���� �̵��ϰų� �ٶ� �� ���˴ϴ�.
        return (target.position - transform.position).normalized;
    }

    /// <summary>
    /// �� �����Ӹ��� ���� �ൿ�� �����ϴ� �޼����Դϴ�.
    /// Ÿ�ٰ��� �Ÿ��� ���� ����, ���� �Ǵ� �������� �����մϴ�.
    /// BaseController�� HandleAction �޼��带 �������մϴ�.
    /// </summary>
    protected override void HandleAction()
    {
        // �θ� Ŭ������ HandleAction �޼��带 ���� ȣ���մϴ�.
        // �̴� ��ӹ��� �⺻ ������ �����ϱ� �����Դϴ�.
        base.HandleAction();

        // ���Ⱑ ���ų� Ÿ���� ������ �������� ����ϴ�.
        if (weaponHandler == null || target == null)
        {
            // Vector2.zero�� ���� ������(�̵� ���̸�) ������ŵ�ϴ�.
            // Equals �޼���� �� ��ü�� ������ ������ ���մϴ�.
            if (!movementDirection.Equals(Vector2.zero))
            {
                movementDirection = Vector2.zero;
            }
            return;
        }

        // Ÿ�ٱ����� �Ÿ��� ������ ����մϴ�.
        float distance = DistancnToTarget();
        Vector2 direction = DirectionTOTarget();

        // �⺻������ ���� ���¸� false�� �����մϴ�.
        isAttacking = false;

        // Ÿ���� ���� ����(followRange) �ȿ� �ִ� ���
        if (distance <= followRange)
        {
            // Ÿ���� �ٶ󺾴ϴ�.
            lookDirection = direction;

            // Ÿ���� ������ ���� ���� �ȿ� �ִ� ���
            if (distance < weaponHandler.AttackRange)
            {
                // Ÿ�� ���̾� ����ũ�� �����ɴϴ�.
                // ���̾� ����ũ�� Ư�� ���̾��� ������Ʈ�� �����ϱ� ���� ���˴ϴ�.
                int layerMaskTarget = weaponHandler.target;

                // ����ĳ��Ʈ�� ����Ͽ� Ÿ�ٰ� �� ���̿� ��ֹ��� �ִ��� Ȯ���մϴ�.
                // Physics2D.Raycast�� Ư�� �������� ���� ��� �浹�ϴ� ������Ʈ�� �����մϴ�.
                // �� ���� ���� ��ġ���� Ÿ�� �������� �߻�˴ϴ�.
                RaycastHit2D hit = Physics2D.Raycast(transform.position,
                    direction, weaponHandler.AttackRange * 1.5f,
                    layerMaskTarget | (1 << LayerMask.NameToLayer("Level")));
                // ����ĳ��Ʈ�� ���� ������ �ʴ� ���� ��°�

                // ����ĳ��Ʈ�� ���𰡿� �¾Ұ�, �װ��� Ÿ�� ���̾��� ���
                // ��Ʈ ������ |�� OR ������ �����Ͽ� �� ���̾� ����ũ�� �����մϴ�.
                // ��Ʈ ������ <<�� ���� ����Ʈ��, 1�� ���̾� ��ȣ��ŭ �̵����� �ش� ���̾��� ����ũ�� �����մϴ�.
                if (hit.collider != null && layerMaskTarget == (layerMaskTarget | (1 << hit.collider.gameObject.layer)))
                // ����ĳ��Ʈ�� ���� ������Ʈ�� ���̾ target ���̾�� ������
                {
                    // ���� ���¸� Ȱ��ȭ�մϴ�.
                    isAttacking = true;
                }
                // ���� ���� �ȿ� ���� ���� �̵��� ����ϴ�.
                movementDirection = Vector2.zero;
                return;
            }
            // Ÿ���� ���� ���� �ȿ� ������ ���� ���� �ۿ� ������, Ÿ���� ���� �̵��մϴ�.
            movementDirection = direction;
        }
        // Ÿ���� ���� ���� �ۿ� ������ �⺻������ �ƹ� �ൿ�� ���� �ʽ��ϴ�.
        // �� ��� movementDirection�� ������� �ʽ��ϴ�.
    }

    /// <summary>
    /// �� ĳ���Ͱ� ������� �� ȣ��Ǵ� �޼����Դϴ�.
    /// BaseController�� Death �޼��带 �������ϰ�, �߰��� EnemyManager�� ��� �˸��� �����ϴ�.
    /// </summary>
    public override void Death()
    {
        // �θ� Ŭ������ Death �޼��带 ���� ȣ���Ͽ� �⺻ ��� ó���� �����մϴ�.
        // �� �޼���� �������� ���߰�, ������ ���߰�, ������Ʈ�� ��Ȱ��ȭ�մϴ�.
        base.Death();

        // ���� ������� �� �� �Ŵ������� ����
        // RemoveEnemyOnDeath �޼���� EnemyManager���� �� �� �ν��Ͻ��� �����ϰ�,
        // �ʿ�� �� ���̺� ���۰� ���� ���� ������ ó���մϴ�.
        enemyManager.RemoveEnemyOnDeath(this);
        // ���� ��� �ִϸ��̼� ���
    }
}
