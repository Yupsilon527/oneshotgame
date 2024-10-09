using UnityEngine;
[CreateAssetMenu(fileName = "WeaponData", menuName = "BulletHell/WeaponData")]
public class WeaponData : ScriptableObject
{
    public ProjectileData projectile;
    public float FireInterval=1;
    public int ProjectileCount=1;
    public float ProjectileArc=0;
    public float ProjectileDistance = 0;
    public float BarrelAccuracy=0;
    public float ProjectileAccuracy=0;
    public float FireSlowDown = 1;

    [Header("Clusters")]
    public ProjectileData clusterData;
    public int clusterAmount = 0;
    public int clusterSpeed = 0;
    public int clusterArc = 0;
    public int clusterArcDelta = 0;
    public int clusterDelay = 0;
}