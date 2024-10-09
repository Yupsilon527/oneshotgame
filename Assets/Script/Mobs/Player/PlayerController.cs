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
        health = 10000;
        FireTimes = new float[weapons.Length];
    }

    bool dashing = false;
    public float lastfireTime = 0f;
    public float Slomo = 1 / 5f;
    public float Speed = 15;

    public float DashTime = 1f;
    public float DashCooldown = 1f;
    public float DashSpeed = 50;
    public float DashDuration = .2f;
    public float DashRefresh = 1f;
    protected override void FixedUpdate()
    {
        snaptobounds = true;

        HandleDashing();
        HandleMovement();
        if (!dashing)
        {
        HandleBulletTime();
        HandleFire();
        }
        base.FixedUpdate();
    }
    public void HandleMovement()
    {
        float realSpeed = (dashing ? DashSpeed : Speed);

        Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (!dashing || direction.sqrMagnitude > 0) { 
        velocity = (direction).normalized * realSpeed * SlowDown;
        }
    }

    public void HandleBulletTime()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Level.main.timeScale = Slomo;
            float orthoSize = Level.main.camOriginalSize * .66f;
            Vector2 zoomPosition = new Vector2(transform.position.x, transform.position.y);
            Level.main.IssueCameraOrder(new Vector3(zoomPosition.x, zoomPosition.y, orthoSize));
            DashCooldown = Level.main.gameTime + 1;
        }
        else
        {
            Level.main.timeScale = 1;
            Level.main.IssueCameraOrder(new Vector3(0, 0, Level.main.camOriginalSize));
            //DashCooldown = Level.main.gameTime + .66f;
        }
    }
    public void HandleFire()
    {
        while (lastfireTime < Level.main.gameTime)
        {
            lastfireTime = Level.main.gameTime + .01f;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetAxis("Fire1") != 0)
            {
                FireWeapon(0, mousePos,true);
            }
            else if (Input.GetAxis("Fire2") != 0)
            {
                FireWeapon(1, mousePos, true);
                SlowDown = weapons[1].FireSlowDown;
            }
            SlowDown = 1;
        }
    }
    public void HandleDashing()
    {
        if (dashing)
        {
            if (DashTime < Level.main.gameTime)
            {
                dashing = false;
                invulnerable = false;
                print("dash end");
            }
        }
        else if (DashCooldown < Level.main.gameTime && Input.GetAxis("Fire3") != 0 /*&& (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)*/)
        {
            print("dash!");
            dashing = true;

            invulnerable = true;
            DashTime = Level.main.gameTime + DashDuration;
            DashCooldown = Level.main.gameTime + DashRefresh;

        }
    }
    public override bool PlayerOwned()
    {
        return true;
    }
    /*public override void CheckCollision()
    {
        base.CheckCollision();
        Body otherB = CheckBodyCollision();
         if (otherB != null)
         {
            otherB.Die();
         }
    }*/
}
