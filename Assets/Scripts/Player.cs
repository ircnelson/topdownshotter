using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerComponent))]
public class Player : MonoBehaviour {

    public float MoveSpeed = 5;

    private Camera _viewCamera;
    private PlayerComponent _controller;
	
	void Start ()
    {
        _controller = GetComponent<PlayerComponent>();

        _viewCamera = Camera.main;
	}
	
	void Update ()
    {
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * MoveSpeed;

        _controller.Move(moveVelocity);

        Ray ray = _viewCamera.ScreenPointToRay(Input.mousePosition);

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);

            Debug.DrawLine(ray.origin, point, Color.red);

            _controller.LookAt(point);
        }
	}
}
