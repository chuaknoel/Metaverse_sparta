using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ĳ���Ͱ� ���� �� ���� ��ƼŬ ȿ���� �����ϴ� Ŭ�����Դϴ�.
/// �ִϸ��̼� �̺�Ʈ�� ���� ȣ��Ǿ� ���ڱ� ���� ȿ���� ǥ���մϴ�.
/// </summary>
public class DustParticleControl : MonoBehaviour
{
    // [SerializeField] �Ӽ��� private ������ ����Ƽ �ν����Ϳ��� ���� �����ϰ� �մϴ�.
    // �̸� ���� �����Ϳ��� ���� ������ �� �����鼭�� �ٸ� Ŭ�������� ���� ������ �Ұ����ϰ� �մϴ�.

    [SerializeField] private bool createDustOnWalk = true; // �ȱ� �� ���� ���� ����
                                                           // bool Ÿ���� true/false ���� �����ϴ� ���� �ڷ����Դϴ�.
                                                           // �� ������ ���� �ν����Ϳ��� ���� ȿ���� �Ѱ� �� �� �ֽ��ϴ�.

    [SerializeField] private ParticleSystem dustParticleSystem; // ���� ��ƼŬ �ý���
    // ParticleSystem�� ����Ƽ�� ��ƼŬ ȿ���� �����ϴ� ������Ʈ�Դϴ�.
    // ����, ����, ��, �� ���� ȿ���� ǥ���� �� ���˴ϴ�.

    /// <summary>
    /// ���� ��ƼŬ�� �����ϴ� �޼����Դϴ�.
    /// �ַ� ĳ���� �ִϸ��̼��� Ư�� �����ӿ��� Animation Event�� ȣ��˴ϴ�.
    /// ĳ���Ͱ� ���� �� ���� ���� ��� ���� ȣ���Ͽ� ���� ȿ���� �����մϴ�.
    /// </summary>
    public void CreateDustParticles()
    {
        // createDustOnWalk�� true�� ���� ���� ȿ���� �����մϴ�.
        // �� ���ǹ��� ���ʿ��� ��ƼŬ ������ �����ϰ�, �ʿ信 ���� ȿ���� ��Ȱ��ȭ�� �� �ְ� �մϴ�.
        if (createDustOnWalk)
        {
            // ���� ��ƼŬ �ý��� ����
            // Stop() �޼���� ���� ��� ���� ��ƼŬ ȿ���� �����մϴ�.
            // �� ȿ���� �����ϱ� ���� ���� ȿ���� �����Ͽ� ����� �ð�ȿ���� ����ϴ�.
            dustParticleSystem.Stop();

            // ���� ��ƼŬ ����
            // Play() �޼���� ��ƼŬ �ý����� Ȱ��ȭ�Ͽ� ȿ���� ����մϴ�.
            // ���� ȿ���� ��ƼŬ �ý����� ����(ũ��, ����, ���ӽð� ��)�� ���� ǥ�õ˴ϴ�.
            dustParticleSystem.Play();
        }
    }
}
