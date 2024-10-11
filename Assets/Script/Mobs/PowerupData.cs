using System;
using UnityEngine;
[CreateAssetMenu(fileName = "PowerupData", menuName = "BulletHell/PowerupData")]

public class PowerupData : ScriptableObject
{
    public enum Bonus
    {
        multiplier,
        duration,
        speed,
        bounces,
        hits,
        projectiles
    }
    public PowerupBonus[] bonuses;
    [Serializable]
    public class PowerupBonus { 
    public Bonus bonus;
    public float amount;
    public void GiveBonus(PlayerController player)
    {
        switch (bonus) { 
            case Bonus.multiplier: 
                player.collectedBonuses.scoreMult += amount;
                break; 
            case Bonus.duration: 
                player.collectedBonuses.pLifeTime += amount;
                break; 
            case Bonus.speed: 
                player.collectedBonuses.pSpeed += amount;
                break; 
            case Bonus.hits: 
                player.collectedBonuses.pHits += (int)amount;
                break;
            case Bonus.projectiles: 
                player.collectedBonuses.pProjectiles += (int)amount;
                break;
            case Bonus.bounces: 
                player.collectedBonuses.pBounces += (int)amount;
                break;
        }
        }
    }
}
