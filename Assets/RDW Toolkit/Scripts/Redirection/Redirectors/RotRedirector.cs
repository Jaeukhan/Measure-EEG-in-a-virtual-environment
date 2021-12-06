using UnityEngine;
using System.Collections;
using Redirection;

public class RotRedirector : Redirector {

    // HODGSON_MIN(MAX)_CURVATURE_GAIN = 1 / redirectionManager.CURVATURE_RADIUS
    // MIN_ROTATION_GAIN = redirectionManager.MIN_ROT_GAIN
    // MAX_ROTATION_GAIN = redirectionManager.MAX_ROT_GAIN

    // Testing Parameters
    bool useBearingThresholdBasedRotationDampeningTimofey = true;
    bool dontUseDampening = false;

    // User Experience Improvement Parameters
    private const float MOVEMENT_THRESHOLD = 0.2f; // meters per second
    private const float CURVATURE_GAIN_CAP_DEGREES_PER_SECOND = 15;  // degrees per second
    private const float ROTATION_THRESHOLD = 1.5f; // degrees per second
    private const float ROTATION_GAIN_CAP_DEGREES_PER_SECOND = 30;  // degrees per second
    private const float BEARING_THRESHOLD_FOR_DAMPENING = 1f; // TIMOFEY: 45.0f; // Bearing threshold to apply dampening (degrees) MAHDI: WHERE DID THIS VALUE COME FROM?
    private const float DISTANCE_THRESHOLD_FOR_DAMPENING = 1.25f; // Distance threshold to apply dampening (meters)
    private const float SMOOTHING_FACTOR = 0.125f; // Smoothing factor for redirection rotations

    // Reference Parameters
    protected Transform currentTarget; //Where the participant  is currently directed?
    protected GameObject tmpTarget;

    // State Parameters
    protected bool noTmpTarget = true;

    // Auxiliary Parameters
    private float rotationFromRotationGain; //Proposed rotation gain based on head's yaw
    private float lastRotationApplied = 0f;

    //S2C Redirector
    bool dontUseTempTargetInS2C = false;

    private const float S2C_BEARING_ANGLE_THRESHOLD_IN_DEGREE = 160;
    private const float S2C_TEMP_TARGET_DISTANCE = 4;
    

    public void PickRedirectionTarget()
    {
        Vector3 trackingAreaPosition = Utilities.FlattenedPos3D(redirectionManager.trackedSpace.position);
        Vector3 userToCenter = trackingAreaPosition - redirectionManager.currPos;

        //Compute steering target for S2C
        float bearingToCenter = Vector3.Angle(userToCenter, redirectionManager.currDir);
        float directionToCenter = Utilities.GetSignedAngle(redirectionManager.currDir, userToCenter);
        if (bearingToCenter >= S2C_BEARING_ANGLE_THRESHOLD_IN_DEGREE && !dontUseTempTargetInS2C)
        {
            //Generate temporary target
            if (noTmpTarget)
            {
                tmpTarget = new GameObject("S2C Temp Target");
                tmpTarget.transform.position = redirectionManager.currPos + S2C_TEMP_TARGET_DISTANCE * (Quaternion.Euler(0, directionToCenter * 90, 0) * redirectionManager.currDir);
                tmpTarget.transform.parent = transform;
                noTmpTarget = false;
            }
            currentTarget = tmpTarget.transform;
        }
        else
        {
            currentTarget = redirectionManager.trackedSpace;
            if (!noTmpTarget)
            {
                GameObject.Destroy(tmpTarget);
                noTmpTarget = true;
            }
        }
        // Debug.Log("CurPos: " + redirectionManager.currPosReal);
    }
    //public override void ApplyRedirection()
    //{
    //    PickRedirectionTarget();
    //    Vector3 deltaPos = redirectionManager.deltaPos;
    //    float deltaDir = redirectionManager.deltaDir;

    //    Vector3 desiredFacingDirection = Utilities.FlattenedPos3D(currentTarget.position) - redirectionManager.currPos;
    //    int desiredSteeringDirection = (-1) * (int)Mathf.Sign(Utilities.GetSignedAngle(redirectionManager.currDir, desiredFacingDirection));
    //    rotationFromRotationGain = 0;

    //    if (redirectionManager.MAX_ROT_GAIN == 0.0f)
    //    {
    //        //Rotating against the user
    //        rotationFromRotationGain = Mathf.Min(Mathf.Abs(deltaDir * redirectionManager.MIN_ROT_GAIN), ROTATION_GAIN_CAP_DEGREES_PER_SECOND * redirectionManager.GetDeltaTime());
    //    }
    //    else if (redirectionManager.MIN_ROT_GAIN == 0.0f)
    //    {
    //        //Rotating with the user
    //        rotationFromRotationGain = Mathf.Min(Mathf.Abs(deltaDir * redirectionManager.MAX_ROT_GAIN), ROTATION_GAIN_CAP_DEGREES_PER_SECOND * redirectionManager.GetDeltaTime());
    //    }
    //    float rotationProposed = desiredSteeringDirection * rotationFromRotationGain;
    //    InjectRotation(rotationProposed);

