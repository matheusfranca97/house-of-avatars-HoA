
using UnityEngine;

public class LightLOD : MonoBehaviour
{
    private Light lightScript;
    [SerializeField] private float enableDistance;

    private void Awake()
    {
        lightScript = GetComponent<Light>();
    }

    private void Update()
    {
        if (Camera.main == null)
            return;

        Vector3 difference = Camera.main.transform.position - transform.position;
        lightScript.enabled = difference.magnitude < enableDistance;
    }
}