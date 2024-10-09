using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Mob : MonoBehaviour
{
    protected Vector2 velocity = Vector2.zero;
    protected SpriteRenderer SpriteRenderer;
    public float scale = 1f;
    public bool snaptobounds = false;
 public    float health = 10;

    #region Unity Calls
    public virtual void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }
    protected virtual void FixedUpdate()
    {
        MoveDirection((Vector3)velocity * Time.deltaTime * Level.main.timeScale);
    }
    protected virtual void OnEnable()
    {
        Scale(scale);
    }
    protected virtual void OnDisable()
    {
    }
    #endregion
    #region Movement
    public virtual void MoveDirection(Vector2 direction)
    {
        Move((Vector2)transform.position + direction);
    }
    public virtual void Move(Vector2 pos)
    {
        if (snaptobounds)
        {
            Rect Levelbounds = Level.main.CameraBounds;
            transform.position = new Vector3(
                Mathf.Clamp(pos.x, Levelbounds.xMin, Levelbounds.xMax),
                Mathf.Clamp(pos.y, Levelbounds.yMin, Levelbounds.yMax),
                transform.position.z);
        }
        else
        {
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        }
    }

    public void Scale(float newscale)
    {
        scale = newscale;
        transform.localScale = Vector3.one * scale;
    }
    #endregion
    #region Life Death
    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health<0)
            Die();
    }
    public virtual void Die()
    {
        gameObject.SetActive(false);
    }
    #endregion
    #region collision
    public bool invulnerable;
    public Body CollidesWithAnotherBody()
    {
            foreach (Body b in Level.main.GetAllBodies())
            {
                if (IsInRangeOfOther(b))
                {
                    return b;
                }
            
        }
        return null;
    }
    public virtual bool CollideBullet(Projectile other)
{
    if (!invulnerable)
    {
        other.Die();
        return true;
    }
    return false;
    }
    public virtual bool CollideBody(Projectile other)
    {
        return false;
    }
    #endregion
    #region Is In Range
    public bool IsInRangeOfPoint(Vector2 point, float range)
    {
        Vector2 delta = (Vector2)transform.position - point;
        return delta.sqrMagnitude < (range + scale * .5f) * (range + scale * .5f);
    }
    public bool IsInRangeOfOther(Mob other)
    {
        return IsInRangeOfPoint(other.transform.position, other.scale * .5f);
    }
    #endregion
    #region Alignment
    public virtual bool IsPlayerControlled()
    {
        return false;
    }
    #endregion
    #region Facing And Rotation
    public void FacePoint(Vector2 point)
    {
        SetForwardVector(point - (Vector2)transform.position);
    }
    public void SetForwardVector(Vector2 forward)
    {
        transform.up = forward;
    }
    public void RotateTowards(Mob otherBody)
    {
        FacePoint(otherBody.transform.position);
    }
    public void RotateAgainst(Mob otherBody)
    {
        SetForwardVector(transform.position - otherBody.transform.position);
    }
    #endregion
}