    //}

    public override void ApplyRedirection()
    { 
        Vector3 deltaPos = redirectionManager.deltaPos;
        float deltaDir = redirectionManager.deltaDir;

        //Transform mapTransform = GameObject.Find("Map").transform;

        //Vector2 tempForward = new Vector2(mapTransform.forward.x, mapTransform.forward.z);
        //Vector2 newForward = Utilities.RotateVector2(tempForward, -deltaDir);
        //mapTransform.forward = new Vector3(newForward.x, 0, newForward.y);

        rotationFromRotationGain = 0;

        rotationFromRotationGain = (deltaDir * (redirectionManager.ROT_GAIN-1));

        float finalRotation = (1.0f - SMOOTHING_FACTOR) * lastRotationApplied + SMOOTHING_FACTOR * rotationFromRotationGain;
        lastRotationApplied = finalRotation;
        InjectRotation(lastRotationApplied);

    }
    // public override void ApplyRedirection()
    // {
    //     PickRedirectionTarget();

    //     // Get Required Data
    //     Vector3 deltaPos = redirectionManager.deltaPos;
    //     float deltaDir = redirectionManager.deltaDir;


    //     //Compute desired facing vector for redirection
    //     Vector3 desiredFacingDirection = Utilities.FlattenedPos3D(currentTarget.position) - redirectionManager.currPos;
    //     //Debug.Log("currentTarget.position: " + currentTarget.position);
    //     int desiredSteeringDirection = (-1) * (int)Mathf.Sign(Utilities.GetSignedAngle(redirectionManager.currDir, desiredFacingDirection)); // We have to steer to the opposite direction so when the user counters this steering, she steers in right direction

    //     //Compute proposed rotation gain
    //     rotationFromRotationGain = 0;

    //     if (Mathf.Abs(deltaDir) / redirectionManager.GetDeltaTime() >= ROTATION_THRESHOLD)  //if User is rotating
    //     {

    //         //Determine if we need to rotate with or against the user
    //         if (deltaDir * desiredSteeringDirection < 0)
    //         {
    //             //Rotating against the user
    //             rotationFromRotationGain = Mathf.Min(Mathf.Abs(deltaDir * redirectionManager.MIN_ROT_GAIN), ROTATION_GAIN_CAP_DEGREES_PER_SECOND * redirectionManager.GetDeltaTime());
    //         }
    //         else
    //         {
    //             //Rotating with the user
    //             rotationFromRotationGain = Mathf.Min(Mathf.Abs(deltaDir * redirectionManager.MAX_ROT_GAIN), ROTATION_GAIN_CAP_DEGREES_PER_SECOND * redirectionManager.GetDeltaTime());
    //         }
    //     }

    //     float rotationProposed = desiredSteeringDirection * rotationFromRotationGain;


    //     // Prevent having gains if user is stationary
    //     if (Mathf.Approximately(rotationProposed, 0))
    //         return;

    //     if (!dontUseDampening)
    //     {
    //         //DAMPENING METHODS
    //         // MAHDI: Sinusiodally scaling the rotation when the bearing is near zero
    //         float bearingToTarget = Vector3.Angle(redirectionManager.currDir, desiredFacingDirection);
    //         if (useBearingThresholdBasedRotationDampeningTimofey)
    //         {
    //             // TIMOFEY
    //             if (bearingToTarget <= BEARING_THRESHOLD_FOR_DAMPENING)
    //                 rotationProposed *= Mathf.Sin(Mathf.Deg2Rad * 90 * bearingToTarget / BEARING_THRESHOLD_FOR_DAMPENING);
    //         }
    //         else
    //         {
    //             // MAHDI
    //             // The algorithm first is explained to be similar to above but at the end it is explained like this. Also the BEARING_THRESHOLD_FOR_DAMPENING value was never mentioned which make me want to use the following even more.
    //             rotationProposed *= Mathf.Sin(Mathf.Deg2Rad * bearingToTarget);
    //         }


    //         // MAHDI: Linearly scaling the rotation when the distance is near zero
    //         if (desiredFacingDirection.magnitude <= DISTANCE_THRESHOLD_FOR_DAMPENING)
    //         {
    //             rotationProposed *= desiredFacingDirection.magnitude / DISTANCE_THRESHOLD_FOR_DAMPENING;
    //         }

    //     }

    //     // Implement additional rotation with smoothing
    //     float finalRotation = (1.0f - SMOOTHING_FACTOR) * lastRotationApplied + SMOOTHING_FACTOR * rotationProposed;
    //     lastRotationApplied = finalRotation;
    //     InjectRotation(finalRotation);

    // }
}
