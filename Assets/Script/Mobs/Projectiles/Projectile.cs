using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Mob
{
    float dTime = 0;
    ProjectileData d;
    Mob Shooter;
    public enum MoveType
    {
        directional,
        arc
    }
    public override void Awake()
    {
        scale = .5f;
        base.Awake();
        SpriteRenderer.color = Color.red;
    }
    public static Projectile FromData(Vector2 position, ProjectileData Data, Body Shooter)
    { 
        Projectile select = Level.main.bulletpool.PoolItem(Level.main.bulletPrefab).GetComponent<Projectile>();
        select.d = Data;
        select.Shooter = Shooter;
        select.transform.position = position;
        select.gameObject.SetActive(true);
        return select;
    }
    public int VelSteps = 1;
    public void AdjustVelocity()
    {
        velocity = transform.up * d.ForwardSpeed + transform.right * d.RightSpeed;
        VelSteps = Mathf.Max(1,Mathf.CeilToInt(velocity.magnitude / scale / 50));


        }
    public void Fire(Vector2 direction, float accuracy)
    {
        Fire(direction, 0, accuracy);
    }
    public void Fire(Vector2 direction, float gunangle, float accuracy)
    {
        Scale(d.Scale);
        PlayerSide = Shooter.PlayerOwned();
        dTime = Level.main.gameTime + d.Life;
        SetForwardVector(direction);
        if (gunangle != 0)
        {
            transform.rotation *= Quaternion.Euler(0, 0, gunangle );
        }
        if (accuracy > 0)
        {
            transform.rotation *= Quaternion.Euler(0, 0,  (Random.value - .5f) * accuracy * 2f);
        }

        AdjustVelocity();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (dTime< Level.main.gameTime)
        {
            Die();
        }
    }
    public override void Move(Vector2 pos)
    {
        Move(pos, transform.localScale.x);
        switch (d.Behavior)
        {
            case MoveType.arc:
                AdjustVelocity();
                RotateAgainst(Shooter);
                break;
        }
    }
    
    public override void MoveDirection(Vector2 direction)
    {
            for (int step = 0; step < VelSteps; step ++)
            {
                Move(direction/ VelSteps + (Vector2)transform.position);
            }

        
    }
    public virtual void Move(Vector2 pos, float interval)
    {
        CheckCollision();
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }
    public void CheckCollision()
    {
        Body other = CheckBodyCollision();
        if (other!=null)
        {
            Debug.Log("[Bullet] " + name + " collides with " + other.name);
            other.TakeDamage(d.Damage);
                Die();
        }
    }
    public bool PlayerSide = false;
    public override bool PlayerOwned()
    {
        return PlayerSide;
    }
    public Body CheckBodyCollision()
    {
        if (!PlayerOwned())
        {
            foreach (Body b in Level.main.players)
            {
                if (!b.invulnerable && IsInRangeOfOther(b))
                {
                    return b;
                }
            }
        }
        else 
        {
            foreach (Body b in Level.main.bodies)
            {
                if (!b.invulnerable && IsInRangeOfOther(b))
                {
                    return b;
                }
            }
        }
        return null;
    }

    public virtual void SnapToPosition(Vector2 pos)
    {
        Move(pos);
    }
    public override void Die()
    {
        base.Die();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        ScoreCounter.main.nBullets++;
    }
    protected override void OnDisable()
    {
        ScoreCounter.main.nBullets--;
    }
}
