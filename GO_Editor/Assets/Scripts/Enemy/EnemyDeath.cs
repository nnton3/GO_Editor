using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    private Vector3 offscreenOffset = new Vector3(0f, 10f, 0f);
    private Board board;
    private float deathDelay = 0f;
    private float offscreenDelay = 1f;
    private float iTweenDelay = 0f;
    private iTween.EaseType easeType = iTween.EaseType.easeInOutQuint;
    private float moveTime = 0.5f;

    public void Initialize()
    {
        board = FindObjectOfType<Board>();
    }

    public void MoveOffBoard(Vector3 target)
    {
        iTween.MoveTo(gameObject, iTween.Hash(
            "x", target.x,
            "y", target.y,
            "z", target.z,
            "delay", iTweenDelay,
            "easetype", easeType,
            "time", moveTime));
    }

    public void Die()
    {
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(deathDelay);
        var offscreenPos = transform.position + offscreenOffset;

        MoveOffBoard(offscreenPos);

        yield return new WaitForSeconds(moveTime + offscreenDelay);

        if (board.CapturePositions.Count != 0 && 
            board.CurrentCapturePosition < board.CapturePositions.Count)
        {
            var capturePos = board.CapturePositions[board.CurrentCapturePosition].position;
            transform.position = capturePos + offscreenOffset;

            MoveOffBoard(capturePos);

            yield return new WaitForSeconds(moveTime);

            board.CurrentCapturePosition++;
            board.CurrentCapturePosition = Mathf.Clamp(
                board.CurrentCapturePosition,
                0,
                board.CapturePositions.Count - 1);
        }
    }
}
