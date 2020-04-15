using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public Material material;

    public Biomes biomes;

    public Transform player;

    Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks,VoxelData.WorldSizeInChunks];

    List<ChunkCoord> activeChunks = new List<ChunkCoord>();
    public List<ChunkCoord> chunksToGenerate = new List<ChunkCoord>();

    ChunkCoord PlayerLastChunkCoord;

    bool isCreatingChunks;

    private void Start() {

        GenerateWorld();
        PlayerLastChunkCoord = GetChunkCoordFormVec3(player.position);

    }

    private void Update() {
    // check if palyer has moved into a new chunck. If so recalculate what chuncks have to be rendered AKA moved in or out of active chuncks list.

        if (!GetChunkCoordFormVec3(player.position).Equals(PlayerLastChunkCoord))
            CheckViewDistance();
            PlayerLastChunkCoord = GetChunkCoordFormVec3(player.position); 

        if (chunksToGenerate.Count > 0 && !isCreatingChunks)
            StartCoroutine("CreateChunks");
    }

    void GenerateWorld () {
    // Creates chunck in a cube defined by atributes VoxelData.WorldSizeInChunks

        for (int x = (VoxelData.WorldSizeInChunks / 2) - VoxelData.ViewDistanceInChunks ; x < (VoxelData.WorldSizeInChunks / 2) + VoxelData.ViewDistanceInChunks ; x++){
            for (int z = (VoxelData.WorldSizeInChunks / 2) - VoxelData.ViewDistanceInChunks ; z < (VoxelData.WorldSizeInChunks / 2) + VoxelData.ViewDistanceInChunks ; z++){

                chunks[x,z] = new Chunk(new ChunkCoord(x,z),this,true);
                ActivateChunk(new ChunkCoord(x, z));
            }
        }
        player.position = new Vector3(VoxelData.WorldSizeInVoxels / 2, biomes.terrainHeight +30 , VoxelData.WorldSizeInVoxels / 2);
    }

    void CheckViewDistance() {

        ChunkCoord coord = GetChunkCoordFormVec3(player.position);

        // Get list of chuncks that was active (AKA was being rendered)
        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);

        for (int x = coord.x - VoxelData.ViewDistanceInChunks; x < coord.x + VoxelData.ViewDistanceInChunks; x++){
            for (int z = coord.z - VoxelData.ViewDistanceInChunks ; z < coord.z + VoxelData.ViewDistanceInChunks ; z++){

               if (IsChuckInWorld(x, z)){
                   // if chuncks are not in world list add to chunksToGenarate to generate them
                   if (chunks[x, z] == null){
                        chunks[x,z] = new Chunk(new ChunkCoord(x,z),this,false);
                        chunksToGenerate.Add(new ChunkCoord(x, z));
                   }
                    else 
                        ActivateChunk(new ChunkCoord(x, z));  
               }

               // Removes all chuncks that should not be removed from the active list. So all chuncks left should be removed 
               for (int i = 0; i < previouslyActiveChunks.Count; i++){
                    if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                        previouslyActiveChunks.RemoveAt(i);
               }
            }
        }
        // Removes chuncks left in previousActive list
        foreach (ChunkCoord c in previouslyActiveChunks){
           chunks[c.x, c.z].IsActive = false;
           activeChunks.Remove(c);
       }   
    }

    IEnumerator CreateChunks (){
        Debug.Log("HIT");
        isCreatingChunks = true;
        while(chunksToGenerate.Count > 0){
            chunks[chunksToGenerate[0].x,chunksToGenerate[0].z].Init();
            ActivateChunk(new ChunkCoord(chunksToGenerate[0].x,chunksToGenerate[0].z));
            chunksToGenerate.RemoveAt(0);
            yield return null;
        isCreatingChunks = false;
        }
    }

    ChunkCoord GetChunkCoordFormVec3(Vector3 pos){

        int x =  Mathf.FloorToInt(pos.x / VoxelData.chunkWidth);
        int z =  Mathf.FloorToInt(pos.z / VoxelData.chunkWidth);

        return new ChunkCoord(x,z); 

    }

//////////////////////////////////////////////////
// REDO THIS !!!!!!!!!!!!!!
/////////////////////////////////////////////
    public byte GetVoxel(Vector3 pos){

        byte returnVal = 0;

        if (!IsVoxelInWorld(pos))
           return 0;

        if (pos.y == 0)
            return 9;

        //FIRST PASS

        int terrainHeight = biomes.soilidGroundHeight + (Mathf.FloorToInt(Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biomes.terrainScale) * biomes.terrainHeight));

        if (pos.y == terrainHeight)
            returnVal = 3;

        else if (pos.y < terrainHeight && pos.y > terrainHeight - 3)
            returnVal = 4;

        else if (pos.y < terrainHeight)
            returnVal = 6;

        else returnVal = 0;

        //SECOND PASS

        if (returnVal == 1){
            foreach (Vein vein in biomes.veins){

                if (pos.y >= vein.minHeight && pos.y <= vein.maxHeight){
                    if (Noise.Get3DPerlin(pos, vein.noiseOffset, vein.scale, vein.threshold))
                        returnVal = vein.blockID;
                }
            }
        }

        return returnVal;
    }

    // Return bool that shows if a block as xyz coord is soilid.
    public bool CheckVoxelIsSoilid(Vector3 _coords){
        // Get the ChunckCoord of the chunck the voxel is located in.
        ChunkCoord thisChunk = new ChunkCoord(_coords);

        if (!IsVoxelInWorld(_coords))
           return false;
        
        // We now know the voxel is in thisChunck so it can get referenced out of thisChunck Block list if the chunck has been created and initialized.
        if (chunks[thisChunk.x,thisChunk.z] != null && chunks[thisChunk.x,thisChunk.z]._IsVoxelMapPopulated)
            return BlockData.isSoilid[chunks[thisChunk.x,thisChunk.z].GetVoxelFromGlobleVec3(_coords)]; 

        // If all else fails the chunk has not jet been creted and the block referenced has to be identified by theGetVoxel funcktion
        // This is posible as the adjasond block does not have to be created jet as this blocks state id defined by the Noise function used.
        // This is the breakthrough I was looking for. All the adjasond blocks does not have to be created or stored in a list. 
        // The GetVoxel function can use only position Vec3 to determine what block it would be long before the block is stored in an array.
        // This is last resort as this function is very expensive.
        return BlockData.isSoilid[GetVoxel(_coords)];
    }


    bool IsChuckInWorld (int x, int z){

         if (x > 0 && x < VoxelData.WorldSizeInChunks - 1 && z > 0 && z < VoxelData.WorldSizeInChunks - 1)
            return true;
        else
            return false;
    }

    bool IsVoxelInWorld(Vector3 pos){

        if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.chunkHeight  && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels)
            return true;
        
        else
            return false;
    }

// Used to set the active chuncks that shoulde be rendered the next frame.
    void ActivateChunk(ChunkCoord coord){

        if (!chunks[coord.x, coord.z].IsActive){
            activeChunks.Add(coord);
            chunks[coord.x, coord.z].IsActive = true;
        }
    }
}
