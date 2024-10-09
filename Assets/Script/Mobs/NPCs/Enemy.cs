using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Body
{
    public static float EnemySpawnRange = 25;
    protected bool DieOffscreen = true;
    public float NextUpdateTime = 0f;
    EnemyData data;
    public static Enemy FromData(string EnemyID)
    {
       return FromData(Resources.Load<EnemyData>("BulletHell/GameData/" + EnemyID));
    }
        public static Enemy FromData(EnemyData eData)
    {
        Enemy e = EnemyController.main.enemypool.PoolItem(eData.enemyPrefab).GetComponent<Enemy>();
        e.data = eData;
        e.gameObject.SetActive(true);
        e.Respawn();
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
        return e;
    }
    public void Respawn()
    {
        Scale(data.Scale);
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        Vector2 center = (Vector2)Camera.main.transform.position + new Vector2(Mathf.Sin(angle),Mathf.Cos(angle))* EnemySpawnRange;
        transform.position = new Vector3(center.x, center.y, transform.position.z);
        NextUpdateTime = 0;
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (DieOffscreen && transform.position.sqrMagnitude> EnemySpawnRange * EnemySpawnRange + 10)
        {
            Die();
        }
        if (NextUpdateTime< Level.main.gameTime)
        {
            NextUpdateTime = Level.main.gameTime + AIUpdate();
        }
    }
    public virtual float AIUpdate()
    {
        Body target = GetTarget();
        switch (data.AI)
        {
            case EnemyData.Behavior.stalker:
            case EnemyData.Behavior.kamikaze:
                if (target != null)
                {
                    Vector2 delta = target.transform.position - transform.position;
                    if (delta.sqrMagnitude < data.SightRange * data.SightRange)
                    {

                        FireWeapon(1, target.transform.position,true);
                        if (data.AI == EnemyData.Behavior.kamikaze)
                            Die();
                        else
                            return .1f;
                    }
                    else
                    {
                        FireWeapon(0, target.transform.position,false);
                        SlowDown = 1;
                    }
                    velocity = delta.normalized * data.Speed * SlowDown;

                }
                break;
        }
        return .5f;
    }
    public Body GetTarget()
    {
        if (Level.main.players.Count>0)
        {
            return Level.main.players[0];
        }
        return null;
    }
    public override void Die()
    {
        FireWeapon(2, velocity,false);
        base.Die();
    }
}
