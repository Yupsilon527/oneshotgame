using UnityEngine;

[System.Serializable]
public class ExplosionData
{
    public GameObject explodeEffect;
    public bool hitsPlayer, hitsEnemies;
    public float distanceUnits = 1;
    /*
     public void Resolve(Vector2 center, Mob owner)
     {
         foreach (var hit in Physics2D.CircleCastAll(center, DamageRadius, Vector2.zero))
         {
             if (hit.collider.TryGetComponent(out HumanHitbox human))
             {
                 Debug.Log("HitHuman " + human.name);
                 Vector2 delta = ((Vector2)human.transform.position - center);
                 float sqrMag = delta.sqrMagnitude;

                 float damageDist = 100;
                 float playerDamage = 1;
                 var damage = HumanDamageable.DamageType.Nothing;
                 if (DistanceMultiplier != null && DistanceMultiplier.Length > 0)
                 {
                     foreach (var d in DistanceMultiplier)
                     {
                         var sqrDist = d.distanceUnits * d.distanceUnits;
                         if (sqrMag < sqrDist && sqrDist < damageDist)
                         {
                             damage = d.mobDamage;
                             playerDamage = d.playerDamage;
                             damageDist = sqrDist;
                         }
                     }
                 }
                 human.parent.damageable.TakeDamage(damage, delta, playerDamage, owner);
             }
             else if (hit.collider.TryGetComponent(out VehicleDamageable vehicle))
             {
                 Debug.Log("HitVehicle " + vehicle.name);
             }
         }
     }*/
}
