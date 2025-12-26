using UnityEngine;

public abstract class EnvironmentInteractionState : BaseState<EnvironmentInteractionStateMachine.EEnvironmentInteractionState>
{
    protected EnvironmentInteractionContext Context;
    private float _movingAwayOffset = .05f;
    bool _shouldReset;

    public EnvironmentInteractionState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EEnvironmentInteractionState stateKey) : base(stateKey)
    {
        Context = context;
    }

    /// <summary>
    /// Method available to all EnvironmentInteractionStates that can be used to determine
    /// if the state machine should transition to the Reset state.
    /// </summary>
    protected bool CheckShouldReset()
    {
        if (_shouldReset)
        {
            Context.LowestDistance = Mathf.Infinity;
            _shouldReset = false;
            return true;
        }

        bool isMovingAway = CheckIsMovingAway();
        // bool isBadAngle = CheckIsBadAngle();
        //bool isPlayerStopped = Context.Rb.velocity == Vector3.zero;
        bool isPlayerStopped = Context.Controller.velocity.magnitude < 0.01f;

        //bool isPlayerJumping = Mathf.Round(Context.Rb.velocity.y) >= 1;
        if (/*isBadAngle || */isMovingAway || isPlayerStopped/*|| Context.InteractionPoint == Vector3.positiveInfinity*/)
        {
            Context.LowestDistance = Mathf.Infinity;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Check distance away from the target and set new lowest value if they get closer
    /// </summary>
    protected bool CheckIsMovingAway()
    {
        float currentDistanceToTarget = Vector3.Distance(Context.RootTransform.position, Context.ClosestPointOnColliderFromShoulder);

        bool isSearchingForNewInteraction = Context.CurrentIntersectingCollider == null;

        if (isSearchingForNewInteraction)
        {
            return false;
        }

        bool isCloserToTarget = currentDistanceToTarget <= Context.LowestDistance;
        if (isCloserToTarget)
        {
            Context.LowestDistance = currentDistanceToTarget;
            return false;
        }

        // can apply a very small offset here if we want to give a bit of leeway
        bool isMovingAwayFromTarget = currentDistanceToTarget > Context.LowestDistance + _movingAwayOffset;

        if (isMovingAwayFromTarget)
        {
            Context.LowestDistance = Mathf.Infinity;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Prevent the arm from reaching across the body unnaturally
    /// </summary>
    protected bool CheckIsBadAngle()
    {
        if (Context.CurrentIntersectingCollider == null)
        {
            return false;
        }

        Vector3 targetDirection = Context.ClosestPointOnColliderFromShoulder - Context.CurrentShoulderTransform.position;

        // Debug.DrawLine(Context.ClosestPointOnColliderFromShoulder, Context.CurrentShoulderTransform.position);

        Vector3 shoulderDirection = Context.CurrentBodySide == EnvironmentInteractionContext.EBodySide.RIGHT ? Context.RootTransform.right : -Context.RootTransform.right;
        float dotProduct = Vector3.Dot(shoulderDirection, targetDirection.normalized);


        return dotProduct < 0;
    }


    protected void StartIkTargetPositionTracking(Collider intersectingCollider)
    {
        if (intersectingCollider.gameObject.layer == LayerMask.NameToLayer("TriggerArea") && Context.CurrentIntersectingCollider == null && Context.InteractionPoint != Vector3.positiveInfinity && Context.ClosestPointOnColliderFromShoulder != Vector3.positiveInfinity)
        {
            Context.CurrentIntersectingCollider = intersectingCollider;
            Vector3 closestPointFromRoot = GetClosestPointOnCollider(intersectingCollider, Context.RootTransform.position);
            Context.SetCurrentSide(closestPointFromRoot);

            SetIkTargetPosition();
        }
    }

    protected void ResetIkTargetPositionTracking(Collider intersectingCollider)
    {
        if (intersectingCollider == Context.CurrentIntersectingCollider)
        {
            Context.CurrentIntersectingCollider = null;
            Context.ClosestPointOnColliderFromShoulder = Vector3.positiveInfinity;
            _shouldReset = true;
        }
    }

    protected void UpdateIKTargetPosition(Collider intersectingCollider)
    {
        if (intersectingCollider == Context.CurrentIntersectingCollider && Context.InteractionPoint != Vector3.positiveInfinity && Context.ClosestPointOnColliderFromShoulder != Vector3.positiveInfinity)
        {
            SetIkTargetPosition();
        }
    }

    /// <summary>
    /// Set the closest point to the collider from the position to check
    /// </summary>
    private Vector3 GetClosestPointOnCollider(Collider intersectingCollider, Vector3 positionToCheck)
    {
        return intersectingCollider.ClosestPoint(positionToCheck);
    }

    /// <summary>
    /// SetIkTargetPosition is a method that calculates the target position for the character's interaction with a given collider.
    /// It first finds the closest point to the character's shoulder position and then computes an offset position to be used for interaction.
    /// The method takes into account the collider's geometry by using the normal vector, ensuring that the interaction point is set properly.
    /// </summary>
    private void SetIkTargetPosition()
    {
        // Context.ClosestPointOnColliderFromShoulder = GetClosestPointOnCollider(Context.CurrentIntersectingCollider, new Vector3(Context.CurrentShoulderTransform.position.x, Context.CharacterShoulderHeight, Context.CurrentShoulderTransform.position.z));
        Context.ClosestPointOnColliderFromShoulder = GetClosestPointOnCollider(Context.CurrentIntersectingCollider, new Vector3(Context.RootTransform.position.x, Context.CharacterShoulderHeight, Context.RootTransform.position.z));
        Vector3 rayDirection = Context.CurrentShoulderTransform.position - Context.ClosestPointOnColliderFromShoulder;
        Vector3 normalizedRayDirection = rayDirection.normalized;
        float offsetDistance = .05f;

        Vector3 offsetPosition = Context.ClosestPointOnColliderFromShoulder + normalizedRayDirection * offsetDistance;
        // Context.InteractionPoint = new Vector3(offsetPosition.x, Context.InteractionPointYOffset, offsetPosition.z);
        Context.InteractionPoint = new Vector3(offsetPosition.x, offsetPosition.y, offsetPosition.z);
        Context.CurrentIkTargetTransform.position = Context.InteractionPoint;
    }
}
