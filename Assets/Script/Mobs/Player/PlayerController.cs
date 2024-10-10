using UnityEngine;

public class PlayerController : Body
{
    public WeaponData startingWeapon;
    public void Start()
    {
        Level.main.players.Add(this);
        weapons = new WeaponData[]{
            startingWeapon
        };
        FireTimes = new float[weapons.Length];
    }

    bool dashing = false;
    public float Speed = 15;

    public float DashTime = 1f;
    public float DashCooldown = 1f;
    public float DashSpeed = 50;
    public float DashDuration = .2f;
    public float DashRefresh = 1f;
    private void Update()
    {
        HandleDashing();
        HandleMovement();
        if (!dashing)
        {
            HandleFire();
        }
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public void HandleMovement()
    {
        float realSpeed = (dashing ? DashSpeed : Speed);

        Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (!dashing || direction.sqrMagnitude > 0)
        {
            rigidbody.velocity = (direction).normalized * realSpeed;
        }
    }
    public enum FireState
    {
        notFired,
        charging,
        fired
    }
    FireState fireState;
    public void HandleFire()
    {
        switch (fireState)
        {
            case FireState.notFired:
                if (Input.GetAxis("Fire1") != 0)
                {
                    fireState = FireState.charging;
                }
                break;
            case FireState.charging:
                if (Input.GetAxis("Fire1") == 0)
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    FireWeapon(0, mousePos);
                    fireState = FireState.fired;
                }
                break;
        }
    }
    public void HandleDashing()
    {
        if (dashing)
        {
            if (DashTime < Level.main.gameTime)
            {
                dashing = false;
            }
        }
        else if (DashCooldown < Level.main.gameTime && Input.GetAxis("Fire3") != 0 /*&& (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)*/)
        {
            dashing = true;

            DashTime = Level.main.gameTime + DashDuration;
            DashCooldown = Level.main.gameTime + DashRefresh;

        }
    }
    public override bool IsInvulnerable()
    {
        return dashing || fireState == FireState.fired;
    }
    public override bool IsPlayerControlled()
    {
        return true;
    }
    public BonusTable collectedBonuses;
    protected override void ShootWeapon(WeaponData w, Vector3 center, Vector2 firedir)
    {
        if (collectedBonuses == null)
        {
            base.ShootWeapon(w, center, firedir);
            return;
        }

        foreach (var p in Projectile.LaunchMultiple(
               w.projectile,
               this,
               w,
              center,
               firedir,
                w.projectile.ForwardSpeed + collectedBonuses.pSpeed,
               w.ProjectileCount + collectedBonuses.pProjectiles,
               w.ProjectileArc,
               w.BarrelAccuracy,
               IsPlayerControlled() ? Projectile.ProjectileAlignment.player : Projectile.ProjectileAlignment.enemy
               ))
        {
            p.GiveDuration(collectedBonuses.pLifeTime);
            p.GiveBounces(collectedBonuses.pBounces);
            p.GiveHits(collectedBonuses.pHits);
        }
    }
    public class BonusTable
    {
        public float pLifeTime = 0;
        public float pSpeed = 0;

        public int pBounces = 0;
        public int pHits = 0;
        public int pProjectiles = 0;

    }
}
