using UnityEngine;

public class TerrainPainter : MonoBehaviour
{
    public Terrain terrain;
    public Texture2D[] terrainTextures;
    public float[] textureHeights;

    void Start()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain reference is not set in the TerrainPainter script.");
            return;
        }

        if (terrainTextures == null || terrainTextures.Length == 0)
        {
            Debug.LogError("Terrain textures are not set in the TerrainPainter script.");
            return;
        }

        if (textureHeights == null || terrainTextures.Length != textureHeights.Length)
        {
            Debug.LogError("Texture heights are not set or don't match the number of terrain textures.");
            return;
        }

        PaintTerrain();
    }

    void PaintTerrain()
    {
        TerrainData terrainData = terrain.terrainData;
        int textureWidth = terrainData.alphamapWidth;
        int textureHeight = terrainData.alphamapHeight;
        int layers = terrainTextures.Length;

        float min = float.MaxValue;
        float max = float.MinValue;

        float[,,] splatmapData = new float[textureWidth, textureHeight, layers];

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                // Get the normalized height at the current (x, y) position
                float normalizedX = x * 1.0f / (textureWidth - 1);
                float normalizedY = y * 1.0f / (textureHeight - 1);
                float height = terrainData.GetInterpolatedHeight(normalizedX, normalizedY);

                // Determine the active texture layer based on height
                int activeTextureIndex = 0;
                for (int i = 0; i < textureHeights.Length; i++)
                {

                    if (height < min)
                    {
                        min = height;
                    }

                    if (height > max)
                    {
                        max = height;
                    }
                    
                    if (height >= textureHeights[i])
                    {
                        activeTextureIndex = i;
                    }
                }

                // Set the blend value for the active texture layer
                for (int i = 0; i < layers; i++)
                {
                    splatmapData[y, x, i] = (i == activeTextureIndex) ? 1f : 0f;
                }
            }
        }
        
        Debug.Log(terrain.ToString());
        Debug.Log("min = " + min + ", max = " + max);
        Debug.Log("brown lvl = " + (max - min) * 0.45); 
        Debug.Log("grey lvl = " + (max - min) * 0.7); 
        Debug.Log("white lvl = " + (max - min) * 0.85); 

        terrainData.SetAlphamaps(0, 0, splatmapData);
    }
}
