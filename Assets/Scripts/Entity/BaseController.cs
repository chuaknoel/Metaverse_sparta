using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// ��� ĳ���� ��Ʈ�ѷ��� �⺻�� �Ǵ� Ŭ�����Դϴ�.
/// �̵�, ȸ��, ���� �� ĳ������ �⺻ ������ �����մϴ�.
/// PlayerController�� EnemyController�� �θ� Ŭ������ ���˴ϴ�.
/// </summary>
public class BaseController : MonoBehaviour
{
    // Rigidbody2D�� ����Ƽ���� ���� ��� �������� �����ϴ� ������Ʈ�Դϴ�.
    // protected ���� �����ڴ� �� Ŭ������ �ڽ� Ŭ���������� ���� �����ϰ� �մϴ�.
    protected Rigidbody2D _rigidbody;

    // [SerializeField] �Ӽ��� private ������ ����Ƽ �ν����Ϳ��� ���� �����ϰ� �մϴ�.
    // �̸� ���� ĸ��ȭ�� �����ϸ鼭�� �����Ϳ��� ���� ������ �� �ֽ��ϴ�.
    [SerializeField] private SpriteRenderer characterRenderer;  // ĳ���� ��������Ʈ ������
    [SerializeField] private Transform weaponPivot;  // ���� ȸ�� �߽���

    // Vector2.zero�� (0,0) ���͸� �ǹ��մϴ�. 
    // �ʱ� �̵� ������ ���� ���·� �����˴ϴ�.
    protected Vector2 movementDirection = Vector2.zero;

    // �ٸ� ������Ʈ�鿡 ���� ������ �����մϴ�.
    protected AnimationHandler animationHandler;
    protected StatHandler statHandler;

    // public �ʵ����� [SerializeField]�� �ν����Ϳ��� ���� �����մϴ�.
    [SerializeField] public WeaponHandler weaponPrefab;  // ������ ���� ������
    protected WeaponHandler weaponHandler;  // ������ ����

    // ���� ���¿� Ÿ�̸Ӹ� �����ϴ� �����Դϴ�.
    protected bool isAttacking;  // ���� �� ����
    private float timeSinceLastAttack = float.MaxValue;  // ������ ���� ���� ��� �ð�

    // ������Ƽ(Property)�� �ʵ忡 ���� ������ �����ϴ� ����� �����մϴ�.
    // �� ������Ƽ�� �б� ����(get�� ����)����, �ܺο��� movementDirection ���� ���� �� �ְ� �մϴ�.
    public Vector2 MovementDirection
    {
        get { return movementDirection; }
    }

    // ĳ���Ͱ� �ٶ󺸴� ������ �����ϴ� ������ ������Ƽ�Դϴ�.
    // Vector2.right�� (1,0) ���ͷ�, �⺻������ �������� �ٶ󺾴ϴ�.
    public Vector2 lookDirection = Vector2.right;
    public Vector2 LookDirection
    {
        get { return lookDirection; }
    }

    // �˹� ȿ���� ���� �������Դϴ�.
    private Vector2 knockback = Vector2.zero;  // �˹� ����� ũ��
    private float knockbackDuration = 0.0f;  // �˹� ���� �ð�

    /// <summary>
    /// Awake�� ��ũ��Ʈ �ν��Ͻ��� �ε�� �� ȣ��Ǵ� ����Ƽ �����ֱ� �޼����Դϴ�.
    /// ������Ʈ ���� ������ �ʱ�ȭ�� �����մϴ�.
    /// virtual Ű����� �ڽ� Ŭ�������� �������� �� �ֽ��ϴ�.
    /// </summary>
    protected virtual void Awake()
    {
        // GetComponent�� ���� ���ӿ�����Ʈ�� �پ��ִ� ������Ʈ�� �����ɴϴ�.
        // ���� �̵��� ���� Rigidbody2D ������Ʈ ������ �����ɴϴ�.
        _rigidbody = GetComponent<Rigidbody2D>();
        animationHandler = GetComponent<AnimationHandler>();
        statHandler = GetComponent<StatHandler>();

        // ���� ���� �� ���� �����Դϴ�.
        // �������� �����Ǿ� �ִٸ� ���� �����ϰ�, ������ �̹� ������ ���⸦ ����մϴ�.
        if (weaponPrefab != null)
            // Instantiate�� �������� �������� �����մϴ�. �� ��° �Ű������� �θ� Transform�Դϴ�.
            weaponHandler = Instantiate(weaponPrefab, weaponPivot);
        else
            // GetComponentInChildren�� �ڽ� ��ü�鿡�� ������Ʈ�� ã���ϴ�.
            weaponHandler = GetComponentInChildren<WeaponHandler>();  // �̹� �پ� �ִ� ���� ���
    }

