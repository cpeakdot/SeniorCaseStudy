using UnityEngine;

namespace UdoCase
{
    public class Dummy : MonoBehaviour
    {
        [SerializeField] private GameObject bloodParticle;
        
        public void Hit()
        {
            Debug.Log("Hit dummy");
            Instantiate(bloodParticle, transform.position, Quaternion.identity);
        }
    }
}

