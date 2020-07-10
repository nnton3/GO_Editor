using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    #region Variables
    protected Vector3 directionToSearch = new Vector3(0f, 0f, Board.spacing);

    private Board_Node nodeToSearch;
    protected Board board;

    protected bool foundPlayer = false;
    public bool FoundPlayer => foundPlayer;
    #endregion

    public virtual void Initialize()
    {
        board = FindObjectOfType<Board>();
    }

    public virtual void UpdateSensor()
    {
        var worldSpacePositionToSearch = 
            transform.TransformVector(directionToSearch) + transform.position;

        if (board == null) return;

        nodeToSearch = board.FindNodeAt(worldSpacePositionToSearch);
        
        if (nodeToSearch == board.PlayerNode)
            if (board.PlayerNode.Type != NodeType.Bush)
                foundPlayer = true;
    }
}
