using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Npc))]
public class Flee : MonoBehaviour
{
    public Transform target;
    Npc npc;

    Vector3 velocity;
    float angularVelocity;

    void Start ()
    {
        npc = GetComponent<Npc>();
    }
    
    void FixedUpdate ()
    {
        if(npc.IsKinematicMode())
        {
            KinematicFlee();
        }
        else
        {
            SteeringFlee();
        }
    }
    
    void KinematicFlee()
    {
        Vector3 fleeDirection = transform.position - target.position;
        if(fleeDirection.magnitude < npc.FleeDistanceThreshold)
        {
            // Step away directly while ignoring orientation (behavior C.i)
            transform.Translate (fleeDirection.normalized * npc.FleeSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            // Stops, rotate, then continue moving away (behavior C.ii)
            if(Vector3.Angle (transform.forward, fleeDirection) <= npc.AngleThreshold)
            {
                transform.Translate (transform.forward * npc.SeekSpeed * Time.deltaTime, Space.World);
            }
        }
        // Rotation for both behaviors
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(fleeDirection), npc.AngularSpeed * Time.deltaTime);
    }

    void SteeringFlee()
    {
        Vector3 fleeDirection = transform.position - target.position;
        Align (fleeDirection);

        Vector3 acceleration = npc.MaxAcceleration * fleeDirection.normalized;
        velocity += acceleration * Time.deltaTime;
        if(velocity.magnitude > npc.MaxAcceleration)
        {
            velocity = velocity.normalized * npc.MaxAcceleration;
        }

        if(fleeDirection.magnitude < npc.FleeDistanceThreshold)
        {
            // Step away directly even if there's sidestepping (behavior C.i)
            transform.Translate (velocity * Time.deltaTime, Space.World);
        }
        else
        {
            // Face away from target and move away (behavior C.ii)
            transform.Translate (transform.forward * velocity.magnitude * Time.deltaTime, Space.World);
        }

    }

    void Align(Vector3 targetAlignment)
    {
        int sign = Vector3.Cross(transform.forward, targetAlignment).y > 0 ? 1 : -1;
        float angleDifference = Vector3.Angle (transform.forward, targetAlignment);
        
        if(angleDifference > npc.AngleThreshold)
        {
            float goalAngularVelocity = npc.MaxAngularVelocity * angleDifference / npc.SlowDownOrientation;
            float angularAcceleration = (goalAngularVelocity - angularVelocity) / npc.AngularTimeToTarget;
            
            if(angularAcceleration > npc.MaxAngularAcceleration)
            {
                angularAcceleration = npc.MaxAngularAcceleration;
            }
            
            angularVelocity += angularAcceleration * Time.deltaTime;
            if(angularVelocity > npc.MaxAngularVelocity)
            {
                angularVelocity = npc.MaxAngularVelocity;
            }
            
            transform.Rotate (transform.up, angularVelocity * Time.deltaTime * sign, Space.World);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(targetAlignment);
        }
    }
}
