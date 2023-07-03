using System.Collections.Generic;
using UnityEngine;

public class BloodParticleCollisionHandler : MonoBehaviour
{
    [SerializeField] private GameObject blood;
    private new ParticleSystem particleSystem;

    private void Awake() {
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other) {
        if(other.TryGetComponent(out Collider collider))
        {
            List<ParticleCollisionEvent> collisionEvents = new (particleSystem.GetSafeCollisionEventSize());
            int numOfCollisions = particleSystem.GetCollisionEvents(other, collisionEvents);

            for (int i = 0; i < numOfCollisions; i++)
            {
                Vector3 intersection = collisionEvents[i].intersection;
                intersection.y = 0f;
                Instantiate(blood, intersection, Quaternion.Euler(-90f, 0f, 0f));
            }
        }
    }
}
