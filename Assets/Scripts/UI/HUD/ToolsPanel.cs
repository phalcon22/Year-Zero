﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolsPanel : MonoBehaviour {

    [SerializeField]
    Transform buttons;

    [SerializeField]
    TaskTool instantiateTaskToolPrefab;
    [SerializeField]
    BuildTool buildToolPrefab;
    [SerializeField]
    SpellTool spellToolPrefab;

    public void ClearTools()
    {
        while (buttons.childCount > 0)
        {
            Transform child = buttons.GetChild(0);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
    }

    public void CheckTools(int x)
    {
        if (SelectUnit.selectUnit.selected.Count > x)
        {
            if (!MultiplayerTools.IsMine(SelectUnit.selectUnit.selected[x].GetComponent<SelectableObj>())) return;

            ShowToolsList(SelectUnit.selectUnit.selected[x].GetComponent<SelectableObj>().tools);
            ShowSpellsList(SelectUnit.selectUnit.selected[x].GetComponent<SelectableObj>().spells);
        }
    }

    public void ShowToolsList(List<GameObject> list, bool clear = true)
    {
        if (clear)
            ClearTools();
        foreach (GameObject tool in list)
        {
            if (tool.GetComponent<MovableUnit>() != null)
            {
                TaskTool obj = Instantiate(instantiateTaskToolPrefab, buttons);
                obj.Init(SelectUnit.selectUnit.selected[0].GetComponent<ConstructedUnit>(), tool.GetComponent<MovableUnit>());
                obj.GetComponent<Button>().interactable = tool.GetComponent<MovableUnit>().IsAvailable();
            }
            else if (tool.GetComponent<ConstructedUnit>() != null)
            {
                BuildTool obj = Instantiate(buildToolPrefab, buttons);
                obj.Init(tool.GetComponent<ConstructedUnit>());
                obj.GetComponent<Button>().interactable = tool.GetComponent<ConstructedUnit>().IsAvailable();
            }
            else if (tool.GetComponent<Tool>() != null)
            {
                Instantiate(tool, buttons);
            }
            else if (tool.GetComponent<Spell>() != null)
            {
                continue;
            }
            else
            {
                Debug.LogError("Wrong gameobject in tools");
            }
        }
    }

    public void ShowSpellsList(List<GameObject> list)
    {
        foreach (GameObject tool in list)
        {
            if (tool.GetComponent<Spell>() != null)
            {
                SpellTool obj = Instantiate(spellToolPrefab, buttons);
                obj.Init(tool.GetComponent<Spell>());
            }
        }
    }

    public void ReloadTools()
    {
        foreach (Transform tool in buttons)
        {
            if (tool.GetComponent<TaskTool>() != null)
            {
                tool.GetComponent<Button>().interactable = tool.GetComponent<TaskTool>().associatedUnit.IsAvailable();
            }
            else if (tool.GetComponent<BuildTool>() != null)
            {
                tool.GetComponent<Button>().interactable = tool.GetComponent<BuildTool>().GetAssociatedBuilding().IsAvailable();
            }
        }
    }
}
