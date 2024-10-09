using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Body : Mob
{
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

            if (w.ProjectileCount>1)
            {
                float AngleDent = (Random.value - .5f) * w.BarrelAccuracy * 2f;
                for (int iFire = 0; iFire < w.ProjectileCount; iFire++)
                {
                    float fDelta = iFire - (w.ProjectileCount - 1) *.5f;
                    Projectile bullet = Projectile.FromData(transform.position - transform.right * fDelta * w.ProjectileDistance , w.projectile, this);
                    bullet.Fire(firingDir, fDelta * w.ProjectileArc + AngleDent, w.ProjectileAccuracy);
                }
            }
            else
            {
                Projectile bullet = Projectile.FromData(transform.position + (LeftHip ? -1 : 1) *transform.right * w.ProjectileDistance, w.projectile, this);
                bullet.Fire(firingDir, w.ProjectileAccuracy);
                LeftHip = !LeftHip;
            }
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
