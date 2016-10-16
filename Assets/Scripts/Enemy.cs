using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State { Idle, Chasing, Attacking };

    private Material _skinMaterial;
    private Color _originalColor;

    private State _currentState;

    private float _attackDistanceThreshold = .5F;
    // attack interval (in milleseconds)
    private float _timeBetweenAttack = 1;

    private float _nextAttackTime;

    private NavMeshAgent _pathFinder;
    private Transform _target;

    private float _myCollisionRadius;
    private float _targetCollisionRadius;

    protected override void Start()
    {
        base.Start();

        _pathFinder = GetComponent<NavMeshAgent>();
        _skinMaterial = GetComponent<Renderer>().material;
        _originalColor = _skinMaterial.color;

        _currentState = State.Chasing;

        _target = GameObject.FindGameObjectWithTag("Player").transform;

        _myCollisionRadius = GetComponent<CapsuleCollider>().radius;
        _targetCollisionRadius = _target.GetComponent<CapsuleCollider>().radius;

        StartCoroutine(UpdatePath());
    }

    void Update()
    {
        if (Time.time > _nextAttackTime)
        {
            float sqrDistanceToTarget = (_target.position - transform.position).sqrMagnitude;

            if (sqrDistanceToTarget < Mathf.Pow(_attackDistanceThreshold + _myCollisionRadius + _targetCollisionRadius, 2))
            {
                _nextAttackTime = Time.time + _timeBetweenAttack;

                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        _currentState = State.Attacking;

        _pathFinder.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 directionToTarget = (transform.position - _target.position).normalized;
        Vector3 attackPosition = _target.position - directionToTarget * (_myCollisionRadius);

        float percent = 0;
        float attackSpeed = 3;

        _skinMaterial.color = Color.red;

        while (percent <= 1)
        {
            percent += Time.deltaTime * attackSpeed;

            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;

            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        _currentState = State.Chasing;
        _pathFinder.enabled = true;
        _skinMaterial.color = _originalColor;
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = .25f;

        while (_target != null)
        {
            if (_currentState == State.Chasing)
            {
                Vector3 directionToTarget = (_target.position - transform.position).normalized;

                Vector3 targetPosition = _target.position - directionToTarget * (_myCollisionRadius + _targetCollisionRadius + _attackDistanceThreshold / 2);

                if (!Dead)
                {
                    _pathFinder.SetDestination(targetPosition);
                }
            }

            yield return new WaitForSeconds(refreshRate);
        }
    }
}
