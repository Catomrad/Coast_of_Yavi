using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    public enum AIState
    {
        Idle,
        Patrol,
        Chasing
    }
    public class EnemyController : MonoBehaviour
    {
        [Range(0f, 5f)] public float moveSpeed = 5.0f;

        public AIState aiState = AIState.Patrol;
        
        private Vector2 _moveDirection = Vector2.zero;
        private Rigidbody2D _rb;
        
        // Start is called before the first frame update
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void SetMoveDirection(Vector2 direction)
        {
            _moveDirection = direction;
        }


        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            var oldVelocity = _rb.velocity;
            // Если вектор приблизительно вверх то прыжок
            if (Vector2.Dot(_moveDirection, Vector2.up) > 0.75)
            {
                Debug.Log($"Jump {Vector2.Dot(_moveDirection, Vector2.up)}>0.75 ({_moveDirection})");
                // TODO: Прыжок
            }

            oldVelocity.x = _moveDirection.x * moveSpeed;
            _rb.velocity = oldVelocity;
        }
    }
}