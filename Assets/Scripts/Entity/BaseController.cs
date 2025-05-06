using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 모든 캐릭터 컨트롤러의 기본이 되는 클래스입니다.
/// 이동, 회전, 공격 등 캐릭터의 기본 동작을 관리합니다.
/// PlayerController와 EnemyController의 부모 클래스로 사용됩니다.
/// </summary>
public class BaseController : MonoBehaviour
{
    // Rigidbody2D는 유니티에서 물리 기반 움직임을 구현하는 컴포넌트입니다.
    // protected 접근 제한자는 이 클래스와 자식 클래스에서만 접근 가능하게 합니다.
    protected Rigidbody2D _rigidbody;

    // [SerializeField] 속성은 private 변수를 유니티 인스펙터에서 수정 가능하게 합니다.
    // 이를 통해 캡슐화를 유지하면서도 에디터에서 값을 조정할 수 있습니다.
    [SerializeField] private SpriteRenderer characterRenderer;  // 캐릭터 스프라이트 렌더러
    [SerializeField] private Transform weaponPivot;  // 무기 회전 중심점

    // Vector2.zero는 (0,0) 벡터를 의미합니다. 
    // 초기 이동 방향은 정지 상태로 설정됩니다.
    protected Vector2 movementDirection = Vector2.zero;

    // 다른 컴포넌트들에 대한 참조를 저장합니다.
    protected AnimationHandler animationHandler;
    protected StatHandler statHandler;

    // public 필드지만 [SerializeField]로 인스펙터에서 설정 가능합니다.
    [SerializeField] public WeaponHandler weaponPrefab;  // 장착할 무기 프리팹
    protected WeaponHandler weaponHandler;  // 장착된 무기

    // 공격 상태와 타이머를 관리하는 변수입니다.
    protected bool isAttacking;  // 공격 중 여부
    private float timeSinceLastAttack = float.MaxValue;  // 마지막 공격 이후 경과 시간

    // 프로퍼티(Property)는 필드에 대한 접근을 제어하는 방법을 제공합니다.
    // 이 프로퍼티는 읽기 전용(get만 구현)으로, 외부에서 movementDirection 값을 읽을 수 있게 합니다.
    public Vector2 MovementDirection
    {
        get { return movementDirection; }
    }

    // 캐릭터가 바라보는 방향을 저장하는 변수와 프로퍼티입니다.
    // Vector2.right는 (1,0) 벡터로, 기본적으로 오른쪽을 바라봅니다.
    public Vector2 lookDirection = Vector2.right;
    public Vector2 LookDirection
    {
        get { return lookDirection; }
    }

    // 넉백 효과를 위한 변수들입니다.
    private Vector2 knockback = Vector2.zero;  // 넉백 방향과 크기
    private float knockbackDuration = 0.0f;  // 넉백 지속 시간

    /// <summary>
    /// Awake는 스크립트 인스턴스가 로드될 때 호출되는 유니티 생명주기 메서드입니다.
    /// 컴포넌트 참조 설정과 초기화를 수행합니다.
    /// virtual 키워드로 자식 클래스에서 재정의할 수 있습니다.
    /// </summary>
    protected virtual void Awake()
    {
        // GetComponent는 같은 게임오브젝트에 붙어있는 컴포넌트를 가져옵니다.
        // 물리 이동을 위한 Rigidbody2D 컴포넌트 참조를 가져옵니다.
        _rigidbody = GetComponent<Rigidbody2D>();
        animationHandler = GetComponent<AnimationHandler>();
        statHandler = GetComponent<StatHandler>();

        // 무기 생성 및 부착 로직입니다.
        // 프리팹이 지정되어 있다면 새로 생성하고, 없으면 이미 부착된 무기를 사용합니다.
        if (weaponPrefab != null)
            // Instantiate는 프리팹의 복제본을 생성합니다. 두 번째 매개변수는 부모 Transform입니다.
            weaponHandler = Instantiate(weaponPrefab, weaponPivot);
        else
            // GetComponentInChildren은 자식 객체들에서 컴포넌트를 찾습니다.
            weaponHandler = GetComponentInChildren<WeaponHandler>();  // 이미 붙어 있는 무기 사용
    }

