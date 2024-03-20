using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    private int worldSizeX;
    private int worldSizeY;
    private int worldSizeZ;

    private int chunkSize = 16; // Assuming chunk size is 16x16x16

    private Dictionary<Vector3, Chunk> chunks;

    [HideInInspector]
    public static World Instance { get; private set; }

    public Material VoxelMaterial;
    [HideInInspector]
    public float[,,] voxelData;
    private float[,,] chunkDensities;
    [HideInInspector]
    public float renderThreshold;
    public VolumeDataReader.Data dataset;
    private Vector3 postLoadScale;
    
    [HideInInspector]
    public Resizer resizer;

    [HideInInspector]
    public new Rigidbody rigidbody;

    // Taken from
    // https://stackoverflow.com/questions/46358717/how-to-loop-through-and-destroy-all-children-of-a-game-object-in-unity#46359133
    private void ClearChildren()
    {
        int i = 0;

        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[transform.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            DestroyImmediate(child);
        }
    }

    // Taken from 
    // https://gamedev.stackexchange.com/questions/86863/calculating-the-bounding-box-of-a-game-object-based-on-its-children#86999
    Bounds GetMaxBounds(GameObject g)
    {
        var renderers = g.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(g.transform.position, Vector3.zero);
        var b = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            b.Encapsulate(r.bounds);
        }
        return b;
    }

    private void SetupWorld(VolumeDataReader.Data dataset)
    {
        ClearChildren();
        transform.localScale = Vector3.one;
        chunks = new Dictionary<Vector3, Chunk>();
        switch (dataset)
        {
            case VolumeDataReader.Data.Skull:
                voxelData = VolumeDataReader.GetSkullData();
                postLoadScale = new Vector3(1, 2, 1);
                renderThreshold = VolumeDataReader.skullDataThreshold;
                break;
            case VolumeDataReader.Data.SkullZScores:
                voxelData = VolumeDataReader.GetSkullZScoreData();
                postLoadScale = new Vector3(1, 2, 1);
                renderThreshold = VolumeDataReader.skullZScoreDataThreshold;
                break;
            case VolumeDataReader.Data.Rabbit:
                voxelData = VolumeDataReader.GetRabbitData();
                postLoadScale = new Vector3(1, 1, 1);
                renderThreshold = VolumeDataReader.rabbitDataThreshold;
                break;
            default:
                return;
        }
        worldSizeX = (int)Mathf.Ceil(voxelData.GetLength(0) / chunkSize);
        worldSizeY = (int)Mathf.Ceil(voxelData.GetLength(1) / chunkSize);
        worldSizeZ = (int)Mathf.Ceil(voxelData.GetLength(2) / chunkSize);
        GenerateWorld();
    }

    public void DisablePhysics() {
        GetComponent<Grabbable>().isActive = false;
        rigidbody.isKinematic = true;
        rigidbody.detectCollisions = false;
    }

    public void EnablePhysics() {
        GetComponent<Grabbable>().isActive = true;
        rigidbody.isKinematic = false;
        rigidbody.detectCollisions = true;
    }

    public void RecenterChunks() {
        Vector3 offset = Vector3.zero;
        // Calculate offset
        for (ushort x = 0; x < worldSizeX; x++)
        {
            for (ushort y = 0; y < worldSizeY; y++)
            {
                for (ushort z = 0; z < worldSizeZ; z++)
                {
                    Vector3 chunkIndex = new Vector3((x - worldSizeX / 2) * chunkSize,
                        (y - worldSizeY / 2) * chunkSize, (z - worldSizeZ / 2) * chunkSize);
                    Vector3 chunkPosition = chunks[chunkIndex].transform.localPosition;
                    offset -= chunkPosition * chunkDensities[x, y, z];
                }
            }
        }

        // Apply offset
        for (ushort x = 0; x < worldSizeX; x++)
        {
            for (ushort y = 0; y < worldSizeY; y++)
            {
                for (ushort z = 0; z < worldSizeZ; z++)
                {
                    Vector3 chunkPosition = new Vector3((x - worldSizeX / 2) * chunkSize,
                        (y - worldSizeY / 2) * chunkSize, (z - worldSizeZ / 2) * chunkSize);
                    chunks[chunkPosition].transform.localPosition += offset;
                }
            }
        }
    }

    public void ResizeWorld(float size) {
        Bounds b = GetMaxBounds(gameObject);
        resizer.Resize(size, b);
        RecenterChunks();
    }


    public void Init() {
        Instance = this;
        rigidbody = GetComponent<Rigidbody>();
        DisablePhysics();
        SetupWorld(dataset);
        transform.localScale = postLoadScale;
        Bounds b = GetMaxBounds(gameObject);
        GetComponent<BoxCollider>().size = b.size / 2;
        resizer = GetComponent<Resizer>();
        ResizeWorld(3);
    }

    public Chunk GetChunkAt(Vector3 globalPosition)
    {
        // Calculate the chunk's starting position based on the global position
        Vector3Int chunkCoordinates = new Vector3Int(
            Mathf.FloorToInt(globalPosition.x / chunkSize) * chunkSize,
            Mathf.FloorToInt(globalPosition.y / chunkSize) * chunkSize,
            Mathf.FloorToInt(globalPosition.z / chunkSize) * chunkSize
        );

        // Retrieve and return the chunk at the calculated position
        if (chunks.TryGetValue(chunkCoordinates, out Chunk chunk))
        {
            return chunk;
        }

        // Return null if no chunk exists at the position
        return null;
    }

    private void GenerateWorld()
    {
        chunkDensities = new float[worldSizeX, worldSizeY, worldSizeZ];
        float totalDensity = 0;
        for (ushort x = 0; x < worldSizeX; x++)
        {
            for (ushort y = 0; y < worldSizeY; y++)
            {
                for (ushort z = 0; z < worldSizeZ; z++)
                {
                    Vector3 chunkPosition = new Vector3((x - worldSizeX / 2) * chunkSize,
                        (y - worldSizeY / 2) * chunkSize, (z - worldSizeZ / 2) * chunkSize);
                    GameObject newChunkObject = new GameObject($"Chunk_{x}_{y}_{z}");
                    newChunkObject.transform.position = chunkPosition;
                    newChunkObject.transform.parent = this.transform;

                    Chunk newChunk = newChunkObject.AddComponent<Chunk>();
                    ushort[] xBounds = new ushort[] { (ushort)(x * 16), (ushort)((x + 1) * 16) };
                    ushort[] yBounds = new ushort[] { (ushort)(y * 16), (ushort)((y + 1) * 16) };
                    ushort[] zBounds = new ushort[] { (ushort)(z * 16), (ushort)Mathf.Min(112, (z + 1) * 16) };

                    float density = newChunk.Initialize(chunkSize, xBounds, yBounds, zBounds);
                    chunks.Add(chunkPosition, newChunk);
                    chunkDensities[x, y, z] = density;
                    totalDensity += chunkDensities[x, y, z];
                }
            }
        }

        // Normalize chunk densities
        for (ushort x = 0; x < worldSizeX; x++)
        {
            for (ushort y = 0; y < worldSizeY; y++)
            {
                for (ushort z = 0; z < worldSizeZ; z++)
                {
                    chunkDensities[x, y, z] /= totalDensity;
                }
            }
        }
    }

}