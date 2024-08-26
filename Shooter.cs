using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class stores information on obtained guns and the currently equipped gun.
/// It also reads input for firing the guns
/// </summary>
public class Shooter : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The list of guns this shooter can potentially have access to")]
    public List<Gun> guns = new List<Gun>();
    [Tooltip("The index in the available gun list of the gun currently equipped")]
    public int equippedGunIndex = 0;
    [Tooltip("The input manager that this reads input from")]
    public InputManager inputManager;
    [Tooltip("Whether or not this shooter is controlled by the player")]
    public bool isPlayerControlled = false;

    
    void Start()
    {
        SetUpInput();
        SetUpGuns();
    }

    
    void Update()
    {
        CheckInput();
    }

    
    void CheckInput()
    {
        if (!isPlayerControlled)
        {
            return;
        }

       
        if (Time.timeScale == 0)
        {
            return;
        }

        if (guns.Count > 0)
        {
            if (guns[equippedGunIndex].fireType == Gun.FireType.semiAutomatic)
            {
                if (inputManager.firePressed)
                {
                    FireEquippedGun();
                }
            }
            else if (guns[equippedGunIndex].fireType == Gun.FireType.automatic)
            {
                if (inputManager.firePressed || inputManager.fireHeld)
                {
                    FireEquippedGun();
                }
            }

            if (inputManager.cycleWeaponInput != 0)
            {
                CycleEquippedGun();
            }

            if (inputManager.nextWeaponPressed)
            {
                GoToNextWeapon();
            }

            if (inputManager.previousWeaponPressed)
            {
                GoToPreviousWeapon();
            }
        }
    }

    
    public void GoToNextWeapon()
    {
        List<Gun> availableGuns = guns.Where(item => item.available == true).ToList();
        int maximumAvailableGunIndex = availableGuns.Count - 1;
        int equippedAvailableGunIndex = availableGuns.IndexOf(guns[equippedGunIndex]);

        equippedAvailableGunIndex += 1;
        if (equippedAvailableGunIndex > maximumAvailableGunIndex)
        {
            equippedAvailableGunIndex = 0;
        }

        EquipGun(guns.IndexOf(availableGuns[equippedAvailableGunIndex]));
    }

    
    public void GoToPreviousWeapon()
    {
        List<Gun> availableGuns = guns.Where(item => item.available == true).ToList();
        int maximumAvailableGunIndex = availableGuns.Count - 1;
        int equippedAvailableGunIndex = availableGuns.IndexOf(guns[equippedGunIndex]);

        equippedAvailableGunIndex -= 1;
        if (equippedAvailableGunIndex < 0)
        {
            equippedAvailableGunIndex = maximumAvailableGunIndex;
        }

        EquipGun(guns.IndexOf(availableGuns[equippedAvailableGunIndex]));
    }

    
    void CycleEquippedGun()
    {
        float cycleInput = inputManager.cycleWeaponInput;
        List<Gun> availableGuns = guns.Where(item => item.available == true).ToList();
        int maximumAvailableGunIndex = availableGuns.Count - 1;
        int equippedAvailableGunIndex = availableGuns.IndexOf(guns[equippedGunIndex]);
        if (cycleInput < 0)
        {
            equippedAvailableGunIndex += 1;
            if (equippedAvailableGunIndex > maximumAvailableGunIndex)
            {
                equippedAvailableGunIndex = 0;
            }
        }
        else if (cycleInput > 0)
        {
            equippedAvailableGunIndex -= 1;
            if (equippedAvailableGunIndex < 0)
            {
                equippedAvailableGunIndex = maximumAvailableGunIndex;
            }
        }

        EquipGun(guns.IndexOf(availableGuns[equippedAvailableGunIndex]));
    }

    
    /// <param name="gunIndex">The index of the gun to make the equipped gun</param>
    public void EquipGun(int gunIndex)
    {
        equippedGunIndex = gunIndex;
        guns[equippedGunIndex].gameObject.SetActive(true);
        for (int i = 0; i < guns.Count; i++)
        {
            if (equippedGunIndex != i)
            {
                guns[i].gameObject.SetActive(false);
            }
        }
        GameManager.UpdateUIElements();
    }

    
    void SetUpGuns()
    {
        foreach(Gun gun in guns)
        {
            if (gun != null)
            {
                if (gun.available && guns[equippedGunIndex] == gun)
                {
                    gun.gameObject.SetActive(true);
                }
                else
                {
                    gun.gameObject.SetActive(false);
                }
            }
        }
    }

    
    void SetUpInput()
    {
        if (inputManager == null)
        {
            inputManager = FindObjectOfType<InputManager>();
        }
        if (inputManager == null)
        {
            Debug.LogError("There is no input manager in the scene, the shooter script requires an input manager in order to work for the player");
        }
    }

   
    public void FireEquippedGun()
    {
        if (guns[equippedGunIndex] != null && guns[equippedGunIndex].available)
        {
            guns[equippedGunIndex].Fire();
        }   
    }

    
    /// <param name="gunIndex">The index of the gun to make available</param>
    public void MakeGunAvailable(int gunIndex)
    {
        if (gunIndex < guns.Count && guns[gunIndex] != null && guns[gunIndex].available == false)
        {
            guns[gunIndex].available = true;
            EquipGun(gunIndex);
        }
    }
}
