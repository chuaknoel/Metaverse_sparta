using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [Header("Attack Info")]
    [SerializeField] private float delay = 1f; // 공격 간 딜레이
    public float Delay { get => delay; set => delay = value; }

    [SerializeField] private float weaponSize = 1f; // 무기 크기
    public float WeaponSize { get => weaponSize; set => weaponSize = value; }

    [SerializeField] private float power = 1f; // 공격력
    public float Power { get => power; set => power = value; }

    [SerializeField] private float speed = 1f; // 공격 속도
    public float Speed { get => speed; set => speed = value; }

    [SerializeField] private float attackRange = 10f; // 공격 가능 범위
    public float AttackRange { get => attackRange; set => attackRange = value; }

    public LayerMask target; // 공격 가능한 대상 레이어

    [Header("Knock Back Info")]
    [SerializeField] private bool isOnKnockback = false; // 넉백 적용 여부
    public bool IsOnKnockback { get => isOnKnockback; set => isOnKnockback = value; }

    [SerializeField] private float knockbackPower = 0.1f; // 넉백 힘
    public float KnockbackPower { get => knockbackPower; set => knockbackPower = value; }

    [SerializeField] private float knockbackTime = 0.5f; // 넉백 지속 시간
    public float KnockbackTime { get => knockbackTime; set => knockbackTime = value; }

    private static readonly int IsAttack = Animator.StringToHash("IsAttack");

    public BaseController Controller { get; private set; } // 이 무기를 사용하는 캐릭터 컨트롤러

    private Animator animator;
    private SpriteRenderer weaponRenderer;

    public AudioClip attackSoundClip;

    protected virtual void Awake()
    {
        Controller = GetComponentInParent<BaseController>();
        animator = GetComponentInChildren<Animator>();
        weaponRenderer = GetComponentInChildren<SpriteRenderer>();

        // 공격 속도에 따라 애니메이션 재생 속도 조절
        animator.speed = 1.0f / delay;

        // 무기 크기 설정
        transform.localScale = Vector3.one * weaponSize;
    }

    protected virtual void Start()
    {

    }

    public virtual void Attack()
    {
        AttackAnimation();

        if (attackSoundClip != null)
        {
            SoundManager.PlayClip(attackSoundClip);
        }
    }

    public void AttackAnimation()
    {
        animator.SetTrigger(IsAttack);
    }

    public virtual void Rotate(bool isLeft)
    {
        weaponRenderer.flipY = isLeft;
    }


}

