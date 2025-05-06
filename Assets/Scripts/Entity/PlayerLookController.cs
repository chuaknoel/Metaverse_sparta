using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 마우스 포인터 방향을 따라 캐릭터의 시야 방향을 제어하는 컨트롤러입니다.
/// 좌우 방향만 바라보며, AnimationHandler와 연동하여 캐릭터를 회전시킵니다.
/// </summary>
public class PlayerLookController : MonoBehaviour
{
    // 카메라 참조
    private Camera mainCamera;

    // 컴포넌트 참조
    private AnimationHandler animationHandler;
    private SpriteRenderer characterRenderer;

    // 마지막 시야 방향
    private Vector2 lookDirection = Vector2.right;

    /// <summary>
    /// 초기화
    /// </summary>
    private void Awake()
    {
        // 메인 카메라 참조 가져오기
        mainCamera = Camera.main;

        // AnimationHandler 참조 가져오기
        animationHandler = GetComponent<AnimationHandler>();

        // 캐릭터의 SpriteRenderer 찾기
        characterRenderer = GetComponentInChildren<SpriteRenderer>();
        if (characterRenderer == null)
        {
            // 연결된 SpriteRenderer가 없을 경우 경고
            Debug.LogWarning("PlayerLookController: SpriteRenderer를 찾을 수 없습니다.");
        }
    }

    /// <summary>
    /// 매 프레임 마우스 위치 확인
    /// </summary>
    private void Update()
    {
        // 메인 카메라가 없으면 리턴
        if (mainCamera == null)
            return;

        // 현재 마우스 위치 가져오기
        Vector2 mousePosition = Input.mousePosition;

        // 마우스 위치를 월드 좌표로 변환
        Vector2 worldMousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        // 캐릭터에서 마우스까지의 방향 벡터 계산
        Vector2 directionToMouse = worldMousePosition - (Vector2)transform.position;

        // 좌우 방향만 고려 (x값만 사용)
        bool isLookingLeft = directionToMouse.x < 0;

        // lookDirection 업데이트 (좌우만 신경쓰기)
        lookDirection = isLookingLeft ? Vector2.left : Vector2.right;

        // 캐릭터 스프라이트 방향 설정
        if (characterRenderer != null)
        {
            characterRenderer.flipX = isLookingLeft;
        }

        // AnimationHandler가 있다면 방향 정보 전달
        if (animationHandler != null)
        {
            // 애니메이션 핸들러에 현재 방향 전달
            // 참고: 기존 미니게임의 AnimationHandler는 방향 설정 메서드가 없을 수 있음
            // 필요시 이 부분을 수정하거나 확장해야 함

            // 예시: BaseController.Rotate 메서드에서 사용하는 방식과 유사하게 구현
            Transform weaponPivot = transform.Find("WeaponPivot");
            if (weaponPivot != null)
            {
                float rotZ = isLookingLeft ? 180f : 0f;
                weaponPivot.rotation = Quaternion.Euler(0, 0, rotZ);
            }

            // WeaponHandler가 있다면 회전 처리
            WeaponHandler weaponHandler = GetComponentInChildren<WeaponHandler>();
            if (weaponHandler != null)
            {
                weaponHandler.Rotate(isLookingLeft);
            }
        }
    }

    /// <summary>
    /// 현재 바라보는 방향 벡터 반환
    /// </summary>
    public Vector2 GetLookDirection()
    {
        return lookDirection;
    }

    /// <summary>
    /// 현재 바라보는 방향이 왼쪽인지 여부 반환
    /// </summary>
    public bool IsLookingLeft()
    {
        return lookDirection.x < 0;
    }
}