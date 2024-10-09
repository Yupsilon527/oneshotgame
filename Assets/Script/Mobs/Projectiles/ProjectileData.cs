using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "BulletHell/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    public Color color; public float ForwardSpeed; public float RightSpeed; public float Life; public float Scale; public float Damage; public Projectile.MoveType Behavior;
}
