using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Body : Mob
{
    public Collider2D collider;
    public Rigidbody2D rigBody;
    public float SlowDown = 0;
    bool LeftHip = false;
    protected WeaponData[] weapons;
    protected float[] FireTimes;
    public void FireWeapon(int weaponID,Vector3 point, bool slowdown)
    {
        if (weaponID>=weapons.Length || weapons[weaponID] == null )
        {
            return;
        }
        if (FireTimes[weaponID] < Time.time)
        {
            FacePoint(point);
            WeaponData w = weapons[weaponID];
            Vector3 firingDir = point - transform.position;

            Vector3 center = transform.position + (LeftHip ? -1 : 1) * transform.right * w.ProjectileDistance;

            //if (equiptedWeapon.weapon.launchEffect != null)
             //   SpecialEffectPool.main.EffectFromPrefab(equiptedWeapon.weapon.launchEffect, center, firePoint.transform.rotation);

            Projectile.LaunchMultiple(
                w.projectile,
                this,
                w,
               center,
                firingDir,
                 w.projectile.ForwardSpeed,
                w.ProjectileCount,
                w.ProjectileArc,
                w.BarrelAccuracy,
                IsPlayerControlled() ? Projectile.ProjectileAlignment.player : Projectile.ProjectileAlignment.enemy
                );
            LeftHip = !LeftHip;


            if (slowdown)
                SlowDown = w.FireSlowDown;

            FireTimes[weaponID] = Time.time + weapons[weaponID].FireInterval;
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        Level.main.RegisterBody(this);
    }
    private void OnDisable()
    {
        Level.main.UnregisterBody(this);
    }
}
