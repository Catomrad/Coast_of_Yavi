using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UI;
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

    // TODO: добавить возможность: перепрыгивать дыры
    // TODO: добавить возможность: прыгать когда зашел в тупик
    public class EnemyPatrolAI : MonoBehaviour
    {
        [SerializeField] public List<Transform> patrolPoints;

        [Tooltip("Will be set to Patrol if patrol points are set")]
        public AIState aiState = AIState.Idle;

        [SerializeField] private Transform _player;

        // circle collider for player detection

        // reference to the enemy controller
        [SerializeField] public EnemyController enemyController;
        private int _currentPointIndex;
        private Transform _currentPoint;

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
            if (patrolPoints.Count > 0) aiState = AIState.Patrol;
        }

        private void Update()
        {
            UpdateState();
            switch (aiState)
            {
                case AIState.Patrol:
                    Patrolling();
                    break;
                case AIState.Idle:
                    enemyController.SetMoveDirection(Vector2.zero);
                    break;
                case AIState.Chasing:
                    Chasing();
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
                    if (_player == null)
                    {
                        FindAndSetClosestPatrolPoint();
                        aiState = AIState.Patrol;
                    }

                    break;
                case AIState.Idle:
                default:
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
        private void FindAndSetClosestPatrolPoint()
        {
            var position = transform.position;
            var minDistance = float.MaxValue;
            int? closestPointIndex = null;

            for (var i = 0; i < patrolPoints.Count; i++)
            {
                var distance = Vector2.Distance(position, patrolPoints[i].position);
                if (!(distance < minDistance)) continue;
                minDistance = distance;
                closestPointIndex = i;
            }

            if (closestPointIndex == null) return;
            _currentPointIndex = (int)closestPointIndex;
            _currentPoint = patrolPoints[_currentPointIndex];
        }


        // TODO: Добавить паузу между точками разворота ( или оставить это на EnemyController )
        private void Patrolling()
        {
            enemyController.SetMoveDirection((_currentPoint.position - transform.position).normalized);

            // Если дистанция до точки больше чем 0.1f не меняем точку
            // Также движемся основываясь на горизонтальном положении
            // if (Math.Abs(transform.position.x - _currentPoint.position.x) > 0.1f) return;
            
            if (Vector2.Distance( transform.position, _currentPoint.position) > 0.1f) return;

            // Следующая точка
            _currentPointIndex = (_currentPointIndex + 1) % patrolPoints.Count;
            _currentPoint = patrolPoints[_currentPointIndex];
        }

        private void Chasing()
        {
            if (_player == null) return;
            var dir = (_player.position - transform.position).normalized;
            enemyController.SetMoveDirection(dir);
        }

        private void PlayerEnter(Collider2D other)
        {
            if (other.CompareTag("Player")) _player = other.transform;
        }

        private void PlayerExit(Collider2D other)
        {
            // Может быть (наверно), что другой игрок (а может и не икрог вовсе) вышел (не тот за которым гнались),
            // поэтому тут условие! 
            if (_player == other.transform) _player = null;
        }
    }
}