    /// <summary>
    /// Start는 첫 번째 프레임 업데이트 직전에 호출되는 유니티 생명주기 메서드입니다.
    /// Awake 이후에 실행되며, 다른 객체들이 초기화된 후 실행되어야 하는 코드에 적합합니다.
    /// </summary>
    protected virtual void Start()
    {
        // 자식 클래스에서 구현할 수 있도록 비워둡니다.
    }

    /// <summary>
    /// Update는 매 프레임마다 호출되는 유니티 생명주기 메서드입니다.
    /// 입력 처리, 움직임 계산 등 주기적인 업데이트가 필요한 로직에 사용됩니다.
    /// </summary>
    protected virtual void Update()
    {
        // 캐릭터의 행동을 처리합니다(움직임, 공격 등).
        HandleAction();
        // 캐릭터가 바라보는 방향으로 회전시킵니다.
        Rotate(lookDirection);
        // 공격 딜레이와 상태를 관리합니다.
        HandleAttackDelay();
    }

    /// <summary>
    /// FixedUpdate는 물리 엔진 업데이트와 동기화된 간격으로 호출되는 유니티 생명주기 메서드입니다.
    /// 물리 관련 계산(이동, 충돌 등)은 Update 대신 FixedUpdate에서 처리하는 것이 좋습니다.
    /// </summary>
    protected virtual void FixedUpdate()
    {
        // 캐릭터 이동을 처리합니다.
        Movement(movementDirection);

        // 넉백 효과가 적용 중이면 타이머를 감소시킵니다.
        if (knockbackDuration > 0.0f)
        {
            // Time.fixedDeltaTime은 FixedUpdate 호출 간격을 나타냅니다.
            knockbackDuration -= Time.fixedDeltaTime;
            if (knockbackDuration <= 0.0f)
            {
                // 넉백 시간이 끝나면 넉백 벡터를 초기화합니다.
                knockback = Vector2.zero;
            }
        }
    }

    /// <summary>
    /// 캐릭터의 행동(이동 방향 설정, 공격 등)을 처리하는 메서드입니다.
    /// 자식 클래스(Player, Enemy)에서 재정의하여 구체적인 동작을 구현합니다.
    /// </summary>
    protected virtual void HandleAction()
    {
        // 자식 클래스에서 구현할 수 있도록 비워둡니다.
    }

    /// <summary>
    /// 캐릭터의 이동을 처리하는 메서드입니다.
    /// 이동 방향과 속도, 넉백 효과를 적용하여 최종 속도를 계산합니다.
    /// </summary>
    /// <param name="direction">이동 방향 벡터(정규화된 벡터)</param>
    private void Movement(Vector2 direction)
    {
        // 이동 방향에 캐릭터의 속도를 곱하여 최종 이동 벡터를 계산합니다.
        direction = direction * statHandler.Speed;

        // 넉백 효과가 적용 중이면 이동 속도를 줄이고 넉백 벡터를 추가합니다.
        if (knockbackDuration > 0.0f)
        {
            direction *= 0.2f;  // 이동 속도를 20%로 감소
            direction += knockback;  // 넉백 방향 추가
        }

        // 애니메이션 핸들러에 이동 정보를 전달합니다.
        animationHandler.Move(direction);
        // Rigidbody2D의 velocity를 설정하여 실제 이동을 구현합니다.
        _rigidbody.velocity = direction;
    }

    /// <summary>
    /// 캐릭터와 무기를 지정된 방향으로 회전시키는 메서드입니다.
    /// </summary>
    /// <param name="direction">회전할 방향 벡터</param>
    private void Rotate(Vector2 direction)
    {
        // Mathf.Atan2는 벡터의 각도(라디안)를 계산합니다. Rad2Deg로 라디안을 도(degree)로 변환합니다.
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // 계산된 각도가 ±90도를 넘으면 왼쪽을 바라보는 것으로 판단합니다.
        bool isLeft = Mathf.Abs(rotZ) > 90f;

        // 캐릭터 스프라이트를 좌우 반전하여 방향에 맞게 표시합니다.
        characterRenderer.flipX = isLeft;

        // 무기 피벗을 회전시켜 무기가 바라보는 방향을 조정합니다.
        if (weaponPivot != null)
        {
            // Quaternion.Euler는 오일러 각도(x, y, z 회전)로부터 회전 쿼터니언을 생성합니다.
            weaponPivot.rotation = Quaternion.Euler(0, 0, rotZ);
        }

        // 무기 핸들러가 있다면 무기도 함께 회전시킵니다.
        // ?. 연산자(null 조건 연산자)는 weaponHandler가 null이 아닐 때만 Rotate 메서드를 호출합니다.
        weaponHandler?.Rotate(isLeft);
    }

