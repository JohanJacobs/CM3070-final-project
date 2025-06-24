using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCamera : MonoBehaviour
{
    [SerializeField] Transform TargetTransform;    
    [SerializeField] Vector3 Offset;
    
    Vector3 previousPosition;
    float targetDistance;
    private void Start()
    {
        targetDistance = Vector3.Magnitude((TargetTransform.position + TargetTransform.TransformDirection(Offset))  - TargetTransform.position);
        previousDistance = targetDistance;
        transform.position = targetPos;
    }

    Vector3 targetPos => TargetTransform.position + TargetTransform.TransformDirection(Offset);
    float previousDistance;
    public void LateUpdate()
    {
        var deltaPos = targetPos - transform.position;
        //current distance 
        var currentDistance  = (deltaPos).magnitude;

        // how fast did we move 
        var displacement = currentDistance - previousDistance;
        var speed = (displacement / Time.deltaTime)*0.9f; // move 90% of the distance there 


        // movement vector 
        transform.position = transform.position + deltaPos.normalized*speed*Time.deltaTime;
        transform.LookAt(TargetTransform);
    }
}
