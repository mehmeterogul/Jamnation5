using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType { SpeedUp, SpeedDown, ExtraShoot }

public class PowerUp : MonoBehaviour
{
    [SerializeField] PowerUpType powerUpType = PowerUpType.SpeedUp;

    public PowerUpType GetPowerUpType()
    {
        return powerUpType;
    }
}
