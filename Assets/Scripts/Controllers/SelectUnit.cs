﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

public class SelectUnit : MonoBehaviourPunCallbacks {

    #region Singleton

    public static SelectUnit selectUnit;

    private void Awake()
    {
        selectUnit = this;
    }

    #endregion

    [SerializeField]
    LayerMask interactableLayer;

    public List<SelectableObj> selected;
    public int underSelected = 0;

    Vector3 mousePos1;
    Vector3 mousePos2;

    [SerializeField]
    ToolsPanel toolsPanel;
    [SerializeField]
    CardsPanel cardsPanel;
    [SerializeField]
    AdvancementBar advancementBar;
    [SerializeField]
    TaskBar taskBar;
    [SerializeField]
    PortraitPanel portraitPanel;
    [SerializeField]
    MonoDescriptionPanel monoDescriptionPanel;

    SelectionBox selectionBox;

    [HideInInspector]
    public bool isSelecting;

    [SerializeField]
    float doubleClickTime = 0.3f;
    float oneClick;

    void Start()
    {
        selectionBox = GetComponent<SelectionBox>();
    }

    public void UpdateSelection()
    {
        selectionBox.CheckBox();
        CheckMonoHighlight();
        CheckSelect();
    }

    void CheckSelect()
    {
        if (oneClick > 0)
        {
            oneClick -= Time.deltaTime;
        }
        if (Input.GetMouseButtonDown(0) && !MouseOverUI())
        {
            isSelecting = true;
            mousePos1 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            if (oneClick > 0)
            {
                SelectKindOfUnit();
                isSelecting = false;
                oneClick = 0;
                return;
            }
            oneClick = doubleClickTime;
        }

        if (Input.GetMouseButton(0) && isSelecting)
        {
            mousePos2 = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            if (mousePos2 != mousePos1)
                HighlightObjects();
        }

        if (Input.GetMouseButtonUp(0) && isSelecting)
        {
            CalculateSelection();
            isSelecting = false;
        }
    }

