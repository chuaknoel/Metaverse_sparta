using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 캐릭터의 WASD 이동을 처리하는 컨트롤러 클래스입니다.
/// AnimationHandler와 연동하여 IsMove 애니메이션 파라미터를 제어합니다.
/// </summary>
public class PlayerMovementController : MonoBehaviour
{
    // 이동 속도 변수
    [SerializeField] private float moveSpeed = 5f;

    // 컴포넌트 참조
    private Animator animator;
    private Rigidbody2D rb;
    private AnimationHandler animationHandler;

    // 애니메이션 파라미터 ID
    private static readonly int IsMoving = Animator.StringToHash("IsMove");

    // 이동 방향 저장 변수
    private Vector2 movementDirection = Vector2.zero;

    /// <summary>
    /// 컴포넌트 초기화
    /// </summary>
    private void Awake()
    {
        // 필요한 컴포넌트 가져오기
        rb = GetComponent<Rigidbody2D>();
        animationHandler = GetComponent<AnimationHandler>();

        // AnimationHandler가 없다면 직접 Animator 참조
        if (animationHandler == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    /// <summary>
    /// 매 프레임마다 입력 처리
    /// </summary>
    private void Update()
    {
        // WASD 또는 화살표 키 입력 감지
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // 이동 방향 설정
        movementDirection = new Vector2(horizontalInput, verticalInput);

        // 방향 벡터 정규화 (대각선 이동 속도 보정)
        if (movementDirection.magnitude > 0)
        {
            movementDirection.Normalize();
        }

        // 애니메이션 상태 설정
        UpdateAnimationState();
    }

    /// <summary>
    /// 물리 업데이트 주기에 맞춰 실제 이동 처리
    /// </summary>
    private void FixedUpdate()
    {
        // 실제 이동 처리
        rb.velocity = movementDirection * moveSpeed;
    }

    /// <summary>
    /// 애니메이션 상태 업데이트
    /// </summary>
    private void UpdateAnimationState()
    {
        // 이동 여부에 따라 IsMove 파라미터 설정
        bool isMoving = movementDirection.magnitude > 0.1f;

        // AnimationHandler가 있으면 그것을 사용
        if (animationHandler != null)
        {
            animationHandler.Move(movementDirection);
        }
        // 없으면 직접 Animator 파라미터 설정
        else if (animator != null)
        {
            animator.SetBool(IsMoving, isMoving);
        }
    }

    /// <summary>
    /// 외부에서 이동을 제어하기 위한 메서드
    /// </summary>
    /// <param name="direction">이동 방향</param>
    public void SetMovementDirection(Vector2 direction)
    {
        movementDirection = direction.normalized;
        UpdateAnimationState();
    }

    /// <summary>
    /// 외부에서 이동을 멈추기 위한 메서드
    /// </summary>
    public void StopMovement()
    {
        movementDirection = Vector2.zero;
        UpdateAnimationState();
        rb.velocity = Vector2.zero;
    }
}