﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotConstructionManager : MonoBehaviour
{
    int currentSize;
    int currentSide;
    int currentIndex;
    Vector3 lastCorner;
    [SerializeField]
    float buildingDistance;
    Vector3 lastBuildingPos;

    [SerializeField]
    GameObject placementGrid;

    string[] buildingList = new string[]
    {
        "Buildings/Combat Station/Combat Station",
        "Buildings/Conquest Station/Conquest Station",
        "Buildings/Energy Farm/Energy Farm",
        "Buildings/Farm/Farm",
        "Buildings/House/House",
        "Buildings/Laboratory/Laboratory",
        "Buildings/Radar/Radar",
        "Buildings/TownHall/TownHall",
        "Buildings/Turrel/Turrel"
    };

    public ObjectiveState CanConstruct(int index)
    {
        ConstructedUnit building = ((GameObject)Resources.Load(buildingList[index])).GetComponent<ConstructedUnit>();
        ObjectiveState pay = GetComponent<BotManager>().ResourceLimiterToObjectiveState(building.costs, building.pop, true);
        return pay;
    }

    public void Construct(int index, BuilderUnit builder, out InConstructionUnit inConstructionUnit)
    {
        ConstructedUnit building = ((GameObject)Resources.Load(buildingList[index])).GetComponent<ConstructedUnit>();
        GameObject obj = InstanceManager.instanceManager.InstantiateUnit(building.GetConstructorPath(), GetPos(building, builder), Quaternion.identity, GetComponent<IAManager>().botIndex);
        obj.GetComponent<InConstructionUnit>().Init(building);
        GetComponent<BotManager>().Pay(building.costs, building.pop, true);
        builder.Build(obj.GetComponent<InConstructionUnit>());
        inConstructionUnit = obj.GetComponent<InConstructionUnit>();
    }

    public void InitPos(Vector3 home)
    {
        currentSize = 1;
        currentSide = 3;
        currentIndex = 0;
        lastBuildingPos = home;
        lastCorner = home;
    }

    public Vector3 GetPos(ConstructedUnit building, BuilderUnit builder)
    {
        Vector3 newPos = Vector3.zero;
        bool found = false;
        int i = 0;

        while (!found && i < 20)
        {
            newPos = GenerateNewPos();
            if (CanConstructHere(building, builder, newPos))
            {
                found = true;
            }
            i++;
        }

        return newPos;
    }

    public bool CanConstructHere(ConstructedUnit building, BuilderUnit builder, Vector3 pos)
    {
        PlacementGrid grid = Instantiate(placementGrid).GetComponent<PlacementGrid>();
        grid.Init(building, builder, true);
        grid.SetPos(pos);
        bool res = grid.AllCellsAvailable();
        Destroy(grid.gameObject);
        return res;
    }

    Vector3 GenerateNewPos()
    {
        currentIndex++;
        if (currentIndex >= currentSize)
        {
            currentSide++;
            currentIndex = (currentSide == 1 || currentSide == 2) ? 1 : 2;       
        }
        if (currentSide >= 4)
        {
            currentIndex = 0;
            currentSide = 0;
            currentSize += 2;
            lastCorner = lastCorner - new Vector3(buildingDistance, 0, buildingDistance);
            lastBuildingPos = lastCorner - new Vector3(0, 0, buildingDistance);
        }

        switch (currentSide)
        {
            case 0:
                lastBuildingPos.z += buildingDistance;
                break;
            case 1:
                lastBuildingPos.x += buildingDistance;
                break;
            case 2:
                lastBuildingPos.z -= buildingDistance;
                break;
            case 3:
                lastBuildingPos.x -= buildingDistance;
                break;
        }
        return lastBuildingPos;
    }

    public int GetHouseCount()
    {
        int res = 0;
        foreach (SelectableObj obj in GetComponent<IAManager>().mySelectableObjs)
        {
            if (obj != null && obj.GetComponent<HouseUnit>() != null)
                res++;
        }
        return res;
    }

    public int GetFarmCount()
    {
        int res = 0;
        foreach (SelectableObj obj in GetComponent<IAManager>().mySelectableObjs)
        {
            if (obj != null && obj.GetComponent<Farm>() != null)
                res++;
        }
        return res;
    }

    public int GetEnergyFarmCount()
    {
        int res = 0;
        foreach (SelectableObj obj in GetComponent<IAManager>().mySelectableObjs)
        {
            if (obj != null && obj.GetComponent<EnergyFarm>() != null)
                res++;
        }
        return res;
    }

    public int GetBuildingIndexFor(int unitIndex)
    {
        MovableUnit unit = GetComponent<BotInstantiationManager>().GetUnitOfIndex(unitIndex);
        for (int i = 0; i < buildingList.Length; i++)
        {
            if (GetBuildingOfIndex(i).GetComponent<ProductionBuilding>() != null && GetBuildingOfIndex(i).GetComponent<ProductionBuilding>().CanProduct(unit))
                return i;
        }
        print("wtf");
        return -1;
    }

    public ConstructedUnit GetBuildingOfIndex(int index)
    {
        return ((GameObject)Resources.Load(buildingList[index])).GetComponent<ConstructedUnit>();
    }
}
