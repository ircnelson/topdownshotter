using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public enum FireMode { Auto, Burst, Single };

    public FireMode CurrentFireMode;

    public Transform[] ProjectileSpawn;
    public Projectile Projectile;
    public float MsBetweenShots = 100;
    public float MuzzleVelocity = 35;

    public int BurstCount;

    public Transform Shell;
    public Transform ShellEjection;

    private MuzzleFlash _muzzleFlash;
    private float nextShootTime;

    private bool _triggerReleasedSinceLastShot;
    private int _shotsRemainingInBurst;

    void Start()
    {
        _muzzleFlash = GetComponent<MuzzleFlash>();

        _shotsRemainingInBurst = BurstCount;
    }

	private void Shoot()
    {
        if (Time.time > nextShootTime)
        {
            if (CurrentFireMode == FireMode.Burst)
            {
                if (_shotsRemainingInBurst == 0)
                {
                    return;
                }

                _shotsRemainingInBurst--;
            }

            else if (CurrentFireMode == FireMode.Single)
            {
                if (!_triggerReleasedSinceLastShot)
                {
                    return;
                }
            }

            for (int i = 0; i < ProjectileSpawn.Length; i++)
            {
                nextShootTime = Time.time + MsBetweenShots / 1000;

                Projectile newProjectile = Instantiate(Projectile, ProjectileSpawn[i].position, ProjectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(MuzzleVelocity);
            }

            Instantiate(Shell, ShellEjection.position, ShellEjection.rotation);
            _muzzleFlash.Activate();
        }
    }

    public void OnTriggerHold()
    {
        Shoot();

        _triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        _triggerReleasedSinceLastShot = true;
        _shotsRemainingInBurst = BurstCount;
    }
}