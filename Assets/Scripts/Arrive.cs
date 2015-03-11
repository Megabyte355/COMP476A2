using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Npc))]
public class Arrive : MonoBehaviour
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
            KinematicArrive();
        }
        else
        {
            SteeringArrive();
        }
    }

    void KinematicArrive()
    {
        Vector3 direction = target.position - transform.position;

        if(direction.magnitude <= npc.DistanceThreshold)
        {
            // Step directly to target (behavior A.i)
            transform.position = target.position;
        }
        else
        {
            float minVelocity = Mathf.Min(npc.ArriveSpeed, direction.magnitude / npc.TimeToTarget);

            // Rotate towards target on the spot before moving (behavior A.ii)
            if(Vector3.Angle (transform.forward, direction) <= npc.AngleThreshold)
            {
                transform.Translate (transform.forward * minVelocity * Time.deltaTime, Space.World);
            }
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), npc.AngularSpeed * Time.deltaTime);
        }
    }

    void SteeringArrive()
    {
        Vector3 direction = target.position - transform.position;

        if(direction.magnitude < npc.DistanceThreshold)
        {
            transform.position = target.position;
        }
        else if(velocity.magnitude < npc.VelocityThreshold)
        {
            // Behavior A
            if(direction.magnitude <= npc.DistanceThreshold)
            {
                // Step directly to target (behavior A.i)
                transform.position = target.position;
            }
            else
            {
                // Rotate then translate (behavior A.ii)
                Align (direction);

                if(Vector3.Angle (transform.forward, direction) <= npc.AngleThreshold)
                {
                    Vector3 acceleration = npc.MaxAcceleration * direction.normalized;
                    velocity += acceleration * Time.deltaTime;
                    if(velocity.magnitude > npc.MaxAcceleration)
                    {
                        velocity = velocity.normalized * npc.MaxAcceleration;
                    }
                    transform.Translate (transform.forward * velocity.magnitude * Time.deltaTime, Space.World);
                }
            }
        }
        else
        {
            // Behavior B
            if(Vector3.Angle (transform.forward, direction) <= npc.ArcAngle)
            {
                // Move while turning (behavior B.i)
                Vector3 acceleration;
                if(direction.magnitude < npc.SlowDownRadius)
                {
                    float goalVelocity = npc.MaxVelocity * direction.magnitude / npc.SlowDownRadius;
                    float acc = (goalVelocity - velocity.magnitude) / npc.TimeToTarget;
                    acceleration = acc * direction.normalized;
                }
                else
                {
                    acceleration = npc.MaxAcceleration * direction.normalized;
                }

                velocity += acceleration * Time.deltaTime;
                if(velocity.magnitude > npc.MaxAcceleration)
                {
                    velocity = velocity.normalized * npc.MaxAcceleration;
                }

                transform.Translate (transform.forward * velocity.magnitude * Time.deltaTime, Space.World);
            }
            // Turn to target before moving (behavior B.ii)
            Align (direction);
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
