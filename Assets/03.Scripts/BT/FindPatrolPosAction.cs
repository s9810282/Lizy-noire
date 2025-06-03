using System;
using Unity.Behavior;
using UnityEngine;
using System.Collections.Generic;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindPatrolPos", story: "[Self] [Range]", category: "Action", id: "763fd034b1fe3bd66b8dcdb3999b3555")]
public partial class FindPatrolPosAction : Action
{
    [SerializeReference] public BlackboardVariable<Monster> Self;
    [SerializeReference] public BlackboardVariable<int> Range;
    
    protected override Status OnStart()
    {
        PathNode current = PathFinder.Instance.pathFinding.grid.GetGridObject3D(Self.Value.gameObject.transform.position);
        List<PathNode> nodes = PathFinder.Instance.GetReachableNodes(current, Range.Value, Range.Value / 2);
        PathNode targetNode = nodes[UnityEngine.Random.Range(0, nodes.Count)];

        Self.Value.TargetNode = targetNode;
        Self.Value.FindPath();

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

