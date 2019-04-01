﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotBuilderManager : MonoBehaviour
{
    List<BuilderUnit> builders = new List<BuilderUnit>();
    BotManager m;

    private void Start()
    {
        m = GetComponent<BotManager>();
    }

    public void Add(BuilderUnit newBuilder)
    {
        builders.Add(newBuilder);
    }

    public void Remove(BuilderUnit builder)
    {
        builders.Remove(builder);
    }

    public ObjectiveState GetOneBuilder(out BuilderUnit builder, bool forHouse, bool toMine)
    {
        builder = GetJoblessBuilder();
        if (builder != null)
            return ObjectiveState.Activated;

        if ((float)builders.Count >= m.GetMaxPopulation() || (float)builders.Count/m.GetMaxPopulation()*100 >= 25)
        {
            float inConstruction = InConstructionBuilders();
            if (inConstruction == builders.Count || inConstruction/builders.Count*10 > 35)
            {
                GetComponent<BotBuilderManager>().DivideMiner();
                return ObjectiveState.NeedWait;
            }
            else
            {
                return TakeOrHouse(out builder, forHouse, toMine);
            }
        }
        else
        {
            if (m.GetPopulation() == m.GetMaxPopulation())
            {
                return TakeOrHouse(out builder, forHouse, toMine);
            }
            else
            {
                return ObjectiveState.NeedBuilder;
            }
        }
    }

    ObjectiveState TakeOrHouse(out BuilderUnit builder, bool forHouse, bool toMine)
    {
        if (GetComponent<BotConstructionManager>().GetHouseCount() < GetComponent<IAObjectivesManager>().step + 2 * GetComponent<IAObjectivesManager>().step)
        {
            ConstructedUnit house = GetComponent<BotConstructionManager>().GetBuildingOfIndex(2);
            if (forHouse || (toMine && !GetComponent<BotManager>().PayCheck(house.costs, house.pop)))
            {
                return TakeBuilder(out builder, forHouse, toMine);
            }
            else
            {
                builder = null;
                return ObjectiveState.NeedPop;
            }
        }
        else
        {
            return TakeBuilder(out builder, forHouse, toMine);
        }
    }

    ObjectiveState TakeBuilder(out BuilderUnit builder, bool forHouse, bool toMine)
    {
        builder = null;

        if (builders.Count == 0)
        {
            return ObjectiveState.SuicideTroop;
        }
        else if (MiningBuilders() > 0)
        {
            if (toMine)
                return ObjectiveState.Done;
            builder = GetMiningBuilder();
            return ObjectiveState.Activated;
        }
        else
        {
            return ObjectiveState.NeedWait;
        }
    }

    BuilderUnit GetInConstructionBuilder()
    {
        foreach (BuilderUnit builder in builders)
        {
            if (builder.IsBuilding())
            {
                return builder;
            }
        }
        return null;
    }

    float InConstructionBuilders()
    {
        float res = 0;
        foreach (BuilderUnit builder in builders)
        {
            if (builder.IsBuilding())
                res++;
        }
        return res;
    }

    BuilderUnit GetMiningBuilder()
    {
        int ore = MiningBuilders(1);
        int food = MiningBuilders(2);
        int index = (ore < food) ? 2 : 1;
        foreach (BuilderUnit builder in builders)
        {
            if (builder.IsMining() && builder.GetComponent<MiningSystem>().currentResourceUnit.GetResourceIndex() == index)
            {
                return builder;
            }
        }
        return null;
    }

    int MiningBuilders()
    {
        int res = 0;
        foreach (BuilderUnit builder in builders)
        {
            if (builder.IsMining())
                res++;
        }
        return res;
    }

    int MiningBuilders(int resourceIndex)
    {
        int res = 0;
        foreach (BuilderUnit builder in builders)
        {
            if (builder.IsMining() && builder.GetComponent<MiningSystem>().currentResourceUnit.GetResourceIndex() == resourceIndex)
                res++;
        }
        return res;
    }

    BuilderUnit GetJoblessBuilder()
    {
        foreach (BuilderUnit builder in builders)
        {
            if (builder.IsDoingNothing())
            {
                return builder;
            }
        }
        return null;
    }

    float JoblessBuilders()
    {
        float res = 0;
        foreach (BuilderUnit builder in builders)
        {
            if (builder.IsDoingNothing())
                res++;
        }
        return res;
    }

    public void DivideMiner()
    {
        int ore = MiningBuilders(1);
        int food = MiningBuilders(2);
        int index = (ore < food) ? 2 : 1;
        int index2 = (ore < food) ? 1 : 2;
        while (Mathf.Abs(ore - food) > 1)
        {
            foreach (BuilderUnit builder in builders)
            {
                if (builder.IsMining() && builder.GetComponent<MiningSystem>().currentResourceUnit.GetResourceIndex() == index)
                {
                    GetComponent<BotMiningManager>().SendToMine(builder, index2);
                    if (ore < food)
                    {
                        ore++;
                        food--;
                    }
                    else
                    {
                        ore--;
                        food++;
                    }
                    break;
                }
            }
        }
    }
}
