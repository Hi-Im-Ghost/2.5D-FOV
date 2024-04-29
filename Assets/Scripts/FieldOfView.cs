using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{

    [SerializeField] private LayerMask objectsMask;
    [SerializeField] private LayerMask wallsMask;
    public float viewRadius = 20f;
    [Range(0f, 360)]
    public float viewAngle = 60f;
    public List<Transform> visibleObjects = new List<Transform>();

    private void Start()
    {
        StartCoroutine(FindObjectsWithDelay(0.2f));
    }

    IEnumerator FindObjectsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleObjects();
        }
    }

    private void FindVisibleObjects()
    {
        visibleObjects.Clear();
        Collider[] objectsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, objectsMask);
        for(int i= 0; i < objectsInViewRadius.Length; i++)
        {
            Transform target = objectsInViewRadius[i].transform;
            Vector3 dirToObject = (target.position - transform.position).normalized;
            if (Vector3.Angle (transform.forward, dirToObject) < viewAngle / 2)
            {
                float distanceToObject = Vector3.Distance (transform.position, target.position);

                if(!Physics.Raycast(transform.position, dirToObject, distanceToObject, wallsMask))
                {
                    visibleObjects.Add(target);
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));

    }

    public ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, wallsMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }



}
