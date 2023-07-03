using UnityEngine;

public class Paint : MonoBehaviour
{
    [SerializeField] private Shader drawShader;

    private RenderTexture splatMap;
    private Material currentMaterial, drawMaterial;
    private RaycastHit hit;

    [SerializeField] [Range(1, 500)] private float size;
    [SerializeField] [Range(0, 1)] private float strength;

    private void Start() 
    {
        drawMaterial = new UnityEngine.Material(drawShader);  

        drawMaterial.SetVector("_Color", Color.red);

        currentMaterial = GetComponent<MeshRenderer>().material;

        splatMap = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        currentMaterial.SetTexture("SplatMap", splatMap);  
    }

    private void Update() 
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                drawMaterial.SetVector("_Coordinates", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));

                drawMaterial.SetFloat("_Strength", strength);

                drawMaterial.SetFloat("_Size", size);

                RenderTexture temp = RenderTexture.GetTemporary(splatMap.width, splatMap.height, 0, RenderTextureFormat.ARGBFloat);

                Graphics.Blit(splatMap, temp);

                Graphics.Blit(temp, splatMap, drawMaterial);

                RenderTexture.ReleaseTemporary(temp);
            }
        }    
    }

    public void PaintHitPosition(RaycastHit hit)
    {
        drawMaterial.SetVector("_Coordinates", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));

        drawMaterial.SetFloat("_Strength", strength);

        drawMaterial.SetFloat("_Size", size);

        RenderTexture temp = RenderTexture.GetTemporary(splatMap.width, splatMap.height, 0, RenderTextureFormat.ARGBFloat);

        Graphics.Blit(splatMap, temp);

        Graphics.Blit(temp, splatMap, drawMaterial);

        RenderTexture.ReleaseTemporary(temp);
    }
}
