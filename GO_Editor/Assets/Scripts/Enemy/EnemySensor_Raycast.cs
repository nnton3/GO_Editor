﻿using UnityEngine;

public class EnemySensor_Raycast : EnemySensor
{
    [SerializeField] private float sensorDistance = 9;
    private LineRenderer aimLine;

    public override void Initialize()
    {
        base.Initialize();
        aimLine = GetComponent<LineRenderer>();

        if (aimLine != null) aimLine.SetPosition(1, new Vector3(0f, 0.6f, sensorDistance));
    }

    public override void UpdateSensor()
    {
        if (board == null) return;
        
        RaycastHit raycastHit;
        var ray = new Ray(transform.position + new Vector3(0f, .6f, 0f), transform.TransformVector(directionToSearch));
        if (Physics.Raycast(ray, out raycastHit, sensorDistance))
            if (raycastHit.transform.GetComponent<PlayerManager>())
                if (board.PlayerNode.Type != NodeType.Bush)
                    foundPlayer = true;
    }
}