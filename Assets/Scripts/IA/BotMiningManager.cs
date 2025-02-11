﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotMiningManager : MonoBehaviour
{
    public void SendToMine(BuilderUnit builder, int index)
    {
        //builder.Mine(GetOptimumResourceUnit(builder, index));
        ResourceUnit tmp = GetOptimumResourceUnit(builder, index);
        if (tmp == null)
            return;
        builder.Mine(tmp);
    }

    ResourceUnit GetNearestAvailableResourceUnit(BuilderUnit unit1, int index)
    {
        List<ResourceUnit> r = InstanceManager.instanceManager.allResourceUnits;
        int max = r.Count;

        ResourceUnit res = null;

        for (int i = 0; i < max; i++)
        {
            if (r[i].GetResourceIndex() == index && ((res == null) || (Vector3.Distance(unit1.transform.position, r[i].transform.position) < Vector3.Distance(unit1.transform.position, res.transform.position))))
            {
                res = r[i];
            }
        }
        return res;
    }

    ResourceUnit GetOptimumResourceUnit(BuilderUnit unit, int index)
    {
        return (index == 1) ? GetOptimumAsteroid(unit): GetOptimumFarm(unit);
    }

    ResourceUnit GetOptimumAsteroid(BuilderUnit unit)
    {
        List<ResourceUnit> r = InstanceManager.instanceManager.allResourceUnits;
        ResourceUnit currentUnit = null;
        float currentVal = Mathf.Infinity;

        for (int i = 0; i < r.Count; i++)
        {
            if (r[i].GetResourceIndex() != 1)
                continue;

            float newVal = (2*(r[i].GetBuildersCount()) + 1) * Vector3.Distance(unit.transform.position, r[i].transform.position);
            if (newVal < currentVal)
            {
                currentUnit = r[i];
                currentVal = newVal;
            }
        }
        return currentUnit;
    }

    ResourceUnit GetOptimumFarm(BuilderUnit unit)
    {
        List<SelectableObj> r = GetComponent<IAManager>().mySelectableObjs;
        ResourceUnit currentUnit = null;
        float currentVal = Mathf.Infinity;

        for (int i = 0; i < r.Count; i++)
        {
            if (r[i].GetComponent<ResourceUnit>() == null || r[i].GetComponent<ResourceUnit>().GetResourceIndex() != 2)
                continue;

            float newVal = (3 * (r[i].GetComponent<ResourceUnit>().GetBuildersCount()) + 1) * Vector3.Distance(unit.transform.position, r[i].transform.position);
            if (newVal < currentVal)
            {
                currentUnit = r[i].GetComponent<ResourceUnit>();
                currentVal = newVal;
            }
        }
        return currentUnit;
    }
}
