using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlockData {

    public const byte numberOfBlockTypes = 11;    
    public static readonly bool[] isSoilid = new bool[numberOfBlockTypes]{

        false,
        true,
        true,
        true,
        true,
        true,
        true,
        true,
        true,
        true,
        true

    };

    public static readonly int[,] textureFaces = new int[numberOfBlockTypes,6]{

        {0,0,0,0,0,0}, //Air
        {0,0,0,0,0,0}, //Stone
        {8,8,8,8,8,8}, //Cobblestone
        {7,1,2,2,2,2}, //Grass
        {1,1,1,1,1,1}, //Dirt
        {6,6,5,5,5,5}, //Oak_Log
        {4,4,4,4,4,4}, //Oak_Plank
        {10,10,10,10,10,10}, //Sand
        {11,11,11,11,11,11}, //Brick
        {9,9,9,9,9,9}, //Bedrock
        {3,3,3,3,3,3} //Coal
    };
    
}

