﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskTool : Tool
{
    [SerializeField]
    GameObject instantiateTaskPrefab;

    ConstructedUnit associatedBuilding;
    MovableUnit associatedUnit;
    [SerializeField] Image image;

    public void Init(ConstructedUnit building, MovableUnit unit)
    {
        associatedBuilding = building;
        associatedUnit = unit;
        SetButtonSprite(unit);

    }

    public void CreateInstantiateTask()
    {
        if (!associatedBuilding.GetComponent<TaskSystem>().Full() && PlayerManager.playerManager.Pay(associatedUnit.costs, associatedUnit.pop, false))
        {
            InstantiateTask task = Instantiate(instantiateTaskPrefab).GetComponent<InstantiateTask>();
            task.transform.SetParent(associatedBuilding.GetComponent<TaskSystem>().taskHolder);
            task.FirstInit(associatedBuilding);
            task.Init(associatedUnit);
            associatedBuilding.GetComponent<TaskSystem>().Add(task);
            SelectUnit.selectUnit.UpdateUI();
        }
    }

    public MovableUnit GetAssociatedUnit()
    {
        return associatedUnit;
    }

    void SetButtonSprite(DestructibleUnit unit)
    {
        if (unit.iconSprite)
        {
            GetComponentInChildren<Text>().gameObject.SetActive(false);
            image.sprite = unit.iconSprite;
        }
        else
        {
            GetComponentInChildren<Text>().text = associatedUnit.objName;
            image.gameObject.SetActive(false);
        }
    }
}
