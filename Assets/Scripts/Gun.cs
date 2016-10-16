using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public Transform Muzzle;
    public Projectile Projectile;
    public float MsBetweenShots = 100;
    public float MuzzleVelocity = 35;

    private float nextShootTime;

	public void Shoot()
    {
        if (Time.time > nextShootTime)
        {
            nextShootTime = Time.time + MsBetweenShots / 1000;

            Projectile newProjectile = Instantiate(Projectile, Muzzle.position, Muzzle.rotation) as Projectile;
            newProjectile.SetSpeed(MuzzleVelocity);
        }
    }
}
