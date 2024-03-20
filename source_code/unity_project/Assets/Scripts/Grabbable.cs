using System.Collections;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public bool isActive = true;

    public void Grab(GameObject grabber) {
        if (!isActive) return;
        Rigidbody rgbd = GetComponent<Rigidbody>();
        transform.parent = grabber.transform;
        rgbd.isKinematic = true;
        rgbd.useGravity = false;
        
    }

    public void Release(Vector3 pos, Vector3 vel, Vector3 angVel)
    {
        if (!isActive) return;
        Rigidbody rgbd = GetComponent<Rigidbody>();
        transform.position = pos; // set the orign to match target
        rgbd.isKinematic = false;
        rgbd.useGravity = true;
        rgbd.velocity = vel;
        rgbd.angularVelocity = angVel;
    }
}
