using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DetectTargetActionCross", story: "[Self] [Range] [IsFindTarget] [Dir]", category: "Action", id: "171ce1f4994de87c683d2512011c8b89")]
public partial class DetectTargetActionCrossAction : Action
{
    [SerializeReference] public BlackboardVariable<Monster> Self;
    [SerializeReference] public BlackboardVariable<float> Range;
    [SerializeReference] public BlackboardVariable<bool> IsFindTarget;
    [SerializeReference] public BlackboardVariable<Vector3Int> Dir;

    RaycastHit bounceHit;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        //Debug.Log("Detect");

        if (Self.Value.RaycaseBouncedPlayer(Vector3Int.forward, out bounceHit, Range))
        {
            Debug.Log("Find");
            Dir.Value = Vector3Int.forward;
            IsFindTarget.Value = true;

            return Status.Success;
        }
        else if (Self.Value.RaycaseBouncedPlayer(Vector3Int.back, out bounceHit, Range))
        {
            Debug.Log("Find");
            Dir.Value = Vector3Int.back;
            IsFindTarget.Value = true;

            return Status.Success;
        }
        else if (Self.Value.RaycaseBouncedPlayer(Vector3Int.right, out bounceHit, Range))
        {
            Debug.Log("Find");
            Dir.Value = Vector3Int.right;
            IsFindTarget.Value = true;

            return Status.Success;
        }
        else if (Self.Value.RaycaseBouncedPlayer(Vector3Int.left, out bounceHit, Range))
        {
            Debug.Log("Find");
            Dir.Value = Vector3Int.left;
            IsFindTarget.Value = true;

            return Status.Success;
        }


        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

