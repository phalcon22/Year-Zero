﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportSpellZone : MonoBehaviour
{
    DestroyerSupportSpell spell;

    List<MovableUnit> units = new List<MovableUnit>();

    bool wasActivated = false;

    float lifeBoost = 1;
    float atkBoost = 1;

    public void Init(Transform spellholder)
    {
        foreach (Transform spell in spellholder)
        {
            if (spell.GetComponent<DestroyerSupportSpell>() != null)
            {
                this.spell = spell.GetComponent<DestroyerSupportSpell>();
            }
        }
    }

    void Update()
    {
        if (spell == null)
            return;
        if (wasActivated && !spell.Activated())
        {
            wasActivated = false;
            for (int i = units.Count-1; i>=0; i--)
            {
                units[i].RemoveBoost();
            }
        }
        if (!wasActivated && spell.Activated())
        {
            wasActivated = true;
            for (int i = units.Count - 1; i >= 0; i--)
            {
                units[i].AddBoost(atkBoost, lifeBoost);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MovableUnit>() != null && !InstanceManager.instanceManager.IsEnemy(other.GetComponent<MovableUnit>()) && !units.Contains(other.GetComponent<MovableUnit>()))
        {
            units.Add(other.GetComponent<MovableUnit>());
            if (spell.Activated())
                other.GetComponent<MovableUnit>().AddBoost(atkBoost, lifeBoost);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<MovableUnit>() != null && units.Contains(other.GetComponent<MovableUnit>()))
        {
            other.GetComponent<MovableUnit>().RemoveBoost();
            units.Remove(other.GetComponent<MovableUnit>());
        }
    }
}
