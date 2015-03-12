using UnityEngine;
using System.Collections;

public class Npc : MonoBehaviour
{
    Mode implementationMode;
    enum NpcState { Idle, Arrive, Wander, Flee, Tagged };
    [SerializeField]
    NpcState currentState = NpcState.Idle;
    
    // Kinematic seek and arrive
    public float SeekSpeed;
    public float ArriveSpeed;
    public float ArriveRadius;
    public float TimeToTarget;
    public float DistanceThreshold;
    
    // Kinematic flee
    public float FleeSpeed;
    public float FleeDistanceThreshold;
    
    // Steering seek and arrive
    public float MaxAcceleration;
    public float MaxVelocity;
    public float VelocityThreshold;
    public float SlowDownRadius;
    
    // Align
    public float MaxAngularAcceleration;
    public float MaxAngularVelocity;
    public float AngularTimeToTarget;
    public float SlowDownOrientation;
    
    // Turning
    public float ArcAngle;
    public float ArcDistance;
    public float AngularSpeed;
    public float AngleThreshold;

    // Behavior
    [SerializeField]
    Arrive arrive;

    void Start()
    {
        implementationMode = GameObject.FindGameObjectWithTag("Settings").GetComponent<Mode>();
        arrive = GetComponent<Arrive>();
    }
    
    public bool IsKinematicMode()
    {
        return implementationMode.IsKinematic();
    }
    
    public bool IsSteeringMode()
    {
        return implementationMode.IsSteering();
    }

    public void SetTarget(Transform target)
    {
        arrive.target = target;
    }

    public Transform GetTarget()
    {
        return arrive.target;
    }

    public void ResetTarget()
    {
        arrive.ResetTarget();
    }
}
