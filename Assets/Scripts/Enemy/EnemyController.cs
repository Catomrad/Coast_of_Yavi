using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [Range(0f, 5f)] public float moveSpeed = 5.0f;
        [Range(0f, 500f)] public float jumpForce = 5.0f;


        private Vector2 _moveDirection = Vector2.zero;
        private Rigidbody2D _rb;
        private Collider2D _col;

        // Start is called before the first frame update
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<Collider2D>();
        }


        public void SetMoveDirection(Vector2 direction)
        {
            _moveDirection = direction;
        }

        public  Vector2 GetVelocity()
        {
            return _rb.velocity;
        }

        bool IsGrounded()
        {
            var pos = transform.position;
            pos.y -= _col.bounds.extents.y;
            // draw a circle in the scene view
            Debug.DrawRay(pos, Vector2.down * 0.1f, Color.green, 1, false);
            Debug.DrawRay(pos, Vector2.up * 0.1f, Color.green, 1, false);
            Debug.DrawRay(pos, Vector2.right * 0.1f, Color.green, 1, false);
            Debug.DrawRay(pos, Vector2.left * 0.1f, Color.green, 1, false);
            
            var res = Physics2D.OverlapCircleAll(pos, 0.1f, Physics2D.AllLayers).Length > 2;
            
            // var res = Physics2D.OverlapCircle(pos, 0.1f, Physics2D.AllLayers - Physics2D.);
            return res;
        }

        private void Jump()
        {
            // Debug.DrawRay(transform.position, _moveDirection, Color.green, 2, false);

            // _rb.velocity += Vector2.up * jumpForce;
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            // Если вектор приблизительно вверх то прыжок
            if (Vector2.Dot(_moveDirection, Vector2.up) > 0.75 && IsGrounded())
            {
                Jump();
            }

            var oldVelocity = _rb.velocity;
            oldVelocity.x = _moveDirection.x * moveSpeed;
            _rb.velocity = oldVelocity;
        }
    }
}