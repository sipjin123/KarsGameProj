
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TrailCollision : MonoBehaviour
{
    MeshRenderer _meshRenderer;

    public Transform Guide;
    public Transform Guide2;
    MeshFilter _meshFilter;
    Mesh _mesh;

    int CurrentVertex;
    int CurrentTriangle;


    List<Vector3> Node;

    void Start()
    {
        Node = new List<Vector3>();
         _meshFilter = GetComponent<MeshFilter>();
         _mesh = new Mesh();
        _meshFilter.mesh = _mesh;

        CurrentVertex = 4;
        CurrentTriangle = 6;
        Node.Add(new Vector3(0, 1, 0));
        Node.Add(new Vector3(0, -1, 0));
    }

    void OnGUI()
    {
        GUI.Box(new Rect(0,0,100,30),""+_mesh.vertexCount+" : "+_mesh.triangles.Length);
        for(int i = 0; i < _mesh.vertexCount; i++)
        {
            GUI.Box(new Rect(0, 30 +(30* i), 200, 30), i+" : " + _mesh.vertices[i]);
        }
        for (int i = 0; i < _mesh.triangles.Length; i++)
        {
            GUI.Box(new Rect(200, 30 + (30 * i), 200, 30), i + " : " + _mesh.triangles[i]);
        }
    }
    void Add()
    {
        try
        {
            float dist = (float)(Vector3.Distance(_mesh.vertices[recentVertex], _mesh.vertices[_mesh.vertexCount - 1]));
            TotalDistanceTrail += dist;
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
        CurrentVertex -= 2;
        CurrentTriangle -= 6;
        Node.Remove(Node[0]);
        Node.Remove(Node[0]);
  }

    float currentRot;
    float timer;

    public  float TotalDistanceTrail;
    int recentVertex = 0;
    
    void Update()
    {
        _Render();

        if (Input.GetKey(KeyCode.W))
        {
            Guide.transform.parent.position += Guide.transform.parent.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Guide.transform.parent.position -= Guide.transform.parent.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            currentRot--;
            Guide.transform.parent.eulerAngles = new Vector3(0, currentRot, 0);

            timer += Time.deltaTime;
            if (timer > .5f)
            {
                Add();
                timer = 0;
            }
         }
        if (Input.GetKey(KeyCode.D))
        {
            currentRot++;
            Guide.transform.parent.eulerAngles = new Vector3(0, currentRot, 0);

            timer += Time.deltaTime;
            if (timer > .5f)
            {
                Add();
                timer = 0;
            }
        }

    }
    

    void _Render()
    {


        Vector3[] vertices = new Vector3[CurrentVertex];

        for (int i = 0; i < Node.Count; i++)
        {
            vertices[i] = Node[i];
        }


        if (Input.GetKey(KeyCode.Space))
        {
            _mesh = new Mesh();
            _meshFilter.mesh = _mesh;
            if (CurrentVertex > 4)
            {
                if (Vector3.Distance(vertices[0], vertices[2]) > 0)
                {
                    vertices[0] = Vector3.MoveTowards(vertices[0], vertices[2], 1.5f);
                    vertices[1] = Vector3.MoveTowards(vertices[1], vertices[3], 1.5f);
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

        vertices[CurrentVertex - 2] = Guide.transform.position;
        vertices[CurrentVertex - 1] = Guide2.transform.position;






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

}
#region
/*
void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found!");
            return;
        }

        Vector3 p0 = new Vector3(0, 0, 0);
        Vector3 p1 = new Vector3(1, 0, 0);
        Vector3 p2 = new Vector3(0.5f, 0, Mathf.Sqrt(0.75f));
        Vector3 p3 = new Vector3(0.5f, Mathf.Sqrt(0.75f), Mathf.Sqrt(0.75f) / 3);

        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            meshFilter.mesh = new Mesh();
            mesh = meshFilter.sharedMesh;
        }
        mesh.Clear();
        mesh.vertices = new Vector3[] { p0, p1, p2, p3 };
        mesh.triangles = new int[]{
    0,1,2,
    0,2,3,
    2,1,3,
    0,3,1
};

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
       // mesh.Optimize();
    }*/
#endregion


    /*
    #region 
    // Material - Must be a particle material that has the "Tint Color" property
    public Material material;
    Material instanceMaterial;

    // Emit
    public bool emit = true;
    bool emittingDone = false;

    // Lifetime of each segment
    public float lifeTime = 1;
    float lifeTimeRatio = 1;
    float fadeOutRatio;

    // Colors
    public Color[] colors;

    // Widths
    public float[] widths;

    // Segment creation data
    public float maxAngle = 2;
    public float minVertexDistance = 100.1f;
    public float maxVertexDistance = 100f;
    public float optimizeAngleInterval = 0.1f;
    public float optimizeDistanceInterval = 0.05f;
    public int optimizeCount = 300;

    // Object
    GameObject trailObj = null;
    Mesh mesh = null;

    // Points
    Point[] points = new Point[100];
    int pointCnt = 0;

    void Start()
    {
        trailObj = new GameObject("Trail");
        trailObj.transform.parent = null;
        trailObj.transform.position = Vector3.zero;
        trailObj.transform.rotation = Quaternion.identity;
        trailObj.transform.localScale = Vector3.one;
        MeshFilter meshFilter = (MeshFilter)trailObj.AddComponent(typeof(MeshFilter));
        mesh = meshFilter.mesh;
        trailObj.AddComponent(typeof(MeshRenderer));
        instanceMaterial = new Material(material);
        fadeOutRatio = 1f / instanceMaterial.GetColor("_TintColor").a;
      //  trailObj.renderer.material = instanceMaterial;
    }

    void Update()
    {
        // Emitting - Designed for one-time use
        if (!emit)
            emittingDone = true;
        if (emittingDone)
            emit = false;

        // Remove expired points
        for (int i = pointCnt - 1; i >= 0; i--)
        {
            Point point = points[i];
            if (point == null || point.timeAlive > lifeTime)
            {
                points[i] = null;
                pointCnt--;
            }
            else
                break;
        }

        // Optimization
        if (pointCnt > optimizeCount)
        {
            maxAngle += optimizeAngleInterval;
            maxVertexDistance += optimizeDistanceInterval;
            optimizeCount += 1;
        }

        // Do we add any new points?
        if (emit)
        {
            if (pointCnt == 0)
            {
                points[pointCnt++] = new Point(transform);
                points[pointCnt++] = new Point(transform);
            }
            if (pointCnt == 1)
                insertPoint();

            bool add = false;
            float sqrDistance = (points[1].position - transform.position).sqrMagnitude;
            if (sqrDistance > minVertexDistance * minVertexDistance)
            {
                if (sqrDistance > maxVertexDistance * maxVertexDistance)
                    add = true;
                else if (Quaternion.Angle(transform.rotation, points[1].rotation) > maxAngle)
                    add = true;
            }
            if (add)
            {
                if (pointCnt == points.Length)
                    System.Array.Resize(ref points, points.Length + 50);
                insertPoint();
            }
            if (!add)
                points[0].update(transform);
        }

        // Do we render this?
        if (pointCnt < 2)
        {
            //trailObj.renderer.enabled = false;
            return;
        }
       // trailObj.renderer.enabled = true;

        Color[] meshColors;
        lifeTimeRatio = 1 / lifeTime;

        // Do we fade it out?
        if (!emit)
        {
            if (pointCnt == 0)
                return;
            Color color = instanceMaterial.GetColor("_TintColor");
            color.a -= fadeOutRatio * lifeTimeRatio * Time.deltaTime;
            if (color.a > 0)
                instanceMaterial.SetColor("_TintColor", color);
            else
            {
                Destroy(trailObj);
                Destroy(this);
            }
            return;
        }

        // Rebuild it
        Vector3[] vertices = new Vector3[pointCnt * 2];
        Vector2[] uvs = new Vector2[pointCnt * 2];
        int[] triangles = new int[(pointCnt - 1) * 6];
        meshColors = new Color[pointCnt * 2];

        float uvMultiplier = 1 / (points[pointCnt - 1].timeAlive - points[0].timeAlive);
        for (int i = 0; i < pointCnt; i++)
        {
            Point point = points[i];
            float ratio = point.timeAlive * lifeTimeRatio;
            // Color
            Color color;
            if (colors.Length == 0)
                color = Color.Lerp(Color.white, Color.clear, ratio);
            else if (colors.Length == 1)
                color = Color.Lerp(colors[0], Color.clear, ratio);
            else if (colors.Length == 2)
                color = Color.Lerp(colors[0], colors[1], ratio);
            else
            {
                float colorRatio = ratio * (colors.Length - 1);
                int min = (int)Mathf.Floor(colorRatio);
                float lerp = Mathf.InverseLerp(min, min + 1, colorRatio);
                color = Color.Lerp(colors[min], colors[min + 1], lerp);
            }
            meshColors[i * 2] = color;
            meshColors[(i * 2) + 1] = color;

            // Width
            float width;
            if (widths.Length == 0)
                width = 1;
            else if (widths.Length == 1)
                width = widths[0];
            else if (widths.Length == 2)
                width = Mathf.Lerp(widths[0], widths[1], ratio);
            else
            {
                float widthRatio = ratio * (widths.Length - 1);
                int min = (int)Mathf.Floor(widthRatio);
                float lerp = Mathf.InverseLerp(min, min + 1, widthRatio);
                width = Mathf.Lerp(widths[min], widths[min + 1], lerp);
            }
            trailObj.transform.position = point.position;
            trailObj.transform.rotation = point.rotation;
            vertices[i * 2] = trailObj.transform.TransformPoint(0, width * 0.5f, 0);
            vertices[(i * 2) + 1] = trailObj.transform.TransformPoint(0, -width * 0.5f, 0);

            // UVs
            float uvRatio;
            uvRatio = (point.timeAlive - points[0].timeAlive) * uvMultiplier;
            uvs[i * 2] = new Vector2(uvRatio, 0);
            uvs[(i * 2) + 1] = new Vector2(uvRatio, 1);

            if (i > 0)
            {
                // Triangles
                int triIndex = (i - 1) * 6;
                int vertIndex = i * 2;
                triangles[triIndex + 0] = vertIndex - 2;
                triangles[triIndex + 1] = vertIndex - 1;
                triangles[triIndex + 2] = vertIndex - 0;

                triangles[triIndex + 3] = vertIndex + 1;
                triangles[triIndex + 4] = vertIndex + 0;
                triangles[triIndex + 5] = vertIndex - 1;
            }
        }
        trailObj.transform.position = Vector3.zero;
        trailObj.transform.rotation = Quaternion.identity;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.colors = meshColors;
        mesh.uv = uvs;
        mesh.triangles = triangles;
    }

    void insertPoint()
    {
        for (int i = pointCnt; i > 0; i--)
            points[i] = points[i - 1];
        points[0] = new Point(transform);
        pointCnt++;
    }

    class Point
    {
        public float timeCreated = 0;
        public float timeAlive
        {
            get { return Time.time - timeCreated; }
        }
        public float fadeAlpha = 0;
        public Vector3 position = Vector3.zero;
        public Quaternion rotation = Quaternion.identity;
        public Point(Transform trans)
        {
            position = trans.position;
            rotation = trans.rotation;
            timeCreated = Time.time;
        }
        public void update(Transform trans)
        {
            position = trans.position;
            rotation = trans.rotation;
            timeCreated = Time.time;
        }
    }
    #endregion
    */
