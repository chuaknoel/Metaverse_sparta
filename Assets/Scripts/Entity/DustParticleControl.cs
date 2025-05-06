using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐릭터가 걸을 때 먼지 파티클 효과를 생성하는 클래스입니다.
/// 애니메이션 이벤트에 의해 호출되어 발자국 먼지 효과를 표현합니다.
/// </summary>
public class DustParticleControl : MonoBehaviour
{
    // [SerializeField] 속성은 private 변수를 유니티 인스펙터에서 수정 가능하게 합니다.
    // 이를 통해 에디터에서 값을 조정할 수 있으면서도 다른 클래스에서 직접 접근은 불가능하게 합니다.

    [SerializeField] private bool createDustOnWalk = true; // 걷기 중 먼지 생성 여부
                                                           // bool 타입은 true/false 값을 저장하는 논리형 자료형입니다.
                                                           // 이 변수를 통해 인스펙터에서 먼지 효과를 켜고 끌 수 있습니다.

    [SerializeField] private ParticleSystem dustParticleSystem; // 먼지 파티클 시스템
    // ParticleSystem은 유니티의 파티클 효과를 제어하는 컴포넌트입니다.
    // 먼지, 연기, 불, 물 등의 효과를 표현할 때 사용됩니다.

    /// <summary>
    /// 먼지 파티클을 생성하는 메서드입니다.
    /// 주로 캐릭터 애니메이션의 특정 프레임에서 Animation Event로 호출됩니다.
    /// 캐릭터가 걸을 때 발이 땅에 닿는 순간 호출하여 먼지 효과를 생성합니다.
    /// </summary>
    public void CreateDustParticles()
    {
        // createDustOnWalk가 true일 때만 먼지 효과를 생성합니다.
        // 이 조건문은 불필요한 파티클 생성을 방지하고, 필요에 따라 효과를 비활성화할 수 있게 합니다.
        if (createDustOnWalk)
        {
            // 먼지 파티클 시스템 정지
            // Stop() 메서드는 현재 재생 중인 파티클 효과를 중지합니다.
            // 새 효과를 생성하기 전에 기존 효과를 중지하여 깔끔한 시각효과를 만듭니다.
            dustParticleSystem.Stop();

            // 먼지 파티클 생성
            // Play() 메서드는 파티클 시스템을 활성화하여 효과를 재생합니다.
            // 먼지 효과는 파티클 시스템의 설정(크기, 색상, 지속시간 등)에 따라 표시됩니다.
            dustParticleSystem.Play();
        }
    }
}
