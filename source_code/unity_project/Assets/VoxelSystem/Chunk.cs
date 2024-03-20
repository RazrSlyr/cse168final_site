using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private Voxel[,,] voxels;
    private int chunkSize;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<Color> colors = new List<Color>();
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private ushort[] xBounds;
    private ushort[] yBounds;
    private ushort[] zBounds;
    private float chunkDensity;
    

    void Start()
    {

    }
    private void GenerateMesh()
    {
        IterateVoxels(); // Make sure this processes all voxels

        Mesh mesh = new()
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uvs.ToArray(),
            colors = colors.ToArray()
        };

        mesh.RecalculateNormals(); // Important for lighting

        meshRenderer.material = World.Instance.VoxelMaterial;
        meshFilter.mesh = mesh;
    }

    // New method to iterate through the voxel data
    public void IterateVoxels()
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    ProcessVoxel(x, y, z);
                }
            }
        }
    }

    private void ProcessVoxel(int x, int y, int z)
    {
        // Check if the voxels array is initialized and the indices are within bounds
        if (voxels == null || x < 0 || x >= voxels.GetLength(0) ||
            y < 0 || y >= voxels.GetLength(1) || z < 0 || z >= voxels.GetLength(2))
        {
            return; // Skip processing if the array is not initialized or indices are out of bounds
        }
        Voxel voxel = voxels[x, y, z];
        if (voxel.isActive)
        {
            // Check each face of the voxel for visibility
            bool[] facesVisible = new bool[6];

            // Check visibility for each face
            facesVisible[0] = IsFaceVisible(x, y + 1, z); // Top
            facesVisible[1] = IsFaceVisible(x, y - 1, z); // Bottom
            facesVisible[2] = IsFaceVisible(x - 1, y, z); // Left
            facesVisible[3] = IsFaceVisible(x + 1, y, z); // Right
            facesVisible[4] = IsFaceVisible(x, y, z + 1); // Front
            facesVisible[5] = IsFaceVisible(x, y, z - 1); // Back

            for (int i = 0; i < facesVisible.Length; i++)
            {
                if (facesVisible[i])
                    AddFaceData(x, y, z, i); // Method to add mesh data for the visible face
            }
        }
    }

    private bool IsFaceVisible(int x, int y, int z)
    {
        return true;
        // // Convert local chunk coordinates to global coordinates
        // Vector3 globalPos = transform.position + new Vector3(x, y, z);

        // // Check if the neighboring voxel is inactive or out of bounds in the current chunk
        // // and also if it's inactive or out of bounds in the world (neighboring chunks)
        // return IsVoxelHiddenInChunk(x, y, z) && IsVoxelHiddenInWorld(globalPos);
    }

    public bool IsVoxelActiveAt(Vector3 localPosition)
    {
        // Round the local position to get the nearest voxel index
        int x = Mathf.RoundToInt(localPosition.x);
        int y = Mathf.RoundToInt(localPosition.y);
        int z = Mathf.RoundToInt(localPosition.z);

        // Check if the indices are within the bounds of the voxel array
        if (x >= 0 && x < chunkSize && y >= 0 && y < chunkSize && z >= 0 && z < chunkSize)
        {
            // Return the active state of the voxel at these indices
            return voxels[x, y, z].isActive;
        }

        // If out of bounds, consider the voxel inactive
        return false;
    }

    private void AddFaceData(int x, int y, int z, int faceIndex)
    {
        // Based on faceIndex, determine vertices and triangles
        // Add vertices and triangles for the visible face
        // Calculate and add corresponding UVs

        if (faceIndex == 0) // Top Face
        {
            vertices.Add(new Vector3(x, y + 1, z));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            vertices.Add(new Vector3(x + 1, y + 1, z));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 1));
        }

        if (faceIndex == 1) // Bottom Face
        {
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x, y, z + 1));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));
        }

        if (faceIndex == 2) // Left Face
        {
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x, y, z + 1));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            vertices.Add(new Vector3(x, y + 1, z));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(0, 1));
        }

        if (faceIndex == 3) // Right Face
        {
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x + 1, y + 1, z));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));
        }

        if (faceIndex == 4) // Front Face
        {
            vertices.Add(new Vector3(x, y, z + 1));
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 1));
        }

        if (faceIndex == 5) // Back Face
        {
            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x, y + 1, z));
            vertices.Add(new Vector3(x + 1, y + 1, z));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 0));

        }
        colors.Add(voxels[x,y,z].color);
        colors.Add(voxels[x,y,z].color);
        colors.Add(voxels[x,y,z].color);
        colors.Add(voxels[x,y,z].color);
        AddTriangleIndices();
    }

    private void AddTriangleIndices()
    {
        int vertCount = vertices.Count;

        // First triangle
        triangles.Add(vertCount - 4);
        triangles.Add(vertCount - 3);
        triangles.Add(vertCount - 2);

        // Second triangle
        triangles.Add(vertCount - 4);
        triangles.Add(vertCount - 2);
        triangles.Add(vertCount - 1);
    }
    private void InitializeVoxels()
    {
        int numActive = 0;

        for (int x = xBounds[0]; x < xBounds[0] + chunkSize; x++)
        {
            for (int y = yBounds[0]; y < yBounds[0] + chunkSize; y++)
            {
                for (int z = zBounds[0]; z < zBounds[0] + chunkSize; z++)
                {
                    if (x > xBounds[1] || y > yBounds[1] || z > zBounds[1]) {
                        voxels[x - xBounds[0], y - yBounds[0], z - zBounds[0]] = new Voxel(transform.position + 
                            new Vector3(x - xBounds[0], y - yBounds[0], z - zBounds[0]), Color.white)
                        {
                            isActive = false
                        };
                        continue;
                    }
                    voxels[x - xBounds[0], y - yBounds[0], z - zBounds[0]] = new Voxel(transform.position + 
                        new Vector3(x - xBounds[0], y - yBounds[0], z - zBounds[0]), 
                        new Color((World.Instance.voxelData[x, y, z] - World.Instance.renderThreshold) / (1 - World.Instance.renderThreshold),
                         0, 
                         1 - (World.Instance.voxelData[x, y, z] - World.Instance.renderThreshold) / (1 - World.Instance.renderThreshold),
                            World.Instance.voxelData[x, y, z]))
                    {
                        isActive = World.Instance.voxelData[x, y, z] > World.Instance.renderThreshold
                    };

                    if (voxels[x - xBounds[0], y - yBounds[0], z - zBounds[0]].isActive) {
                        numActive += 1;
                    }
                }
            }
        }
        chunkDensity = ((float) numActive) / (chunkSize * chunkSize * chunkSize);
    }
    public float Initialize(int size, ushort[] xBounds, ushort[] yBounds, ushort[] zBounds)
    {
        chunkSize = size;
        this.xBounds = xBounds;
        this.yBounds = yBounds;
        this.zBounds = zBounds;
        voxels = new Voxel[size, size, size];
        InitializeVoxels();

        // Initialize Mesh Components
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        // Call this to generate the chunk mesh
        GenerateMesh();

        return chunkDensity;
    }
}