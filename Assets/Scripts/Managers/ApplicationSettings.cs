using UnityEngine;

public class ApplicationSettings : MonoBehaviour
{
    private void Awake() 
    {
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = false;
    }
}