    /// <summary>
    /// Start�� ù ��° ������ ������Ʈ ������ ȣ��Ǵ� ����Ƽ �����ֱ� �޼����Դϴ�.
    /// Awake ���Ŀ� ����Ǹ�, �ٸ� ��ü���� �ʱ�ȭ�� �� ����Ǿ�� �ϴ� �ڵ忡 �����մϴ�.
    /// </summary>
    protected virtual void Start()
    {
        // �ڽ� Ŭ�������� ������ �� �ֵ��� ����Ӵϴ�.
    }

    /// <summary>
    /// Update�� �� �����Ӹ��� ȣ��Ǵ� ����Ƽ �����ֱ� �޼����Դϴ�.
    /// �Է� ó��, ������ ��� �� �ֱ����� ������Ʈ�� �ʿ��� ������ ���˴ϴ�.
    /// </summary>
    protected virtual void Update()
    {
        // ĳ������ �ൿ�� ó���մϴ�(������, ���� ��).
        HandleAction();
        // ĳ���Ͱ� �ٶ󺸴� �������� ȸ����ŵ�ϴ�.
        Rotate(lookDirection);
        // ���� �����̿� ���¸� �����մϴ�.
        HandleAttackDelay();
    }

    /// <summary>
    /// FixedUpdate�� ���� ���� ������Ʈ�� ����ȭ�� �������� ȣ��Ǵ� ����Ƽ �����ֱ� �޼����Դϴ�.
    /// ���� ���� ���(�̵�, �浹 ��)�� Update ��� FixedUpdate���� ó���ϴ� ���� �����ϴ�.
    /// </summary>
    protected virtual void FixedUpdate()
    {
        // ĳ���� �̵��� ó���մϴ�.
        Movement(movementDirection);

        // �˹� ȿ���� ���� ���̸� Ÿ�̸Ӹ� ���ҽ�ŵ�ϴ�.
        if (knockbackDuration > 0.0f)
        {
            // Time.fixedDeltaTime�� FixedUpdate ȣ�� ������ ��Ÿ���ϴ�.
            knockbackDuration -= Time.fixedDeltaTime;
            if (knockbackDuration <= 0.0f)
            {
                // �˹� �ð��� ������ �˹� ���͸� �ʱ�ȭ�մϴ�.
                knockback = Vector2.zero;
            }
        }
    }

    /// <summary>
    /// ĳ������ �ൿ(�̵� ���� ����, ���� ��)�� ó���ϴ� �޼����Դϴ�.
    /// �ڽ� Ŭ����(Player, Enemy)���� �������Ͽ� ��ü���� ������ �����մϴ�.
    /// </summary>
    protected virtual void HandleAction()
    {
        // �ڽ� Ŭ�������� ������ �� �ֵ��� ����Ӵϴ�.
    }

    /// <summary>
    /// ĳ������ �̵��� ó���ϴ� �޼����Դϴ�.
    /// �̵� ����� �ӵ�, �˹� ȿ���� �����Ͽ� ���� �ӵ��� ����մϴ�.
    /// </summary>
    /// <param name="direction">�̵� ���� ����(����ȭ�� ����)</param>
    private void Movement(Vector2 direction)
    {
        // �̵� ���⿡ ĳ������ �ӵ��� ���Ͽ� ���� �̵� ���͸� ����մϴ�.
        direction = direction * statHandler.Speed;

        // �˹� ȿ���� ���� ���̸� �̵� �ӵ��� ���̰� �˹� ���͸� �߰��մϴ�.
        if (knockbackDuration > 0.0f)
        {
            direction *= 0.2f;  // �̵� �ӵ��� 20%�� ����
            direction += knockback;  // �˹� ���� �߰�
        }

        // �ִϸ��̼� �ڵ鷯�� �̵� ������ �����մϴ�.
        animationHandler.Move(direction);
        // Rigidbody2D�� velocity�� �����Ͽ� ���� �̵��� �����մϴ�.
        _rigidbody.velocity = direction;
    }

