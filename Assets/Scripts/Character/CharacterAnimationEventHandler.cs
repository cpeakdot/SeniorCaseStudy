using UnityEngine;

namespace UdoCase.Character
{
    public class CharacterAnimationEventHandler : MonoBehaviour
    {
        private CharacterController characterController;
        private CamShake camShake;

        private void Awake() 
        {
            characterController = GetComponentInParent<CharacterController>();
            camShake = CamShake.Instance;    
        }

        public void Mine()
        {
            characterController.MineAction();
            camShake.ShakeCamera();
        }

        public void Attack()
        {
            characterController.Attack();
            camShake.ShakeCamera();
        }
    }
}

