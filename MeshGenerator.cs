using System.Security.Cryptography;
using System.Xml.Schema;
using System.Net.Http;
using System;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    
    UnityEngine.Vector3[] vertices;
    int[] triangles;
    private Color[] colors;
    [SerializeField] private Gradient gradient;
    
    public float offsetX;
    public float offsetZ;

    public int xSize = 250;
    public int zSize = 250;
    

    float minTerrainHeight;
    float maxTerrainHeight;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        offsetX = UnityEngine.Random.Range(0f, 100f);
        offsetZ = UnityEngine.Random.Range(0f, 100f);
        
        CreateNewMap();
    }
    public void CreateNewMap()
    {
        CreateShape();
        ColorMap();
        UpdateMesh();
    }
    public void RegenerateMap()
    {
        Start();
    }
    void CreateShape()
    {
        vertices = new UnityEngine.Vector3[(xSize + 1) * (zSize+1)];
            
        for (int i=0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * offsetX * .1f , z * offsetZ * .1f) * 1.5f;
                vertices[i] = new UnityEngine.Vector3(x, y, z);

                if (y > maxTerrainHeight)
                    maxTerrainHeight = y;
                if (y < minTerrainHeight)
                    minTerrainHeight = y;

                i++;

            }
        }
        
        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
           for (int x = 0; x < xSize; x++)
           {
            
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

    }
    public void ColorMap()
    {
        colors = new Color[vertices.Length];

        for (int i = 0, z = 0; z < zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    public void doExitGame() {
     Application.Quit();
    }
}
