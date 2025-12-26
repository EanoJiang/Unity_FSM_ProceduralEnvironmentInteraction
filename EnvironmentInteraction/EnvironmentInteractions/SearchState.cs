using UnityEngine;

public class SearchState : EnvironmentInteractionState
{
    public SearchState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EEnvironmentInteractionState estate) : base(context, estate) { }

    // public float _approachDistanceThreshold = 2.0f;

    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {
        var _approachDistanceThreshold = Context.ScriptableObject.searchValue.approachDistanceThreshold;
        bool isCloseToTarget = Vector3.Distance(Context.ClosestPointOnColliderFromShoulder, Context.RootTransform.position) < _approachDistanceThreshold;
        bool isClosestPointOnColliderReal = Context.ClosestPointOnColliderFromShoulder != Vector3.positiveInfinity;

        if (isClosestPointOnColliderReal && isCloseToTarget)
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Approach;
        }

        if (CheckShouldReset())
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Reset;
        }

        return StateKey;
    }

    public override void EnterState()
    {
        Debug.Log($"进入State: {StateKey} ");
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

    public override void UpdateState() { }
}
