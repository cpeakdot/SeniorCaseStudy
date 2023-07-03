using UnityEngine;

public class Collectable : MonoBehaviour
{
    private bool canBeCollected = true;
    public UdoCase.Material material;
    private Rigidbody rb;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody>();    
    }

    private void OnEnable() 
    {
        canBeCollected = true;
        rb.isKinematic = false;
    }

    public bool TryCollect()
    {
        if(canBeCollected)
        {
            rb.isKinematic = true;
            canBeCollected = false;
            return true;
        }
        return false;
    }
}