    public void CalculateSelection()
    {
        bool changement = false;
        mousePos2 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        if (mousePos2 == mousePos1)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, interactableLayer.value))
            {
                if (hit.collider.GetComponent<SelectableObj>() != null)
                {
                    if (!Input.GetKey("left ctrl") || hit.collider.GetComponent<SelectableObj>().photonView.IsMine || hit.collider.GetComponent<SelectableObj>().botIndex == -1)
                        ClearSelection();
                    changement = SelectObject(hit.collider.GetComponent<SelectableObj>());
                }
            }
        }
        else
            changement = SelectObjects();

        if (changement)
        {
            UpdateUI(); 
        }
    }

    public void UpdateUI()
    {
        underSelected = 0;
        monoDescriptionPanel.ResetBar();
        advancementBar.ResetBar();
        taskBar.ResetBar();
        cardsPanel.ClearCards();
        toolsPanel.ClearTools();
        if (selected.Count == 0) return;

        if (selected[0].GetComponent<DestructibleUnit>() != null)
        {
            portraitPanel.Init(selected[0].GetComponent<DestructibleUnit>());
            if (selected.Count == 1)
                monoDescriptionPanel.Init(selected[0].GetComponent<DestructibleUnit>());
        }
        if (selected[0].GetComponent<MovableUnit>() != null)
        {
            if (selected.Count > 1)
                cardsPanel.CheckCards();
            else
                toolsPanel.CheckTools(0);
        }
        else
        {
            if (selected[0].GetComponent<InConstructionUnit>() != null)
            {
                advancementBar.Init(selected[0].GetComponent<InConstructionUnit>());
            }
            else if (selected[0].GetComponent<ConstructedUnit>() != null)
            {
                toolsPanel.CheckTools(0);
                if (selected[0].GetComponent<TaskSystem>().GetTasks().Count > 0)
                    taskBar.Init(selected[0].GetComponent<ConstructedUnit>());
            }
        }
    }

    bool SelectObjects()
    {
        bool changement = false;
        Rect selectRect = new Rect(mousePos1.x, mousePos1.y, mousePos2.x - mousePos1.x, mousePos2.y - mousePos1.y);

        bool metSelectableObj = false;
        List<SelectableObj> tmp = new List<SelectableObj>();
        foreach (SelectableObj selectableObj in InstanceManager.instanceManager.mySelectableObjs)
        {
            if (selectRect.Contains(Camera.main.WorldToViewportPoint(selectableObj.transform.position), true))
            {
                tmp.Add(selectableObj);
                metSelectableObj = true;
            }
        }

        if (!metSelectableObj)
            return false;

        if (!Input.GetKey("left ctrl") && metSelectableObj)
            ClearSelection();


        bool mine = false;
        foreach (SelectableObj obj in tmp)
        {
            if (obj.photonView.IsMine)
            {
                mine = true;
            }
        }
        if (mine)
        {
            for (int i = tmp.Count - 1; i >= 0; i--)
            {
                if (!tmp[i].photonView.IsMine)
                {
                    if (tmp[i].GetComponent<SelectableObj>() != null)
                        tmp[i].GetComponent<SelectableObj>().Dehighlight();
                    tmp.RemoveAt(i);
                }
            }
        }

        bool movable = false;
        foreach (SelectableObj obj in tmp)
        {
            if (obj.GetComponent<MovableUnit>() != null)
            {
                movable = true;
            }
        }

        if (movable)
        {
            int nb = 0;
            for (int i = 0; i < tmp.Count; i++)
            {
                if (nb < 24 && tmp[i].GetComponent<MovableUnit>() != null)
                {
                    if (SelectObject(tmp[i]))
                        changement = true;
                    nb++;
                }
                else
                {
                    if (tmp[i].GetComponent<SelectableObj>() != null)
                        tmp[i].GetComponent<SelectableObj>().Dehighlight();
                }
            }
        }
        else
        {
            if (SelectObject(tmp[0]))
                changement = true;
        }
        return changement;
    }

    public bool SelectObject(SelectableObj hitObj)
    {
        bool already = false;
        if (!selected.Contains(hitObj))
        {
            selected.Add(hitObj);
            hitObj.Select();
        }
        else
        {
            already = true;
        }
        return !already;
    }

    public void ClearSelection()
    {
        if (selected.Count == 0)
            return;

        foreach (SelectableObj obj in selected)
        {
            obj.Deselect();
        }
        selected.Clear();
    }



    SelectableObj monohighlighted;
    void CheckMonoHighlight()
    {
        RaycastHit hit;
        bool gotHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, interactableLayer.value);

        if (monohighlighted == null)
        {
            if (gotHit && hit.collider.GetComponent<SelectableObj>() != null)
            {
                monohighlighted = hit.collider.GetComponent<SelectableObj>();
                monohighlighted.Highlight(false);
            }
        }
        else
        {
            if (gotHit && hit.collider.GetComponent<SelectableObj>() != null)
            {
                if (hit.collider.GetComponent<SelectableObj>() != monohighlighted)
                {
                    monohighlighted.Dehighlight();
                    monohighlighted = hit.collider.GetComponent<SelectableObj>();
                    monohighlighted.Highlight(false);
                }
            }
            else
            {
                monohighlighted.Dehighlight();
                monohighlighted = null;
            }
        }
    }

    public void ClearHighlight()
    {
        foreach (SelectableObj selectableObj in InstanceManager.instanceManager.mySelectableObjs)
        {
            if (selectableObj.highlighted)
                selectableObj.Dehighlight();
        }
    }


    void HighlightObjects()
    {
        List<SelectableObj> tmp = new List<SelectableObj>();
        Rect selectRect = new Rect(mousePos1.x, mousePos1.y, mousePos2.x - mousePos1.x, mousePos2.y - mousePos1.y);

        foreach (SelectableObj selectableObj in InstanceManager.instanceManager.allSelectableObjs)
        {
            if (selectableObj == null)
                continue;

            if (selectRect.Contains(Camera.main.WorldToViewportPoint(selectableObj.transform.position), true))
            {
                tmp.Add(selectableObj);
            }
        }

        foreach (SelectableObj obj in tmp)
        {
            obj.Highlight(true);
        }
    }

    public void ChangeUnderSelected(int newVal)
    {
        underSelected = newVal;
        toolsPanel.CheckTools(underSelected);
        portraitPanel.Init(selected[underSelected].GetComponent<DestructibleUnit>());
    }

    public bool InstantSelect()
    {
        if (selected.Count == 0)
            return false;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, interactableLayer.value))
        {
            foreach (SelectableObj obj in selected)
            {
                obj.Interact(hit.collider.GetComponent<Interactable>());
            }
            return true;
        }
        return false;
    }

    public bool MouseOverUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
        for (int i = raycastResultList.Count - 1; i >= 0; i--)
        {
            if (raycastResultList[i].gameObject.GetComponent<MouseThrough>() != null)
            {
                raycastResultList.RemoveAt(i);
            }
        }
        return raycastResultList.Count > 0;
    }

    void SelectKindOfUnit()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, interactableLayer.value))
        {
            if (hit.collider.GetComponent<MovableUnit>() != null && hit.collider.GetComponent<MovableUnit>().photonView.IsMine && hit.collider.GetComponent<MovableUnit>().botIndex == -1)
            {
                ClearSelection();

                int nb = 0;
                int i = 0;
                while (i < InstanceManager.instanceManager.mySelectableObjs.Count && nb < 24)
                {
                    if (InstanceManager.instanceManager.mySelectableObjs[i].GetComponent<MovableUnit>() != null && InstanceManager.instanceManager.mySelectableObjs[i].GetComponent<MovableUnit>().objName == hit.collider.GetComponent<MovableUnit>().objName)
                    {
                        if (Camera.main.rect.Contains(Camera.main.WorldToViewportPoint(InstanceManager.instanceManager.mySelectableObjs[i].transform.position), true))
                        {
                            SelectObject(InstanceManager.instanceManager.mySelectableObjs[i]);
                            nb++;
                        }
                    }
                    i++;
                }
                UpdateUI();
            }
        }
    }

    public void ReloadTools()
    {
        toolsPanel.ReloadTools();
    }
}
    