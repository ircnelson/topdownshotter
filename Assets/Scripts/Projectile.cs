using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask CollisionMask;

    public Color TrailColor;

    private float _speed;
    private float _damage = 1;
    private float _lifetime = 2;
    private float _skinWidth = .1f;
    
    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    void Start()
    {
        Destroy(gameObject, _lifetime);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, CollisionMask);

        if (initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0], transform.position);
        }

        GetComponent<TrailRenderer>().material.SetColor("_TintColor", TrailColor);
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

        if (Physics.Raycast(ray, out hit, moveDistance + _skinWidth, CollisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider, hit.point);
        }
    }

    private void OnHitObject(Collider c, Vector3 hitPoint)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();

        if (damageableObject != null)
        {
            damageableObject.TakeHit(_damage, hitPoint, transform.forward);
        }

        GameObject.Destroy(gameObject);
    }
}
