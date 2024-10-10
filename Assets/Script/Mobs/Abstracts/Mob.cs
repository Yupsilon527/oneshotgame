using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Mob : MonoBehaviour
{
    public Collider2D collider;
    public Rigidbody2D rigidbody;
    protected SpriteRenderer SpriteRenderer;

    public float scale = 1f;
    public bool snaptobounds = false;

    #region Unity Calls
    public virtual void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }
    protected virtual void FixedUpdate()
    {
        SnapToBounds(transform.position);
    }
    #endregion
    #region Movement
    public virtual void SnapToBounds(Vector2 pos)
    {
        if (snaptobounds)
        {
            Rect Levelbounds = Level.main.CameraBounds;
            transform.position = new Vector3(
                Mathf.Clamp(pos.x, Levelbounds.xMin, Levelbounds.xMax),
                Mathf.Clamp(pos.y, Levelbounds.yMin, Levelbounds.yMax),
                transform.position.z);
        }
    }
    #endregion
    #region Life Death
    public virtual void TakeDamage(float damage)
    {
            Die();
    }
    public virtual void Die()
    {
        gameObject.SetActive(false);
    }
    #endregion
    #region collision
    public virtual bool IsInvulnerable()
    {
        return false;
    }
    public Body CollidesWithAnotherBody()
    {
        foreach (Body b in Level.main.bodies)
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
        if (!IsInvulnerable())
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
