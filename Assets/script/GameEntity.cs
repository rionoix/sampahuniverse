using System;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using Unity.VisualScripting;
using
    System.Collections.Generic;
using Unity.Collections;
public interface IHealth
{
    public void Damage(int amount);
}
[CreateAssetMenu(fileName = "EntityStats", menuName = "Stats/Entity", order = 1)]
public class EntityStats : ScriptableObject, ICloneable
{
    public int maxHealth;
    public float movementSpeed;
    public float sprintMultiplier;
    public string entityName;
    public float attackCooldown;










    public object Clone()

    {
        EntityStats clone = ScriptableObject.CreateInstance<EntityStats>();
        clone.maxHealth = this.maxHealth;
        clone.movementSpeed = this.movementSpeed;
        clone.name = this.name;

        clone.attackCooldown = this.attackCooldown;
        clone.sprintMultiplier = this.sprintMultiplier;
        return clone;
    }

}
public class GameEntity : MonoBehaviour
{
    public EntityStats stats;

    Cooldown healthCooldown;




    void RegenerateHealth()
    {

    }


    public float GetSprintingMultiplier()
    {
        return Math.Max(stats.sprintMultiplier, 1.0f);
    }









}