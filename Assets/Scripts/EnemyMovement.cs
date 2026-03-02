using System;
using UnityEditor.Tilemaps;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 2f;
    public float groundCheckDistance = 0.6f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool moveRight = true;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        CheckEdge();
    }

    private void CheckEdge()
    {
        float direction = moveRight ? 1f : -1f;

        BoxCollider2D col = GetComponent<BoxCollider2D>();

        Vector2 rayOrigin = new Vector2(
            col.bounds.center.x + direction * col.bounds.extents.x,
            col.bounds.min.y);

        RaycastHit2D hit = Physics2D.Raycast(
            rayOrigin,
            Vector2.down,
            groundCheckDistance,
            groundLayer);


        if (hit.collider == null)
        {
            Flip();
        }

        Debug.DrawRay(rayOrigin, Vector2.down * groundCheckDistance, Color.red);
    }

    private void Move()
    {
        float direction = moveRight ? 1f : -1f;
        Vector2 targetPos = rb.position + Vector2.right * direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {   
                if(contact.normal.y > 0.5f)
                {
                    Destroy(gameObject);
                    Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        // прыжок игрока от врага
                        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 10f);
                    }
                }
                if (Math.Abs(contact.normal.x) > 0.5f)
                {
                    if ((moveRight && contact.normal.x < 0f) ||
                        (!moveRight && contact.normal.x > 0f))
                    {
                        Flip();
                    }
                }
            }
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // contact.normal — направление контакта относительно врага
                // normal.y > 0.5 → игрок сверху
                // normal.x > 0.5 или < -0.5 → удар сбоку

                if (contact.normal.y > 0.5f)
                {
                    // Игрок сверху → убиваем врага
                    Destroy(gameObject); // или любая логика смерти врага
                    Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        // прыжок игрока от врага
                        playerRb.velocity = new Vector2(playerRb.velocity.x, 10f);
                    }
                }
                else if (Mathf.Abs(contact.normal.x) > 0.5f)
                {
                    // Игрок сбоку → игрок умирает
                    collision.gameObject.GetComponent<PlayerController>().Die();
                }
            }
        }
    }



    private void Flip()
    {
        moveRight = !moveRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
