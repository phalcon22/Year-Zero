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
        if (!MultiplayerTools.IsMine(building)) return;

        obj.SetActive(true);
        currentBuilding = building;
        UpdateQueue(building.GetComponent<TaskSystem>().GetTasks());
    }

    void Update()
    {
        if (obj.activeInHierarchy)
        {
            if (content.childCount > 0 && currentBuilding != null)
            {
                if (!bar.gameObject.activeInHierarchy)
                    ShowBar();
                UpdateBar();
            }
            else if (currentBuilding == null)
            {
                SelectUnit.selectUnit.ClearSelection();
                SelectUnit.selectUnit.UpdateUI();
            }
            else
            {
                HideBar();
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

    void ShowBar()
    {
        bar.gameObject.SetActive(true);
    }

    void HideBar()
    {
        bar.gameObject.SetActive(false);
    }

    void UpdateBar()
    {
        bar.value = currentBuilding.GetComponent<TaskSystem>().GetCurrentAdvancement();
    }

    public void ResetBar()
    {
        obj.SetActive(false);
        currentBuilding = null;
        RemoveAllTaskTools();
    }

    public void Cancel()
    {
        Transform tmp = EventSystem.current.currentSelectedGameObject.transform;
        currentBuilding.GetComponent<TaskSystem>().Cancel(tmp.GetComponent<TaskButton>().task);
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
