
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    float trailDistanceTotal = 20;
    float trailDistanceChid = 5;

    MeshCollider _meshCollider;
    float lerpTimer = 0;

    bool _emitTrail;

    public Car_Movement _carMovement;
    public Car_DataReceiver _carReceiveData;

    TronGameManager _tronGameManager;
    #endregion
    //=============================================================================================================================================================
    #region INITIALIZATION
    void Start()
    {
        _tronGameManager = TronGameManager.Instance.GetComponent<TronGameManager>();
        Node = new List<Vector3>();
         _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
         _mesh = new Mesh();
        _meshFilter.mesh = _mesh;

        CurrentVertex = 4;
        CurrentTriangle = 6;
        
        Node.Add(Guide.transform.position);
        Node.Add(Guide2.transform.position);

        //_emitTrail = true;
    }
    #endregion
    //=============================================================================================================================================================
    #region TEST
    void sdadaOnGUI()
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
    #region ADD/SUBTRACT MESH LENGTH
    void Add()
    {
        try
        {
            //float dist = (float)(Vector3.Distance(_mesh.vertices[_mesh.vertexCount -3], _mesh.vertices[_mesh.vertexCount - 1]));
            TotalDistanceTrail += trailDistanceChid;
        }
        catch
        { }

        Node.Add(Guide.transform.position);
        Node.Add(Guide2.transform.position);
        CurrentVertex += 2;
        CurrentTriangle += 6;

        _meshCollider.sharedMesh = _mesh;
    }

    void Minus()
    {
        TotalDistanceTrail -= trailDistanceChid;

        CurrentVertex -= 2;
        CurrentTriangle -= 6;
        Node.Remove(Node[0]);
        Node.Remove(Node[0]);
    }
    #endregion
    //=============================================================================================================================================================
    void Update()
    {
        //trailDistanceTotal = _tronGameManager.trailDistanceTotal;
        trailDistanceTotal = _carReceiveData.TrailValue();
        //trailDistanceChid = _tronGameManager.trailDistanceChild;
        trailDistanceChid = trailDistanceTotal / 4;

        if (_emitTrail)
        {
            _Render();
        }
        else
        {

        }
    }
    //=============================================================================================================================================================
    #region MESH SWITCH
    public void SetEmiision(bool _switch)
    {
        _emitTrail = _switch;
        if(_switch)
        {
            _mesh = new Mesh();


            TotalDistanceTrail = 0;
            CurrentVertex = 4;
            CurrentTriangle = 6;
            Node.Clear();
            Node.Add(Guide.transform.position);
            Node.Add(Guide2.transform.position);



            Vector3[] vertices = new Vector3[CurrentVertex];

            for (int i = 0; i < Node.Count; i++)
                vertices[i] = Node[i];
            vertices[CurrentVertex - 2] = Guide.transform.position;
            vertices[CurrentVertex - 1] = Guide2.transform.position;


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
            try
            {
                _mesh.vertices = vertices;
                _mesh.triangles = tri;

                _meshFilter.mesh = _mesh;
            }
            catch
            { }
        }
        else
        {
            try
            {
                GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n" + _carMovement.GetComponent<Car_DataReceiver>().Network_ID + " :MESH DISABLING";
            }
            catch
            {

            }
            Reset_Mesh();
        }
    }
    public void Reset_Mesh()
    {
        TotalDistanceTrail = 0;
        CurrentVertex = 4;
        CurrentTriangle = 6;
        _mesh = new Mesh();
        _meshFilter.mesh = _mesh;

        Node.Clear();
        Node.Add(Guide.transform.position);
        Node.Add(Guide2.transform.position);
        _meshCollider.sharedMesh = _mesh;
    }
    #endregion
    //=============================================================================================================================================================
    #region MESH TRAIL
    public void _Render()
    {
        if (_mesh.vertexCount > 3)
        {
            if (Vector3.Distance(_mesh.vertices[_mesh.vertexCount - 3], _mesh.vertices[_mesh.vertexCount - 1]) > trailDistanceChid)
            {
                Add();
            }
        }
        #region VERTICES
        Vector3[] vertices = new Vector3[CurrentVertex];

        for (int i = 0; i < Node.Count; i++)
            vertices[i] = Node[i];
        

        if(TotalDistanceTrail > trailDistanceTotal)
        {
            _mesh = new Mesh();
            _meshFilter.mesh = _mesh;



            if (CurrentVertex > 4)
            {
                if (Vector3.Distance(vertices[0], vertices[2]) > 0)
                {
                    lerpTimer += .25f;
                    vertices[0] = Vector3.Lerp(vertices[0], vertices[2], lerpTimer);
                    vertices[1] = Vector3.Lerp(vertices[1], vertices[3], lerpTimer);
                    Node[0] = vertices[0];
                    Node[1] = vertices[1];
                }
                else
                {
                    lerpTimer = 0;
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
    #endregion
    //=============================================================================================================================================================
}