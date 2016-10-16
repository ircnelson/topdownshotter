using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    private float _speed;

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * _speed);
    }
}
