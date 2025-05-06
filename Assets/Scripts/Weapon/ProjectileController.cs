using System.Collections; // 컬렉션 클래스(리스트 등)를 사용하기 위한 네임스페이스
using System.Collections.Generic; // 제네릭 컬렉션(List<T> 등)을 사용하기 위한 네임스페이스
using UnityEngine; // Unity 엔진 기능을 사용하기 위한 네임스페이스

/// <summary>
/// 발사체의 동작을 제어하는 클래스입니다.
/// 발사체의 이동, 충돌 처리, 지속 시간 등을 관리합니다.
/// </summary>
public class ProjectileController : MonoBehaviour // MonoBehaviour는 Unity의 기본 컴포넌트 클래스입니다.
{
    [SerializeField] private LayerMask levelCollisionLayer; // 발사체가 충돌할 레벨(환경) 레이어 마스크입니다.

    private RangeWeaponHandler rangeWeaponHandler; // 이 발사체를 생성한 원거리 무기 핸들러 참조

    private float currentDuration; // 발사체가 생성된 후 경과 시간
    private Vector2 direction; // 발사체의 이동 방향
    private bool isReady; // 발사체가 초기화되었는지 여부
    private Transform pivot; // 발사체의 회전 기준점 (자식 오브젝트)

    private Rigidbody2D _rigidbody; // 물리 이동을 위한 Rigidbody2D 컴포넌트
    private SpriteRenderer spriteRenderer; // 발사체의 시각적 표현을 위한 SpriteRenderer 컴포넌트

    public bool fxOnDestroy = true; // 발사체가 제거될 때 이펙트를 생성할지 여부
    //삭제될때 이펙트 나올지 여부

    ProjectileManager ProjectileManager; // 발사체 관리자 참조

    /// <summary>
    /// Awake는 스크립트 인스턴스가 로드될 때 호출되는 Unity 생명주기 메서드입니다.
    /// 컴포넌트 참조를 설정합니다.
    /// </summary>
    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(); // 자식 오브젝트에서 SpriteRenderer 컴포넌트를 찾습니다.
        _rigidbody = GetComponent<Rigidbody2D>(); // 이 게임 오브젝트의 Rigidbody2D 컴포넌트를 가져옵니다.
        pivot = transform.GetChild(0); // 첫 번째 자식 오브젝트를 피벗(회전 중심점)으로 설정합니다.
    }

    /// <summary>
    /// Update는 매 프레임마다 호출되는 Unity 생명주기 메서드입니다.
    /// 발사체의 지속 시간을 관리하고 이동을 처리합니다.
    /// </summary>
    private void Update()
    {
        if (!isReady) // 발사체가 아직 초기화되지 않았다면
        {
            return; // 메서드를 종료합니다.
        }

        currentDuration += Time.deltaTime; // 경과 시간을 업데이트합니다.

        // 발사체의 지속 시간이 초과되었다면
        if (currentDuration > rangeWeaponHandler.Duration)
        {
            DestroyProjectile(transform.position, false); // 발사체를 제거합니다 (이펙트 없이).
        }

        // 발사체를 설정된 방향과 속도로 이동시킵니다.
        _rigidbody.velocity = direction * rangeWeaponHandler.Speed;
    }

    /// <summary>
    /// OnTriggerEnter2D는 트리거 콜라이더와 다른 콜라이더가 접촉하기 시작할 때 호출되는 Unity 메서드입니다.
    /// 발사체가 레벨이나 적과 충돌했을 때의 처리를 담당합니다.
    /// </summary>
    /// <param name="collision">충돌한 콜라이더</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 레벨 충돌 레이어와 충돌했다면 (환경, 벽 등)
        // 비트 연산자 |는 OR 연산을 수행하여 두 레이어 마스크의 비트를 결합합니다.
        // 1 << collision.gameObject.layer는 충돌한 오브젝트의 레이어 비트를 생성합니다.
        if (levelCollisionLayer.value == (levelCollisionLayer.value | (1 << collision.gameObject.layer)))
        {
            // 발사체를 제거합니다. 충돌 지점에서 약간 뒤로 조정한 위치를 사용합니다.
            DestroyProjectile(collision.ClosestPoint(transform.position) - direction * .2f, fxOnDestroy);
        }
        // 대상 레이어(적 또는 플레이어)와 충돌했다면
        else if (rangeWeaponHandler.target.value == (rangeWeaponHandler.target.value | (1 << collision.gameObject.layer)))
        {
            // 대미지 적용을 위해 체력 시스템이 있는지 확인
            ResourceController resourceController = collision.GetComponent<ResourceController>();
            if (resourceController != null)
            {
                // 대미지 적용
                resourceController.ChangeHealth(-rangeWeaponHandler.Power);

                // 넉백 설정이 되어 있다면 넉백도 적용
                if (rangeWeaponHandler.IsOnKnockback)
                {
                    BaseController controller = collision.GetComponent<BaseController>();
                    if (controller != null)
                    {
                        controller.ApplyKnockback(transform, rangeWeaponHandler.KnockbackPower, rangeWeaponHandler.KnockbackTime);
                    }
                }
            }

            // 발사체 제거 (이펙트 포함)
            DestroyProjectile(collision.ClosestPoint(transform.position), fxOnDestroy);
        }
    }

    /// <summary>
    /// 발사체를 초기화하는 메서드입니다.
    /// 원거리 무기 핸들러로부터 필요한 속성을 설정받습니다.
    /// </summary>
    /// <param name="direction">발사체의 이동 방향</param>
    /// <param name="weaponHandler">이 발사체를 생성한 무기 핸들러</param>
    /// <param name="projectileManager">발사체 관리자</param>
    public void Init(Vector2 direction, RangeWeaponHandler weaponHandler, ProjectileManager projectileManager)
    {
        this.ProjectileManager = projectileManager; // 발사체 관리자 참조 설정
        rangeWeaponHandler = weaponHandler; // 무기 핸들러 참조 설정

        this.direction = direction; // 이동 방향 설정
        currentDuration = 0; // 경과 시간 초기화
        transform.localScale = Vector3.one * weaponHandler.BulletSize; // 발사체 크기 설정
        spriteRenderer.color = weaponHandler.ProjectileColor; // 발사체 색상 설정

        transform.right = this.direction; // 발사체가 이동 방향을 바라보도록 회전

        // 방향에 따라 피벗 회전 (왼쪽이나 오른쪽을 향하도록)
        if (this.direction.x < 0)
            pivot.localRotation = Quaternion.Euler(180, 0, 0); // 왼쪽 방향
        else
            pivot.localRotation = Quaternion.Euler(0, 0, 0); // 오른쪽 방향

        isReady = true; // 초기화 완료 표시
    }

    /// <summary>
    /// 발사체를 제거하는 메서드입니다.
    /// 필요에 따라 충돌 이펙트를 생성합니다.
    /// </summary>
    /// <param name="position">발사체가 제거될 위치</param>
    /// <param name="createFx">이펙트를 생성할지 여부</param>
    private void DestroyProjectile(Vector3 position, bool createFx)
    {
        if (createFx) // 이펙트를 생성해야 한다면
        {
            // 발사체 관리자를 통해 충돌 파티클 이펙트를 생성합니다.
            ProjectileManager.CreateImpactParticlesAtPosition(position, rangeWeaponHandler);
        }

        Destroy(gameObject); // 발사체 게임 오브젝트를 제거합니다.
    }
}
