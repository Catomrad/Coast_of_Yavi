using System;
using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [Range(0f, 5f)] public float moveSpeed = 5.0f;
        [Range(0f, 500f)] public float jumpForce = 5.0f;

        private Vector2 _moveDirection = Vector2.zero; // can be zero
        private Rigidbody2D _rb;
        private Collider2D _col;
        private int _entityLayer = 1 << 3;

        // Start is called before the first frame update
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<Collider2D>();
        }


        public void SetMoveDirection(Vector2 direction)
        {
            // check if the direction is normalized
            if (direction.sqrMagnitude > 1.0f && direction != Vector2.zero) direction.Normalize();

            _moveDirection = direction;
        }

        public Vector2 GetVelocity()
        {
            return _rb.velocity;
        }

        private bool IsGrounded()
        {
            var pos = transform.position;
            pos.y -= _col.bounds.extents.y;
            // draw a circle in the scene view
            Debug.DrawRay(pos, Vector2.down * 0.1f, Color.green, 1, false);
            Debug.DrawRay(pos, Vector2.up * 0.1f, Color.green, 1, false);
            Debug.DrawRay(pos, Vector2.right * 0.1f, Color.green, 1, false);
            Debug.DrawRay(pos, Vector2.left * 0.1f, Color.green, 1, false);

            // TODO: Сделать коллизию только с тем от чего можно оттолкнуться (всё ещё коллизия с триггерами)
            var res = Physics2D.OverlapCircleAll(pos, 0.1f, Physics2D.DefaultRaycastLayers & ~_entityLayer).Length > 1;

            return res;
        }

        private void Jump()
        {
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            // Если вектор приблизительно вверх то прыжок.
            // Довольно эффективно, ведь пока объект не захочет прыгать(1-ое условие),
            // условие IsGrounded не будет выполняться.
            if (Vector2.Dot(_moveDirection, Vector2.up) > 0.75 && IsGrounded())
            {
                Jump();
            }

            var oldVelocity = _rb.velocity;
            var horizontal = Math.Sign(_moveDirection.x); // 1 or 0 or -1
            // Debug.Log($"horizontal: {horizontal} _moveDirection: {_moveDirection} {_moveDirection.magnitude} {_moveDirection.normalized}");
            oldVelocity.x = horizontal * moveSpeed;
            _rb.velocity = oldVelocity;
        }
    }
}