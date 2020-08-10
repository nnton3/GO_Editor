using UnityEngine;

public class SpotlightSensor : EnemySensor
{
    public override void UpdateSensor()
    {
        if (board.PlayerNode.Type == NodeType.Bush) return;
        foundPlayer = transform.position == board.PlayerNode.transform.position;
    }
}
