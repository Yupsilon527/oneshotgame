using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "BulletHell/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    public enum ContactBehavior
    {
        nothing = 0,
        detonate = 1,
        delete = 2,
        inflict_damage = 3,
        cluster_forward = 4,
        cluster_backward = 5,
        cluster_upwards = 6,
    }
    public Color color;
    public bool directionAligned; public float ForwardSpeed; public float RightSpeed; public float LifeTime; public float Scale; public float Damage; public Projectile.MoveType Behavior; 


    public GameObject prefab;
    public bool oosBounce;
    public int bounces = 0;
    public int targets=1;


    public ContactBehavior bounceBehavior = ContactBehavior.detonate;
    public ContactBehavior contactBehavior = ContactBehavior.delete;
    public ContactBehavior expireBehavior = ContactBehavior.delete;
    public ExplosionData explosion;
}
