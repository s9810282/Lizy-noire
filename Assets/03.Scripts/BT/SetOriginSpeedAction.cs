using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetOriginSpeed", story: "[Monster]", category: "Action", id: "5e85d039865e2d3ae4e4ffc7975dd48f")]
public partial class SetOriginSpeedAction : Action
{
    [SerializeReference] public BlackboardVariable<Monster> Monster;

    protected override Status OnStart()
    {
        Monster.Value.SetSpeed(Monster.Value.Data.speed);
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

