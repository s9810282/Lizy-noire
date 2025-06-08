using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UltimateExecutor : MonoBehaviour
{
    [SerializeField] List<UltmateRange> blowRange;
    [SerializeField] List<UltmateRange> slashRange;

    [Header("Layers")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask monsterLayer;

    void Start()
    {
        //StartCoroutine(tests());
    }

    IEnumerator tests()
    {
        yield return new WaitForSeconds(1f);
        ExcuteUlt(EAttakcType.Slash, 1, transform.position, transform.forward);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExcuteUlt(EAttakcType e, int ultCount, Vector3 playerPos, Vector3 forward)
    {
        if (ultCount == 0) return;

        ultCount -= 1;
        List<UltLine> attackLines = 
            e == EAttakcType.Blow ? blowRange[ultCount].range : slashRange[ultCount].range;

        int combinedMask = wallLayer | monsterLayer;

        forward = GetDiscreteForward(forward);

        List<Vector3> effectPos = new List<Vector3>();

        foreach (UltLine line in attackLines)
        {
            foreach (var offset in line.cells)
            {
                Debug.Log("asd");

                Vector3 worldPos = GetWorldPosition(playerPos, forward, offset);
                Collider[] hits = Physics.OverlapBox(worldPos, Vector3.one * 0.45f, Quaternion.identity, combinedMask);
                Debug.DrawLine(worldPos, worldPos + Vector3.forward * 2f, Color.red, 2f);
                bool blocked = false;

                foreach (var hit in hits)
                {
                    Debug.Log(hit.gameObject.name);

                    if (hit.CompareTag("Wall"))
                    {
                        blocked = true;
                        break;
                    }

                    effectPos.Add(hit.gameObject.transform.position);

                    if (hit.TryGetComponent(out Monster monster))
                        monster.TakeDamage(9999, EAttakcType.Ult);
                    else if (hit.CompareTag("BreakableWall"))
                    {
                        hit.gameObject.SetActive(false);
                        PathFinder.Instance.RemoveClostNode(hit.gameObject.transform.position);
                        //hit.GetComponent<BreakableWall>().Break();
                    }
                }

            
                if (blocked) break; // ÀÌ ÁÙ ¸ØÃß±â!
            }
        }

        EventBus.Publish(new UltEvent
        {
            range = effectPos,
        });

    }

    public static Vector3 GetWorldPosition(Vector3 origin, Vector3 forward, Vector2Int offset)
    {     
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

        Vector3 offsetWorld =
            forward.normalized * offset.x +
            right.normalized * offset.y;

        Vector3 worldPos = origin + offsetWorld;
        worldPos = new Vector3(Mathf.Round(worldPos.x), 1, Mathf.Round(worldPos.z));

        return worldPos;
    }
    public static Vector3 GetDiscreteForward(Vector3 dir)
    {
        dir.y = 0f;
        dir.Normalize();

        if (Vector3.Dot(dir, Vector3.forward) > 0.7f) return Vector3.forward;
        if (Vector3.Dot(dir, Vector3.back) > 0.7f) return Vector3.back;
        if (Vector3.Dot(dir, Vector3.right) > 0.7f) return Vector3.right;
        if (Vector3.Dot(dir, Vector3.left) > 0.7f) return Vector3.left;

        return Vector3.forward;
    }
}
