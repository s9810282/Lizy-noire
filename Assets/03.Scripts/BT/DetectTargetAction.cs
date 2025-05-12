using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DetectTarget", story: "Center [self] Radius [range] Result [IsFindTarget] Target [target] [OriginPos]", category: "Action", id: "e4f908141cb733b39b4c871fd68c3ee7")]
public partial class DetectTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> Range;
    [SerializeReference] public BlackboardVariable<bool> IsFindTarget;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    [SerializeReference] public BlackboardVariable<Vector3> OriginPos;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Debug.Log("Detect");

        var self = Self.Value.GetComponent<Transform>();

        Collider[] hits = Physics.OverlapSphere(self.position, Range, LayerMask.GetMask("Player"));

        foreach (var hit in hits)
        {
            Target.Value = hit.gameObject.transform;
            OriginPos.Value = hit.gameObject.transform.position;

            IsFindTarget.Value = true;
            return Status.Success;
        }

        return Status.Running;
    }
    protected override void OnEnd()
    {

    }
}




