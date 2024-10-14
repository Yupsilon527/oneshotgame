using UnityEngine;
[CreateAssetMenu(fileName = "EnemyData", menuName = "BulletHell/EnemyData")]
public class EnemyData : ScriptableObject
{
    public enum Behavior
    {
        wander,
        stalker,
        horizontal,
        diagonal,
        passive
    }
    public bool dieOfScreen = true;
    public bool moveWhileFiring = false;
    public bool kamikaze = false;
    public GameObject enemyPrefab;
    public Behavior AI;
    public float SightRange;
    public float Scale;
    public float Score;
    public float Speed;
    public float ThinkInterval;
    public WeaponData HarassWeapon;
    public WeaponData AttackWeapon;
    public WeaponData SuicideWeapon;
}
