using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    public class EnemyPatrolAI : MonoBehaviour
    {
        [SerializeField] public List<Transform> patrolPoints;

        // reference to the enemy controller
        [SerializeField] public EnemyController enemyController;
        [SerializeField] private int _currentPointIndex;
        [SerializeField] private Transform _currentPoint;

        private void Start()
        {
            _currentPointIndex = 0;
            _currentPoint = patrolPoints[_currentPointIndex];
            enemyController = GetComponent<EnemyController>();
        }

        private void Update()
        {
            enemyController.SetMoveDirection((_currentPoint.position - transform.position).normalized);

            // Если дистанция до точки больше чем 0.1f не меняем точку
            // Также движемся основываясь на горизонтальном положении
            if (Math.Abs(transform.position.x - _currentPoint.position.x) > 0.1f) return;

            // Следующая точка
            _currentPointIndex = (_currentPointIndex + 1) % patrolPoints.Count;
            _currentPoint = patrolPoints[_currentPointIndex];
        }
    }
}