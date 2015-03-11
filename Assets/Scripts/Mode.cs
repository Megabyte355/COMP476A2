using UnityEngine;
using System.Collections;

public class Mode : MonoBehaviour
{
    enum ImplementationMode { Kinematic, Steering };
    [SerializeField]
    ImplementationMode mode;
    
    void Start ()
    {
        mode = ImplementationMode.Kinematic;
    }
    
    void Update ()
    {
        // "Jump" is mapped to spacebar key
        if (Input.GetButtonDown ("Jump")) {
            // Change mode
            mode = (mode == ImplementationMode.Kinematic) ? ImplementationMode.Steering : ImplementationMode.Kinematic;
        }
    }
    
    public bool IsKinematic ()
    {
        return mode == ImplementationMode.Kinematic;
    }
    
    public bool IsSteering ()
    {
        return mode == ImplementationMode.Steering;
    }
}
