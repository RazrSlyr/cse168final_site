using System.Collections;
using UnityEngine;

public class ModelGrabber : MonoBehaviour
{
    [SerializeField] private Transform trackingspace;
    [SerializeField] private GameObject rightControllerPivot;
    [SerializeField] private GameObject leftControllerPivot;
    [SerializeField] private OVRInput.RawButton leftGrabButton;
    [SerializeField] private OVRInput.RawButton rightGrabButton;
    private bool rightHandFull = false;
    private bool leftHandFull = false;
    private GameObject leftGrabbableCube = null;
    private GameObject rightGrabbableCube = null;


    private GameObject FindGrabbableCube(GameObject pivot) {
        GameObject closestCube = null;
        Collider[] cubes = Physics.OverlapSphere(pivot.transform.position, 0.1f);
        foreach (Collider item in cubes)
        {
            if (item.GetComponent<Grabbable>() && (closestCube == null || 
                Vector3.Distance(pivot.transform.position, item.transform.position) <
                Vector3.Distance(pivot.transform.position, closestCube.transform.position))
                && item.GetComponent<Grabbable>().isActive) 
            {
                closestCube = item.gameObject;
            }
        }
        return closestCube;
    }

    private void Update()
    {
        if (!leftHandFull)
        {
            leftGrabbableCube = FindGrabbableCube(leftControllerPivot);
        }

        if (leftGrabbableCube != null && !leftHandFull && OVRInput.GetDown(leftGrabButton)) {
            leftGrabbableCube.GetComponent<Grabbable>().Grab(leftControllerPivot);
            leftHandFull = true;
        }

        if (leftHandFull && OVRInput.GetUp(leftGrabButton))
        {
            leftGrabbableCube.transform.parent = null;
            var ballPos = leftGrabbableCube.transform.position;
            var vel = trackingspace.rotation * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
            var angVel = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.LTouch);
            leftGrabbableCube.GetComponent<Grabbable>().Release(ballPos, vel, angVel);
            leftHandFull = false;
            leftGrabbableCube = null;
        }

        if (!rightHandFull)
        {
            rightGrabbableCube = FindGrabbableCube(rightControllerPivot);
        }

        if (rightGrabbableCube != null && !rightHandFull && OVRInput.GetDown(rightGrabButton)) {
            rightGrabbableCube.GetComponent<Grabbable>().Grab(rightControllerPivot);
            rightHandFull = true;
        }

        if (rightHandFull && OVRInput.GetUp(rightGrabButton))
        {
            rightGrabbableCube.transform.parent = null;
            var ballPos = rightGrabbableCube.transform.position;
            var vel = trackingspace.rotation * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            var angVel = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
            rightGrabbableCube.GetComponent<Grabbable>().Release(ballPos, vel, angVel);
            rightHandFull = false;
            rightGrabbableCube = null;
        }
    }
}
