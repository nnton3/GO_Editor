using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Mover : MonoBehaviour
{
    #region Variables
    [SerializeField] protected iTween.EaseType easeType = iTween.EaseType.easeInOutExpo;
    [SerializeField] protected float moveSpeed = 1.5f;
    [SerializeField] protected float iTweenDelay = 0f;
    [SerializeField] protected float rotateTime = 0.5f;

    protected Vector3 destination;
    protected bool faceDestination = true;
    protected Board board;
    protected bool isMoving = false;
    public bool IsMoving => isMoving;

    protected Board_Node currentNode;
    public Board_Node CurrentNode => currentNode;

    [HideInInspector] public UnityEvent FinishMovementEvent;
    #endregion

    public virtual void Initialize()
    {
        board = FindObjectOfType<Board>();
        UpdateCurrentNode();
    }

    public virtual void Move(Vector3 destinationPos, float delayTime = 0.025f)
    {
        if (board == null) return;
        var targetNode = board.FindNodeAt(destinationPos);
        if (targetNode == null) return;
        if (currentNode == null) return;
        if (!currentNode.LinkedNodes.Contains(targetNode)) return;
        StartCoroutine(MoveRoutine(destinationPos, delayTime));
    }

    protected virtual IEnumerator MoveRoutine(Vector3 destinationPos, float delayTime)
    {
        isMoving = true;
        CheckYPos(ref destinationPos);
        destination = destinationPos;

        if (faceDestination)
        {
            FaceDestination();
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(delayTime);
        iTween.MoveTo(gameObject, iTween.Hash(
            "x", destinationPos.x,
            "y", destinationPos.y,
            "z", destinationPos.z,
            "delay", iTweenDelay,
            "easetype", easeType,
            "speed", moveSpeed));

        while (Vector3.Distance(destinationPos, transform.position) > 0.01f)
            yield return null;

        iTween.Stop(gameObject);
        transform.position = destinationPos;

        UpdateCurrentNode();

        var barrier = currentNode.GetComponent<Barrier>();
        if (barrier != null)
        {
            barrier.SwapBarrier();
            yield return new WaitForSeconds(1f);
        }
        isMoving = false;
    }

    public void MoveLeft()
    {
        Vector3 newPosition = transform.position + new Vector3(-Board.spacing, 0, 0);
        Move(newPosition, 0);
    }

    public void MoveRight()
    {
        Vector3 newPosition = transform.position + new Vector3(Board.spacing, 0, 0);
        Move(newPosition, 0);
    }

    public void MoveForward()
    {
        Vector3 newPosition = transform.position + new Vector3(0, 0, Board.spacing);
        Move(newPosition, 0);
    }

    public void MoveBackward()
    {
        Vector3 newPosition = transform.position + new Vector3(0, 0, -Board.spacing);
        Move(newPosition, 0);
    }

    private void CheckYPos(ref Vector3 newPosition)
    {
        var node = board.FindNodeAt(newPosition);
        if (newPosition.y != node.transform.position.y)
        {
            var deltaDir = (newPosition.y < node.transform.position.y) ? Vector3.up : Vector3.down;
            var deltaApm = (int)Mathf.Abs(newPosition.y - node.transform.position.y);
            deltaDir *= deltaApm;
            newPosition += deltaDir;
        }
    }

    protected void UpdateCurrentNode()
    {
        if (board == null) return;
        currentNode = board.FindNodeAt(transform.position);
    }

    protected void FaceDestination()
    {
        var relativePosition = destination - transform.position;

        var newRotation = Quaternion.LookRotation(relativePosition, Vector3.up);

        var newY = newRotation.eulerAngles.y;

        iTween.RotateTo(gameObject, iTween.Hash(
            "y", newY,
            "delay", 0f,
            "easetype", easeType,
            "time", rotateTime));
    }
}
