using UnityEngine;

public class ComputeShaderHandler : MonoBehaviour
{
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private RenderTexture texture;

    void Start()
    {
        InitializeTexture();
        PassTextureToShader();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (texture == null)
        {
            InitializeTexture();
        }
        PassTextureToShader();
        Graphics.Blit(texture, destination);
    }

    private void InitializeTexture()
    {
        texture = new RenderTexture(512, 512, 24);
        texture.enableRandomWrite = true;
        texture.Create();
    }

    private void PassTextureToShader()
    {
        computeShader.SetTexture(0, "Result", texture);
        computeShader.SetFloat("Resolution", texture.width);
        computeShader.Dispatch(0, texture.width / 8, texture.height / 8, 1);
    }

    private void Update()
    {
        
    }
}
