using UnityEngine;
using System.Collections;
using System;

public class GunController : MonoBehaviour
{
    public Transform WeaponHold;
    public Gun StartingGun;

    private Gun _equippedGun;

    void Start()
    {
        if (StartingGun != null)
        {
            EquipGun(StartingGun);  
        }
    }

    public void EquipGun(Gun gunToEquip)
    {
        if (_equippedGun != null)
        {
            Destroy(_equippedGun.gameObject);
        }

        _equippedGun = Instantiate(gunToEquip, WeaponHold.position, WeaponHold.rotation) as Gun;
        _equippedGun.transform.parent = WeaponHold;
    }

    public void Shoot()
    {
        if (_equippedGun != null)
        {
            _equippedGun.Shoot();
        }
    }
}
