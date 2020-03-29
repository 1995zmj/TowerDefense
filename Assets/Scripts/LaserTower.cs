using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LaserTower  : Tower
{    
    // const int enemyLayerMask = 1 << 9;
    //
    // static Collider[] targetBuffer = new Collider[100];
    //
    // [SerializeField, Range(1.5f, 10.5f)] 
    // protected float targetingRange = 1.5f;
    public override TowerType TowerType => TowerType.Laser;
    
    [SerializeField, Range(1f, 100f)] 
    private float damagePerSecond = 10f;
    
    [SerializeField] 
    private Transform turret = default, laserBeam = default;
    
    private TargetPoint target;

    Vector3 laserBeamScale;

    private void Awake()
    {
        laserBeamScale = laserBeam.localScale;
    }

    public override void GameUpdate()
    {
        if (TrackTarget(ref target) || AcquireTarget(out target))
        {
            Shoot();
        }
        else
        {
            laserBeam.localScale = Vector3.zero;
        }
    }

    

    void Shoot()
    {
        Vector3 point = target.Position;
        turret.LookAt(point);
        laserBeam.localRotation = turret.localRotation;
    
        float d = Vector3.Distance(turret.position, point);
        laserBeamScale.z = d;
        laserBeam.localScale = laserBeamScale;
        laserBeam.localPosition =
            turret.localPosition + 0.5f * d * laserBeam.forward;
    
        target.Enemy.ApplyDamage(damagePerSecond * Time.deltaTime);
    }
}