using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// �÷��̾� ĳ���͸� �����ϴ� Ŭ�����Դϴ�.
/// BaseController�� ��ӹ޾� �⺻ ĳ���� ����� Ȯ���ϰ�,
/// ����� �Է¿� ���� �̵�, ����, ���� ���� ������ ó���մϴ�.
/// </summary>
public class PlayerController : BaseController
{
    // GameManager ������ �����ϴ� �����Դϴ�.
    // GameManager�� ������ ��ü���� ���¿� �帧�� �����ϴ� Ŭ�����Դϴ�.
    private GameManager gameManager;

    // ���� ī�޶� ������ �����ϴ� �����Դϴ�.
    // �̴� ���콺 ��ġ�� ���� ��ǥ�� ��ȯ�� �� ���˴ϴ�.
    private Camera camera;

    /// <summary>
    /// GameManager ������ �����ϴ� �ʱ�ȭ �޼����Դϴ�.
    /// GameManager���� �÷��̾ ������ �� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="gameManager">���� �Ŵ��� ����</param>
    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager; // ���� �Ŵ��� ����
        camera = Camera.main; // ���� ī�޶� ���� ȹ��
        // Camera.main�� MainCamera �±װ� ������ ī�޶� ã�� ����Ƽ ����Դϴ�.
        // ȭ�鿡 ���̴� �� ī�޶� �����ϱ� ���� ���˴ϴ�.
    }

    /// <summary>
    /// �� �����Ӹ��� �÷��̾��� �ൿ�� �����ϴ� �޼����Դϴ�.
    /// ���콺 Ŭ�� �Է��� �����Ͽ� ���� ���¸� �����մϴ�.
    /// BaseController�� HandleAction �޼��带 �������մϴ�.
    /// </summary>
    protected override void HandleAction()
    {
        // ���콺 ���� ��ư�� �������� Ȯ���Ͽ� ���� ���¸� �����մϴ�.
        // Input.GetMouseButtonDown(0)�� ���콺 ���� ��ư�� �̹� �����ӿ� ���ȴ��� Ȯ���մϴ�.
        // 0�� ���� ��ư, 1�� ������ ��ư, 2�� ��� ��ư�� �ǹ��մϴ�.
        isAttacking = Input.GetMouseButtonDown(0);
    }

    /// <summary>
    /// �÷��̾� ĳ���Ͱ� ������� �� ȣ��Ǵ� �޼����Դϴ�.
    /// BaseController�� Death �޼��带 �������ϰ�, �߰��� GameManager�� ���� ���� �˸��� �����ϴ�.
    /// </summary>
    public override void Death()
    {
        // �θ� Ŭ������ Death �޼��带 ���� ȣ���Ͽ� �⺻ ��� ó���� �����մϴ�.
        // �� �޼���� �������� ���߰�, ������ ���߰�, ������Ʈ�� ��Ȱ��ȭ�մϴ�.
        base.Death();
        gameManager.GameOver(); // ���� ���� ó��
        // GameOver �޼���� GameManager���� ���� ���� ���·� ��ȯ�ϰ� 
        // �� ������ �ߴ��ϰ�, ���� ���� UI�� ǥ���ϴ� ���� �۾��� �����մϴ�.
    }

    /// <summary>
    /// �÷��̾��� �̵� �Է��� ó���ϴ� �޼����Դϴ�.
    /// �� �Է� �ý��ۿ� ���� �ڵ����� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="inputValue">�̵� ���� �Է� ��</param>
    void OnMove(InputValue inputValue)
    {
        // Get<Vector2>()�� InputValue���� Vector2 Ÿ���� ���� �����մϴ�.
        // �̵� ������ WASD�� ����Ű ���� �Է¿� ���� �����˴ϴ�.
        movementDirection = inputValue.Get<Vector2>();

        // normalized�� ������ ������ �����ϸ鼭 ũ�⸦ 1�� ����ϴ�.
        // �̷��� �ϸ� �밢�� �̵��� �� ������ �ʰ�, ��� ������ �̵� �ӵ��� �����ϰ� �˴ϴ�.
        movementDirection = movementDirection.normalized;
    }

    /// <summary>
    /// �÷��̾��� ���� �Է��� ó���ϴ� �޼����Դϴ�.
    /// ���콺 ��ġ�� ������� ĳ���Ͱ� �ٶ󺸴� ������ �����մϴ�.
    /// �� �Է� �ý��ۿ� ���� �ڵ����� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="inputValue">���콺 ��ġ �Է� ��</param>
    void OnLook(InputValue inputValue)
    {
        // ���콺 ��ġ�� �����ɴϴ�.
        Vector2 mousePosition = inputValue.Get<Vector2>();

        // ��ũ�� ��ǥ(���콺 ��ġ)�� ���� ��ǥ�� ��ȯ�մϴ�.
        // ScreenToWorldPoint�� ȭ����� ��ġ�� ���� ���� �� ��ġ�� ��ȯ�ϴ� �޼����Դϴ�.
        Vector2 worldPos = camera.ScreenToWorldPoint(mousePosition);

        // �÷��̾�� ���콺 ��ġ���� ���� ���͸� ����ϰ� ����ȭ�մϴ�.
        // (Vector2)transform.position�� ĳ������ ���� ��ġ�� Vector2�� ����ȯ�մϴ�.
        lookDirection = (worldPos - (Vector2)transform.position).normalized;

        // ���� ������ ũ�Ⱑ �ʹ� ������ (���� ���ڸ�) ������ 0���� �����մϴ�.
        // �̴� �̼��� ���������� ���� ���ʿ��� ȸ���� �����մϴ�.
        if (lookDirection.magnitude < 0.9f)
        {
            lookDirection = Vector2.zero;
        }
        else
        {
            // ���� ���͸� ����ȭ�Ͽ� ũ�⸦ 1�� ����ϴ�.
            lookDirection = lookDirection.normalized;
        }
    }

    /// <summary>
    /// �÷��̾��� ���� �Է��� ó���ϴ� �޼����Դϴ�.
    /// ���콺 Ŭ���� UI ��� ���� ���� ���� ���� ������ Ȱ��ȭ�մϴ�.
    /// �� �Է� �ý��ۿ� ���� �ڵ����� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="inputValue">���� ��ư �Է� ��</param>
    void ONFire(InputValue inputValue)
    {
        // ���� ���콺 �����Ͱ� UI ��� ���� �ִ��� Ȯ���մϴ�.
        // IsPointerOverGameObject�� ���콺�� UI ��� ���� ������ true�� ��ȯ�մϴ�.
        // �̴� UI ��ư�� Ŭ���� �� �Ǽ��� �����ϴ� ���� �����մϴ�.
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // isPressed�� ��ư�� ���� �����ִ��� ���θ� ��ȯ�մϴ�.
        // �̸� ���� ���� ��ư�� ������ �ִ� ���� ����ؼ� ������ �� �ֽ��ϴ�.
        isAttacking = inputValue.isPressed;
    }
}