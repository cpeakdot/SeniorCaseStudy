using UnityEngine;
using System.Collections.Generic;
using cpeak.cPool;

public class Interactable : MonoBehaviour
{
    private const string ROCK_TAG = "rockCollectable";
    private const string INGOT_TAG = "ingotCollectable";

    [SerializeField] private UdoCase.Material requiredMaterial;
    [SerializeField] private GameObject outputObj;
    [SerializeField] private int requiredAmountToCraft = 3;
    [SerializeField] private Transform inputTransform;
    [SerializeField] private Transform outputTransform;
    private int recievedMaterialCount = 0;
    private List<Collectable> recievedMaterials = new ();
    private List<GameObject> outputObjs = new ();
    public bool Interact(out UdoCase.Material inputMaterial, out Transform inputTranform)
    {
        Debug.Log($"<color=red> Interacted.</color>");

        inputMaterial = requiredMaterial;

        inputTranform = this.inputTransform;

        return recievedMaterialCount < requiredAmountToCraft;
    }

    public void TakeMaterial(Collectable material)
    {
        recievedMaterialCount++;

        recievedMaterials.Add(material);

        if(recievedMaterialCount >= requiredAmountToCraft)
        {
            for (int i = 0; i < recievedMaterials.Count; i++)
            {
                if(recievedMaterials[i].material == UdoCase.Material.Rock)
                {
                    cPool.instance.ReleaseObject(ROCK_TAG, recievedMaterials[i].gameObject);
                }
                else
                {
                    cPool.instance.ReleaseObject(INGOT_TAG, recievedMaterials[i].gameObject);
                }
            }
            recievedMaterials.Clear();
            recievedMaterialCount = 0;
            
            float objOffset = 1f;

            Vector3 instantiatePosition = (outputObjs.Count > 0 ? outputObjs[^1].transform.position + (Vector3.up * objOffset) : outputTransform.position);

            if(material.material == UdoCase.Material.Rock)
            {
                cPool.instance.GetPoolObject(INGOT_TAG, instantiatePosition, Quaternion.identity);
            }
            else if(material.material == UdoCase.Material.Ingot)
            {
                Instantiate(outputObj, outputTransform.position, Quaternion.identity);
            }
        }
    }
}
