using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ĳ������ �⺻ ����(ü��, �̵� �ӵ� ��)�� �����ϴ� Ŭ�����Դϴ�.
/// �� Ŭ������ �� ĳ������ �⺻ �ɷ�ġ�� �����ϰ�, �� ������ �ٸ� ������Ʈ���� ����� �� �ְ� �մϴ�.
/// </summary>
public class StatHandler : MonoBehaviour
{
    // [Range] �Ӽ��� ����Ƽ �ν����Ϳ��� �����̴��� ���� ���� ������ �� �ְ� ���ݴϴ�.
    // �� ��� ü���� 1~100 ������ ������ �����մϴ�.
    // [SerializeField] �Ӽ��� private ������ ����Ƽ �ν����Ϳ��� ���� �����ϰ� �մϴ�.
    [Range(1, 100)][SerializeField] private int health = 10;

    // ������Ƽ(Property)�� �ʵ忡 ���� ������ �����ϴ� ����� �����մϴ�.
    // �� ������Ƽ�� health ������ ���� ���ٰ� ������ �����մϴ�.
    public int Health
    {
        // get �����ڴ� health ���� ��ȯ�մϴ�.
        // => �����ڸ� ����� �� ���� ���·� �����ϰ� �ۼ��Ǿ����ϴ�.
        get => health;

        // set �����ڴ� health ���� �����ϵ�, ��ȿ�� ����(0~100) ���� �����մϴ�.
        // value�� set �����ڿ� ���޵� ���� ��Ÿ���� �Ͻ��� �Ű������Դϴ�.
        // Mathf.Clamp�� ���� ������ �ּҰ��� �ִ밪 ���̷� �����ϴ� ����Ƽ �Լ��Դϴ�.
        set => health = Mathf.Clamp(value, 0, 100);
    }

    // �̵� �ӵ� ������ ������Ƽ�Դϴ�.
    // �̵� �ӵ��� 1~20 ������ ������ ���ѵ˴ϴ�.
    [Range(1f, 20f)][SerializeField] private float speed = 3;

    // �̵� �ӵ��� ���� ������Ƽ��, ���� Health ������Ƽ�� �����ϰ� �۵��մϴ�.
    public float Speed
    {
        // ���� speed ���� ��ȯ�մϴ�.
        get => speed;

        // speed ���� �����ϵ�, 1~20 ���̷� �����մϴ�.
        set => speed = Mathf.Clamp(value, 1f, 20f);
    }
}
