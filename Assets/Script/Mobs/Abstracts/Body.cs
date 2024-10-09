using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Body : Mob
{
    public Collider2D collider;
    public Rigidbody2D rigBody;
    bool LeftHip = false;
    protected WeaponData[] weapons;
    protected float[] FireTimes;
    public  void FireWeapon(int weaponID,Vector3 point)
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
            ShootWeapon(w, center, firingDir);

           LeftHip = !LeftHip;

            FireTimes[weaponID] = Time.time + weapons[weaponID].FireInterval;
        }
    }
    protected virtual void ShootWeapon(WeaponData w, Vector3 center, Vector2 firedir)
    {
        Projectile.LaunchMultiple(
            w.projectile,
            this,
            w,
           center,
            firedir,
             w.projectile.ForwardSpeed,
            w.ProjectileCount,
            w.ProjectileArc,
            w.BarrelAccuracy,
            IsPlayerControlled() ? Projectile.ProjectileAlignment.player : Projectile.ProjectileAlignment.enemy
            );
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
