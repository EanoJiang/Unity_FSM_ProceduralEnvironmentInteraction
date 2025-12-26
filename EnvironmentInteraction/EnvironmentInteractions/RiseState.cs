using UnityEngine;

public class RiseState : EnvironmentInteractionState
{
    // float _lerpDuration = 5.0f;
    float _elapsedTime = 0.0f;
    // float _rotationSpeed = 1000f;
    // float _riseWeight = 1.0f;
    // float _touchDistanceThreshold = .05f;
    // float _touchTimeThreshold = 1.0f;
    protected LayerMask _interactableLayerMask = LayerMask.GetMask("TriggerArea");

    Quaternion _expectedHandRotation;

    public RiseState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EEnvironmentInteractionState estate) : base(context, estate) { }

    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {
        var _touchDistanceThreshold = Context.ScriptableObject.riseValue.touchDistanceThreshold;
        var _touchTimeThreshold = Context.ScriptableObject.riseValue.touchTimeThreshold;

        if (CheckShouldReset())
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Reset;
        }

        if (Vector3.Distance(Context.CurrentIkTargetTransform.position, Context.ClosestPointOnColliderFromShoulder) < _touchDistanceThreshold && _elapsedTime > _touchTimeThreshold)
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Touch;
        }

        return StateKey;
    }

    public override void EnterState()
    {
        _elapsedTime = 0.0f;
        Debug.Log($"进入State: {StateKey} ");
    }

    public override void ExitState()
    {
        Debug.Log($"退出State: {StateKey} ");
        Context.CurrentIkTargetTransform.rotation = _expectedHandRotation;
    }

    public override void OnTriggerEnter(Collider other)
    {
        StartIkTargetPositionTracking(other);
    }

    public override void OnTriggerStay(Collider other)
    {
        UpdateIKTargetPosition(other);
    }

    public override void OnTriggerExit(Collider other)
    {
        ResetIkTargetPositionTracking(other);
    }

    /// <summary>
    /// RiseState's Update method is responsible for raising the weight of the
    /// currently assigned (right or left) 2BoneIKConstraint and MultiRotation component,
    /// incrementing time, and rotating the current 2BoneIKConstraint target over time
    /// </summary>
    public override void UpdateState()
    {
        var _lerpDuration = Context.ScriptableObject.riseValue.lerpDuration;
        var _rotationSpeed = Context.ScriptableObject.riseValue.rotationSpeed;
        var _riseWeight = Context.ScriptableObject.riseValue.riseWeight;
        // TODO: check if there is value to using character speed to modify animation speed
        // float speedAdjustedDuration = _lerpDuration / Mathf.Max(1.0f, Mathf.Min(4.0f, Context.Rb.velocity.magnitude));

        CalculateExpectedHandRotation();
        Context.InteractionPointYOffset = Mathf.Lerp(Context.InteractionPointYOffset, Context.ClosestPointOnColliderFromShoulder.y, _elapsedTime / _lerpDuration);

        Context.CurrentArmIK.solver.IKRotationWeight = Mathf.Lerp(Context.CurrentArmIK.solver.IKRotationWeight, _riseWeight, _elapsedTime / _lerpDuration);

        Context.CurrentArmIK.solver.IKRotationWeight = Mathf.Lerp(Context.CurrentArmIK.solver.IKRotationWeight, _riseWeight, _elapsedTime / _lerpDuration);

        if (Context.CurrentIkTargetTransform.rotation != _expectedHandRotation)
        {
            Context.CurrentIkTargetTransform.rotation = Quaternion.RotateTowards(Context.CurrentIkTargetTransform.rotation, _expectedHandRotation, _rotationSpeed * Time.deltaTime);
        }

        _elapsedTime += Time.deltaTime;
    }

    /// <summary>
    /// CalculateExpecetedHandRotation can be performed one time in EnterState or
    /// in UpdateState. Most accurate results will be Update state, but results in more raycasts.
    /// Assumes that the ZForward of the target matches the palm of the hand.
    /// </summary>
    private void CalculateExpectedHandRotation()
    {
        Vector3 startPos = Context.CurrentShoulderTransform.position;
        Vector3 endPos = Context.ClosestPointOnColliderFromShoulder;
        Vector3 direction = (endPos - startPos).normalized;
        float maxDistance = .5f;

        RaycastHit hit;

        if (Physics.Raycast(startPos, direction, out hit, maxDistance, _interactableLayerMask))
        {
            Vector3 surfaceNormal = hit.normal;

            Debug.Log("SNORMAL" + surfaceNormal);
            Vector3 targetForward = - surfaceNormal ;
            _expectedHandRotation = Quaternion.LookRotation(targetForward, Vector3.up);
        }
    }
}
