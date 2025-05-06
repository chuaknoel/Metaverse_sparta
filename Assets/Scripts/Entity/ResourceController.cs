using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ĳ������ ü�°� ���õ� �ڿ��� �����ϴ� Ŭ�����Դϴ�.
/// ü�� ��ȭ, ������ ó��, ��� ���� ���� ����մϴ�.
/// </summary>
public class ResourceController : MonoBehaviour
{
    // [SerializeField] �Ӽ��� private ������ ����Ƽ �ν����Ϳ��� ���� �����ϰ� �մϴ�.
    // ü�� ��ȭ �� ���� �ð��� �����ϴ� �����Դϴ�.
    [SerializeField] private float healthChangeDelay = 0.5f;

    // �ٸ� ������Ʈ�鿡 ���� ������ �����մϴ�.
    private BaseController baseController;
    private StatHandler statHandler;
    private AnimationHandler animationHandler;

    // ������ ü�� ��ȭ ���� ��� �ð��� �����ϴ� Ÿ�̸� �����Դϴ�.
    // float.MaxValue�� �ʱ�ȭ�Ͽ� ù ������ ��� ����ǵ��� �մϴ�.
    private float timeSinceLastHealthChange = float.MaxValue;

    // ������Ƽ(Property)�� �ʵ忡 ���� ������ �����ϴ� ����� �����մϴ�.
    // get; private set; ������ �ܺο��� �б�� ���������� ����� �Ұ����ϰ� �մϴ�.

    // ���� ü�� ���� �����ϴ� ������Ƽ�Դϴ�.
    public float CurrentHealth { get; private set; }

    // �ִ� ü�� ���� StatHandler���� �������� �б� ���� ������Ƽ�Դϴ�.
    // => ������ ������ ���� ǥ������ ��ȯ�ϴ� �� ���� ���·� ���ǵǾ����ϴ�.
    public float MaxHealth => statHandler.Health;

    // �������� �޾��� �� ����� ����� Ŭ���� �����ϴ� �����Դϴ�.
    public AudioClip damageClip;

    // ü�� ��ȭ �� ȣ��� �̺�Ʈ ��������Ʈ�Դϴ�.
    // Action<T1, T2>�� T1, T2 Ÿ���� �Ű������� �ް� ��ȯ���� ���� �޼��带 ������ �� �ִ� ��������Ʈ�Դϴ�.
    // �� ��� float, float Ÿ���� �� �Ű�����(���� ü��, �ִ� ü��)�� �޴� �޼��带 ����� �� �ֽ��ϴ�.
    private Action<float, float> OnchangeHealth;

    /// <summary>
    /// Awake�� ��ũ��Ʈ �ν��Ͻ��� �ε��� �� ȣ��Ǵ� ����Ƽ �����ֱ� �޼����Դϴ�.
    /// ������Ʈ ������ �����մϴ�.
    /// </summary>
    private void Awake()
    {
        // GetComponent�� ���� ���ӿ�����Ʈ�� �پ��ִ� ������Ʈ�� �����ɴϴ�.
        baseController = GetComponent<BaseController>();
        statHandler = GetComponent<StatHandler>();
        // GetComponentInChildren�� �ڽ� ������Ʈ�� �����Ͽ� ������Ʈ�� ã���ϴ�.
        animationHandler = GetComponentInChildren<AnimationHandler>();
    }

    /// <summary>
    /// Start�� ù ��° ������ ������Ʈ ������ ȣ��Ǵ� ����Ƽ �����ֱ� �޼����Դϴ�.
    /// �ʱ� ü�� ���� �����մϴ�.
    /// </summary>
    private void Start()
    {
        // ���� ü���� StatHandler�� ������ �ִ� ü������ �ʱ�ȭ�մϴ�.
        CurrentHealth = statHandler.Health;
    }

    /// <summary>
    /// Update�� �� �����Ӹ��� ȣ��Ǵ� ����Ƽ �����ֱ� �޼����Դϴ�.
    /// ü�� ��ȭ �� ���� �ð��� �����մϴ�.
    /// </summary>
    private void Update()
    {
        // ������ ü�� ��ȭ ���� ��� �ð��� ���� �ð����� ������ Ÿ�̸Ӹ� ������ŵ�ϴ�.
        if (timeSinceLastHealthChange < healthChangeDelay)
        {
            // Time.deltaTime�� ���� �����ӿ��� ���� �����ӱ����� ��� �ð��� ��Ÿ���ϴ�.
            timeSinceLastHealthChange += Time.deltaTime;

            // ��� �ð��� ���� �ð��� �ʰ��ϸ� ���� ���¸� �����մϴ�.
            if (timeSinceLastHealthChange >= healthChangeDelay)
            {
                // ������ �ִϸ��̼� ���¸� ������ �޼��带 ȣ���մϴ�.
                animationHandler.InvincibilityEnd();
            }
        }
    }

