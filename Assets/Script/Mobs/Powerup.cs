using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
     PowerupData powerup;
    public void FromPowerupDta(PowerupData data) { powerup = data; }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController player))
        {
            foreach (var b in powerup.bonuses)
            {
                b.GiveBonus(player);
            }
            Die();
        }
    }
    void Die()
    {
        PupController.main.ReplenishPowerup();
        PupController.main.powerups.Remove(this);
        PupController.main.puppool.DeactivateObject(gameObject);
    }
}
