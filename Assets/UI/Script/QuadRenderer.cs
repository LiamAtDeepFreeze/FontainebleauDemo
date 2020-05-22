using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class QuadRenderer : MonoBehaviour
{
    public Color PanelColor = Color.white;
    private MeshFilter _filter;

    //Buffers
    private List<Vector3> _vertices;
    private List<Vector2> _uvs;
    private readonly int[] _indices =
    {
        0, 1, 2, 0 ,2 , 3
    };

	// Use this for initialization
	void Start ()
    {
        _filter = GetComponent<MeshFilter>();
		UpdateMesh();
	}
	
	// Update is called once per frame
    private void Update ()
    {
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        Mesh mesh;
        if (_filter.sharedMesh == null)
        {
            mesh = new Mesh();
            _filter.sharedMesh = mesh;
        }
        else
            mesh = _filter.sharedMesh;

        _vertices = new List<Vector3>
        {
            new Vector3(-0.5f, -0.5f), 
            new Vector3(-0.5f, 0.5f), 
            new Vector3(0.5f, 0.5f), 
            new Vector3(0.5f, -0.5f)
        };


        _uvs = new List<Vector2>
        {
            new Vector2(0.0f, 0.0f), 
            new Vector2(0.0f, 1.0f), 
            new Vector2(1.0f, 1.0f), 
            new Vector2(1.0f, 0.0f)
        };

        var colors = new List<Color>();
        for (int i = 0; i < 4; i++)
            colors.Add(this.PanelColor);

        mesh.SetVertices(_vertices);
        mesh.SetColors(colors);
        mesh.SetUVs(0,_uvs);
        mesh.SetIndices(_indices, MeshTopology.Triangles, 0);

        mesh.MarkDynamic();
        mesh.RecalculateBounds();
    }

}
