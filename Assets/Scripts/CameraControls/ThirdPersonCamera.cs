
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private float sphereCastRadius;
    [SerializeField] private Camera thirdPersonCamera;
    [SerializeField] private Transform minimumDistanceCameraPosition;
    [SerializeField] private Transform maximumDistanceCameraPosition;

    private void Update()
    {
        Vector3 startPosition = minimumDistanceCameraPosition.position;
        Vector3 endPosition = maximumDistanceCameraPosition.position;

        Vector3 difference = endPosition - startPosition;
        Vector3 direction = difference.normalized;

        int layerMask = (int) LayerMaskValue.Default;
        layerMask += (int)LayerMaskValue.Floor;
        layerMask += (int)LayerMaskValue.Environment;
        layerMask += (int)LayerMaskValue.Interactable;

        if (Physics.CheckSphere(startPosition, sphereCastRadius, layerMask))
        {
            thirdPersonCamera.transform.position = minimumDistanceCameraPosition.position;
            return;
        }

        if (!Physics.SphereCast(startPosition, sphereCastRadius, direction, out RaycastHit hit, difference.magnitude, layerMask))
        {
            thirdPersonCamera.transform.position = maximumDistanceCameraPosition.position;
            return;
        }

        thirdPersonCamera.transform.position = startPosition + direction * hit.distance;
    }
}