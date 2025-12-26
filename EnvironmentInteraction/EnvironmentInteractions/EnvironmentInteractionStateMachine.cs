using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using RootMotion.FinalIK;

public class EnvironmentInteractionStateMachine : StateManager<EnvironmentInteractionStateMachine.EEnvironmentInteractionState>
{
    public EnvironmentInteractionScriptableObject EnvironmentInteraction_SO;
    public enum EEnvironmentInteractionState
    {
        Search,
        Approach,
        Rise,
        Touch,
        Reset,
    }

    private EnvironmentInteractionContext _context;

    private LimbIK _leftLimbIK;
    private LimbIK _rightLimbIK;
    private Rigidbody _rigidbody;
    private CharacterController _characterController;

    // void Awake()
    // {
    //     Initialize();
    // }

    public void Initialize()
    {
        InitEnvironmentInteractionMachine();
        ValidateConstraints();

        _context = new EnvironmentInteractionContext(
            _leftLimbIK,
            _rightLimbIK,
            _characterController,
            transform,
            EnvironmentInteraction_SO);

        ConstructEnvironmentDetectionCollider();
        InitializeStates();
    }
    private void InitEnvironmentInteractionMachine()
    {
        _characterController = GetCharacterControllerInParents(gameObject);
        _rigidbody = gameObject.TryGetComponent(out Rigidbody rb) ? rb : gameObject.AddComponent<Rigidbody>();
        _rigidbody.useGravity = false;

        var limbIKs = gameObject.GetComponentsInChildren<LimbIK>();
        _leftLimbIK = limbIKs.FirstOrDefault(t => t.name == "LeftArmRig");
        _rightLimbIK = limbIKs.FirstOrDefault(t => t.name == "RightArmRig");
    }
    CharacterController GetCharacterControllerInParents(GameObject obj)
    {
        CharacterController controller = obj.GetComponent<CharacterController>();

        if (controller != null)
        {
            return controller;
        }
        if (obj.transform.parent != null)
        {
            return GetCharacterControllerInParents(obj.transform.parent.gameObject);
        }
        return null;
    }
    private void InitializeStates()
    {
        // Add States and Set Initial State
        States.Clear();
        States.Add(EEnvironmentInteractionState.Reset, new ResetState(_context, EEnvironmentInteractionState.Reset));
        States.Add(EEnvironmentInteractionState.Search, new SearchState(_context, EEnvironmentInteractionState.Search));
        States.Add(EEnvironmentInteractionState.Approach, new ApproachState(_context, EEnvironmentInteractionState.Approach));
        // States.Add(EEnvironmentInteractionState.Rise, new RiseState(_context, EEnvironmentInteractionState.Rise));
        // States.Add(EEnvironmentInteractionState.Touch, new TouchState(_context, EEnvironmentInteractionState.Touch));
        CurrentState = States[EEnvironmentInteractionState.Reset];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (_context != null && _context.ClosestPointOnColliderFromShoulder != Vector3.positiveInfinity)
        {
            Gizmos.DrawSphere(_context.ClosestPointOnColliderFromShoulder, .05f);
        }
    }

    private void ConstructEnvironmentDetectionCollider()
    {
        float wingspan = _characterController.height;

        BoxCollider boxCollider =  gameObject.TryGetComponent(out BoxCollider box) ? box : gameObject.AddComponent<BoxCollider>();
        boxCollider.center = new Vector3(_characterController.center.x, _characterController.center.y + (.25f * wingspan), _characterController.center.z + (.5f * wingspan));

        _context.ColliderCenterY = _characterController.center.y;

        boxCollider.size = new Vector3(wingspan, wingspan, wingspan);
        boxCollider.isTrigger = true;
    }


    private void ValidateConstraints()
    {
        Assert.IsNotNull(_leftLimbIK, "Left LimbIK is not assigned.");
        Assert.IsNotNull(_rightLimbIK, "Right LimbIK is not assigned.");
        Assert.IsNotNull(_rigidbody, "Rigidbody used to control character is not assigned.");
    }
}
