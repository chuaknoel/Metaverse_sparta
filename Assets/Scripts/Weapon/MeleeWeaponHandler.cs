using System.Collections; // �÷��� Ŭ����(����Ʈ ��)�� ����ϱ� ���� ���ӽ����̽�
using System.Collections.Generic; // ���׸� �÷���(List<T> ��)�� ����ϱ� ���� ���ӽ����̽�
using UnityEngine; // Unity ���� ����� ����ϱ� ���� ���ӽ����̽�

/// <summary>
/// ���� ������ ������ �����ϴ� Ŭ�����Դϴ�.
/// WeaponHandler�� ��ӹ޾� ���� ���ݿ� �ʿ��� ����� �����մϴ�.
/// �ڽ�ĳ��Ʈ�� ����Ͽ� ���� ���� ���� ���� �����ϰ� ������� �����ϴ�.
/// </summary>
public class MeleeWeaponHandler : WeaponHandler // WeaponHandler Ŭ������ ��ӹ޽��ϴ�.
{
    [Header("Melee Attack Info")] // �ν����Ϳ��� ���� ������ ǥ���ϴ� �Ӽ��Դϴ�.
    public Vector2 coliderBoxSize = Vector2.one; // ���� ������ ��Ÿ���� �ڽ� �ݶ��̴��� ũ���Դϴ�. Vector2.one�� (1,1) ũ���Դϴ�.

    /// <summary>
    /// Start �޼���� ù ������ ������Ʈ ������ ȣ��Ǵ� Unity �����ֱ� �޼����Դϴ�.
    /// �θ� Ŭ������ Start �޼��带 ���� ȣ���ϰ�, �ݶ��̴� ũ�⸦ �����մϴ�.
    /// </summary>
    protected override void Start() // �θ� Ŭ������ Start �޼��带 �������մϴ�.
    {
        base.Start(); // �θ� Ŭ������ Start �޼��带 ȣ���մϴ�.
        // �ݶ��̴� ũ�� ����
        coliderBoxSize = coliderBoxSize * WeaponSize; // ���� ũ�⿡ ����Ͽ� �ݶ��̴� ũ�� ����
    }

    /// <summary>
    /// ���� ������ �����ϴ� �޼����Դϴ�.
    /// �θ� Ŭ������ Attack �޼��带 ȣ���� ��, ���� ���� ���� �ִ� ���� �����ϰ� ������� �����ϴ�.
    /// </summary>
    public override void Attack() // �θ� Ŭ������ Attack �޼��带 �������մϴ�.
    {
        base.Attack(); // �θ� Ŭ������ Attack �޼��带 ȣ���մϴ� (�ִϸ��̼� ��� ��).

        // BoxCast�� ������ ��ġ���� ������ ũ���� ���� ������� ����ĳ��Ʈ�� �����մϴ�.
        // transform.position: ������ ���� ��ġ
        // (Vector3)Controller.LookDirection * coliderBoxSize.x: ĳ���Ͱ� �ٶ󺸴� �������� �ݶ��̴� ������
        // coliderBoxSize: �ݶ��̴� ũ��
        // 0: ȸ�� ���� (���⼭�� ȸ�� ����)
        // Vector2.zero: ���� (���⼭�� ���� ���� �� �ڸ����� üũ)
        // 0: �Ÿ� (���⼭�� �Ÿ� ���� �� �ڸ����� üũ)
        // target: Ÿ�� ���̾��ũ (���Ⱑ ������ ���̾�)
        RaycastHit2D hit = Physics2D.BoxCast(transform.position +
            (Vector3)Controller.LookDirection * coliderBoxSize.x, coliderBoxSize, 0, Vector2.zero, 0, target);

        // ����ĳ��Ʈ�� ��ü�� �����Ǿ��ٸ�
        if (hit.collider != null)
        {
            // ������ ��ü���� ResourceController ������Ʈ�� �����ɴϴ�.
            ResourceController resourceController = hit.collider.GetComponent<ResourceController>();
            if (resourceController != null) // ResourceController�� �ִٸ� (ü�� �ý����� �ִ� ��ü���)
            {
                resourceController.ChangeHealth(-Power); // ������� �����ϴ� (Power ��ŭ�� ü�� ����).

                // �˹� ����� Ȱ��ȭ�Ǿ� �ִٸ�
                if (IsOnKnockback)
                {
                    // ����� BaseController ������Ʈ�� �����ɴϴ�.
                    BaseController controller = resourceController.GetComponent<BaseController>();
                    if (controller != null)
                    {
                        // �˹��� �����մϴ� (�� ������ ��ġ���� ��� ��������, ������ ���� �ð���ŭ).
                        controller.ApplyKnockback(transform, KnockbackPower, KnockbackTime);
                    }
                }
            }
        }
    }

    /// <summary>
    /// ���⸦ ȸ����Ű�� �޼����Դϴ�.
    /// ĳ���Ͱ� �ٶ󺸴� ���⿡ ���� ������ ������ �����մϴ�.
    /// </summary>
    /// <param name="isLeft">ĳ���Ͱ� ������ �ٶ󺸴��� ����</param>
    public override void Rotate(bool isLeft) // �θ� Ŭ������ Rotate �޼��带 �������մϴ�.
    {
        if (isLeft) // ĳ���Ͱ� ������ �ٶ󺻴ٸ�
        {
            transform.eulerAngles = new Vector3(0, 180, 0); // Y���� �������� 180�� ȸ�� (������ ����)
        }
        else // ĳ���Ͱ� �������� �ٶ󺻴ٸ�
        {
            transform.eulerAngles = new Vector3(0, 0, 0); // ȸ������ ���� (�������� ����)
        }
    }
}