using UnityEngine;
using System.Collections;

public class MovimientoCamion : MonoBehaviour
{
    public float duration = 8.0f;

    private Coroutine activeMoveRoutine;
    public Valores ScriptValores;
    public void MoveToFirstPosition()
    {
        TriggerMovement(new Vector3(11f, 1f, transform.position.z));
    }

    public void MoveToSecondPosition()
    {
        TriggerMovement(new Vector3(11f, -15f, transform.position.z));
        ScriptValores.EstadoDelivery=0;
    }

    private void TriggerMovement(Vector3 targetPos)
    {
        if (activeMoveRoutine != null)
        {
            StopCoroutine(activeMoveRoutine);
        }
        
        activeMoveRoutine = StartCoroutine(SmoothMoveRoutine(targetPos, duration));
    }

    private IEnumerator SmoothMoveRoutine(Vector3 target, float time)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            float linearPercentage = elapsedTime / time;
            
            float smoothPercentage = Mathf.SmoothStep(0f, 1f, linearPercentage);

            transform.position = Vector3.Lerp(startPosition, target, smoothPercentage);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
        activeMoveRoutine = null;

    }
}