    /// <summary>
    /// 다른 객체(공격자)로부터 넉백 효과를 적용하는 메서드입니다.
    /// </summary>
    /// <param name="other">넉백을 가하는 객체의 Transform</param>
    /// <param name="power">넉백 힘의 크기</param>
    /// <param name="duration">넉백 지속 시간</param>
    public void ApplyKnockback(Transform other, float power, float duration)
    {
        knockbackDuration = duration;
        // normalized는 벡터의 방향은 유지하고 크기를 1로 만듭니다(정규화).
        // 이 경우 other에서 this 방향으로의 단위 벡터를 구합니다.
        knockback = (other.position - transform.position).normalized * power;
    }

    /// <summary>
    /// 공격 입력과 쿨다운을 관리하는 메서드입니다.
    /// 공격 딜레이가 끝났을 때 공격을 실행합니다.
    /// </summary>
    private void HandleAttackDelay()
    {
        // 무기가 없으면 공격 처리를 하지 않습니다.
        if (weaponHandler == null)
            return;

        // 공격 쿨다운 중이면 시간 누적
        if (timeSinceLastAttack <= weaponHandler.Delay)
        {
            // Time.deltaTime은 이전 프레임에서 현재 프레임까지의 경과 시간을 나타냅니다.
            timeSinceLastAttack += Time.deltaTime;
        }

        // 공격 입력 중이고 쿨타임이 끝났으면 공격 실행
        if (isAttacking && timeSinceLastAttack > weaponHandler.Delay)
        {
            timeSinceLastAttack = 0;
            Attack();  // 실제 공격 실행
        }
    }

    /// <summary>
    /// 실제 공격을 실행하는 메서드입니다.
    /// 바라보는 방향이 설정되어 있을 때만 공격을 실행합니다.
    /// </summary>
    protected virtual void Attack()
    {
        // 바라보는 방향이 있을 때만 공격
        // != 연산자는 두 객체가 다른지 비교합니다. Vector2.zero와 다르면 방향이 설정된 것입니다.
        if (lookDirection != Vector2.zero)
            // ?. 연산자(null 조건 연산자)는 weaponHandler가 null이 아닐 때만 Attack 메서드를 호출합니다.
            weaponHandler?.Attack();
    }

    /// <summary>
    /// 캐릭터 사망 처리를 위한 메서드입니다.
    /// 움직임 중지, 투명도 조정, 컴포넌트 비활성화 등을 수행합니다.
    /// </summary>
    public virtual void Death()
    {
        // 움직임 정지
        _rigidbody.velocity = Vector3.zero;

        // 모든 SpriteRenderer의 투명도를 낮춰서 죽은 효과 연출
        // GetComponentsInChildren은 자식 객체를 포함한 모든 컴포넌트를 배열로 반환합니다.
        foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            Color color = renderer.color;
            color.a = 0.3f;  // 알파값(투명도) 조정
            renderer.color = color;
        }

        // 모든 컴포넌트(스크립트 포함) 비활성화
        // Behaviour는 MonoBehaviour, Animator 등을 포함하는 기본 클래스입니다.
        foreach (Behaviour component in transform.GetComponentsInChildren<Behaviour>())
        {
            component.enabled = false;
        }

        // 2초 후 오브젝트 파괴
        // Destroy는 지정된 게임 오브젝트나 컴포넌트를 제거합니다.
        // 두 번째 매개변수는 제거까지의 대기 시간입니다.
        Destroy(gameObject, 2f);
    }
}
