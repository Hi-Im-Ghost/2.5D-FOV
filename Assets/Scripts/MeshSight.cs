using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSight : MonoBehaviour
{
    [SerializeField] private float meshSize;
    [SerializeField] private int edgeDetectIteration;
    [SerializeField] private float edgeDstThreshold;
    [SerializeField] private MeshFilter meshFilter;
    private FieldOfView fov;

    private Mesh mesh;
    private void Start()
    {
        fov = GetComponent<FieldOfView>();
        mesh = new Mesh();
        mesh.name = "MeshSight";
        meshFilter.mesh = mesh;
    }
    private void LateUpdate()
    {
        DrawMeshSight();
    }

    private void DrawMeshSight()
    {
        int stepCount = Mathf.RoundToInt(fov.viewAngle * meshSize);
        float stepAngleSize = fov.viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewInfo = new ViewCastInfo();

        for (int i = 0; i < stepCount; i++)
        {           
            float angle = transform.eulerAngles.y - fov.viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewInfo = fov.ViewCast(angle);

            if(i > 0)
            {
                bool checkEdgeDstThreshold = Mathf.Abs(oldViewInfo.dst - newViewInfo.dst) > edgeDstThreshold;
                if(oldViewInfo.hit != newViewInfo.hit || (oldViewInfo.hit && newViewInfo.hit && checkEdgeDstThreshold))
                {
                    EdgeInfo edge = FindEdge(oldViewInfo, newViewInfo);
                    if(edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if(edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }
            viewPoints.Add(newViewInfo.point);
            oldViewInfo = newViewInfo;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount -2) * 3];

        vertices[0] = Vector2.zero;
        for(int i = 0; i < vertexCount-1; i++)
        {
            vertices[i+1] = transform.InverseTransformPoint(viewPoints[i]);

            if(i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[(i * 3 + 2)] = i + 2;
            }
        }

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeDetectIteration; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = fov.ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded || newViewCast.hit != minViewCast.hit && edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }


}
