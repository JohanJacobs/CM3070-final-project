using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.ShaderGraph;
using UnityEngine;


/*
    TODO : Fix the camera to be more like a follow camera;
 */
public class CarCamera : MonoBehaviour
{
    [SerializeField] Transform targetTransform;    
    [SerializeField] Vector3 Offset;
    GameObject anchorPoint;
    public float defaultDistance;
    private void Start()
    {
        transform.position = targetPos;
        anchorPoint = new GameObject("Camera Anchor point");
        anchorPoint.transform.position = targetPos;
        defaultDistance = Vector3.Distance(targetTransform.position, targetPos);
    }

    Vector3 targetPos => targetTransform.position + targetTransform.TransformDirection(Offset);
    Vector3 currentPos => transform.position;
    public void LateUpdate()
    {
        //TestCameraOne();

        var dirVec = (targetPos - currentPos);
        var distance = dirVec.sqrMagnitude;
        var norm = dirVec.normalized;

        transform.position = targetPos;
        transform.LookAt(targetTransform);
    }


    public float dotP;
    void TestCameraOne()
    {
        // vector from the target to the destination position
        var v = (targetPos - targetTransform.position).normalized;
        Debug.DrawRay(targetTransform.position, v * 10f, Color.green); // aim towards this line

        var m = (transform.position - targetTransform.position).normalized;
        Debug.DrawRay(targetTransform.position, m * 5f, Color.red); // currentPath

        dotP = (1-Vector3.Dot(v, m))*4f;

        var dirVec = (targetPos - transform.position); // current vector between camera and target 
        
        var dirVecNorm = dirVec.normalized;
        var newPos = transform.position + dirVecNorm * 20f * Time.deltaTime * dotP;
        
        var newNorm = (newPos - targetTransform.position).normalized;
                
        Debug.DrawRay(targetTransform.position, newNorm * defaultDistance, Color.cyan);

        // normalize the vector
        var nnPos = targetTransform.position + newNorm * defaultDistance;

        transform.position = nnPos;
        transform.LookAt(targetTransform);
    }
}
