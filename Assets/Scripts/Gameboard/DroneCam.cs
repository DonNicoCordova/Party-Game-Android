using UnityEngine;
using Cinemachine;

public class DroneCam : MonoBehaviour
{
    public CinemachineVirtualCamera highlightCamera;
    private int oldCullingMask;
    private static DroneCam instance;
    public static DroneCam Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DroneCam>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(DroneCam).Name;
                    instance = obj.AddComponent<DroneCam>();
                }
            }
            return instance;
        }
    }
    private void Start()
    {
        oldCullingMask = Camera.main.cullingMask;
    }
    public void ActivateMinimapLayerOnMainCam()
    {
        Camera.main.cullingMask = ( oldCullingMask | 1 << LayerMask.NameToLayer("Minimap"));
    }

    public void DeactivateMinimapLayerOnMainCam()
    {
        Camera.main.cullingMask = oldCullingMask;
    }
    private void SetHighlightCamera()
    {
        highlightCamera.Priority = 12;
    }
    private void RemoveHighlightCamera()
    {
        highlightCamera.Priority = 9;
    }
}
