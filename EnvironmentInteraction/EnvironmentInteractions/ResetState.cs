using UnityEngine;
public class ResetState : EnvironmentInteractionState
{
    // float _lerpDuration = 10f;
    float _elapsedTime = 0.0f;
    // float _rotationSpeed = 500f;
    // float _resetDuration = 2.0f;


    public ResetState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EEnvironmentInteractionState estate) : base(context, estate)
    {
        EnvironmentInteractionContext Context = context;
    }

    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {
        var _resetDuration = Context.ScriptableObject.resetValue.resetDuration;
        bool isPastResetDuration = _elapsedTime >= _resetDuration;
        //bool isMoving = Context.Rb.velocity != Vector3.zero;
        bool isMoving = Context.Controller.velocity.magnitude >= 0.01f;
        if (isPastResetDuration && isMoving)
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Search;
        }
        return StateKey;
    }

    public override void EnterState()
    {
        Debug.Log($"进入State: {StateKey} ");
        _elapsedTime = 0.0f;
        Context.ClosestPointOnColliderFromShoulder = Vector3.positiveInfinity;
        Context.CurrentIntersectingCollider = null;
    }

    public override void ExitState()
    {
        Debug.Log($"退出State: {StateKey} ");

    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerExit(Collider other) { }
    public override void OnTriggerStay(Collider other) { }


    public override void UpdateState()
    {
        _elapsedTime += Time.deltaTime;
        ResetAnimationVars();
    }

    private void ResetAnimationVars()
    {
        var _lerpDuration = Context.ScriptableObject.resetValue.resetDuration;
        var _rotationSpeed = Context.ScriptableObject.resetValue.rotationSpeed;
        Context.CurrentIkTargetTransform.localPosition = Vector3.Lerp(Context.CurrentIkTargetTransform.localPosition, Context.CurrentOriginalConstraintPosition, _elapsedTime / _lerpDuration);
        Context.CurrentArmIK.solver.IKPositionWeight = Mathf.Lerp(Context.CurrentArmIK.solver.IKPositionWeight, 0, _elapsedTime / _lerpDuration);
        Context.CurrentArmIK.solver.IKRotationWeight = Mathf.Lerp(Context.CurrentArmIK.solver.IKRotationWeight, 0, _elapsedTime / _lerpDuration);
        Context.InteractionPointYOffset = Mathf.Lerp(Context.InteractionPointYOffset, Context.RootTransform.position.y + Context.ColliderCenterY, _elapsedTime / _lerpDuration);
        Context.CurrentIkTargetTransform.rotation = Quaternion.RotateTowards(Context.CurrentIkTargetTransform.rotation, Context.OriginalTargetRotation, _rotationSpeed * Time.deltaTime);
    }
}
