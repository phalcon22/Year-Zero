﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MiningSystem))]
[RequireComponent(typeof(BuildingSystem))]
public class BuilderUnit : MovableUnit {

    public List<GameObject> buildings;

    //patrolSystem
    MiningSystem miningSystem;
    BuildingSystem buildingSystem;
    JoblessConstructorsPanel jobless;

    public override void Awake()
    {
        base.Awake();
        miningSystem = GetComponent<MiningSystem>();
        buildingSystem = GetComponent<BuildingSystem>();
        jobless = GameObject.Find("JoblessConstructorsPanel").GetComponent<JoblessConstructorsPanel>();
        jobless.UpdatePanel();
    }

    public override void Interact(Interactable obj)
    {
        if (obj.GetComponent<InConstructionUnit>() != null)
        {
            Build(obj.GetComponent<InConstructionUnit>());
        }
        else if (obj.GetComponent<ResourceUnit>() != null && (obj.GetComponent<ConstructedUnit>() == null || obj.photonView.IsMine))
        {
            Mine(obj.GetComponent<ResourceUnit>());
        }
        jobless.UpdatePanel();
    }

    public void Build(InConstructionUnit obj)
    {
        ResetAction();
        buildingSystem.InitBuild(obj);
        jobless.UpdatePanel();
    }

    public void StopBuild()
    {
        buildingSystem.StopBuilding();
        jobless.UpdatePanel();
    }

    public void Mine(ResourceUnit obj)
    {
        ResetAction();
        miningSystem.InitMining(home, obj);
        jobless.UpdatePanel();
    }

    public void StopMine()
    {
        miningSystem.StoptMining();
        jobless.UpdatePanel();
    }

    public override void Patrol(Vector3 pos1, Vector3 pos2, float stoppingDistance)
    {
        ResetAction();
        base.Patrol(pos1, pos2, stoppingDistance);
        jobless.UpdatePanel();
    }

    public override void ResetAction()
    {
        base.ResetAction();
        if (miningSystem.IsMining())
            StopMine();
        if (buildingSystem.IsBuilding())
            StopBuild();
        jobless.UpdatePanel();
    }

    public bool IsDoingNothing()
    {
        bool immobile = Vector3.Distance(agent.destination, transform.position) <= 1;
        return (!patrolSystem.IsPatroling() && !miningSystem.IsMining() && !buildingSystem.IsBuilding() && immobile);
    }
}