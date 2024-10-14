using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : Body
{
    public float NextUpdateTime = 0f;
    EnemyData data;
    public AudioClip attackSound;
    public static Enemy FromData(EnemyData eData)
    {
        Enemy e = EnemyController.main.enemypool.PoolItem(eData.enemyPrefab).GetComponent<Enemy>();
        e.data = eData;
        e.gameObject.SetActive(true);
        e.weapons = new WeaponData[]
        {
            eData.HarassWeapon,
            eData.AttackWeapon,
            eData.SuicideWeapon
        };
        e.FireTimes = new float[]
        {
            0,0,0
        };
        e.Respawn();
        return e;
    }
    public void Respawn()
    {
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        Vector2 center = (Vector2)Camera.main.transform.position + new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * EnemyController.main. EnemySpawnRange;
        transform.position = new Vector3(center.x, center.y, transform.position.z);
        NextUpdateTime = 0;
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (data.dieOfScreen && transform.position.sqrMagnitude > EnemyController.main.EnemySpawnRange * EnemyController.main.EnemySpawnRange + 10)
        {
            Die();
        }
        if (NextUpdateTime < Time.time)
        {
            FireAtTarget();
            NextUpdateTime = Time.time + MoveTowardsTarget();
        }
    }
    protected virtual void FireAtTarget()
    {
        Body target = GetTarget();
        if (target != null)
        {
            Vector2 delta = target.transform.position - transform.position;
            if (delta.sqrMagnitude < data.SightRange * data.SightRange)
            {
                FireWeapon(1, target.transform.position);
                if (data.kamikaze)
                {
                    Die();
                }
            }
            else
            {
                FireWeapon(0, target.transform.position);
            }
            }
        }
    public virtual float MoveTowardsTarget()
    {
        Body target = GetTarget();
        switch (data.AI)
        {
            case EnemyData.Behavior.wander:
                if (rigidbody.velocity.sqrMagnitude > 0)
                {
                    rigidbody.velocity *= 0;
                    return Random.value * data.ThinkInterval;
                }
                else
                {
                    rigidbody.velocity = Random.insideUnitCircle.normalized * data.Speed;
                    return Random.value ;
                }
            case EnemyData.Behavior.stalker:
            case EnemyData.Behavior.horizontal:
            case EnemyData.Behavior.diagonal:
                if (target != null)
                {
                    Vector2 delta = target.transform.position - transform.position;
                    if (!HasWeapon(1) || data.moveWhileFiring || CanFireWeapon(1))
                    {
                        if (data.AI == EnemyData.Behavior.horizontal)
                        {
                            if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
                            {
                                delta = Vector2.right * Mathf.Sign(delta.x);
                            }
                            else
                            {
                                delta = Vector2.up * Mathf.Sign(delta.y);
                            }
                        }
                        else if (data.AI == EnemyData.Behavior.diagonal)
                        {
                            delta.x = delta.x > 0 ? 1 : -1;
                            delta.y = delta.y > 0 ? 1 : -1;
                        }
                        else
                        {
                            delta = delta.normalized;
                        }
                        rigidbody.velocity = delta * data.Speed;
                    }
                    else
                    {
                        rigidbody.velocity *= 0;
                    }

                }
                break;
        }
        return data.ThinkInterval;
    }
    void PursueTarget(Body target)
    {

    }
    public Body GetTarget()
    {
        if (PlayerController.main!=null)
        {
            return PlayerController.main;
        }
        return null;
    }

    public NPCAnimationRandomiser npcAnimationRandomiser;
    public override void TakeDamage(float damage)
    {
        PlayerController.main.IncreaseScore(damage);
        Level.main.TextEffect("+" + damage, transform.position, Color.green, scale:0.06f, animation: "Hover Up");
        if (audioSource != null && hurtClips!=null && hurtClips.Length >0) {
            audioSource.PlayOneShot(hurtClips[Mathf.FloorToInt(hurtClips.Length * Random.value)]);
            npcAnimationRandomiser.NpcCough();
        }

    }
    public override void Die()
    {
        FireWeapon(2, rigidbody.velocity);
        EnemyController.main.enemypool.DeactivateObject(gameObject);
    }
}
