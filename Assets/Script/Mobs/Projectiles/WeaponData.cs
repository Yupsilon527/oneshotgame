using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "WeaponData", menuName = "BulletHell/WeaponData")]
public class WeaponData : ScriptableObject
{
    public ProjectileData projectile;
    public float FireInterval=1;
    public float ProjectileCount=1;
    public float ProjectileArc=0;
    public float ProjectileDistance = 0;
    public float BarrelAccuracy=0;
    public float ProjectileAccuracy=0;
    public float FireSlowDown = 1;
}