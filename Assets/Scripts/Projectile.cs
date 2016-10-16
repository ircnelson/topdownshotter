using UnityEngine;
using System.Collections;
using System;

public class Projectile : MonoBehaviour
{
    public LayerMask CollisionMask;

    private float _speed;

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    void Update()
    {
        float moveDistance = _speed * Time.deltaTime;

        CheckCollisions(moveDistance);

        transform.Translate(Vector3.forward * Time.deltaTime * _speed);
    }

    private void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance, CollisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        GameObject.Destroy(gameObject);
    }
}
