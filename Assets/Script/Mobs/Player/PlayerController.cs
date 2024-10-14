using UnityEngine;

public class PlayerController : Body
{
    public static PlayerController main;
    public WeaponData startingWeapon;
    (int, string)[] thing = new(int, string)[] {
        (0, "Fart!"),
        (100, "Gross!"),
        (150, "Eww!"),
        (200, "Disgusting!"),
        (250, "EWWW!!!"),
        (300, "RampFART!"),
        (350, "FARTMAGEDDON!!"),
    };
    private void Awake()
    {
        main = this;
    }
    public void Start()
    {
        main = this;
        IncreaseScore(0);
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
    public ParticleSystem dashParticle;
    public ParticleSystem fartParticle;

    public void Restart(bool hard)
    {
        dashing = false;
        fireState = FireState.notFired;
        collectedBonuses = new();
        transform.position = Level.main.levelStartPosition.transform.position;
        dashParticle.Stop();

        if (hard)
        {
            Score = 0;
        }
        IncreaseScore(0);
        scoreRating = 0;
        lastRoundScore = Score;
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
        fired,
        dud
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
                dashParticle.Stop();
            }
        }
        else if (DashCooldown < Time.time && Input.GetAxis("Fire3") != 0 /*&& (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)*/)
        {
            dashing = true;
            dashParticle.Play();

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
        fartParticle.Play();
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
    int scoreRating = 0;
    public float Score = 0;
    public float lastRoundScore = 0;
    public float IncreaseScore(float value)
    {
        float total = collectedBonuses == null ? value : (value * collectedBonuses.scoreMult);

        for (int i = scoreRating; i< thing.Length; i++)
        {
            if (Score+ total > thing[i].Item1)
            {
                scoreRating = i+1;

                if (NotificationWidget.instance != null)
                    NotificationWidget.instance.DisplayNotification(thing[i].Item2, true);

                break;
            }
        }

        Score += total;
        ScoreCounter.main.SetScore(Score);
        
        return total;
    }
    public override void TakeDamage(float damage)
    {
        Level.main.TextEffect("-0:0"+damage, transform.position, Color.red); 
        Level.main.ExtendPenaltyTime(damage);
    }
    public override void Die()
    {
        if (fireState < FireState.fired)
            fartParticle.Play();
        if (NotificationWidget.instance != null)
            NotificationWidget.instance.DisplayNotification("YOU FAILED!",true);
    }
}
