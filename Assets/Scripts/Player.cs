using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float MoveSpeed = 5;

    private Camera _viewCamera;
    private PlayerController _playerController;
    private GunController _gunController;

    protected override void Start()
    {
        base.Start();

        _playerController = GetComponent<PlayerController>();
        _gunController = GetComponent<GunController>();

        _viewCamera = Camera.main;
	}
	
	void Update ()
    {
        // Movement
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * MoveSpeed;

        _playerController.Move(moveVelocity);

        // Look
        Ray ray = _viewCamera.ScreenPointToRay(Input.mousePosition);

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);

            Debug.DrawLine(ray.origin, point, Color.red);

            _playerController.LookAt(point);
        }

        // Weapon
        if (Input.GetMouseButton(0))
        {
            _gunController.OnTriggerHold();
        }

        if (Input.GetMouseButtonUp(0))
        {
            _gunController.OnTriggerRelease();
        }
    }
}
