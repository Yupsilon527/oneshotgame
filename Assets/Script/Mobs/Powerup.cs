using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    PowerupData powerup;
    public SpriteRenderer sprite;
    public void FromPowerupDta(PowerupData data)
    {
        powerup = data;
        if (sprite != null)
            sprite.sprite = data.powerupSprite;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController player))
        {
            Level.main.TextEffect(powerup.pickupText, transform.position, Color.green, animation: "Splash");

            if (player.audioSource != null && player.powerupSound != null)
                player.audioSource.PlayOneShot(player.powerupSound);

            foreach (var b in powerup.bonuses)
            {
                b.GiveBonus(player);
                PlayerController.main._playerAnimator.SetBool("IsEating", true);
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
