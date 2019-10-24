using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public bool camMoving;
    public Camera cam;
    public float camTransTime;
    private Vector3 lastPos;

    private void Awake()
    {
        cam = Camera.main;
        lastPos = cam.transform.position;
    }

    public Vector3 camPosStatic(Vector3 posTo)
    {
        return new Vector3(posTo.x + 1.6f, posTo.y + 4.33f, posTo.z - 1.65f);
    }

    public IEnumerator LerpCam(Vector3 lastPos, Vector3 targetPos)
    {
        camMoving = true;
        for (float t = 0f; t < camTransTime; t += Time.deltaTime)
        {
            cam.transform.position = Vector3.Slerp(lastPos, targetPos, t / camTransTime);
            yield return 0;
        }
        transform.position = targetPos;
        camMoving = false;
    }

}
