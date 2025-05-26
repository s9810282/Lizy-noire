using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UpdateStraightPath", story: "[Self] [Dir] [Range] [IsFindTarget]", category: "Action", id: "4b61d6aad685ccfe355e2ac29e59172d")]
public partial class UpdateStraightPathAction : Action
{
    [SerializeReference] public BlackboardVariable<Monster> Self;
    [SerializeReference] public BlackboardVariable<Vector3Int> Dir;
    [SerializeReference] public BlackboardVariable<float> Range;
    [SerializeReference] public BlackboardVariable<bool> IsFindTarget;
    RaycastHit bounceHit;

    protected override Status OnStart()
    {
        Vector3 targetPos = Self.Value.transform.position;

        if (Self.Value.RaycaseWall(Dir.Value, out RaycastHit hit, Range))
        {
            targetPos = hit.transform.position - Dir.Value;
        }
        else
        {
            targetPos = Self.Value.transform.position + (Dir.Value * ((int)Range.Value - 1));
        }

        PathNode current = PathFinder.Instance.pathFinding.grid.GetGridObject3D(Self.Value.transform.position);
        PathNode targetNode = PathFinder.Instance.pathFinding.grid.GetGridObject3D(targetPos);

        Self.Value.TargetNode = targetNode;
        Self.Value.FindPath();
        Self.Value.SetSpeed(Self.Value.Data.speed * 2);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

