﻿using UnityEngine;

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

    public void OnTriggerHold()
    {
        if (_equippedGun != null)
        {
            _equippedGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease()
    {
        if (_equippedGun != null)
        {
            _equippedGun.OnTriggerRelease();
        }
    }

    public float GunHeight
    {
        get
        {
            return WeaponHold.position.y;
        }
    }
}
