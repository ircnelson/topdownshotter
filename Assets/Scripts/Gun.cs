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
    public int ProjectilesPerMag;
    public float ReloadTime = .3f;

    [Header("Recoil")]
    public Vector2 KickMinMax = new Vector2(.05f, .2f);
    public Vector2 RecoilAngleMinMax = new Vector2(3, 5);
    public float RecoilMoveSettleTime = .1f;
    public float RecoilRotationSettleTime = .1f;

    [Header("Effects")]
    public Transform Shell;
    public Transform ShellEjection;
    public AudioClip ShootAudio;
    public AudioClip ReloadAudio;

    private MuzzleFlash _muzzleFlash;
    private float _nextShootTime;
    private bool _isReloading;

    private bool _triggerReleasedSinceLastShot;
    private int _shotsRemainingInBurst;
    private int _projectilesRemainingInMag;

    private Vector3 _recoilSmoothDampVelocity;
    private float _recoilAngle;
    private float _recoilRotationSmoothDampVelocity;
    
    void Start()
    {
        _muzzleFlash = GetComponent<MuzzleFlash>();
        _shotsRemainingInBurst = BurstCount;
        _projectilesRemainingInMag = ProjectilesPerMag;
    }

    void LateUpdate()
    {
        // animate recoil
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref _recoilSmoothDampVelocity, RecoilMoveSettleTime);
        _recoilAngle = Mathf.SmoothDamp(_recoilAngle, 0, ref _recoilRotationSmoothDampVelocity, RecoilRotationSettleTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * _recoilAngle;

        if (!_isReloading && _projectilesRemainingInMag == 0)
        {
            Reload();
        }
    }

	private void Shoot()
    {
        if (!_isReloading && Time.time > _nextShootTime && _projectilesRemainingInMag > 0)
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
                if (_projectilesRemainingInMag == 0)
                {
                    break;
                }

                _projectilesRemainingInMag--;

                _nextShootTime = Time.time + MsBetweenShots / 1000;

                Projectile newProjectile = Instantiate(Projectile, ProjectileSpawn[i].position, ProjectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(MuzzleVelocity);
            }

            Instantiate(Shell, ShellEjection.position, ShellEjection.rotation);
            _muzzleFlash.Activate();

            transform.localPosition -= Vector3.forward * Random.Range(KickMinMax.x, KickMinMax.y);

            _recoilAngle += Random.Range(RecoilAngleMinMax.x, RecoilAngleMinMax.y);
            _recoilAngle = Mathf.Clamp(_recoilAngle, 0, 30);

            AudioManager.Instance.PlaySound(ShootAudio, transform.position);
        }
    }

    public void Reload()
    {
        if (!_isReloading && _projectilesRemainingInMag != ProjectilesPerMag)
        {
            StartCoroutine(AnimateReload());

            AudioManager.Instance.PlaySound(ReloadAudio, transform.position);
        }
    }

    IEnumerator AnimateReload()
    {
        _isReloading = true;
        yield return new WaitForSeconds(.2f);

        float reloadSpeed = 1f / ReloadTime;
        float percent = 0;
        Vector3 initialRotation = transform.localEulerAngles;
        float maxReloadAngle = 30;

        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;

            float interpolation = (-Mathf.Pow(percent, 2) * percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRotation + Vector3.left * reloadAngle;

            yield return null;
        }

        _isReloading = false;
        _projectilesRemainingInMag = ProjectilesPerMag;
    }

    public void Aim(Vector3 aimPoint)
    {
        if (!_isReloading)
        {
            transform.LookAt(aimPoint);
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