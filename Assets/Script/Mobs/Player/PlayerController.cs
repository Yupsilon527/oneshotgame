using UnityEngine;

public class PlayerController : Body
{
    public static PlayerController main;
    public WeaponData startingWeapon;
    private void Awake()
    {
        main = this;
    }
    public void Start()
    {
        main = this;
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

    //Animations
    public Animator _playerAnimator;

    public void Restart()
    {
        dashing = false;
        fireState = FireState.notFired;
        collectedBonuses = new();
        transform.position = Vector3.zero;

        Score = 0;
        IncreaseScore(0);
    }
    private void Update()
    {
        HandleDashing();
        HandleMovement();
        if (!dashing)
        {
            HandleFire();
        }
        }
    public void HandleMovement()
    {
        //float realSpeed = (dashing ? DashSpeed : (fireState == FireState.notFired ? Speed : SprintSpeed)) ;
        float realSpeed = (dashing ? DashSpeed : Speed) ;

        Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        _playerAnimator.SetBool("IsMoving", direction.sqrMagnitude > 0);
        _playerAnimator.SetBool("FaceRight", direction.x >= 0);

        if (!dashing || direction.sqrMagnitude > 0)
        {
            rigidbody.velocity = direction * realSpeed;        
        }
    }
    public enum FireState
    {
        notFired,
        charging,
        fired
    }
    public FireState fireState;
    public void HandleFire()
    {
        switch (fireState)
        {
            case FireState.notFired:
                if (Level.main.state == Level.GameState.victorious && Input.GetKeyDown(KeyCode.F))
                {
                    fireState = FireState.charging;
                }
                break;
            case FireState.charging:
                if (Input.GetKeyUp(KeyCode.F))
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    FireWeapon(0, mousePos);
                    fireState = FireState.fired;
                    Level.main.RoundProgress(false);
                }
                break;
        }
    }
    public void HandleDashing()
    {
        if (dashing)
        {
            if (DashTime < Time.time)
            {
                dashing = false;
            }
        }
        else if (DashCooldown < Time.time && Input.GetAxis("Fire3") != 0 /*&& (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)*/)
        {
            dashing = true;

            DashTime = Time.time + DashDuration;
            DashCooldown = Time.time + DashRefresh;

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
        public float scoreMult = 1;
        public float pLifeTime = 0;
        public float pSpeed = 0;

        public int pBounces = 0;
        public int pHits = 0;
        public int pProjectiles = 0;
    }
    public float Score = 0;
    public float IncreaseScore(float value)
    {
        float total = value * collectedBonuses.scoreMult;

        Score += total;
        ScoreCounter.main.SetScore(Score);
        
        return total;
    }
}
