using System.Collections; // 컬렉션 클래스(리스트 등)를 사용하기 위한 네임스페이스
using System.Collections.Generic; // 제네릭 컬렉션(List<T> 등)을 사용하기 위한 네임스페이스
using UnityEngine; // Unity 엔진 기능을 사용하기 위한 네임스페이스

/// <summary>
/// 근접 무기의 동작을 제어하는 클래스입니다.
/// WeaponHandler를 상속받아 근접 공격에 필요한 기능을 구현합니다.
/// 박스캐스트를 사용하여 공격 범위 내의 적을 감지하고 대미지를 입힙니다.
/// </summary>
public class MeleeWeaponHandler : WeaponHandler // WeaponHandler 클래스를 상속받습니다.
{
    [Header("Melee Attack Info")] // 인스펙터에서 섹션 제목을 표시하는 속성입니다.
    public Vector2 coliderBoxSize = Vector2.one; // 공격 범위를 나타내는 박스 콜라이더의 크기입니다. Vector2.one은 (1,1) 크기입니다.

    /// <summary>
    /// Start 메서드는 첫 프레임 업데이트 직전에 호출되는 Unity 생명주기 메서드입니다.
    /// 부모 클래스의 Start 메서드를 먼저 호출하고, 콜라이더 크기를 설정합니다.
    /// </summary>
    protected override void Start() // 부모 클래스의 Start 메서드를 재정의합니다.
    {
        base.Start(); // 부모 클래스의 Start 메서드를 호출합니다.
        // 콜라이더 크기 설정
        coliderBoxSize = coliderBoxSize * WeaponSize; // 무기 크기에 비례하여 콜라이더 크기 조정
    }

    /// <summary>
    /// 근접 공격을 실행하는 메서드입니다.
    /// 부모 클래스의 Attack 메서드를 호출한 후, 공격 범위 내에 있는 적을 감지하고 대미지를 입힙니다.
    /// </summary>
    public override void Attack() // 부모 클래스의 Attack 메서드를 재정의합니다.
    {
        base.Attack(); // 부모 클래스의 Attack 메서드를 호출합니다 (애니메이션 재생 등).

        // BoxCast는 지정된 위치에서 지정된 크기의 상자 모양으로 레이캐스트를 수행합니다.
        // transform.position: 무기의 현재 위치
        // (Vector3)Controller.LookDirection * coliderBoxSize.x: 캐릭터가 바라보는 방향으로 콜라이더 오프셋
        // coliderBoxSize: 콜라이더 크기
        // 0: 회전 각도 (여기서는 회전 없음)
        // Vector2.zero: 방향 (여기서는 방향 없이 그 자리에서 체크)
        // 0: 거리 (여기서는 거리 없이 그 자리에서 체크)
        // target: 타겟 레이어마스크 (무기가 감지할 레이어)
        RaycastHit2D hit = Physics2D.BoxCast(transform.position +
            (Vector3)Controller.LookDirection * coliderBoxSize.x, coliderBoxSize, 0, Vector2.zero, 0, target);

        // 레이캐스트에 물체가 감지되었다면
        if (hit.collider != null)
        {
            // 감지된 물체에서 ResourceController 컴포넌트를 가져옵니다.
            ResourceController resourceController = hit.collider.GetComponent<ResourceController>();
            if (resourceController != null) // ResourceController가 있다면 (체력 시스템이 있는 물체라면)
            {
                resourceController.ChangeHealth(-Power); // 대미지를 입힙니다 (Power 만큼의 체력 감소).

                // 넉백 기능이 활성화되어 있다면
                if (IsOnKnockback)
                {
                    // 대상의 BaseController 컴포넌트를 가져옵니다.
                    BaseController controller = resourceController.GetComponent<BaseController>();
                    if (controller != null)
                    {
                        // 넉백을 적용합니다 (이 무기의 위치에서 대상 방향으로, 설정된 힘과 시간만큼).
                        controller.ApplyKnockback(transform, KnockbackPower, KnockbackTime);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 무기를 회전시키는 메서드입니다.
    /// 캐릭터가 바라보는 방향에 따라 무기의 방향을 설정합니다.
    /// </summary>
    /// <param name="isLeft">캐릭터가 왼쪽을 바라보는지 여부</param>
    public override void Rotate(bool isLeft) // 부모 클래스의 Rotate 메서드를 재정의합니다.
    {
        if (isLeft) // 캐릭터가 왼쪽을 바라본다면
        {
            transform.eulerAngles = new Vector3(0, 180, 0); // Y축을 기준으로 180도 회전 (왼쪽을 향함)
        }
        else // 캐릭터가 오른쪽을 바라본다면
        {
            transform.eulerAngles = new Vector3(0, 0, 0); // 회전하지 않음 (오른쪽을 향함)
        }
    }
}