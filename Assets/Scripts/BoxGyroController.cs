using System;
using UnityEngine;

public class BoxGyroController : MonoBehaviour
{
    [Header("Tweaks")]
    [SerializeField] private Quaternion baseRotation = new Quaternion(0, 0, 1, 0);

    private Quaternion ClampRotation(Quaternion q, Vector3 bounds)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, -bounds.x, bounds.x);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);
        angleY = Mathf.Clamp(angleY, -bounds.y, bounds.y);
        q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

        float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
        angleZ = Mathf.Clamp(angleZ, -bounds.z, bounds.z);
        q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

        return q;
    }

    // Start is called before the first frame update
    void Start()
    {

        GyroManager.Instance.EnableGyro();    
    }
    
    // Update is called once per frame
    void Update()
    {
        Quaternion rotation = GyroManager.Instance.GetGyroRotation() * baseRotation;
        Debug.Log("Rotation after * baseRotation: " + rotation);
        Vector3 bounds = new Vector3(18, 2, 12);
        transform.localRotation = Quaternion.Lerp(transform.localRotation,ClampRotation(rotation, bounds),0.5f);
    }
}
