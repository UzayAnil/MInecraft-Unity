using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    GameObject chunkObject;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    int vertIndex = 0;
    List<Vector3> vertices = new List<Vector3> ();   
    List<int> triangles = new List<int> ();
    List<Vector2> uvs = new List<Vector2> ();

    World world;
    ChunkCoord coord;

    bool _IsActive;
    public bool _IsVoxelMapPopulated;

    // This is the list that stores all the bvlockes and block states in the current chunck.
    public byte[,,] voxelMap = new byte[VoxelData.chunkWidth,VoxelData.chunkHeight,VoxelData.chunkWidth];

    public Chunk(ChunkCoord chunkCoord, World _world, bool createbool){

        world = _world; 
        coord = chunkCoord;
        
        if(createbool){
            Init();
        }
    }

    public void Init(){
        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshRenderer.material = world.material;
        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(coord.x * VoxelData.chunkWidth, 0f, coord.z * VoxelData.chunkWidth);
        chunkObject.name = "Chunk " + coord.x + ", " + coord.z;

        PopulateVoxelMap();
        CreateChunkData();
        
        Render();
    }

    void PopulateVoxelMap(){
    // Populate voxel map with blocks. This is where blocks are added into the chunk

        for (int x = 0; x < VoxelData.chunkWidth; x++){
            for (int y = 0; y < VoxelData.chunkHeight; y++){
                for (int z = 0; z < VoxelData.chunkWidth; z++){
                    voxelMap[x,y,z] = world.GetVoxel(new Vector3(x,y,z) + position);
                } 
            }
        }
        _IsVoxelMapPopulated = true;
    }

    public bool IsActive{
    // Get Activce retuns if the Chuck is active AKA is renderd and can be interacted with
    // Set Activce can load and unload chuncks if is out of render distance

        get {return _IsActive;}
        set {_IsActive = value;
            if (chunkObject != null)
                chunkObject.SetActive(value);
        }
    }

    public Vector3 position {
    // Just a simple function to return the world position of the chunk
        get{ return chunkObject.transform.position;}
    }

    bool IsVoxelInChunk(int x, int y, int z){
    // Check to see if the block that is being checked is in the current chunck. is used to determine if a face should be renderd.

        if (x < 0 || x > VoxelData.chunkWidth -1 || z < 0 || z > VoxelData.chunkWidth - 1 || y < 0 || y > VoxelData.chunkHeight - 1){
            return false;
        }
        else{
            return true;
        }

    }

    //Get voxel in this chunck from a globle Vec3
    public byte GetVoxelFromGlobleVec3 (Vector3 _coord){
        int check_x = Mathf.FloorToInt(_coord.x);
        int check_y = Mathf.FloorToInt(_coord.y);
        int check_z = Mathf.FloorToInt(_coord.z);

        check_x -= Mathf.FloorToInt(chunkObject.transform.position.x);
        check_z -= Mathf.FloorToInt(chunkObject.transform.position.z);

        // return the byte ID of the block at _coord 
        return voxelMap[check_x,check_y,check_z];
    }

    void CreateChunkData(){
    // Creates all vertices in a chunk

        for (int x = 0; x < VoxelData.chunkWidth; x++){
            for (int y = 0; y < VoxelData.chunkHeight; y++){
                for (int z = 0; z < VoxelData.chunkWidth; z++){
                    FaceVerts(new Vector3Int(x,y,z));
                } 
            }
        }
    }

    // Check to see if block is soilid. 
    // FIRST PASS : IS the voxel in the current Chunck ? If so get the voxel out of blocks list check if its soilid.
    // SECOND PASS : If Voxel not in current Chunck Run CheckVoxelIsSoilid which is a function in World CLass.
    private bool CheckVoxelChunk(Vector3 pos){

        int check_x = Mathf.FloorToInt(pos.x);
        int check_y = Mathf.FloorToInt(pos.y);
        int check_z = Mathf.FloorToInt(pos.z);

        if (IsVoxelInChunk(check_x,check_y, check_z))
            return BlockData.isSoilid[voxelMap[check_x,check_y,check_z]];

        else
            return world.CheckVoxelIsSoilid(pos + position);

    }
    void FaceVerts(Vector3Int pos){ 
    // Function that adds all faces that should be drawn into the buffer that is sent to the GPU to render all tris.
    // Also handels which textures are assinged to each block

        for (int n = 0; n < 6; n++){
            if (CheckVoxelChunk((pos + VoxelData.checkNeighbour[n])) != true && CheckVoxelChunk(pos) == true){
                for (int i = 0; i < 6; i++){

                    int triangleIndex = VoxelData.voxelTris[n,i];

                    vertices.Add(VoxelData.voxelVerts[triangleIndex] + pos);
                    triangles.Add(vertIndex);

                    vertIndex ++;
                } 
                AddTexure(BlockData.textureFaces[voxelMap[pos.x,pos.y,pos.z],n]); 
            }
        }
    }

    void Render(){
    // Renders mesh created in FaceVerts

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    void AddTexure (int texureID) {
    // Function called in Face Verts to assign and apply a certain tecture to a face

        float y = texureID / VoxelData.TextureBlocksOnAtlas;
        float x = texureID - (y * VoxelData.TextureBlocksOnAtlas);

        x *= VoxelData.NormalizedTexureSize;
        y *= VoxelData.NormalizedTexureSize;

        y = 1f - y - VoxelData.NormalizedTexureSize;

        uvs.Add(new Vector2(x,y));
        uvs.Add(new Vector2(x,y + VoxelData.NormalizedTexureSize));
        uvs.Add(new Vector2(x + VoxelData.NormalizedTexureSize,y));
        uvs.Add(new Vector2(x + VoxelData.NormalizedTexureSize,y));
        uvs.Add(new Vector2(x,y + VoxelData.NormalizedTexureSize));
        uvs.Add(new Vector2(x + VoxelData.NormalizedTexureSize,y + VoxelData.NormalizedTexureSize));
    }
}

public class ChunkCoord {
// Simple class to make using and storing chunck coords easyer.

    public int x;
    public int z;

    public ChunkCoord (int _x, int _z){
        x = _x;
        z = _z;
    }

    public ChunkCoord(Vector3 _coord){

        int xCheck = Mathf.FloorToInt(_coord.x);
        int zCheck = Mathf.FloorToInt(_coord.z);

        x = Mathf.FloorToInt(xCheck / VoxelData.chunkWidth);
        z = Mathf.FloorToInt(zCheck / VoxelData.chunkWidth);
    }

    public bool Equals(ChunkCoord other ){

        if (other == null)
            return false;
        else if (other.x == x && other.z == z)
            return true;
        else
            return false;
    }
}
 