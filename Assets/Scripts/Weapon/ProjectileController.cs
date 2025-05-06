using System.Collections; // �÷��� Ŭ����(����Ʈ ��)�� ����ϱ� ���� ���ӽ����̽�
using System.Collections.Generic; // ���׸� �÷���(List<T> ��)�� ����ϱ� ���� ���ӽ����̽�
using UnityEngine; // Unity ���� ����� ����ϱ� ���� ���ӽ����̽�

/// <summary>
/// �߻�ü�� ������ �����ϴ� Ŭ�����Դϴ�.
/// �߻�ü�� �̵�, �浹 ó��, ���� �ð� ���� �����մϴ�.
/// </summary>
public class ProjectileController : MonoBehaviour // MonoBehaviour�� Unity�� �⺻ ������Ʈ Ŭ�����Դϴ�.
{
    [SerializeField] private LayerMask levelCollisionLayer; // �߻�ü�� �浹�� ����(ȯ��) ���̾� ����ũ�Դϴ�.

    private RangeWeaponHandler rangeWeaponHandler; // �� �߻�ü�� ������ ���Ÿ� ���� �ڵ鷯 ����

    private float currentDuration; // �߻�ü�� ������ �� ��� �ð�
    private Vector2 direction; // �߻�ü�� �̵� ����
    private bool isReady; // �߻�ü�� �ʱ�ȭ�Ǿ����� ����
    private Transform pivot; // �߻�ü�� ȸ�� ������ (�ڽ� ������Ʈ)

    private Rigidbody2D _rigidbody; // ���� �̵��� ���� Rigidbody2D ������Ʈ
    private SpriteRenderer spriteRenderer; // �߻�ü�� �ð��� ǥ���� ���� SpriteRenderer ������Ʈ

    public bool fxOnDestroy = true; // �߻�ü�� ���ŵ� �� ����Ʈ�� �������� ����
    //�����ɶ� ����Ʈ ������ ����

    ProjectileManager ProjectileManager; // �߻�ü ������ ����

