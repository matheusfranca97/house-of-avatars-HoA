using UnityEngine;

public class CustomPostProcessing : MonoBehaviour
{

    [SerializeField]
    private Shader _shader; // Reference to your custom shader
    private Material material;

    private void Start()
    {
        material = new Material(_shader);
    }

    public void SetShader(Shader shader)
    {
        _shader = shader;
    }

    void Update()
    {
        ValidateCamera();
    }

    void ValidateCamera()
    {
        if (PlayerSettingsManager.instance == null) return;
        Camera thisCamera = GetComponent<Camera>();
        if (thisCamera == null)
        {
            GameObject camera = GameObject.FindAnyObjectByType<Camera>().gameObject;
            CustomPostProcessing CPP = camera.AddComponent<CustomPostProcessing>();
            CPP.SetShader(_shader);
            Debug.Log($"Translated to {camera.name}");
            gameObject.SetActive(false);
            return;
        } else 
        {
            thisCamera.depthTextureMode = DepthTextureMode.Depth;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material == null) return;
        Graphics.Blit(source, destination, material);
    }
}
