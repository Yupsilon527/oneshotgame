using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public abstract class Body : Mob
{

    bool LeftHip = false;
    
    public AudioSource audioSource;
    public AudioClip[] hurtClips;

    protected WeaponData[] weapons;
    protected float[] FireTimes;
    public override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
    }
    public void FireWeapon(int weaponID, Vector3 point)
    {
        if (CanFireWeapon(weaponID)) 
        {
            //FacePoint(point);
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
    protected bool HasWeapon(int weaponID)
    {
        return weaponID < weapons.Length && weapons[weaponID] != null;
    }
    protected bool CanFireWeapon(int weaponID)
    {
        return (HasWeapon(weaponID) && FireTimes[weaponID] < Time.time);
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
    public override void SnapToBounds(Vector2 pos)
    {
        if (snaptobounds)
        {
            Rect Levelbounds = Level.main.CameraBounds;
            rigidbody.position = new Vector3(
                Mathf.Clamp(pos.x, Levelbounds.xMin, Levelbounds.xMax),
                Mathf.Clamp(pos.y, Levelbounds.yMin, Levelbounds.yMax));
        }
        else
        {
            rigidbody.position = new Vector2(pos.x, pos.y);

        }
    }
    protected  void OnEnable()
    {
        Level.main.RegisterBody(this);
    }
    private void OnDisable()
    {
        Level.main.UnregisterBody(this);
    }
}
