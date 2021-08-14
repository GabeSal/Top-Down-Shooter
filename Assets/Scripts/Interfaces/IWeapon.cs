using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    public string WeaponName { get; }
    public bool IsActive { get; }
    public int SlotPosition { get; }

}
