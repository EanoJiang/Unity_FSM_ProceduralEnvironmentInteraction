using UnityEngine;

public class ApproachState : EnvironmentInteractionState
{
    // float _lerpDuration = 5.0f;
    float _elapsedTime = 0.0f;
    // float _approachDuration = 2.0f;
    // float _rotationSpeed = 1000f;
    // float _approachWeight = 0.5f;
    // float _approachRotationWeight = .75f;
    // float _riseDistanceThreshold = .5f;

    public ApproachState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EEnvironmentInteractionState estate) : base(context, estate) { }

    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {
        var _approachDuration = Context.ScriptableObject.approachValue.approachDuration;
        bool isOverStateLifeDuration = _elapsedTime > _approachDuration;
        if (CheckShouldReset() || isOverStateLifeDuration)
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Reset;
        }

        // var _riseDistanceThreshold = Context.ScriptableObject.approachValue.riseDistanceThreshold;
        // bool isClosestPointOnColliderReal = Context.ClosestPointOnColliderFromShoulder != Vector3.positiveInfinity;
        // bool isWithinsReach = Vector3.Distance(Context.ClosestPointOnColliderFromShoulder, Context.CurrentShoulderTransform.position) < _riseDistanceThreshold;
        // if (isClosestPointOnColliderReal && isWithinsReach)
        // {
        //     return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Rise;
        // }

        return StateKey;
    }

    public override void EnterState()
    {
        Debug.Log($"进入State: {StateKey} ");
        // Debug.Log("Original Target Rotation: " + Context.OriginalTargetRotation);
        _elapsedTime = 0.0f;
    }

    public override void ExitState()
    {
        Debug.Log($"退出State: {StateKey} ");

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
    /// ApproachState's Update method is responsible for raising the weight of the
    /// currently assigned (right or left) 2BoneIKConstraint and MultiRotation component
    /// and incrementing time
    /// </summary>
    public override void UpdateState()
    {
        var _lerpDuration = Context.ScriptableObject.approachValue.lerpDuration;
        var _rotationSpeed = Context.ScriptableObject.approachValue.rotationSpeed;
        var _approachWeight= Context.ScriptableObject.approachValue.approachWeight;
        var _approachRotationWeight= Context.ScriptableObject.approachValue.approachRotationWeight;

        // Create a expected quaternion with Z-axis pointing down towards the ground
        // Quaternion expectedGroundRotation = Quaternion.LookRotation(-Vector3.up, Context.RootTransform.forward);
        // Context.CurrentIkTargetTransform.rotation = Quaternion.RotateTowards(Context.CurrentIkTargetTransform.rotation, expectedGroundRotation, _rotationSpeed * Time.deltaTime);

        //目标朝向：让手掌朝向地面，forwad=向下，up=角色的朝向
        Quaternion targetGroundRotation = Quaternion.LookRotation(-Vector3.up, Context.RootTransform.forward);

        _elapsedTime += Time.deltaTime;

        // 控制手腕旋转ik的控制器朝向 旋转到 目标朝向
        Context.CurrentIkTargetTransform.rotation = Quaternion.RotateTowards(
            Context.CurrentIkTargetTransform.rotation,
            targetGroundRotation,
            _rotationSpeed * Time.deltaTime);

        // 更新权重：从当前的权重过渡到接近状态的对应权重
        Context.CurrentArmIK.solver.IKRotationWeight = Mathf.Lerp(
            Context.CurrentArmIK.solver.IKRotationWeight,
            _approachRotationWeight,
            _elapsedTime / _lerpDuration);
        Context.CurrentArmIK.solver.IKPositionWeight = Mathf.Lerp(
            Context.CurrentArmIK.solver.IKPositionWeight,
            _approachWeight,
            _elapsedTime / _lerpDuration);
    }
}
