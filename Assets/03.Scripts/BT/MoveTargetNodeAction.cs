using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveTargetNode", story: "[Monster]", category: "Action", id: "8e606d4e8bf9a8192d21d4238c852a42")]
public partial class MoveTargetNodeAction : Action
{
    [SerializeReference] public BlackboardVariable<Monster> Monster;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Monster.Value.Move();
    }

    protected override void OnEnd()
    {

    }
}

