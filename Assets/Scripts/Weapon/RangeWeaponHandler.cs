using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RangeWeaponHandler : WeaponHandler
{

    [Header("Ranged Attack Data")]
    [SerializeField] private Transform projectileSpawnPosition; // 총알이 발사되는 위치

    [SerializeField] private int bulletIndex; // 사용할 총알 프리팹 인덱스
    public int BulletIndex { get { return bulletIndex; } }

    [SerializeField] private float bulletSize = 1; // 총알 크기
    public float BulletSize { get { return bulletSize; } }

    [SerializeField] private float duration; // 총알이 살아있는 시간
    public float Duration { get { return duration; } }

    [SerializeField] private float spread; // 총알 퍼짐 각도 범위
    public float Spread { get { return spread; } }

    [SerializeField] private int numberofProjectilesPerShot; // 한 번에 발사할 총알 수
    public int NumberofProjectilesPerShot { get { return numberofProjectilesPerShot; } }

    [SerializeField] private float multipleProjectilesAngel; // 총알들 간의 고정 각도 간격
    public float MultipleProjectilesAngel { get { return multipleProjectilesAngel; } }

    [SerializeField] private Color projectileColor; // 총알 색상 (시각적 효과)
    public Color ProjectileColor { get { return projectileColor; } }

    private ProjectileManager projectileManager;
    protected override void Start()
    {
        base.Start();
        projectileManager = ProjectileManager.Instance; // ProjectileManager 인스턴스 가져오기
    }

    public override void Attack()
    {
        base.Attack();

        float projectilesAngleSpace = multipleProjectilesAngel; // 총알 간 각도 간격
        int numberOfProjectilesPerShot = numberofProjectilesPerShot; // 발사할 총알 수

        // 총알들을 좌우 대칭으로 퍼지게 하기 위한 시작 각도 계산
        float minAngle = -(numberOfProjectilesPerShot / 2f) * projectilesAngleSpace;


        // 각 총알마다 회전 각도 계산 후 발사
        for (int i = 0; i < numberOfProjectilesPerShot; i++)
        {
            float angle = minAngle + projectilesAngleSpace * i; // 기본 각도
            float randomSpread = Random.Range(-spread, spread); // 랜덤 퍼짐 적용
            angle += randomSpread;

            // 실제 투사체 생성 (Controller.LookDirection = 캐릭터가 바라보는 방향)
            CreateProjectile(Controller.LookDirection, angle);
        }
    }

    private void CreateProjectile(Vector2 _lookDirection, float angle)
    {
        projectileManager.ShootBullet(
            this,
            projectileSpawnPosition.position,
            RotateVector2(_lookDirection, angle)
        );
    }

    // 방향 벡터를 주어진 각도만큼 회전
    private static Vector2 RotateVector2(Vector2 v, float degree)
    {
        return Quaternion.Euler(0, 0, degree) * v;
    }
}
