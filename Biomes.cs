using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeAttributes", menuName = "Minecraft/Biome Attribute")]
public class Biomes : ScriptableObject {

        public string biomeName;
        
        public int soilidGroundHeight;
        public int terrainHeight;
        public float terrainScale;

        public Vein[] veins;

}

[System.Serializable]
public class Vein {

    public string nodeName;

    public byte blockID;
    public int minHeight;
    public int maxHeight;
    public float scale;
    public float threshold;
    public float noiseOffset;
    
}