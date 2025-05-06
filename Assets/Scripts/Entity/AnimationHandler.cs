using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐릭터의 애니메이션을 관리하는 클래스입니다.
/// 움직임, 데미지 등의 상태에 따라 애니메이션을 제어합니다.
/// </summary>
public class AnimationHandler : MonoBehaviour
{
    // Animator.StringToHash는 문자열을 정수 ID로 변환하여 성능을 최적화합니다.
    // 매번 문자열을 비교하는 것보다 정수 비교가 더 빠르기 때문에 사용됩니다.
    // 이 해시 값들은 애니메이터 컨트롤러의 파라미터 이름과 일치해야 합니다.
    private static readonly int IsMoving = Animator.StringToHash("isMove");  // 움직임 상태를 제어하는 해시 값
    private static readonly int IsDamage = Animator.StringToHash("isDamage");  // 데미지 상태를 제어하는 해시 값

    // protected 접근 제한자를 사용하여 자식 클래스에서도 접근 가능하게 합니다.
    // Animator 컴포넌트는 유니티에서 애니메이션을 제어하는 핵심 컴포넌트입니다.
    protected Animator animator;

    /// <summary>
    /// Awake는 스크립트 인스턴스가 로딩될 때 호출되는 유니티 생명주기 메서드입니다.
    /// Start보다 먼저 호출되며, 다른 스크립트의 참조를 설정하기에 적합합니다.
    /// virtual 키워드를 사용하여 자식 클래스에서 재정의할 수 있게 합니다.
    /// </summary>
    protected virtual void Awake()
    {
        // GetComponentInChildren은 이 게임오브젝트와 그 자식에서 지정된 타입의 컴포넌트를 찾습니다.
        // 이 경우 Animator 컴포넌트를 찾아 참조를 저장합니다.
        // 자식 계층 구조에서 첫 번째로 발견되는 컴포넌트만 반환합니다.
        animator = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// 캐릭터의 움직임 상태를 설정하는 메서드입니다.
    /// 캐릭터의 이동 속도가 0.5보다 크면 움직임 애니메이션을 활성화합니다.
    /// </summary>
    /// <param name="obj">이동 방향과 속도를 나타내는 2D 벡터</param>
    public void Move(Vector2 obj)
    {
        // Vector2.magnitude는 벡터의 길이(크기)를 반환합니다.
        // 여기서는 이동 벡터의 크기가 0.5보다 크면 캐릭터가 움직이는 것으로 판단합니다.
        // SetBool은 애니메이터의 불리언 파라미터 값을 설정합니다.
        animator.SetBool(IsMoving, obj.magnitude > .5f);
    }

    /// <summary>
    /// 캐릭터가 데미지를 입었을 때 호출되는 메서드입니다.
    /// 데미지 애니메이션을 활성화합니다.
    /// </summary>
    public void Damage()
    {
        // 데미지 상태를 true로 설정하여 데미지 애니메이션을 시작합니다.
        animator.SetBool(IsDamage, true);
    }

    /// <summary>
    /// 캐릭터의 무적 상태가 끝났을 때 호출되는 메서드입니다.
    /// 데미지 애니메이션을 비활성화합니다.
    /// </summary>
    public void InvincibilityEnd()
    {
        // 데미지 상태를 false로 설정하여 데미지 애니메이션을 종료합니다.
        animator.SetBool(IsDamage, false);
    }
}
