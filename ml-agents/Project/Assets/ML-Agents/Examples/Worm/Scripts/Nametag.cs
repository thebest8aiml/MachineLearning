using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nametag : MonoBehaviour
{
    public Transform nametag;
    public Camera lookAtCamera;
    public bool revertDirection = true;
    public bool copyCameraRotationMode = false;
    public Transform follow;

    // Start is called before the first frame update
    void Start()
    {
        if (lookAtCamera == null)
        {
            lookAtCamera = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (copyCameraRotationMode)
        {
            nametag.rotation = lookAtCamera.transform.rotation;
        }
        else
        {
            nametag.LookAt(lookAtCamera.transform);
        }

        if (revertDirection)
        {
            nametag.Rotate(0, 180, 0);
        }

        nametag.position = new Vector3(follow.position.x, nametag.position.y, follow.position.z);
    }
}
