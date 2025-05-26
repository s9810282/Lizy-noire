using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using static UnityEngine.Rendering.DebugUI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetBool ", story: "[target] [Value]", category: "Action", id: "3aa7766d1f495872c862218ce5e65d6f")]
public partial class SetBoolAction : Action
{
    [SerializeReference] public BlackboardVariable<bool> Target;
    [SerializeReference] public BlackboardVariable<bool> Value;

    protected override Status OnStart()
    {
        Target.Value = Value;
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

