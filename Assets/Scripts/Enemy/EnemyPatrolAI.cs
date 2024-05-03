using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Enemy
{
    public enum AIState
    {
        Idle,
        Patrol,
        Chasing
    }

    // TODO: добавить возможность: прыгать когда зашел в тупик
    [RequireComponent(typeof(EnemyController))]
    public class EnemyPatrolAI : MonoBehaviour
    {
        [SerializeField] public List<Transform> patrolPoints;

        [Tooltip("Will be set to Patrol if patrol points are set")]
        public AIState aiState = AIState.Idle;

        [Range(0f, 3f)] public float stopTimeBetweenPoints = 1.0f;

        private Transform _player;
        [SerializeField] private bool _isPaused;

        // reference to the enemy controller
        [SerializeField] public EnemyController enemyController;

        [Tooltip("Distance threshold to when the enemy should stop moving closer to the player")] [Range(0f, 3f)]
        public float distanceThreshold = 1.0f;

        private int _currentPointIndex;
        private Transform _currentPoint;

        // boundaries if enemy goes out of bounds while chasing the player
        private Vector2 _leftBoundary;

        private Vector2 _rightBoundary;
        // private Vector2 _topBoundary;
        // private Vector2 _bottomBoundary;

        #region events

        [Tooltip("Collider MUST be set \"is Trigger\" to TRUE")]
        public ColliderEventConduit detectionEvent;


        private void OnEnable()
        {
            // detectionEvent = transform.GetComponentInChildren<ColliderEventConduit>();
            detectionEvent.OnTriggerEnter2DEvent += PlayerEnter;
            detectionEvent.OnTriggerExit2DEvent += PlayerExit;
        }

        private void OnDisable()
        {
            if (detectionEvent == null) return;
            detectionEvent.OnTriggerEnter2DEvent -= PlayerEnter;
            detectionEvent.OnTriggerExit2DEvent -= PlayerExit;
        }

        #endregion

        private void Start()
        {
            _currentPointIndex = 0;
            _currentPoint = patrolPoints[_currentPointIndex];
            enemyController = GetComponent<EnemyController>();

            if (patrolPoints.Count <= 0) return;

            // ↓ patrol points are set ↓
            _leftBoundary = patrolPoints[0].position;
            _rightBoundary = patrolPoints[0].position;
            foreach (var point in patrolPoints.Where(point => point != null))
            {
                if (point.position.x > _rightBoundary.x) _rightBoundary = point.position;
                if (point.position.x < _leftBoundary.x) _leftBoundary = point.position;
            }

            aiState = AIState.Patrol;
        }

        private void Update()
        {
            UpdateState();
            switch (aiState)
            {
                case AIState.Patrol:
                    Patrolling();
                    break;
                case AIState.Chasing:
                    Chasing();
                    break;
                case AIState.Idle:
                default:
                    enemyController.SetMoveDirection(Vector2.zero);
                    break;
            }
        }

        private void UpdateState()
        {
            switch (aiState)
            {
                case AIState.Patrol:
                    if (_player != null) aiState = AIState.Chasing;
                    break;
                case AIState.Chasing:
                    if (_player != null) break;
                    aiState = !FindAndSetClosestPatrolPoint() ? AIState.Idle : AIState.Patrol;
                    break;
                case AIState.Idle:
                    if (_player != null) aiState = AIState.Chasing;
                    break;
                default:
                    aiState = AIState.Idle;
                    break;
            }
        }

        /// <summary>
        /// Finds and sets the closest patrol point based on the current position of the object.
        /// </summary>
        /// <remarks>
        /// This method calculates the distance between the current position of the object and each patrol point.
        /// It then selects the patrol point with the minimum distance and sets it as the current point.
        /// </remarks>
        /// <returns>
        /// True if a patrol point was found and set, false otherwise.
        /// </returns>
        private bool FindAndSetClosestPatrolPoint()
        {
            var position = transform.position;
            var minDistance = float.MaxValue;
            int? closestPointIndex = null;

            for (var i = 0; i < patrolPoints.Count; i++)
            {
                var distance = Vector2.Distance(position, patrolPoints[i].position);
                if (distance >= minDistance) continue;
                minDistance = distance;
                closestPointIndex = i;
            }

            if (closestPointIndex == null) return false;
            _currentPointIndex = (int)closestPointIndex;
            _currentPoint = patrolPoints[_currentPointIndex];
            return true;
        }

        private void MoveToNextPoint()
        {
            // Выбираем следующую точку патрулирования
            _currentPointIndex = (_currentPointIndex + 1) % patrolPoints.Count;
            _currentPoint = patrolPoints[_currentPointIndex];
        }

        private void Patrolling()
        {
            if (_isPaused)
            {
                enemyController.SetMoveDirection(Vector2.zero);
                return;
            }

            enemyController.SetMoveDirection((_currentPoint.position - transform.position).normalized);
            // Если дистанция до точки больше чем 0.1f не меняем точку
            // Также движемся основываясь на горизонтальном положении
            // if (Math.Abs(transform.position.x - _currentPoint.position.x) > 0.1f) return;
            if (Vector2.Distance(transform.position, _currentPoint.position) > 0.1f) return;
            // Пауза
            var pos = transform.position;
            var dtnext = patrolPoints[(_currentPointIndex + 1) % patrolPoints.Count].position - pos;
            var dtprev = patrolPoints[_currentPointIndex].position - pos;
            if (dtnext.x * dtprev.x < 0) StartCoroutine(PauseBeforeNextPoint());
            else MoveToNextPoint();
        }

        private IEnumerator PauseBeforeNextPoint()
        {
            // Вход в состояние паузы
            _isPaused = true;
            yield return new WaitForSeconds(stopTimeBetweenPoints);
            _isPaused = false;
            MoveToNextPoint();
        }

        private void Chasing()
        {
            if (_player == null) return;
            var targetPos = _player.position;

            // clamp the target position to the boundaries
            targetPos.x = Mathf.Clamp(targetPos.x, _leftBoundary.x, _rightBoundary.x);

            // don't come to close
            if (Vector2.Distance(transform.position, targetPos) < distanceThreshold)
            {
                enemyController.SetMoveDirection(Vector2.zero);
                return;
            }

            var dir = (targetPos - transform.position).normalized;
            enemyController.SetMoveDirection(dir);
        }

        private void PlayerEnter(Collider2D other)
        {
            if (other.CompareTag("Player")) _player = other.transform;
        }

        private void PlayerExit(Collider2D other)
        {
            // Может быть (наверно), что другой игрок (а может и не игрок вовсе) вышел (не тот за которым гнались),
            // поэтому тут условие! 
            if (_player == other.transform) _player = null;
        }
    }
}