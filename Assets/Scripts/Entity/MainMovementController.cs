using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾� ĳ������ WASD �̵��� ó���ϴ� ��Ʈ�ѷ� Ŭ�����Դϴ�.
/// AnimationHandler�� �����Ͽ� IsMove �ִϸ��̼� �Ķ���͸� �����մϴ�.
/// </summary>
public class PlayerMovementController : MonoBehaviour
{
    // �̵� �ӵ� ����
    [SerializeField] private float moveSpeed = 5f;

    // ������Ʈ ����
    private Animator animator;
    private Rigidbody2D rb;
    private AnimationHandler animationHandler;

    // �ִϸ��̼� �Ķ���� ID
    private static readonly int IsMoving = Animator.StringToHash("IsMove");

    // �̵� ���� ���� ����
    private Vector2 movementDirection = Vector2.zero;

    /// <summary>
    /// ������Ʈ �ʱ�ȭ
    /// </summary>
    private void Awake()
    {
        // �ʿ��� ������Ʈ ��������
        rb = GetComponent<Rigidbody2D>();
        animationHandler = GetComponent<AnimationHandler>();

        // AnimationHandler�� ���ٸ� ���� Animator ����
        if (animationHandler == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    /// <summary>
    /// �� �����Ӹ��� �Է� ó��
    /// </summary>
    private void Update()
    {
        // WASD �Ǵ� ȭ��ǥ Ű �Է� ����
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // �̵� ���� ����
        movementDirection = new Vector2(horizontalInput, verticalInput);

        // ���� ���� ����ȭ (�밢�� �̵� �ӵ� ����)
        if (movementDirection.magnitude > 0)
        {
            movementDirection.Normalize();
        }

        // �ִϸ��̼� ���� ����
        UpdateAnimationState();
    }

    /// <summary>
    /// ���� ������Ʈ �ֱ⿡ ���� ���� �̵� ó��
    /// </summary>
    private void FixedUpdate()
    {
        // ���� �̵� ó��
        rb.velocity = movementDirection * moveSpeed;
    }

    /// <summary>
    /// �ִϸ��̼� ���� ������Ʈ
    /// </summary>
    private void UpdateAnimationState()
    {
        // �̵� ���ο� ���� IsMove �Ķ���� ����
        bool isMoving = movementDirection.magnitude > 0.1f;

        // AnimationHandler�� ������ �װ��� ���
        if (animationHandler != null)
        {
            animationHandler.Move(movementDirection);
        }
        // ������ ���� Animator �Ķ���� ����
        else if (animator != null)
        {
            animator.SetBool(IsMoving, isMoving);
        }
    }

    /// <summary>
    /// �ܺο��� �̵��� �����ϱ� ���� �޼���
    /// </summary>
    /// <param name="direction">�̵� ����</param>
    public void SetMovementDirection(Vector2 direction)
    {
        movementDirection = direction.normalized;
        UpdateAnimationState();
    }

    /// <summary>
    /// �ܺο��� �̵��� ���߱� ���� �޼���
    /// </summary>
    public void StopMovement()
    {
        movementDirection = Vector2.zero;
        UpdateAnimationState();
        rb.velocity = Vector2.zero;
    }
}