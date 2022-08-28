using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReduceRenderDistance : MonoBehaviour
{
    
    [SerializeField] private CinemachineVirtualCamera cam;
    
    // Start is called before the first frame update
    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "1 10")
        {
            cam.m_Lens.FarClipPlane = 475f;
        }
        else
        {
            cam.m_Lens.FarClipPlane = 750f;
        }
    }
}
