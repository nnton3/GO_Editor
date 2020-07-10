
public class BarbedWire : Opener
{
    public override void TryToOpen()
    {
        if (inventory.HaveCutter)
        {
            inventory.HaveCutter = false;
            var obstacle = GetComponent<DynamicObstacle>();
            var node1 = obstacle.Point1.GetComponent<Board_Node>();
            var node2 = obstacle.Point2.GetComponent<Board_Node>();
            node1.AddLink(node2);
            node1.Type = NodeType.Default;
            node2.Type = NodeType.Default;

            Destroy(obstacle.Obstacle.gameObject);
            Destroy(node1.GetComponent<Opener>());
            Destroy(node2.GetComponent<Opener>());
            Destroy(obstacle);
        }
    }
}
