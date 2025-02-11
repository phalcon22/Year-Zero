﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildToolControls : PlayerControls
{
    [SerializeField]
    PlacementGrid placementGrid;
    PlacementGrid currentPlacementGrid;

    public void CreatePlacementGrid(ConstructedUnit building, BuilderUnit builder)
    {
        currentPlacementGrid = Instantiate(placementGrid).GetComponent<PlacementGrid>();
        currentPlacementGrid.Init(building, builder, false);
    }

    public override void Update()
    {
        if (!CanUpdate()) return;
        base.Update();
        if (currentPlacementGrid == null)
        {
            isActive = false;
        }
    }

    public override void Cancel()
    {
        base.Cancel();
        if (currentPlacementGrid != null)
        {
            GameObject tmp = currentPlacementGrid.gameObject;
            currentPlacementGrid = null;
            Destroy(tmp);
        }
    }
}
