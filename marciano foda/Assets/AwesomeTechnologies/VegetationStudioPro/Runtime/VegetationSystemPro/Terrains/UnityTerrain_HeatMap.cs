using AwesomeTechnologies.Utility;
using UnityEngine;

namespace AwesomeTechnologies.VegetationSystem
{
    public partial class UnityTerrain
    {


        private void SetupHeatmapMaterial()
        {
            TerrainHeatmapMaterial = Instantiate(Resources.Load("TerrainHeatmap") as Material);
        }
        public void OverrideTerrainMaterial()
        {
            if (!Terrain) return;
            if (TerrainHeatmapMaterial == null) SetupHeatmapMaterial();

            if (!TerrainMaterialOverridden)
            {
                _originalTerrainMaterialType = Terrain.materialType;
                _originalTerrainMaterial = Terrain.materialTemplate;
                _originalTerrainheightmapPixelError = Terrain.heightmapPixelError;
                _originalBasemapDistance = Terrain.basemapDistance;
                    
#if UNITY_2018_3_OR_NEWER
                _originalTerrainInstanced = Terrain.drawInstanced;
                Terrain.drawInstanced = false;
#endif                
                TerrainMaterialOverridden = true;
            }

            Terrain.materialType = Terrain.MaterialType.Custom;
            Terrain.materialTemplate = TerrainHeatmapMaterial;
            Terrain.basemapDistance = 0;
            Terrain.heightmapPixelError = 1;
        }

        public void RestoreTerrainMaterial()
        {
            if (!Terrain || !TerrainMaterialOverridden) return;
            Terrain.materialType = _originalTerrainMaterialType;
            Terrain.materialTemplate = _originalTerrainMaterial;
            Terrain.heightmapPixelError = _originalTerrainheightmapPixelError;
            Terrain.basemapDistance = _originalBasemapDistance;
#if UNITY_2018_3_OR_NEWER
            Terrain.drawInstanced = _originalTerrainInstanced;
#endif        
            TerrainMaterialOverridden = false;
        }

        public void UpdateTerrainMaterial(float worldspaceSeaLevel, float worldspaceMaxTerrainHeight, TerrainTextureSettings terrainTextureSettings)
        {
            if (!TerrainHeatmapMaterial) return;
            TerrainHeatmapMaterial.SetFloat("_TerrainMinHeight", worldspaceSeaLevel);
            TerrainHeatmapMaterial.SetFloat("_TerrainMaxHeight", worldspaceMaxTerrainHeight);
            TerrainHeatmapMaterial.SetFloat("_MinHeight", 0);
            TerrainHeatmapMaterial.SetFloat("_MaxHeight", 0);
            TerrainHeatmapMaterial.SetFloat("_MinSteepness", 0);
            TerrainHeatmapMaterial.SetFloat("_MaxSteepness", 90);
            TerrainHeatmapMaterial.SetTexture("_CurveTexture", new Texture2D(1, 1));
            TerrainHeatmapMaterial.SetFloatArray("_HeightCurve", terrainTextureSettings.TextureHeightCurve.GenerateCurveArray(256));
            TerrainHeatmapMaterial.SetFloatArray("_SteepnessCurve", terrainTextureSettings.TextureSteepnessCurve.GenerateCurveArray(256));
            TerrainHeatmapMaterial.SetFloat("_UseNoise", terrainTextureSettings.UseNoise ? 1 : 0);
            TerrainHeatmapMaterial.SetFloat("_InverseNoise", terrainTextureSettings.InverseNoise ? 1 : 0);
            TerrainHeatmapMaterial.SetFloat("_NoiseScale", terrainTextureSettings.NoiseScale);
            TerrainHeatmapMaterial.SetVector("_NoiseOffset", new Vector4(terrainTextureSettings.NoiseOffset.x,0,terrainTextureSettings.NoiseOffset.y,0));
        }
    }
}
