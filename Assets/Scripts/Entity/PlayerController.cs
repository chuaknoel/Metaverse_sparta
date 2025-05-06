using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 캐릭터를 제어하는 클래스입니다.
/// BaseController를 상속받아 기본 캐릭터 기능을 확장하고,
/// 사용자 입력에 따라 이동, 조준, 공격 등의 동작을 처리합니다.
/// </summary>
public class PlayerController : BaseController
{
    // GameManager 참조를 저장하는 변수입니다.
    // GameManager는 게임의 전체적인 상태와 흐름을 관리하는 클래스입니다.
    private GameManager gameManager;

    // 메인 카메라 참조를 저장하는 변수입니다.
    // 이는 마우스 위치를 월드 좌표로 변환할 때 사용됩니다.
    private Camera camera;

    /// <summary>
    /// GameManager 참조를 설정하는 초기화 메서드입니다.
    /// GameManager에서 플레이어를 생성할 때 호출됩니다.
    /// </summary>
    /// <param name="gameManager">게임 매니저 참조</param>
    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager; // 게임 매니저 연결
        camera = Camera.main; // 메인 카메라 참조 획득
        // Camera.main은 MainCamera 태그가 지정된 카메라를 찾는 유니티 기능입니다.
        // 화면에 보이는 주 카메라에 접근하기 위해 사용됩니다.
    }

    /// <summary>
    /// 매 프레임마다 플레이어의 행동을 결정하는 메서드입니다.
    /// 마우스 클릭 입력을 감지하여 공격 상태를 설정합니다.
    /// BaseController의 HandleAction 메서드를 재정의합니다.
    /// </summary>
    protected override void HandleAction()
    {
        // 마우스 왼쪽 버튼을 눌렀는지 확인하여 공격 상태를 결정합니다.
        // Input.GetMouseButtonDown(0)은 마우스 왼쪽 버튼이 이번 프레임에 눌렸는지 확인합니다.
        // 0은 왼쪽 버튼, 1은 오른쪽 버튼, 2는 가운데 버튼을 의미합니다.
        isAttacking = Input.GetMouseButtonDown(0);
    }

    /// <summary>
    /// 플레이어 캐릭터가 사망했을 때 호출되는 메서드입니다.
    /// BaseController의 Death 메서드를 재정의하고, 추가로 GameManager에 게임 오버 알림을 보냅니다.
    /// </summary>
    public override void Death()
    {
        // 부모 클래스의 Death 메서드를 먼저 호출하여 기본 사망 처리를 수행합니다.
        // 이 메서드는 움직임을 멈추고, 투명도를 낮추고, 컴포넌트를 비활성화합니다.
        base.Death();
        gameManager.GameOver(); // 게임 오버 처리
        // GameOver 메서드는 GameManager에서 게임 오버 상태로 전환하고 
        // 적 스폰을 중단하고, 게임 오버 UI를 표시하는 등의 작업을 수행합니다.
    }

    /// <summary>
    /// 플레이어의 이동 입력을 처리하는 메서드입니다.
    /// 새 입력 시스템에 의해 자동으로 호출됩니다.
    /// </summary>
    /// <param name="inputValue">이동 방향 입력 값</param>
    void OnMove(InputValue inputValue)
    {
        // Get<Vector2>()는 InputValue에서 Vector2 타입의 값을 추출합니다.
        // 이동 방향은 WASD나 방향키 등의 입력에 따라 결정됩니다.
        movementDirection = inputValue.Get<Vector2>();

        // normalized는 벡터의 방향은 유지하면서 크기를 1로 만듭니다.
        // 이렇게 하면 대각선 이동이 더 빠르지 않고, 모든 방향의 이동 속도가 동일하게 됩니다.
        movementDirection = movementDirection.normalized;
    }

    /// <summary>
    /// 플레이어의 조준 입력을 처리하는 메서드입니다.
    /// 마우스 위치를 기반으로 캐릭터가 바라보는 방향을 설정합니다.
    /// 새 입력 시스템에 의해 자동으로 호출됩니다.
    /// </summary>
    /// <param name="inputValue">마우스 위치 입력 값</param>
    void OnLook(InputValue inputValue)
    {
        // 마우스 위치를 가져옵니다.
        Vector2 mousePosition = inputValue.Get<Vector2>();

        // 스크린 좌표(마우스 위치)를 월드 좌표로 변환합니다.
        // ScreenToWorldPoint는 화면상의 위치를 게임 세계 내 위치로 변환하는 메서드입니다.
        Vector2 worldPos = camera.ScreenToWorldPoint(mousePosition);

        // 플레이어에서 마우스 위치로의 방향 벡터를 계산하고 정규화합니다.
        // (Vector2)transform.position은 캐릭터의 현재 위치를 Vector2로 형변환합니다.
        lookDirection = (worldPos - (Vector2)transform.position).normalized;

        // 방향 벡터의 크기가 너무 작으면 (거의 제자리) 방향을 0으로 설정합니다.
        // 이는 미세한 움직임으로 인한 불필요한 회전을 방지합니다.
        if (lookDirection.magnitude < 0.9f)
        {
            lookDirection = Vector2.zero;
        }
        else
        {
            // 방향 벡터를 정규화하여 크기를 1로 만듭니다.
            lookDirection = lookDirection.normalized;
        }
    }

    /// <summary>
    /// 플레이어의 공격 입력을 처리하는 메서드입니다.
    /// 마우스 클릭이 UI 요소 위에 있지 않을 때만 공격을 활성화합니다.
    /// 새 입력 시스템에 의해 자동으로 호출됩니다.
    /// </summary>
    /// <param name="inputValue">공격 버튼 입력 값</param>
    void ONFire(InputValue inputValue)
    {
        // 현재 마우스 포인터가 UI 요소 위에 있는지 확인합니다.
        // IsPointerOverGameObject는 마우스가 UI 요소 위에 있으면 true를 반환합니다.
        // 이는 UI 버튼을 클릭할 때 실수로 공격하는 것을 방지합니다.
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // isPressed는 버튼이 현재 눌려있는지 여부를 반환합니다.
        // 이를 통해 공격 버튼을 누르고 있는 동안 계속해서 공격할 수 있습니다.
        isAttacking = inputValue.isPressed;
    }
}