using System.Collections.Generic;
using UnityEngine;

public class Projectile : Mob
{
    public enum ProjectileAlignment
    {
        player,
        enemy,
        neutral
    }
    protected float birthtime = 0;
    protected float deathtime = 0;

    int numBounces = 0;
    int maxTargets = 0;

    ProjectileData data;
    WeaponData wdata;
    Body shooter;
    List<Mob> hitEntities = new List<Mob>();
    bool dead;

    Collider2D launcherCollider;
    public Collider2D mobCollider;
    public Collider2D mobTrigger;

    public ProjectileAlignment alignment = ProjectileAlignment.neutral;
    public enum MoveType
    {
        directional,
        arc
    }
    public void AssignProperties(ProjectileData ndata)
    {
        data = ndata;
    }
    public static Projectile[] LaunchMultiple(ProjectileData projectileData, Body owner, WeaponData weapon, Vector3 origin, Vector3 direction, float launchSpeed, int amount, float arc, float accuracy, ProjectileAlignment alignment)
    {
        List<Projectile> fired = new();
        float baseAng = -amount / 2f * arc;
        for (int shot = 0; shot < amount; shot++)
        {
            GameObject pooled = Level.main.bulletpool.PoolItem(projectileData.prefab);
            if (pooled.TryGetComponent(out Projectile other))
            {
                other.data = projectileData;
                other.Launch(origin, direction, launchSpeed, owner, weapon, accuracy, baseAng + arc * shot, alignment);
                fired.Add(other);
            }
        }
        return fired.ToArray();
    }
    public void Launch(Vector2 center, Vector2 direction, float launchPower, Body launcher, WeaponData weapon, float accuracy, float deltaAngle, ProjectileAlignment a)
    {
        Refresh();
        transform.position = new Vector3(center.x, center.y, launcher.transform.position.z);

        alignment = a;
        this.shooter = launcher;
        wdata = weapon;
        launcherCollider = launcher.collider;
        Physics2D.IgnoreCollision(collider, launcherCollider, true);
        hitEntities.Add(launcher);
        maxTargets = data.targets;

        float lifeTime = data.LifeTime;
        if (lifeTime > 0)
        {
            deathtime = Time.time + lifeTime;
        }
        else
        {
            deathtime = -1;
        }
        gameObject.SetActive(true);
        LaunchDirection(direction, launchPower, deltaAngle, accuracy);
    }
    void LaunchDirection(Vector2 direction, float launchPower, float deltaAngle, float accuracy)
    {
        Vector2 velocity = direction.normalized * launchPower;

        float acc = deltaAngle * Mathf.Deg2Rad;

        if (accuracy > 0)
        {
            acc += UnityEngine.Random.Range(-accuracy, accuracy) * Mathf.Deg2Rad;
        }
        if (acc != 0)
        {
            velocity = new Vector2(velocity.x * Mathf.Cos(acc) - velocity.y * Mathf.Sin(acc), velocity.x * Mathf.Sin(acc) + velocity.y * Mathf.Cos(acc));
        }
        Launch(velocity);
    }
    public void Fire(Vector2 direction, float gunangle, float accuracy)
    {
        deathtime = Time.time + data.LifeTime;
        SetForwardVector(direction);
        if (gunangle != 0)
        {
            transform.rotation *= Quaternion.Euler(0, 0, gunangle);
        }
        if (accuracy > 0)
        {
            transform.rotation *= Quaternion.Euler(0, 0, (Random.value - .5f) * accuracy * 2f);
        }

    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (GetTimeRemaining() <= 0)
        {
            HandleBehavior(data.expireBehavior);
            Debug.Log("STOP " + name);
        }
    }
    public override void SnapToBounds(Vector2 pos)
    {
        if (snaptobounds)
        {
            var VP = Camera.main.WorldToViewportPoint(transform.position);
            Vector2 v = rigidbody.velocity;
            if (VP.x < 0 || VP.x > 1)
            {
                OnBounceScreenEdge();
                v.x *= -1;
            }
            if (VP.y < 0 || VP.y > 1)
            {
                OnBounceScreenEdge();
                v.y *= -1;
            }
            rigidbody.velocity = v;
        }
    }

