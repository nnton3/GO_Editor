using System.Collections;
using UnityEngine;

public class PlayerMover : Mover
{
    private Vector3 startPos;
    public Vector3 StartPos => startPos;
    private Quaternion startRot;
    public Quaternion StartRot => startRot;

    public override void Initialize()
    {
        base.Initialize();
        UpdateBoard();

        startPos = transform.position;
        startRot = transform.rotation;
    }

    private void UpdateBoard()
    {
        if (board == null) return;
        board.UpdatePlayerNode();
    }

    protected override IEnumerator MoveRoutine(Vector3 destinationPos, float delayTime)
    {
        yield return base.MoveRoutine(destinationPos, delayTime);
        UpdateBoard();

        var door = currentNode.GetComponent<Opener>();
        if (door != null)
        {
            door.TryToOpen();
            yield return new WaitForSeconds(1f);
        }

        base.FinishMovementEvent.Invoke();
    }

    public void Reset()
    {
        UpdateCurrentNode();
    }
}
