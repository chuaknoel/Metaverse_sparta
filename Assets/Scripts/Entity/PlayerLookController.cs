using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���콺 ������ ������ ���� ĳ������ �þ� ������ �����ϴ� ��Ʈ�ѷ��Դϴ�.
/// �¿� ���⸸ �ٶ󺸸�, AnimationHandler�� �����Ͽ� ĳ���͸� ȸ����ŵ�ϴ�.
/// </summary>
public class PlayerLookController : MonoBehaviour
{
    // ī�޶� ����
    private Camera mainCamera;

    // ������Ʈ ����
    private AnimationHandler animationHandler;
    private SpriteRenderer characterRenderer;

    // ������ �þ� ����
    private Vector2 lookDirection = Vector2.right;

    /// <summary>
    /// �ʱ�ȭ
    /// </summary>
    private void Awake()
    {
        // ���� ī�޶� ���� ��������
        mainCamera = Camera.main;

        // AnimationHandler ���� ��������
        animationHandler = GetComponent<AnimationHandler>();

        // ĳ������ SpriteRenderer ã��
        characterRenderer = GetComponentInChildren<SpriteRenderer>();
        if (characterRenderer == null)
        {
            // ����� SpriteRenderer�� ���� ��� ���
            Debug.LogWarning("PlayerLookController: SpriteRenderer�� ã�� �� �����ϴ�.");
        }
    }

    /// <summary>
    /// �� ������ ���콺 ��ġ Ȯ��
    /// </summary>
    private void Update()
    {
        // ���� ī�޶� ������ ����
        if (mainCamera == null)
            return;

        // ���� ���콺 ��ġ ��������
        Vector2 mousePosition = Input.mousePosition;

        // ���콺 ��ġ�� ���� ��ǥ�� ��ȯ
        Vector2 worldMousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        // ĳ���Ϳ��� ���콺������ ���� ���� ���
        Vector2 directionToMouse = worldMousePosition - (Vector2)transform.position;

        // �¿� ���⸸ ��� (x���� ���)
        bool isLookingLeft = directionToMouse.x < 0;

        // lookDirection ������Ʈ (�¿츸 �Ű澲��)
        lookDirection = isLookingLeft ? Vector2.left : Vector2.right;

        // ĳ���� ��������Ʈ ���� ����
        if (characterRenderer != null)
        {
            characterRenderer.flipX = isLookingLeft;
        }

        // AnimationHandler�� �ִٸ� ���� ���� ����
        if (animationHandler != null)
        {
            // �ִϸ��̼� �ڵ鷯�� ���� ���� ����
            // ����: ���� �̴ϰ����� AnimationHandler�� ���� ���� �޼��尡 ���� �� ����
            // �ʿ�� �� �κ��� �����ϰų� Ȯ���ؾ� ��

            // ����: BaseController.Rotate �޼��忡�� ����ϴ� ��İ� �����ϰ� ����
            Transform weaponPivot = transform.Find("WeaponPivot");
            if (weaponPivot != null)
            {
                float rotZ = isLookingLeft ? 180f : 0f;
                weaponPivot.rotation = Quaternion.Euler(0, 0, rotZ);
            }

            // WeaponHandler�� �ִٸ� ȸ�� ó��
            WeaponHandler weaponHandler = GetComponentInChildren<WeaponHandler>();
            if (weaponHandler != null)
            {
                weaponHandler.Rotate(isLookingLeft);
            }
        }
    }

    /// <summary>
    /// ���� �ٶ󺸴� ���� ���� ��ȯ
    /// </summary>
    public Vector2 GetLookDirection()
    {
        return lookDirection;
    }

    /// <summary>
    /// ���� �ٶ󺸴� ������ �������� ���� ��ȯ
    /// </summary>
    public bool IsLookingLeft()
    {
        return lookDirection.x < 0;
    }
}