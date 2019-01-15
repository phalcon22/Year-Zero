﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TaskBar : MonoBehaviour
{
    ConstructedUnit currentBuilding;

    [SerializeField]
    GameObject obj;
    [SerializeField]
    Slider bar;
    [SerializeField]
    Transform content;

    [SerializeField]
    GameObject taskCardButtonPrefab;

    public void Init(ConstructedUnit building)
    {
        obj.SetActive(true);
        currentBuilding = building;
        UpdateQueue(building.GetComponent<TaskSystem>().GetTasks());
    }

    void Update()
    {
        if (obj.activeInHierarchy)
        {
            if (content.childCount > 0)
            {
                UpdateBar();
            }
        }
    }

    public void UpdateQueue(List<Task> tasks)
    {
        RemoveAllTaskTools();
        foreach (Task task in tasks)
        {
            TaskButton obj = Instantiate(taskCardButtonPrefab, content).GetComponent<TaskButton>();
            obj.Init(task);
        }
    }

    void UpdateBar()
    {
        bar.value = currentBuilding.GetComponent<TaskSystem>().GetCurrentAdvancement();
    }

    public void Reset()
    {
        obj.SetActive(false);
        currentBuilding = null;
        RemoveAllTaskTools();
    }

    public void Cancel(int index = -1)
    {
        Transform tmp;
        if (index == 1)
        {
            tmp = EventSystem.current.currentSelectedGameObject.transform;
        }
        else
        {
            tmp = content.GetChild(index);
        }

        SelectUnit.selectUnit.selected[0].GetComponent<TaskSystem>().Cancel(tmp.GetComponent<TaskButton>().GetTask());
        Destroy(tmp);
    }

    void RemoveAllTaskTools()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }
}
