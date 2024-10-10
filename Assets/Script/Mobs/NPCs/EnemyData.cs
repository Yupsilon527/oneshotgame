using UnityEngine;
[CreateAssetMenu(fileName = "EnemyData", menuName = "BulletHell/EnemyData")]
public class EnemyData : ScriptableObject
{
    public enum Behavior
    {
        stalker,
        kamikaze,
        passive
    }
    public GameObject enemyPrefab;
    public Behavior AI;
    public float SightRange;
    public float Scale;
    public float Speed;
    public WeaponData HarassWeapon;
    public WeaponData AttackWeapon;
    public WeaponData SuicideWeapon;
}
