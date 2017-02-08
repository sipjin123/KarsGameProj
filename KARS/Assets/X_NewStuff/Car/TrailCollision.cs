
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailCollision : MonoBehaviour
{
    //=============================================================================================================================================================
    #region VARIABLES
    MeshRenderer _meshRenderer;

    public Transform Guide;
    public Transform Guide2;
    MeshFilter _meshFilter;
    Mesh _mesh;

    int CurrentVertex;
    int CurrentTriangle;


    List<Vector3> Node;
    float currentRot;
    float timer;

    public float TotalDistanceTrail;
    int recentVertex = 0;
    float trailDepleteSpeed = .1f;
    float trailDistanceCap = 50;
    float const_trailDistance = 5;
    #endregion
    //=============================================================================================================================================================
    #region INITIALIZATION
    void Start()
    {
        Node = new List<Vector3>();
         _meshFilter = GetComponent<MeshFilter>();
         _mesh = new Mesh();
        _meshFilter.mesh = _mesh;

        CurrentVertex = 4;
        CurrentTriangle = 6;
        Node.Add(transform.position);
        Node.Add(new Vector3(transform.position.x, -1, transform.position.z));
    }
    #endregion
    //=============================================================================================================================================================
    #region TEST
    void OjnGUI()
    {
        GUI.Box(new Rect(0,0,300,30),"("+_mesh.vertexCount+" : "+_mesh.triangles.Length+") "+TotalDistanceTrail);
        for(int i = 0; i < _mesh.vertexCount; i++)
        {
            GUI.Box(new Rect(0, 30 +(30* i), 200, 30), i+" : " + _mesh.vertices[i]);
        }
        for (int i = 0; i < _mesh.triangles.Length; i++)
        {
            GUI.Box(new Rect(200, 30 + (30 * i), 200, 30), i + " : " + _mesh.triangles[i]);
        }
    }
    #endregion
    //=============================================================================================================================================================
    void Add()
    {
        try
        {
            //float dist = (float)(Vector3.Distance(_mesh.vertices[_mesh.vertexCount -3], _mesh.vertices[_mesh.vertexCount - 1]));
            TotalDistanceTrail += const_trailDistance;
        }
        catch
        { }

        Node.Add(Guide.transform.position);
        Node.Add(Guide2.transform.position);
        CurrentVertex += 2;
        CurrentTriangle += 6;

        GetComponent<MeshCollider>().sharedMesh = _mesh;
    }

    void Minus()
    {
        TotalDistanceTrail -= const_trailDistance;

        CurrentVertex -= 2;
        CurrentTriangle -= 6;
        Node.Remove(Node[0]);
        Node.Remove(Node[0]);
    }
    //=============================================================================================================================================================
    void Update()
    {

    }
    //=============================================================================================================================================================
  

    public void _Render()
    {
        if (_mesh.vertexCount > 3)
        {
            if (Vector3.Distance(_mesh.vertices[_mesh.vertexCount - 3], _mesh.vertices[_mesh.vertexCount - 1]) > const_trailDistance)
            {
                Add();
            }
        }
        #region VERTICES
        Vector3[] vertices = new Vector3[CurrentVertex];

        for (int i = 0; i < Node.Count; i++)
            vertices[i] = Node[i];
        

        if(TotalDistanceTrail > trailDistanceCap)
        {
            _mesh = new Mesh();
            _meshFilter.mesh = _mesh;



            if (CurrentVertex > 4)
            {
                if (Vector3.Distance(vertices[0], vertices[2]) > 0)
                {
                    vertices[0] = Vector3.MoveTowards(vertices[0], vertices[2], trailDepleteSpeed);
                    vertices[1] = Vector3.MoveTowards(vertices[1], vertices[3], trailDepleteSpeed);
                    Node[0] = vertices[0];
                    Node[1] = vertices[1];
                }
                else
                {
                    Minus();
                    return;
                }
            }
        }
        else
        {

        }

        vertices[CurrentVertex - 2] = Guide.transform.position;
        vertices[CurrentVertex - 1] = Guide2.transform.position;
        #endregion

        #region TRIANGLES
        int[] tri = new int[CurrentTriangle];
        bool Reverse = false;
        int q = 0;
        for (int i = 0; i < CurrentTriangle; i += 3)
        {
            if (i % 2 != 0)
                Reverse = true;

            if (!Reverse)
            {
                tri[i] = q;
                tri[i + 1] = q + 2;
                tri[i + 2] = q + 1;
            }
            else
            {
                tri[i] = q + 1;
                tri[i + 1] = q + 2;
                tri[i + 2] = q;
            }
            q++;
        }
        #endregion

        #region NORAMLS AND UV
        /*
        Vector3[] normals = new Vector3[CurrentVertex];
        for (int i = 0; i < CurrentVertex; i++)
            normals[i] = Vector3.forward;
            */

        /*
        Vector2[] uv = new Vector2[CurrentVertex];

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);
        //_mesh.uv = uv;
        */
        #endregion


        try
        {
            _mesh.vertices = vertices;
            _mesh.triangles = tri;
            //_mesh.normals = normals;
        }
        catch
        {
            Debug.LogError(vertices.Length);
        }
    }
    //=============================================================================================================================================================
}