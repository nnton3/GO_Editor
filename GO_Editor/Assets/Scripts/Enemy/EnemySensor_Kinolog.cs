using UnityEngine;

public class EnemySensor_Kinolog : EnemySensor
{
    [SerializeField] private float sensorDistance = 9;

    public override void UpdateSensor()
    {
        base.UpdateSensor();

        RaycastHit raycastHit;
        var ray = new Ray(transform.position + new Vector3(0f, .6f, 0f), transform.TransformVector(directionToSearch));
        if (Physics.Raycast(ray, out raycastHit, sensorDistance))
            if (raycastHit.transform.GetComponent<PlayerManager>())
                if (board.PlayerNode.Type != NodeType.Bush)
                    RaiseAlarmAround();
    }

    private void RaiseAlarmAround()
    {
        foreach (var node in GetComponent<EnemieMover>().CurrentNode.NeighborNodes)
        {
            var enemies = board.FindEnemiesAt(node);
            if (enemies.Count != 0)
            {
                GameManager.RaiseAlarmEvent?.Invoke(GetComponent<EnemieMover>().CurrentNode);
                return;
            }
        }
    }
}
