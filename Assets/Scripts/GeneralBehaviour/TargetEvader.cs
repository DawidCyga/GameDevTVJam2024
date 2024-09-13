using UnityEngine;

public class TargetEvader : MonoBehaviour
{
    public void MoveAwayHorizontally(Vector3 targetPosition, float moveDelta)
    {
        float targetHorizontalDirection = targetPosition.x - transform.position.x;
        Vector3 escapeDirection = new Vector3(-Mathf.Sign(targetHorizontalDirection), 0, 0);
        transform.position += escapeDirection * moveDelta * Time.deltaTime;
    }
}
