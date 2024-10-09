using System.Collections;
using UnityEngine;
using static PlayerController;

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
                    FireWeapon(0, mousePos, true);
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
                print("dash end");
            }
        }
        else if (DashCooldown < Level.main.gameTime && Input.GetAxis("Fire3") != 0 /*&& (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)*/)
        {
            print("dash!");
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
}