    /// <summary>
    /// ĳ���Ϳ� ���⸦ ������ �������� ȸ����Ű�� �޼����Դϴ�.
    /// </summary>
    /// <param name="direction">ȸ���� ���� ����</param>
    private void Rotate(Vector2 direction)
    {
        // Mathf.Atan2�� ������ ����(����)�� ����մϴ�. Rad2Deg�� ������ ��(degree)�� ��ȯ�մϴ�.
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // ���� ������ ��90���� ������ ������ �ٶ󺸴� ������ �Ǵ��մϴ�.
        bool isLeft = Mathf.Abs(rotZ) > 90f;

        // ĳ���� ��������Ʈ�� �¿� �����Ͽ� ���⿡ �°� ǥ���մϴ�.
        characterRenderer.flipX = isLeft;

        // ���� �ǹ��� ȸ������ ���Ⱑ �ٶ󺸴� ������ �����մϴ�.
        if (weaponPivot != null)
        {
            // Quaternion.Euler�� ���Ϸ� ����(x, y, z ȸ��)�κ��� ȸ�� ���ʹϾ��� �����մϴ�.
            weaponPivot.rotation = Quaternion.Euler(0, 0, rotZ);
        }

        // ���� �ڵ鷯�� �ִٸ� ���⵵ �Բ� ȸ����ŵ�ϴ�.
        // ?. ������(null ���� ������)�� weaponHandler�� null�� �ƴ� ���� Rotate �޼��带 ȣ���մϴ�.
        weaponHandler?.Rotate(isLeft);
    }

    /// <summary>
    /// �ٸ� ��ü(������)�κ��� �˹� ȿ���� �����ϴ� �޼����Դϴ�.
    /// </summary>
    /// <param name="other">�˹��� ���ϴ� ��ü�� Transform</param>
    /// <param name="power">�˹� ���� ũ��</param>
    /// <param name="duration">�˹� ���� �ð�</param>
    public void ApplyKnockback(Transform other, float power, float duration)
    {
        knockbackDuration = duration;
        // normalized�� ������ ������ �����ϰ� ũ�⸦ 1�� ����ϴ�(����ȭ).
        // �� ��� other���� this ���������� ���� ���͸� ���մϴ�.
        knockback = (other.position - transform.position).normalized * power;
    }

    /// <summary>
    /// ���� �Է°� ��ٿ��� �����ϴ� �޼����Դϴ�.
    /// ���� �����̰� ������ �� ������ �����մϴ�.
    /// </summary>
    private void HandleAttackDelay()
    {
        // ���Ⱑ ������ ���� ó���� ���� �ʽ��ϴ�.
        if (weaponHandler == null)
            return;

        // ���� ��ٿ� ���̸� �ð� ����
        if (timeSinceLastAttack <= weaponHandler.Delay)
        {
            // Time.deltaTime�� ���� �����ӿ��� ���� �����ӱ����� ��� �ð��� ��Ÿ���ϴ�.
            timeSinceLastAttack += Time.deltaTime;
        }

        // ���� �Է� ���̰� ��Ÿ���� �������� ���� ����
        if (isAttacking && timeSinceLastAttack > weaponHandler.Delay)
        {
            timeSinceLastAttack = 0;
            Attack();  // ���� ���� ����
        }
    }

    /// <summary>
    /// ���� ������ �����ϴ� �޼����Դϴ�.
    /// �ٶ󺸴� ������ �����Ǿ� ���� ���� ������ �����մϴ�.
    /// </summary>
    protected virtual void Attack()
    {
        // �ٶ󺸴� ������ ���� ���� ����
        // != �����ڴ� �� ��ü�� �ٸ��� ���մϴ�. Vector2.zero�� �ٸ��� ������ ������ ���Դϴ�.
        if (lookDirection != Vector2.zero)
            // ?. ������(null ���� ������)�� weaponHandler�� null�� �ƴ� ���� Attack �޼��带 ȣ���մϴ�.
            weaponHandler?.Attack();
    }

    /// <summary>
    /// ĳ���� ��� ó���� ���� �޼����Դϴ�.
    /// ������ ����, ���� ����, ������Ʈ ��Ȱ��ȭ ���� �����մϴ�.
    /// </summary>
    public virtual void Death()
    {
        // ������ ����
        _rigidbody.velocity = Vector3.zero;

        // ��� SpriteRenderer�� ������ ���缭 ���� ȿ�� ����
        // GetComponentsInChildren�� �ڽ� ��ü�� ������ ��� ������Ʈ�� �迭�� ��ȯ�մϴ�.
        foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            Color color = renderer.color;
            color.a = 0.3f;  // ���İ�(����) ����
            renderer.color = color;
        }

        // ��� ������Ʈ(��ũ��Ʈ ����) ��Ȱ��ȭ
        // Behaviour�� MonoBehaviour, Animator ���� �����ϴ� �⺻ Ŭ�����Դϴ�.
        foreach (Behaviour component in transform.GetComponentsInChildren<Behaviour>())
        {
            component.enabled = false;
        }

        // 2�� �� ������Ʈ �ı�
        // Destroy�� ������ ���� ������Ʈ�� ������Ʈ�� �����մϴ�.
        // �� ��° �Ű������� ���ű����� ��� �ð��Դϴ�.
        Destroy(gameObject, 2f);
    }
}
