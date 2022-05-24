using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
///   A simple editor component to make easier work with weapon
///   prototypes and adjust values.
/// </summary>
[CustomEditor(typeof(WeaponController))]
public class WeaponEditor : Editor
{
    WeaponType variant;
    WeaponController controller;

    // The function that makes the custom editor work
    public override void OnInspectorGUI()
    {
        controller = (WeaponController)target;
        variant = (WeaponType)EditorGUILayout.EnumPopup("Variant", variant);

        // Create a space to separate this enum popup from the other variables 
        EditorGUILayout.Space();
        switch (variant)
        {
            case WeaponType.PISTOL:
                controller.weapon = Weapon.PISTOL;
                DisplayPistol();
                break;
            case WeaponType.GRENADE:
                controller.weapon = Weapon.GRENADE;
                DisplayGrenade();
                break;
            case WeaponType.FLAMETHROWER:
                controller.weapon = Weapon.FLAMETHROWER;
                DisplayFlamer();
                break;
            case WeaponType.LIGHTNING:
                controller.weapon = Weapon.LIGHTNING;
                DisplayPikachu();
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    ///   Serializes all the fields of the given generic parameter.
    ///   Remember that must be one of Weapon type!
    /// </summary>
    private void SerializeFields<T>()
    {
        var fields = typeof(T).GetFields().Select(f => f.Name);
        var weapon = serializedObject.FindProperty("weapon");
        var reference = weapon.objectReferenceValue;
        var current = new SerializedObject(reference);
        foreach (var name in fields)
            EditorGUILayout.PropertyField(current.FindProperty(name));
    }

    /// <summary>Displays pistol properties</summary>
    void DisplayPistol()
    {
        SerializeFields<PistolType>();
    }

    /// <summary>Displays grenade launcher properties</summary>
    void DisplayGrenade()
    {
        SerializeFields<GrenadeLauncherType>();
    }

    /// <summary>Displays flamethrower properties</summary>
    void DisplayFlamer()
    {
        SerializeFields<FlamethrowerType>();
    }

    /// <summary>
    ///   ピカチュウみたいね？　ピカちゃんは凄くピカピカ✨！
    ///   後で、雷⚡ができった…
    /// </summary>
    void DisplayPikachu()
    {
        SerializeFields<LightningType>();
    }
}