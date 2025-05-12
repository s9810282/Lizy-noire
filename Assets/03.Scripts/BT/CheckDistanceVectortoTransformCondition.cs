using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Check Distance VectortoTransform", story: "distance between [Transform] and [TargetPos] [Equal] to [value]", category: "Conditions", id: "c7ea24c60ef4a058d078f08e2814c7ee")]
public partial class CheckDistanceVectortoTransformCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Transform> Transform;
    [SerializeReference] public BlackboardVariable<Vector3> TargetPos;
    [Comparison(comparisonType: ComparisonType.All)]
    [SerializeReference] public BlackboardVariable<ConditionOperator> Equal;
    [SerializeReference] public BlackboardVariable<float> Value;

    public override bool IsTrue()
    {
        if (Transform.Value == null || TargetPos.Value == null)
        {
            return false;
        }

        float distance = Vector3.Distance(Transform.Value.position, TargetPos.Value);
        return ConditionUtils.Evaluate(distance, Equal, Value.Value);
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
