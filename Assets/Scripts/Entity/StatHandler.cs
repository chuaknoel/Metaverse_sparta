using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐릭터의 기본 스탯(체력, 이동 속도 등)을 관리하는 클래스입니다.
/// 이 클래스는 각 캐릭터의 기본 능력치를 정의하고, 이 값들을 다른 컴포넌트에서 사용할 수 있게 합니다.
/// </summary>
public class StatHandler : MonoBehaviour
{
    // [Range] 속성은 유니티 인스펙터에서 슬라이더를 통해 값을 조정할 수 있게 해줍니다.
    // 이 경우 체력을 1~100 사이의 값으로 제한합니다.
    // [SerializeField] 속성은 private 변수를 유니티 인스펙터에서 수정 가능하게 합니다.
    [Range(1, 100)][SerializeField] private int health = 10;

    // 프로퍼티(Property)는 필드에 대한 접근을 제어하는 방법을 제공합니다.
    // 이 프로퍼티는 health 변수에 대한 접근과 수정을 제어합니다.
    public int Health
    {
        // get 접근자는 health 값을 반환합니다.
        // => 연산자를 사용한 식 본문 형태로 간결하게 작성되었습니다.
        get => health;

        // set 접근자는 health 값을 설정하되, 유효한 범위(0~100) 내로 제한합니다.
        // value는 set 접근자에 전달된 값을 나타내는 암시적 매개변수입니다.
        // Mathf.Clamp는 값을 지정된 최소값과 최대값 사이로 제한하는 유니티 함수입니다.
        set => health = Mathf.Clamp(value, 0, 100);
    }

    // 이동 속도 변수와 프로퍼티입니다.
    // 이동 속도는 1~20 사이의 값으로 제한됩니다.
    [Range(1f, 20f)][SerializeField] private float speed = 3;

    // 이동 속도에 대한 프로퍼티로, 위의 Health 프로퍼티와 유사하게 작동합니다.
    public float Speed
    {
        // 현재 speed 값을 반환합니다.
        get => speed;

        // speed 값을 설정하되, 1~20 사이로 제한합니다.
        set => speed = Mathf.Clamp(value, 1f, 20f);
    }
}