    /// <summary>
    /// Awake�� ��ũ��Ʈ �ν��Ͻ��� �ε�� �� ȣ��Ǵ� Unity �����ֱ� �޼����Դϴ�.
    /// ������Ʈ ������ �����մϴ�.
    /// </summary>
    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(); // �ڽ� ������Ʈ���� SpriteRenderer ������Ʈ�� ã���ϴ�.
        _rigidbody = GetComponent<Rigidbody2D>(); // �� ���� ������Ʈ�� Rigidbody2D ������Ʈ�� �����ɴϴ�.
        pivot = transform.GetChild(0); // ù ��° �ڽ� ������Ʈ�� �ǹ�(ȸ�� �߽���)���� �����մϴ�.
    }

    /// <summary>
    /// Update�� �� �����Ӹ��� ȣ��Ǵ� Unity �����ֱ� �޼����Դϴ�.
    /// �߻�ü�� ���� �ð��� �����ϰ� �̵��� ó���մϴ�.
    /// </summary>
    private void Update()
    {
        if (!isReady) // �߻�ü�� ���� �ʱ�ȭ���� �ʾҴٸ�
        {
            return; // �޼��带 �����մϴ�.
        }

        currentDuration += Time.deltaTime; // ��� �ð��� ������Ʈ�մϴ�.

        // �߻�ü�� ���� �ð��� �ʰ��Ǿ��ٸ�
        if (currentDuration > rangeWeaponHandler.Duration)
        {
            DestroyProjectile(transform.position, false); // �߻�ü�� �����մϴ� (����Ʈ ����).
        }

        // �߻�ü�� ������ ����� �ӵ��� �̵���ŵ�ϴ�.
        _rigidbody.velocity = direction * rangeWeaponHandler.Speed;
    }

    /// <summary>
    /// OnTriggerEnter2D�� Ʈ���� �ݶ��̴��� �ٸ� �ݶ��̴��� �����ϱ� ������ �� ȣ��Ǵ� Unity �޼����Դϴ�.
    /// �߻�ü�� �����̳� ���� �浹���� ���� ó���� ����մϴ�.
    /// </summary>
    /// <param name="collision">�浹�� �ݶ��̴�</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ���� �浹 ���̾�� �浹�ߴٸ� (ȯ��, �� ��)
        // ��Ʈ ������ |�� OR ������ �����Ͽ� �� ���̾� ����ũ�� ��Ʈ�� �����մϴ�.
        // 1 << collision.gameObject.layer�� �浹�� ������Ʈ�� ���̾� ��Ʈ�� �����մϴ�.
        if (levelCollisionLayer.value == (levelCollisionLayer.value | (1 << collision.gameObject.layer)))
        {
            // �߻�ü�� �����մϴ�. �浹 �������� �ణ �ڷ� ������ ��ġ�� ����մϴ�.
            DestroyProjectile(collision.ClosestPoint(transform.position) - direction * .2f, fxOnDestroy);
        }
        // ��� ���̾�(�� �Ǵ� �÷��̾�)�� �浹�ߴٸ�
        else if (rangeWeaponHandler.target.value == (rangeWeaponHandler.target.value | (1 << collision.gameObject.layer)))
        {
            // ����� ������ ���� ü�� �ý����� �ִ��� Ȯ��
            ResourceController resourceController = collision.GetComponent<ResourceController>();
            if (resourceController != null)
            {
                // ����� ����
                resourceController.ChangeHealth(-rangeWeaponHandler.Power);

                // �˹� ������ �Ǿ� �ִٸ� �˹鵵 ����
                if (rangeWeaponHandler.IsOnKnockback)
                {
                    BaseController controller = collision.GetComponent<BaseController>();
                    if (controller != null)
                    {
                        controller.ApplyKnockback(transform, rangeWeaponHandler.KnockbackPower, rangeWeaponHandler.KnockbackTime);
                    }
                }
            }

            // �߻�ü ���� (����Ʈ ����)
            DestroyProjectile(collision.ClosestPoint(transform.position), fxOnDestroy);
        }
    }

    /// <summary>
    /// �߻�ü�� �ʱ�ȭ�ϴ� �޼����Դϴ�.
    /// ���Ÿ� ���� �ڵ鷯�κ��� �ʿ��� �Ӽ��� �����޽��ϴ�.
    /// </summary>
    /// <param name="direction">�߻�ü�� �̵� ����</param>
    /// <param name="weaponHandler">�� �߻�ü�� ������ ���� �ڵ鷯</param>
    /// <param name="projectileManager">�߻�ü ������</param>
    public void Init(Vector2 direction, RangeWeaponHandler weaponHandler, ProjectileManager projectileManager)
    {
        this.ProjectileManager = projectileManager; // �߻�ü ������ ���� ����
        rangeWeaponHandler = weaponHandler; // ���� �ڵ鷯 ���� ����

        this.direction = direction; // �̵� ���� ����
        currentDuration = 0; // ��� �ð� �ʱ�ȭ
        transform.localScale = Vector3.one * weaponHandler.BulletSize; // �߻�ü ũ�� ����
        spriteRenderer.color = weaponHandler.ProjectileColor; // �߻�ü ���� ����

        transform.right = this.direction; // �߻�ü�� �̵� ������ �ٶ󺸵��� ȸ��

        // ���⿡ ���� �ǹ� ȸ�� (�����̳� �������� ���ϵ���)
        if (this.direction.x < 0)
            pivot.localRotation = Quaternion.Euler(180, 0, 0); // ���� ����
        else
            pivot.localRotation = Quaternion.Euler(0, 0, 0); // ������ ����

        isReady = true; // �ʱ�ȭ �Ϸ� ǥ��
    }

    /// <summary>
    /// �߻�ü�� �����ϴ� �޼����Դϴ�.
    /// �ʿ信 ���� �浹 ����Ʈ�� �����մϴ�.
    /// </summary>
    /// <param name="position">�߻�ü�� ���ŵ� ��ġ</param>
    /// <param name="createFx">����Ʈ�� �������� ����</param>
    private void DestroyProjectile(Vector3 position, bool createFx)
    {
        if (createFx) // ����Ʈ�� �����ؾ� �Ѵٸ�
        {
            // �߻�ü �����ڸ� ���� �浹 ��ƼŬ ����Ʈ�� �����մϴ�.
            ProjectileManager.CreateImpactParticlesAtPosition(position, rangeWeaponHandler);
        }

        Destroy(gameObject); // �߻�ü ���� ������Ʈ�� �����մϴ�.
    }
}
