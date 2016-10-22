﻿using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State { Idle, Chasing, Attacking };

    public ParticleSystem DeathEffect;
    public Color AttackColor = Color.red;

    private Material _skinMaterial;
    private Color _originalColor;

    private State _currentState;

    private float _attackDistanceThreshold = .5F;
    // attack interval (in milleseconds)
    private float _timeBetweenAttack = 1;
    private float _damage = 1;

    private float _nextAttackTime;

    private NavMeshAgent _pathFinder;
    private Transform _target;

    private float _myCollisionRadius;
    private float _targetCollisionRadius;

    private LivingEntity _targetEntity;
    private bool _hasTarget;

    protected override void Start()
    {
        base.Start();

        _pathFinder = GetComponent<NavMeshAgent>();
        _skinMaterial = GetComponent<Renderer>().material;
        _originalColor = _skinMaterial.color;
        
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            _currentState = State.Chasing;
            
            _target = GameObject.FindGameObjectWithTag("Player").transform;
            _targetEntity = _target.GetComponent<LivingEntity>();
            _targetEntity.OnDeath += OnTargetDeath;

            _hasTarget = true;

            _myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            _targetCollisionRadius = _target.GetComponent<CapsuleCollider>().radius;

            StartCoroutine(UpdatePath());
        }
    }
    
    void Update()
    {
        if (_hasTarget)
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

        _skinMaterial.color = AttackColor;
        bool hasAppliedDamage = false;

        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                _targetEntity.TakeDamage(_damage);
            }
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

        while (_hasTarget)
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

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (damage >= Health)
        {
            Destroy(Instantiate(DeathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)), DeathEffect.startLifetime);
        }

        base.TakeHit(damage, hitPoint, hitDirection);
    }

    private void OnTargetDeath()
    {
        _hasTarget = false;
        _currentState = State.Idle;
    }
}
