using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐릭터의 체력과 관련된 자원을 관리하는 클래스입니다.
/// 체력 변화, 데미지 처리, 사망 판정 등을 담당합니다.
/// </summary>
public class ResourceController : MonoBehaviour
{
    // [SerializeField] 속성은 private 변수를 유니티 인스펙터에서 편집 가능하게 합니다.
    // 체력 변화 후 무적 시간을 설정하는 변수입니다.
    [SerializeField] private float healthChangeDelay = 0.5f;

    // 다른 컴포넌트들에 대한 참조를 저장합니다.
    private BaseController baseController;
    private StatHandler statHandler;
    private AnimationHandler animationHandler;

    // 마지막 체력 변화 이후 경과 시간을 추적하는 타이머 변수입니다.
    // float.MaxValue로 초기화하여 첫 공격이 즉시 적용되도록 합니다.
    private float timeSinceLastHealthChange = float.MaxValue;

    // 프로퍼티(Property)는 필드에 대한 접근을 제어하는 방법을 제공합니다.
    // get; private set; 구문은 외부에서 읽기는 가능하지만 쓰기는 불가능하게 합니다.

    // 현재 체력 값을 저장하는 프로퍼티입니다.
    public float CurrentHealth { get; private set; }

    // 최대 체력 값을 StatHandler에서 가져오는 읽기 전용 프로퍼티입니다.
    // => 연산자 다음에 오는 표현식을 반환하는 식 본문 형태로 정의되었습니다.
    public float MaxHealth => statHandler.Health;

    // 데미지를 받았을 때 재생할 오디오 클립을 설정하는 변수입니다.
    public AudioClip damageClip;

    // 체력 변화 시 호출될 이벤트 델리게이트입니다.
    // Action<T1, T2>는 T1, T2 타입의 매개변수를 받고 반환값이 없는 메서드를 참조할 수 있는 델리게이트입니다.
    // 이 경우 float, float 타입의 두 매개변수(현재 체력, 최대 체력)를 받는 메서드를 등록할 수 있습니다.
    private Action<float, float> OnchangeHealth;

    /// <summary>
    /// Awake는 스크립트 인스턴스가 로딩될 때 호출되는 유니티 생명주기 메서드입니다.
    /// 컴포넌트 참조를 설정합니다.
    /// </summary>
    private void Awake()
    {
        // GetComponent는 같은 게임오브젝트에 붙어있는 컴포넌트를 가져옵니다.
        baseController = GetComponent<BaseController>();
        statHandler = GetComponent<StatHandler>();
        // GetComponentInChildren은 자식 오브젝트를 포함하여 컴포넌트를 찾습니다.
        animationHandler = GetComponentInChildren<AnimationHandler>();
    }

    /// <summary>
    /// Start는 첫 번째 프레임 업데이트 직전에 호출되는 유니티 생명주기 메서드입니다.
    /// 초기 체력 값을 설정합니다.
    /// </summary>
    private void Start()
    {
        // 현재 체력을 StatHandler에 설정된 최대 체력으로 초기화합니다.
        CurrentHealth = statHandler.Health;
    }

    /// <summary>
    /// Update는 매 프레임마다 호출되는 유니티 생명주기 메서드입니다.
    /// 체력 변화 후 무적 시간을 관리합니다.
    /// </summary>
    private void Update()
    {
        // 마지막 체력 변화 이후 경과 시간이 무적 시간보다 작으면 타이머를 증가시킵니다.
        if (timeSinceLastHealthChange < healthChangeDelay)
        {
            // Time.deltaTime은 이전 프레임에서 현재 프레임까지의 경과 시간을 나타냅니다.
            timeSinceLastHealthChange += Time.deltaTime;

            // 경과 시간이 무적 시간을 초과하면 무적 상태를 종료합니다.
            if (timeSinceLastHealthChange >= healthChangeDelay)
            {
                // 데미지 애니메이션 상태를 끝내는 메서드를 호출합니다.
                animationHandler.InvincibilityEnd();
            }
        }
    }

    /// <summary>
    /// 캐릭터의 체력을 변경하는 메서드입니다.
    /// 데미지를 입거나 회복할 때 사용됩니다.
    /// </summary>
    /// <param name="change">체력 변화량 (음수: 데미지, 양수: 회복)</param>
    /// <returns>체력 변화가 적용되었는지 여부 (true/false)</returns>
    public bool ChangeHealth(float change)
    {
        // 변화량이 0이거나 무적 시간 중이면 체력 변화를 적용하지 않습니다.
        if (change == 0 || timeSinceLastHealthChange < healthChangeDelay)
        {
            return false;
        }

        // 무적 타이머를 리셋합니다.
        timeSinceLastHealthChange = 0f;

        // 체력을 변경하고 최대/최소 값을 제한합니다.
        CurrentHealth += change;
        // 최대 체력보다 많아지면 최대 체력으로 제한합니다.
        CurrentHealth = CurrentHealth > MaxHealth ? MaxHealth : CurrentHealth;
        // 0보다 작아지면 0으로 제한합니다.
        CurrentHealth = CurrentHealth < 0 ? 0 : CurrentHealth;

        // 체력 변화 이벤트가 등록되어 있으면 호출합니다.
        // ?. 연산자(null 조건 연산자)는 OnchangeHealth가 null이 아닐 때만 Invoke를 호출합니다.
        OnchangeHealth?.Invoke(CurrentHealth, MaxHealth);

        // 데미지를 입었을 때 (change가 음수일 때) 추가 처리를 합니다.
        if (change < 0)
        {
            // 데미지 애니메이션을 재생합니다.
            animationHandler.Damage();

            // 데미지 사운드가 설정되어 있으면 재생합니다.
            if (damageClip != null)
            {
                SoundManager.PlayClip(damageClip);
            }
        }

        // 체력이 0 이하로 떨어지면 사망 처리를 합니다.
        if (CurrentHealth <= 0)
        {
            Death();
        }

        // 체력 변화가 성공적으로 적용되었음을 반환합니다.
        return true;
    }

    /// <summary>
    /// 캐릭터 사망 처리를 수행하는 메서드입니다.
    /// </summary>
    private void Death()
    {
        // BaseController의 Death 메서드를 호출하여 사망 처리를 수행합니다.
        // 이 메서드는 움직임을 멈추고, 투명도를 낮추고, 컴포넌트를 비활성화합니다.
        baseController.Death();
    }

    /// <summary>
    /// 체력 변화 이벤트에 리스너를 추가하는 메서드입니다.
    /// UI 업데이트나 사운드 재생 등을 위해 사용됩니다.
    /// </summary>
    /// <param name="action">체력 변화 시 호출될 메서드</param>
    public void AddHealthCahngeEvent(Action<float, float> action)
    {
        // += 연산자는 델리게이트에 새 메서드를 추가합니다.
        // 이를 통해 여러 리스너가 체력 변화 이벤트를 구독할 수 있습니다.
        OnchangeHealth += action;
    }

    /// <summary>
    /// 체력 변화 이벤트에서 리스너를 제거하는 메서드입니다.
    /// </summary>
    /// <param name="action">제거할 메서드</param>
    public void RemoveHealthChangeEvent(Action<float, float> action)
    {
        // -= 연산자는 델리게이트에서 메서드를 제거합니다.
        // 이를 통해 더 이상 필요하지 않은 리스너를 이벤트에서 해제할 수 있습니다.
        OnchangeHealth -= action;
    }
}