    /// <summary>
    /// ĳ������ ü���� �����ϴ� �޼����Դϴ�.
    /// �������� �԰ų� ȸ���� �� ���˴ϴ�.
    /// </summary>
    /// <param name="change">ü�� ��ȭ�� (����: ������, ���: ȸ��)</param>
    /// <returns>ü�� ��ȭ�� ����Ǿ����� ���� (true/false)</returns>
    public bool ChangeHealth(float change)
    {
        // ��ȭ���� 0�̰ų� ���� �ð� ���̸� ü�� ��ȭ�� �������� �ʽ��ϴ�.
        if (change == 0 || timeSinceLastHealthChange < healthChangeDelay)
        {
            return false;
        }

        // ���� Ÿ�̸Ӹ� �����մϴ�.
        timeSinceLastHealthChange = 0f;

        // ü���� �����ϰ� �ִ�/�ּ� ���� �����մϴ�.
        CurrentHealth += change;
        // �ִ� ü�º��� �������� �ִ� ü������ �����մϴ�.
        CurrentHealth = CurrentHealth > MaxHealth ? MaxHealth : CurrentHealth;
        // 0���� �۾����� 0���� �����մϴ�.
        CurrentHealth = CurrentHealth < 0 ? 0 : CurrentHealth;

        // ü�� ��ȭ �̺�Ʈ�� ��ϵǾ� ������ ȣ���մϴ�.
        // ?. ������(null ���� ������)�� OnchangeHealth�� null�� �ƴ� ���� Invoke�� ȣ���մϴ�.
        OnchangeHealth?.Invoke(CurrentHealth, MaxHealth);

        // �������� �Ծ��� �� (change�� ������ ��) �߰� ó���� �մϴ�.
        if (change < 0)
        {
            // ������ �ִϸ��̼��� ����մϴ�.
            animationHandler.Damage();

            // ������ ���尡 �����Ǿ� ������ ����մϴ�.
            if (damageClip != null)
            {
                SoundManager.PlayClip(damageClip);
            }
        }

        // ü���� 0 ���Ϸ� �������� ��� ó���� �մϴ�.
        if (CurrentHealth <= 0)
        {
            Death();
        }

        // ü�� ��ȭ�� ���������� ����Ǿ����� ��ȯ�մϴ�.
        return true;
    }

    /// <summary>
    /// ĳ���� ��� ó���� �����ϴ� �޼����Դϴ�.
    /// </summary>
    private void Death()
    {
        // BaseController�� Death �޼��带 ȣ���Ͽ� ��� ó���� �����մϴ�.
        // �� �޼���� �������� ���߰�, ������ ���߰�, ������Ʈ�� ��Ȱ��ȭ�մϴ�.
        baseController.Death();
    }

    /// <summary>
    /// ü�� ��ȭ �̺�Ʈ�� �����ʸ� �߰��ϴ� �޼����Դϴ�.
    /// UI ������Ʈ�� ���� ��� ���� ���� ���˴ϴ�.
    /// </summary>
    /// <param name="action">ü�� ��ȭ �� ȣ��� �޼���</param>
    public void AddHealthCahngeEvent(Action<float, float> action)
    {
        // += �����ڴ� ��������Ʈ�� �� �޼��带 �߰��մϴ�.
        // �̸� ���� ���� �����ʰ� ü�� ��ȭ �̺�Ʈ�� ������ �� �ֽ��ϴ�.
        OnchangeHealth += action;
    }

    /// <summary>
    /// ü�� ��ȭ �̺�Ʈ���� �����ʸ� �����ϴ� �޼����Դϴ�.
    /// </summary>
    /// <param name="action">������ �޼���</param>
    public void RemoveHealthChangeEvent(Action<float, float> action)
    {
        // -= �����ڴ� ��������Ʈ���� �޼��带 �����մϴ�.
        // �̸� ���� �� �̻� �ʿ����� ���� �����ʸ� �̺�Ʈ���� ������ �� �ֽ��ϴ�.
        OnchangeHealth -= action;
    }
}
