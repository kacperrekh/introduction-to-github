using BepInEx;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

[BepInPlugin("com.twojmod.infiniteammo", "Infinite Ammo Mod", "1.0.0")]
public class InfiniteAmmoMod : BaseUnityPlugin
{
    private object weapon;
    private FieldInfo currentAmmoField;
    private FieldInfo maxAmmoField;

    void Update()
    {
        if (weapon == null)
        {
            var weaponType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .FirstOrDefault(t => t.Name == "WeaponHandler");

            if (weaponType == null)
            {
                Logger.LogWarning("Nie znaleziono typu WeaponHandler.");
                return;
            }

            weapon = GameObject.FindObjectOfType(weaponType);
            if (weapon != null)
            {
                Logger.LogInfo("Znaleziono broń!");
                currentAmmoField = weaponType.GetField("currentAmmo", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                maxAmmoField = weaponType.GetField("maxAmmo", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                if (currentAmmoField == null || maxAmmoField == null)
                {
                    Logger.LogWarning("Nie znaleziono pól currentAmmo lub maxAmmo.");
                    weapon = null;
                }
            }

            return;
        }

        try
        {
            var maxAmmo = maxAmmoField.GetValue(weapon);
            var current = currentAmmoField.GetValue(weapon);

            if (!current.Equals(maxAmmo))
                currentAmmoField.SetValue(weapon, maxAmmo);
        }
        catch (Exception e)
        {
            Logger.LogError($"Błąd podczas ustawiania amunicji: {e.Message}");
            weapon = null;
        }
    }
}