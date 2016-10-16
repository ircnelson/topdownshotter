using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerComponent : MonoBehaviour {

    private Vector3 _velocity;
    private Rigidbody _rigidbody;
    
	void Start ()
    {
        _rigidbody = GetComponent<Rigidbody>();	
	}
	
	void FixedUpdate ()
    {
        _rigidbody.MovePosition(_rigidbody.position + _velocity * Time.fixedDeltaTime);
	}

    public void Move(Vector3 velocity)
    {
        _velocity = velocity;
    }

    public void LookAt(Vector3 point)
    {
        Vector3 heightCorrectedPoint = new Vector3(point.x, transform.position.y, point.z);

        transform.LookAt(heightCorrectedPoint);
    }
}
