using System;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/EnvironmentInteractionScriptableObject", order = 1)]
public class EnvironmentInteractionScriptableObject : ScriptableObject
{
    // enum InteractionType
    // {
    //     Layer,
    //     Tag
    // }
    [Serializable]
    public class ResetValue
    {
        [LabelText("插值持续时间")]
        public float lerpDuration = 10.0f;
        [LabelText("旋转速度")]
        public float rotationSpeed = 500f;
        [LabelText("重置持续时间")]
        public float resetDuration = 2.0f;

    }
    [Serializable]
    public class SearchValue
    {
        [LabelText("接近距离阈值")]
        public float approachDistanceThreshold = 2.0f;
    }
    [Serializable]
    public class ApproachValue
    {
        [LabelText("插值持续时间")]
        public float lerpDuration = 5.0f;
        [LabelText("接近持续时间")]
        public float approachDuration = 2.0f;
        [LabelText("旋转速度")]
        public float rotationSpeed = 1000f;
        [LabelText("接近权重")]
        public float approachWeight = 0.5f;
        [LabelText("接近旋转权重")]
        public float approachRotationWeight = 0.75f;
        [LabelText("到上升距离阈值")]
        public float riseDistanceThreshold = 0.5f;

    }
    [Serializable]
    public class RiseValue
    {
        [LabelText("插值持续时间")]
        public float lerpDuration = 5.0f;
        [LabelText("旋转速度")]
        public float rotationSpeed = 1000f;
        [LabelText("上升权重")]
        public float riseWeight = 1f;
        [LabelText("触碰距离阈值")]
        public float touchDistanceThreshold = 0.05f;
        [LabelText("触碰时间阈值")]
        public float touchTimeThreshold = 0.8f;

    }
    [Serializable]
    public class TouchValue
    {
        [LabelText("重置阈值")]
        public float resetThreshold = .1f;
    }

    // [SerializeField, LabelText("环境交互类型")]
    // private InteractionType interactionType;
    // [SerializeField, LabelText("交互Collider名称")]
    // private string colliderName = "Default";

    [HideLabel, BoxGroup("重置状态")]
    public ResetValue resetValue;
    [HideLabel, BoxGroup("搜索状态")]
    public SearchValue searchValue;
    [HideLabel, BoxGroup("接近状态")]
    public ApproachValue approachValue;
    [HideLabel, BoxGroup("上升状态")]
    public RiseValue riseValue;
    [HideLabel, BoxGroup("触碰状态")]
    public TouchValue touchValue;

    // public bool IsInteractable(Collider collider)
    // {
    //     if (Convert.ToBoolean(GetInteraction()))
    //     {
    //         return IsTag(collider);
    //     }
    //     return collider.gameObject.layer == LayerMask.NameToLayer(colliderName);
    // }
    // public int GetInteraction()
    // {
    //     return interactionType == InteractionType.Layer ? 0 : 1;
    // }
    // public bool IsTag(Collider collider)
    // {
    //     return collider.gameObject.CompareTag(colliderName);
    // }
    // public LayerMask GetRaycastLayerMask()
    // {
    //     return LayerMask.GetMask(colliderName);
    // }
}