    protected virtual void Launch(Vector2 v)
    {
        rigidbody.velocity = v;
        if (data.directionAligned)
            transform.right = v;
    }
    private void Refresh()
    {
        hitEntities.Clear();
        dead = false;
        numBounces = 0;
        maxTargets = 0;
        birthtime = Time.time;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Body other) )
        {
            Debug.Log("[Bullet] " + name + " collides with " + other.name);
            CollideBody(other);
        }
    }
    public override bool IsPlayerControlled()
    {
        return shooter != null && shooter.IsPlayerControlled();
    }
    public Body CheckBodyCollision()
    {
        if (!IsPlayerControlled())
        {
                if (!PlayerController.main.IsInvulnerable() && IsInRangeOfOther(PlayerController.main))
                {
                    return PlayerController.main;
                }
        }
        else
        {
            foreach (Body b in Level.main.bodies)
            {
                if (!b.IsInvulnerable() && IsInRangeOfOther(b))
                {
                    return b;
                }
            }
        }
        return null;
    }
    public void CollideBody(Body other)
    {
        if (other == shooter) return;
        if ((alignment == ProjectileAlignment.enemy && other.IsPlayerControlled()) ||
          (alignment == ProjectileAlignment.player && !other.IsPlayerControlled()) ||
            alignment == ProjectileAlignment.neutral)
        {
            ContactEntity(other);
            Debug.Log("CONTACT " + other);
        }

    }
    #region Bounces and water
    void OnBounceTerrain(Collision2D collision)
    {
        numBounces++;
        if (numBounces >= data.bounces)
        {
            transform.position = collision.contacts[0].point;
            HandleBehavior(data.bounceBehavior);
        }
        if (data.directionAligned)
            transform.right = rigidbody.velocity;
    }
    void OnBounceScreenEdge()
    {
        numBounces++;
        if (numBounces >= data.bounces)
        {
            HandleBehavior(data.bounceBehavior);
            Debug.Log("WALL " + name);
        }
        if (data.directionAligned)
            transform.right = rigidbody.velocity;
    }
    void ContactEntity(Body hit)
    {
        if (!hitEntities.Contains(hit))
        {
            hit.TakeDamage(data.Damage);
            hitEntities.Add(hit);
        }
        if (hitEntities.Count >= maxTargets + 1)
        {
            Debug.Log("CRITTER " + name);
            HandleBehavior(data.contactBehavior);
        }
    }
    protected void HandleBehavior(ProjectileData.ContactBehavior b)
    {
        switch (b)
        {
            case ProjectileData.ContactBehavior.detonate:
                Explode();
                break;
            case ProjectileData.ContactBehavior.delete:
                Die();
                break;
            case ProjectileData.ContactBehavior.cluster_upwards:
                if (wdata != null)
                    Cluster(Vector3.up);
                Explode();
                break;
            case ProjectileData.ContactBehavior.cluster_forward:
                Cluster(GetComponent<Rigidbody>().velocity.normalized);
                Explode();
                break;
            case ProjectileData.ContactBehavior.cluster_backward:
                Cluster(-GetComponent<Rigidbody>().velocity.normalized);
                Explode();
                break;
        }
    }
    void Cluster(Vector2 dir)
    {
        if (wdata != null)
            LaunchMultiple(wdata.clusterData, shooter, wdata, transform.position, dir, wdata.clusterSpeed, wdata.clusterAmount, wdata.clusterArc, wdata.clusterArcDelta, alignment);

    }
    void SpecialEffect(GameObject effectPrefab, float scale = 1)
    {
        if (effectPrefab != null)
        {
            //TODO SpecialEffectPool.main.EffectFromPrefab(effectPrefab, transform.position, Quaternion.identity, scale: scale);
        }
    }
    #endregion

    public void Explode()
    {
        if (!dead)
        {
            if (data.explosion != null)
            {
                //TODO SpecialEffect(data.explosion.explodeEffect, data.explosion.EffectRadius);
                //data.explosion.Resolve(transform.position, shooter);
            }
            Die();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
            OnBounceTerrain(collision);
    }
    public override void Die()
    {
        if (dead) return;
        dead = true;
        //TODO SpecialEffect(data.expireEffect);
        if (launcherCollider != null)
            Physics2D.IgnoreCollision(collider, launcherCollider, false);
        base.Die();
        Level.main.UnregisterBody(this);
    }
    float GetTimeRemaining()
    {
        return deathtime - Time.time;
    }
    protected  void OnEnable()
    {
        Level.main.RegisterBody(this);
    }
    public void GiveBounces(int value)
    {
        numBounces -= value;
    }
    public void GiveDuration(float value)
    {
        deathtime += value;
    }
    public void GiveHits(int value)
    {
        maxTargets += value;
    }
}