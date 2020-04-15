using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData{

    public static readonly int chunkWidth = 16;
    public static readonly int chunkHeight = 128;
    public static readonly int WorldSizeInChunks = 100;

    public static readonly int ViewDistanceInChunks = 4; 

    public static int WorldSizeInVoxels{
        get{return WorldSizeInChunks * chunkWidth;}
    }

    public static readonly int TextureBlocksOnAtlas = 4;

    public static float NormalizedTexureSize {
        get{return 1f / TextureBlocksOnAtlas;}
    }

    public static readonly Vector3[] voxelVerts = new Vector3[8] {

        new Vector3(0.0f,0.0f,0.0f),
        new Vector3(1.0f,0.0f,0.0f),
        new Vector3(1.0f,1.0f,0.0f),
        new Vector3(0.0f,1.0f,0.0f),
        new Vector3(0.0f,0.0f,1.0f),
        new Vector3(1.0f,0.0f,1.0f),
        new Vector3(1.0f,1.0f,1.0f),
        new Vector3(0.0f,1.0f,1.0f)
    };

    public static readonly Vector2[] uvData = new Vector2[6] {
        new Vector2 (0.0f, 0.0f),
        new Vector2 (0.0f, 1.0f),
        new Vector2 (1.0f, 0.0f),
        new Vector2 (1.0f, 0.0f),
        new Vector2 (0.0f, 1.0f),
        new Vector2 (1.0f, 1.0f)
    };

    public static readonly Vector3[] checkNeighbour = new Vector3[6] {

        new Vector3(0.0f,1.0f,0.0f),
        new Vector3(0.0f,-1.0f,0.0f),
        new Vector3(0.0f,0.0f,1.0f),
        new Vector3(0.0f,0.0f,-1.0f),
        new Vector3(1.0f,0.0f,0.0f),
        new Vector3(-1.0f,0.0f,0.0f)

    };

    public static readonly byte[,] voxelTris = new byte[6,6]{
        
        {3,7,2,2,7,6}, //top
        {1,5,0,0,5,4}, //Bottom
        {5,6,4,4,6,7}, //Front
        {0,3,1,1,3,2}, //Back
        {1,2,5,5,2,6}, //Right
        {4,7,0,0,7,3}  //Left 

    };
    
}
