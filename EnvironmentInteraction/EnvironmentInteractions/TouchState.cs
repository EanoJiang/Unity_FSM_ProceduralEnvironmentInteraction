using UnityEngine;

public class TouchState : EnvironmentInteractionState
{
    float _elapsedTime = 0.0f;
    // float _resetThreshold = 1.0f;

    Quaternion _expectedHandRotation;

    public TouchState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EEnvironmentInteractionState estate) : base(context, estate) { }

    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {
        var _resetThreshold = Context.ScriptableObject.touchValue.resetThreshold;
        if (CheckShouldReset() || _elapsedTime > _resetThreshold)
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Reset;
        }

        return StateKey;
    }


    /// <summary>
    /// TouchState's EnterState method is responsible for reseting time,
    /// and possibly setting the desired rotation for the hand to rotate to the specied TouchRotation
    /// </summary>
    public override void EnterState()
    {
        Debug.Log($"进入State: {StateKey} ");

        _elapsedTime = 0.0f;
    }

    public override void ExitState()
    {
        Debug.Log($"退出State: {StateKey} ");
    }

    public override void OnTriggerEnter(Collider other)
    {
        ResetIkTargetPositionTracking(other);
    }

    public override void OnTriggerStay(Collider other)
    {
        ResetIkTargetPositionTracking(other);
    }

    public override void OnTriggerExit(Collider other)
    {
        ResetIkTargetPositionTracking(other);
    }

    /// <summary>
    /// TouchState's Update method is responsible for placing the target at the closest point
    /// and rotating it to the specifed degree over time... In the uncharted example,
    /// it looks like the hand is rotated about 45 degrees to the
    /// </summary>
    public override void UpdateState()
    {
        _elapsedTime += Time.deltaTime;
    }
}
