﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlacementGrid : MonoBehaviourPunCallbacks {

    [SerializeField]
    private float cellSize;
    [SerializeField]
    private GameObject detectionCell;
    [SerializeField]
    private LayerMask groundLayer;
    private string constructionUnit;
    private BuilderUnit builder;

    private List<DetectionCell> cells = new List<DetectionCell>();

    private float lines;
    private float columns;

    public void Init(float lines, float columns, string unit, BuilderUnit builder)
    {
        this.builder = builder;
        constructionUnit = unit;
        this.lines = lines;
        this.columns = columns;
        CreateGrid();
        CreateGhost();
    }

    private void Update()
    {
        UpdateGrid();
    }

    private void UpdateGrid()
    {
        FollowMouse();

        if (AllCellsAvailable() && Input.GetMouseButtonUp(0))
        {
            Construct();
        }
    }

    private void CreateGrid()
    {
        Vector3 vec = transform.position;
        for (int i = 0; i < lines; i++, vec.x += cellSize)
        {
            vec.z = transform.position.z;
            for (int j = 0; j < columns; j++, vec.z += cellSize)
            {
                GameObject obj = Instantiate(detectionCell, vec, Quaternion.identity, transform);
                cells.Add(obj.GetComponent<DetectionCell>());
            }
        }
    }

    private void CreateGhost()
    {
        GameObject obj = ((GameObject)Resources.Load(constructionUnit + "Ghost"));
        Instantiate(obj, GetCenter(), Quaternion.identity, transform);
    }

    public void FollowMouse()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, groundLayer))
        {
            transform.position = GridFix(SetNewCenterTo(hit.point));
        }
    }

    private Vector3 SetNewCenterTo(Vector3 newPos)
    {
        return new Vector3((newPos.x + cellSize / 2) - (lines / 2 * cellSize),
            newPos.y + 0.01f,
            (newPos.z + cellSize / 2) - (columns * cellSize / 2));
    }

    //Create Grid Effect
    private Vector3 GridFix(Vector3 pos)
    {
        float newX = Mathf.FloorToInt(pos.x / cellSize) * cellSize;
        float newZ = Mathf.FloorToInt(pos.z / cellSize) * cellSize;

        return new Vector3(newX, pos.y, newZ);
    }

    private bool AllCellsAvailable()
    {
        bool allAvailable = true;
        foreach (DetectionCell cell in cells)
        {
            if (!cell.CheckAvailability())
                allAvailable = false;
        }
        return allAvailable;
    }

    private void Construct()
    {
        GameObject obj = PhotonNetwork.Instantiate(constructionUnit + "Cons", GetCenter(), Quaternion.identity);
        obj.GetComponent<InConstructionUnit>().Init(constructionUnit + "Cons");
        InstanceManager.instanceManager.mySelectableObjs.Add(obj.GetComponent<SelectableObj>());
        Pay(((GameObject)Resources.Load(constructionUnit + "Unit")).GetComponent<ConstructedUnit>());
        builder.Build(obj.GetComponent<InConstructionUnit>());
        Destroy(this.gameObject);
    }

    private Vector3 GetCenter()
    {
        return new Vector3((transform.position.x - cellSize / 2) + (lines / 2 * cellSize),
            transform.position.y + 0.01f,
            (transform.position.z - cellSize / 2) + (columns * cellSize / 2));
    }

    void Pay(ConstructedUnit unit)
    {
        PlayerManager.playerManager.RemoveWood(unit.costs[0]);
        PlayerManager.playerManager.RemoveStone(unit.costs[1]);
        PlayerManager.playerManager.RemoveGold(unit.costs[2]);
        PlayerManager.playerManager.RemoveMeat(unit.costs[3]);
    }
}
