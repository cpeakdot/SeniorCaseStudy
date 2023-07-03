using UnityEngine;
using Cinemachine;

public class CamShake : MonoBehaviour
{
    public static CamShake Instance { get; private set;}
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;
    private bool shakingON = false;
    private float shakeDur = .2f;
    private float shakeTimer = 0f;

    private void Awake() 
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update() 
    {
        if(shakingON)
        {
            shakeTimer += Time.deltaTime;
            if(shakeTimer >= shakeDur)
            {
                shakeTimer = 0f;
                shakingON = false;
                noise.m_AmplitudeGain = 0f;
            }
        }    
    }

    public void ShakeCamera()
    {
        shakingON = true;
        shakeTimer = 0f;
        noise.m_AmplitudeGain = 1f;
    }
}
