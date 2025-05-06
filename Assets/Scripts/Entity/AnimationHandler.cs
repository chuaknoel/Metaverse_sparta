using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ĳ������ �ִϸ��̼��� �����ϴ� Ŭ�����Դϴ�.
/// ������, ������ ���� ���¿� ���� �ִϸ��̼��� �����մϴ�.
/// </summary>
public class AnimationHandler : MonoBehaviour
{
    // Animator.StringToHash�� ���ڿ��� ���� ID�� ��ȯ�Ͽ� ������ ����ȭ�մϴ�.
    // �Ź� ���ڿ��� ���ϴ� �ͺ��� ���� �񱳰� �� ������ ������ ���˴ϴ�.
    // �� �ؽ� ������ �ִϸ����� ��Ʈ�ѷ��� �Ķ���� �̸��� ��ġ�ؾ� �մϴ�.
    private static readonly int IsMoving = Animator.StringToHash("isMove");  // ������ ���¸� �����ϴ� �ؽ� ��
    private static readonly int IsDamage = Animator.StringToHash("isDamage");  // ������ ���¸� �����ϴ� �ؽ� ��

    // protected ���� �����ڸ� ����Ͽ� �ڽ� Ŭ���������� ���� �����ϰ� �մϴ�.
    // Animator ������Ʈ�� ����Ƽ���� �ִϸ��̼��� �����ϴ� �ٽ� ������Ʈ�Դϴ�.
    protected Animator animator;

    /// <summary>
    /// Awake�� ��ũ��Ʈ �ν��Ͻ��� �ε��� �� ȣ��Ǵ� ����Ƽ �����ֱ� �޼����Դϴ�.
    /// Start���� ���� ȣ��Ǹ�, �ٸ� ��ũ��Ʈ�� ������ �����ϱ⿡ �����մϴ�.
    /// virtual Ű���带 ����Ͽ� �ڽ� Ŭ�������� �������� �� �ְ� �մϴ�.
    /// </summary>
    protected virtual void Awake()
    {
        // GetComponentInChildren�� �� ���ӿ�����Ʈ�� �� �ڽĿ��� ������ Ÿ���� ������Ʈ�� ã���ϴ�.
        // �� ��� Animator ������Ʈ�� ã�� ������ �����մϴ�.
        // �ڽ� ���� �������� ù ��°�� �߰ߵǴ� ������Ʈ�� ��ȯ�մϴ�.
        animator = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// ĳ������ ������ ���¸� �����ϴ� �޼����Դϴ�.
    /// ĳ������ �̵� �ӵ��� 0.5���� ũ�� ������ �ִϸ��̼��� Ȱ��ȭ�մϴ�.
    /// </summary>
    /// <param name="obj">�̵� ����� �ӵ��� ��Ÿ���� 2D ����</param>
    public void Move(Vector2 obj)
    {
        // Vector2.magnitude�� ������ ����(ũ��)�� ��ȯ�մϴ�.
        // ���⼭�� �̵� ������ ũ�Ⱑ 0.5���� ũ�� ĳ���Ͱ� �����̴� ������ �Ǵ��մϴ�.
        // SetBool�� �ִϸ������� �Ҹ��� �Ķ���� ���� �����մϴ�.
        animator.SetBool(IsMoving, obj.magnitude > .5f);
    }

    /// <summary>
    /// ĳ���Ͱ� �������� �Ծ��� �� ȣ��Ǵ� �޼����Դϴ�.
    /// ������ �ִϸ��̼��� Ȱ��ȭ�մϴ�.
    /// </summary>
    public void Damage()
    {
        // ������ ���¸� true�� �����Ͽ� ������ �ִϸ��̼��� �����մϴ�.
        animator.SetBool(IsDamage, true);
    }

    /// <summary>
    /// ĳ������ ���� ���°� ������ �� ȣ��Ǵ� �޼����Դϴ�.
    /// ������ �ִϸ��̼��� ��Ȱ��ȭ�մϴ�.
    /// </summary>
    public void InvincibilityEnd()
    {
        // ������ ���¸� false�� �����Ͽ� ������ �ִϸ��̼��� �����մϴ�.
        animator.SetBool(IsDamage, false);
    }
}
