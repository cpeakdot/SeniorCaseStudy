using UnityEngine;
using cpeak.cPool;

public class Ore : MonoBehaviour
{
    private bool test;
    private const int LIVES = 3;
    private int remainingLives;
    [SerializeField] private Collectable outputMaterial;
    [SerializeField] private GameObject hitParticle;
    [SerializeField] private float growthDuration = 5f;
    private float growthTimer = 0f;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    [SerializeField] private float radius = 2f;
    [SerializeField] private float deformationStr = 2f;
    [SerializeField] private float smoothing = 2f;
    private Mesh mesh;
    private Vector3[] verticies, modifiedVerts;
    public bool CanBeMined => remainingLives > 0;

    private void Awake() 
    {
        remainingLives = LIVES;   

        meshCollider = GetComponent<MeshCollider>();

        meshFilter = GetComponent<MeshFilter>();

        mesh = meshFilter.mesh;

        verticies = mesh.vertices;

        modifiedVerts = verticies.Clone() as Vector3[];
    }

    private void Update() 
    {
        if(remainingLives <= 0)
        {
            growthTimer += Time.deltaTime;
            if(growthTimer >= growthDuration)
            {
                remainingLives = 3;
                ResetMesh();
                growthTimer = 0f;
            }
        }    
    }

    private void RecalculateMesh()
    {
        
        meshFilter.sharedMesh = mesh;

        mesh.RecalculateNormals();

        meshCollider.sharedMesh = null;

        meshCollider.sharedMesh = mesh;

    }

    [ContextMenu("Test")]
    private void Test()
    {
        test = !test;
        
    }

    private void Deform(Vector3 playerPosition)
    {
        RaycastHit hit;

        Physics.Raycast(playerPosition,(transform.position - playerPosition).normalized,out hit, 100f, LayerMask.GetMask("Ore"));

        cPool.instance.GetPoolObject("mineParticle", hit.point, Quaternion.identity);

        cPool.instance.GetPoolObject("rockCollectable", hit.point, Quaternion.identity);

        for (int v = 0; v < modifiedVerts.Length; v++)
            {
                Vector3 distance = modifiedVerts[v] - hit.point;

                Vector3 force = meshFilter.transform.InverseTransformDirection(Vector3.one * deformationStr) * (1 / smoothing);

                float forceMagnitude = force.magnitude;

                if(distance.sqrMagnitude < radius)
                {
                        var p = modifiedVerts[v];
                        float d = (p - hit.point).sqrMagnitude;

                        modifiedVerts[v] = force * (1 - (d / radius));
                }
            }

        mesh.vertices = modifiedVerts;

        RecalculateMesh();
    }

    [ContextMenu("Reset Mesh")]
    private void ResetMesh()
    {
        mesh.vertices = verticies;
        modifiedVerts = verticies.Clone() as Vector3[];
        meshFilter.sharedMesh = null;
        meshFilter.sharedMesh = mesh;
        RecalculateMesh();
    }

    public void Hit(Vector3 playerPosition)
    {
        if(remainingLives > 0)
        {
            Debug.Log($"<color=green>Output Material</color>");
            Deform(playerPosition);
            remainingLives--;
        }
    }

    private void OnDrawGizmos() {
        if(test)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireMesh(meshCollider.sharedMesh, transform.position, Quaternion.identity);
        }
    }

}
