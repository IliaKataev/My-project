using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    [Header("Player Interaction")]
    public LayerMask playerLayer;
    public float bounceForce = 10f; // прыжок игрока при убийстве врага сверху
    public int contactDamage = 1;

    private Rigidbody2D rb;
    private BoxCollider2D col;
    private bool moveRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void FixedUpdate()
    {
        Move();
        CheckEdges();
        CheckPlayerCollision();
    }

    private void Move()
    {
        float direction = moveRight ? 1f : -1f;
        Vector2 velocity = new Vector2(direction * speed, 0f); // движение по X
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    private void CheckEdges()
    {
        float direction = moveRight ? 1f : -1f;

        Vector2 rayOrigin = new Vector2(
            col.bounds.center.x + direction * col.bounds.extents.x,
            col.bounds.min.y);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayer);

        // если края нет → повернуть
        if (hit.collider == null)
        {
            Flip();
        }

        // проверка столкновения со стеной вперед
        RaycastHit2D wallHit = Physics2D.Raycast(rayOrigin, Vector2.right * direction, col.size.x / 2f, groundLayer);
        if (wallHit.collider != null)
        {
            Flip();
        }

        Debug.DrawRay(rayOrigin, Vector2.down * groundCheckDistance, Color.red);
        Debug.DrawRay(rayOrigin, Vector2.right * direction * col.size.x / 2f, Color.blue);
    }

    private void CheckPlayerCollision()
    {
        Collider2D playerHit = Physics2D.OverlapBox(transform.position, col.size, 0f, playerLayer);
        if (playerHit != null)
        {
            Vector2 normal = (playerHit.transform.position - transform.position).normalized;

            if (normal.y > 0.5f)
            {
                // игрок сверху → убиваем врага
                Destroy(gameObject);

                Rigidbody2D playerRb = playerHit.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce); // бесконечный прыжок
                }
            }
            else
            {
                // сбоку → наносим урон игроку
                Health health = playerHit.GetComponent<Health>();
                if (health != null)
                    health.TakeDamage(contactDamage); /// сильно быстро наносится урон + отбрасывание или временная неуязвимость
            }
        }
    }

    private void Flip()
    {
        moveRight = !moveRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (col != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, col.size);
        }
    }
}
