
public class SpotlightSensor : EnemySensor
{
    public override void UpdateSensor()
    {
        foundPlayer = transform.position == board.PlayerNode.transform.position;
    }
}
