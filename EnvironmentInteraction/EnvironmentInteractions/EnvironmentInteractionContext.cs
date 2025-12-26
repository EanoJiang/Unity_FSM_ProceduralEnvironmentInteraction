using UnityEngine;
using RootMotion.FinalIK;
public class EnvironmentInteractionContext
{
    public enum EBodySide
    {
        RIGHT,
        LEFT
    }

    private EnvironmentInteractionScriptableObject _scriptableObject;

    public LimbIK _leftArmIK;
    public LimbIK _rightArmIK;
    public Vector3 _rightOriginalTargetPosition;
    public Vector3 _leftOriginalTargetPosition;

    public EnvironmentInteractionContext(LimbIK leftArmIK, LimbIK rightArmIK, CharacterController controller, Transform rootTransform, EnvironmentInteractionScriptableObject scriptableObject)
    {
        _leftArmIK = leftArmIK;
        _rightArmIK = rightArmIK;
        Controller = controller;
        _scriptableObject = scriptableObject;

        _rightOriginalTargetPosition = _rightArmIK.solver.target.localPosition;
        _leftOriginalTargetPosition = _leftArmIK.solver.target.localPosition;
        RootTransform = rootTransform;
        OriginalTargetRotation = _leftArmIK.solver.target.rotation;
        CharacterShoulderHeight = _leftArmIK.solver.bone1.transform.position.y; // 角色的肩部高度(左右侧一样)

        //TODO IN SCRIPT: ADD THESE IN BECAUSE YOU TOOK THEM OUT TO UNTIL YOU ADD THE CHARACTER SHOULDER HEIGHT
        SetCurrentSide(ClosestPointOnColliderFromShoulder);
    }

    // Shared variables and properties
    public LimbIK CurrentArmIK { get; private set; }
    public Vector3 CurrentOriginalConstraintPosition { get; private set; }
    public Quaternion OriginalTargetRotation { get; private set; }
    public EBodySide CurrentBodySide { get; private set; }

    public float CharacterShoulderHeight { get; private set; }
    public Transform RootTransform { get; private set; }

    public Transform CurrentShoulderTransform { get; private set; }
    public Transform CurrentIkTargetTransform { get; private set; }

    public Collider CurrentIntersectingCollider { get; set; }
    //public Rigidbody Rb { get; private set; }

    public CharacterController Controller { get; private set; }
    public Vector3 ClosestPointOnColliderFromShoulder { get; set; } = Vector3.positiveInfinity;
    public Vector3 InteractionPoint { get; set; }
    public float InteractionPointYOffset { get; set; }
    public float ColliderCenterY { get; set; }

    public float LowestDistance { get; set; } = Mathf.Infinity;

    public EnvironmentInteractionScriptableObject ScriptableObject => _scriptableObject;

    // TODO: Find a way to move this out of the context.
    /// <summary>
    ///SetCurrentSide is a method that determines the closest side (left or right) of the character's body for interacting with a given position.
    /// It compares the distances from the left and right IK constraint root transforms to the position, and updates the current body side, IK constraint, multi-rotation constraint, original constraint position, shoulder transform, and IK target transform properties within the context accordingly.
    /// </summary>
    public void SetCurrentSide(Vector3 positionToCheck)
    {
        Vector3 leftShoulder = _leftArmIK.solver.bone1.transform.position;
        Vector3 rightShoulder = _rightArmIK.solver.bone1.transform.position;
        if (Vector3.Distance(positionToCheck, leftShoulder) < Vector3.Distance(positionToCheck, rightShoulder))
        {
            // Debug.Log("LEFT IS CLOSER");
            CurrentBodySide = EBodySide.LEFT;
            CurrentArmIK = _leftArmIK;
            CurrentOriginalConstraintPosition = _leftOriginalTargetPosition;

        }
        else
        {
            // Debug.Log("RIGHT IS CLOSER");
            CurrentBodySide = EBodySide.RIGHT;
            CurrentArmIK = _rightArmIK;
            CurrentOriginalConstraintPosition = _rightOriginalTargetPosition;
        }

        CurrentShoulderTransform = CurrentArmIK.solver.bone1.transform;
        CurrentIkTargetTransform = CurrentArmIK.solver.target;
    }
}
