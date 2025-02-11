﻿using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    GameResource[] resources = new GameResource[] { new GameResource("Energy")
,new GameResource("Ore")
,new GameResource("Food")
    ,new GameResource("Tech")};

    Population population = new Population();

    List<TownHall> homes = new List<TownHall>();

    public bool Pay(int[] costs, int pop, bool forBuilding)
    {
        if (PayCheck(costs, pop, forBuilding))
        {
            int i = 0;
            foreach (GameResource resource in resources)
            {
                resource.Remove(costs[i]);
                i++;
            }
            population.Add(pop);
            return true;
        }
        print("wtf");
        return false;
    }

    public bool PayCheck(int[] costs, int pop, bool forBuilding)
    {
        int i = 0;
        bool possible = true;
        foreach (GameResource resource in resources)
        {
            if (resource.GetValue() < costs[i])
                possible = false;
            i++;
        }
        if (!forBuilding && possible && population.GetCurrentMaxValue() < population.GetValue() + pop)
        {
            possible = false;
        }
        return possible;
    }

    public int GetPayLimiterIndex(int[] costs, int pop, bool forBuilding)
    {
        int res = -1;
        if (!PayCheck(costs, pop, forBuilding))
        {
            for (int i = 0; i < costs.Length; i++)
            {
                if (resources[i].GetValue() < costs[i])
                    return i;
            }
            if (!forBuilding && population.GetCurrentMaxValue() < population.GetValue() + pop)
                return -2;
        }
        return res;
    }

    public ObjectiveState ResourceLimiterToObjectiveState(int[] costs, int pop, bool forBuilding)
    {
        int res = GetPayLimiterIndex(costs, pop, forBuilding);
        switch (res)
        {
            case -2:
                return ObjectiveState.NeedPop;
            case -1:
                return ObjectiveState.Activated;
            case 0:
                return ObjectiveState.NeedEnergy;
            case 1:
                return ObjectiveState.NeedOre;
            case 2:
                return ObjectiveState.NeedFood;
            default:
                print("wtf");
                return ObjectiveState.Done;
        }
    }

    public void Add(int val, int index)
    {
        resources[index].Add(val);
    }

    public int Get(int index)
    {
        return resources[index].GetValue();
    }

    public void Remove(int val, int index)
    {
        resources[index].Remove(val);
    }

    #region homes

    public void AddHome(TownHall home)
    {
        homes.Add(home);
    }

    public List<TownHall> GetHomes()
    {
        return homes;
    }

    public void RemoveHome(TownHall home)
    {
        homes.Remove(home);
    }

    public TownHall GetNearestHome(Vector3 pos)
    {
        TownHall res = null;

        for (int i = 0; i < homes.Count; i++)
        {
            if (res == null || Vector3.Distance(pos, homes[i].transform.position) < Vector3.Distance(pos, res.transform.position))
            {
                res = homes[i];
            }
        }

        return res;
    }

    #endregion

    #region population

    public void AddMaxPopulation(int val)
    {
        population.AddMax(val);
    }

    public int GetPopulation()
    {
        return population.GetValue();
    }

    public int GetMaxPopulation()
    {
        return population.GetCurrentMaxValue();
    }

    public void RemovePopulation(int val)
    {
        population.Remove(val);
    }

    public void RemoveMaxPopulation(int val)
    {
        population.RemoveMax(val);
    }

    #endregion
}
