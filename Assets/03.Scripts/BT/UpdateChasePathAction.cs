using System;
using Unity.Behavior;
using UnityEngine;
using System.Collections.Generic;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UpdateChasePath", story: "Monster [monster] Target [targetPos] [OriginPos]", category: "Action", id: "39d807644bc1fed91fa655fc99d8a062")]
public partial class UpdateChasePathAction : Action
{
    [SerializeReference] public BlackboardVariable<Monster> Monster;
    [SerializeReference] public BlackboardVariable<Transform> TargetPos;
    [SerializeReference] public BlackboardVariable<Vector3> OriginPos;
    protected override Status OnStart()
    {
        //Debug.Log("Update Path");

        if (TargetPos.Value == null) return Status.Failure;

        int x = Mathf.FloorToInt(TargetPos.Value.transform.position.x);
        int y = 1;
        int z = Mathf.FloorToInt(TargetPos.Value.transform.position.z);

        OriginPos.Value = new Vector3(x, y, z);

        PathNode current = PathFinder.Instance.pathFinding.grid.GetGridObject3D(Monster.Value.transform.position);
        PathNode targetNode = PathFinder.Instance.pathFinding.grid.GetGridObject3D(TargetPos.Value.transform.position);

        Monster.Value.TargetNode = targetNode;
        Monster.Value.FindPath();

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

