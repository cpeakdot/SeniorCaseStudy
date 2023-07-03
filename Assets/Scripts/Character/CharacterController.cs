using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace UdoCase.Character
{
    public class CharacterController : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private FloatingJoystick floatingJoystick;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform bagTransform;
        [SerializeField] private GameObject broom; 
        [SerializeField] private GameObject axe;
        
        [Header("Values")]
        [SerializeField] private float movementSpeed = 1f;
        [SerializeField] private float rotationDuration = .2f;
        [SerializeField] private float overlapRadius = 1f;
        [SerializeField] private Vector3 overlapOffset;
        [SerializeField] private LayerMask collectableLayerMask;
        [SerializeField] private LayerMask interactableLayerMask;
        private List<Collectable> collectableList = new();
        private Collider[] overlapCollectableResults = new Collider[10];
        private Collider[] overlapInteractableResults;
        public bool debugOn = false;

        private PlayerState playerState;
        private RaycastHit hit;
        private bool hasAxe = false;

        private void Update() 
        {
            Vector3 movementDir = GetMovementDirection();

            switch (playerState)
            {
                case PlayerState.Idle_Walk:

                    CheckOverlapCollectable();
            
                    CheckOverlapInteractable();

                    break;

                case PlayerState.Attacking:
                    break;

                case PlayerState.Mining:

                    CheckOverlapCollectable();

                    break;

                case PlayerState.Sweeping:

                    if(Physics.Raycast(transform.position, Vector3.down, out hit, 10f, LayerMask.GetMask("Liquid")))
                    {
                        if(hit.transform.TryGetComponent(out Paint paint))
                        {
                            paint.PaintHitPosition(hit);
                        }
                    }

                    break;

                default:
                    break;
            }

            Movement(movementDir);

            Rotation(movementDir);

            Animation(movementDir);
        }

        private void SwitchState(PlayerState newState)
        {
            axe.SetActive(false);
            broom.SetActive(false);
            switch (newState)
            {
                case PlayerState.Idle_Walk:

                    SetLayerWeight(0);

                    animator.SetBool("IsMining", false);
                    animator.SetBool("IsSweeping", false);
                    animator.SetBool("IsAttacking", false);
                    break;

                case PlayerState.Attacking:

                    SetLayerWeight(1);
                    axe.SetActive(true);

                    animator.SetLayerWeight(1, 1);
                    animator.SetBool("IsMining", false);
                    animator.SetBool("IsSweeping", false);
                    animator.SetBool("IsAttacking", true);
                    break;

                case PlayerState.Mining:

                    SetLayerWeight(1);

                    animator.SetLayerWeight(1, 1);
                    animator.SetBool("IsAttacking", false);
                    animator.SetBool("IsSweeping", false);
                    animator.SetBool("IsMining", true);
                    break;

                case PlayerState.Sweeping:

                    SetLayerWeight(1);
                    broom.SetActive(true);

                    animator.SetLayerWeight(1, 1);
                    animator.SetBool("IsAttacking", false);
                    animator.SetBool("IsMining", false);
                    animator.SetBool("IsSweeping", true);
                    break;

                default:
                    break;
            }

            playerState = newState;
        }

        private void Movement(Vector3 movementDir)
        {
            transform.position += movementDir * movementSpeed * Time.deltaTime;
        }

        private void Rotation(Vector3 movementDir)
        {
            if(movementDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movementDir);

                transform.DORotateQuaternion(targetRotation, rotationDuration);
            }
        }

        private void Animation(Vector3 movementDir)
        {
            animator.SetBool("IsRunning", (movementDir == Vector3.zero) ? false : true);
        }

        private void SetLayerWeight(float targetLayerWeight)
        {
            float currentLayerWeight = animator.GetLayerWeight(1);
            float layerWeightSwapDuration = .3f;

            DOTween.Kill(999);

            DOTween.To(()=> currentLayerWeight, x => currentLayerWeight = x, targetLayerWeight, layerWeightSwapDuration)
            .OnUpdate(()=>{
                animator.SetLayerWeight(1, currentLayerWeight);
            })
            .SetId(999);
        }

        private Vector3 GetMovementDirection()
        {
            var direction = (Vector3.forward * floatingJoystick.Vertical) + (Vector3.right * floatingJoystick.Horizontal);

            return direction;
        }

        public void MineAction()
        {
            Vector3 offSet = Vector3.up * UnityEngine.Random.Range(0f, 1f);

            Collider[] mines = new Collider[1];

            Physics.OverlapSphereNonAlloc(transform.position + overlapOffset, overlapRadius, mines, LayerMask.GetMask("Ore"));
                
            if(mines[0] != null && mines[0].TryGetComponent(out Ore ore))
            {
                ore.Hit(this.transform.position + offSet);

                if(!ore.CanBeMined)
                {
                    SwitchState(PlayerState.Idle_Walk);
                }
            }
            else
            {
                SwitchState(PlayerState.Idle_Walk);
            }
        }

        public void Attack()
        {
            Vector3 offSet = Vector3.up * UnityEngine.Random.Range(0f, 1f);

            Collider[] dummies = new Collider[1];

            Physics.OverlapSphereNonAlloc(transform.position + overlapOffset, overlapRadius, dummies, LayerMask.GetMask("Dummy"));
                
            if(dummies[0] != null && dummies[0].TryGetComponent(out Dummy dummy))
            {
                dummy.Hit();
            }
            else
            {
                SwitchState(PlayerState.Idle_Walk);
            }
        }

        private void CheckOverlapCollectable()
        {
            Physics.OverlapSphereNonAlloc(transform.position + overlapOffset, overlapRadius, overlapCollectableResults, collectableLayerMask, QueryTriggerInteraction.Collide);

            for (int i = 0; i < overlapCollectableResults.Length; i++)
            {
                if(overlapCollectableResults[i] != null && overlapCollectableResults[i].TryGetComponent(out Collectable collectable)
                && collectable.TryCollect())
                {
                    if ((collectableList.Count > 0))
                    {
                        collectable.transform.SetParent(bagTransform);
                        collectable.transform.localPosition = collectableList[^1].transform.localPosition + (.2f * Vector3.up);
                    }
                    else
                    {
                        collectable.transform.SetParent(bagTransform);
                        collectable.transform.localPosition = Vector3.zero;
                    }
                    collectable.transform.localEulerAngles = Vector3.zero;
                    collectableList.Add(collectable);
                    break;
                }
            }
        }

        private void CheckOverlapInteractable()
        {
            overlapInteractableResults = Physics.OverlapSphere(transform.position + overlapOffset, overlapRadius, interactableLayerMask);

            Debug.Log(overlapInteractableResults.Length);

            if(overlapInteractableResults.Length > 0 && overlapInteractableResults[0] != null 
            && overlapInteractableResults[0].TryGetComponent(out Interactable interactable))
            {
                interactable.Interact(out UdoCase.Material material, out Transform inputTransform);
                GiveMaterial(interactable, material, inputTransform);
            }
        }

        private void GiveMaterial(Interactable interactable, UdoCase.Material material, Transform inputTransform)
        {
            for (int i = collectableList.Count - 1; i >= 0; i--)
            {
                if(collectableList[i].material == material)
                {
                    float moveDuration = .3f;

                    Collectable collectable = collectableList[i];
                    
                    collectable.transform.SetParent(null);

                    collectable.transform.DOMove(inputTransform.position, moveDuration)
                    .OnComplete(()=>{
                        collectable.transform.localEulerAngles = Vector3.zero;
                    });

                    interactable.TakeMaterial(collectableList[i]);
                    
                    collectableList.Remove(collectable);

                    break;
                }
            }
        }

        private void OnDrawGizmos() 
        {
            if(debugOn)
            {
                Gizmos.color = Color.red;

                Gizmos.DrawWireSphere(transform.position + overlapOffset, overlapRadius);
            }
        }

        private void OnTriggerEnter(Collider other) 
        {
            if(other.CompareTag("Ore") && other.TryGetComponent(out Ore ore) && ore.CanBeMined && playerState == PlayerState.Idle_Walk)
            {
                SwitchState(PlayerState.Mining);
            }    
            else if(other.CompareTag("Blood") && playerState == PlayerState.Idle_Walk)
            {
                SwitchState(PlayerState.Sweeping);
            }
            else if(other.CompareTag("Dummy") && hasAxe && playerState == PlayerState.Idle_Walk)
            {
                SwitchState(PlayerState.Attacking);
            }
            else if(other.CompareTag("Axe"))
            {
                hasAxe = true;
                Destroy(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other) 
        {
            if(other.CompareTag("Ore") || other.CompareTag("Blood") || other.CompareTag("Dummy"))
            {
                SwitchState(PlayerState.Idle_Walk);
                broom.SetActive(false);
                broom.SetActive(false);
            }
        }
    }
}
