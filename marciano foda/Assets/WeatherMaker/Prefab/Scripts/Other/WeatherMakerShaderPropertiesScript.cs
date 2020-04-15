using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    public class WeatherMakerShaderPropertiesScript
    {
        private static readonly object emptyObject = new object();
        private object lastMaterial = emptyObject;
        private Material mat;
        private MaterialPropertyBlock block;
        private ComputeShader compute;

        private readonly System.Action<int, int> setIntMaterial;
        private readonly System.Action<int, float> setFloatMaterial;
        private readonly System.Action<int, float[]> setFloatArrayMaterial;
        private readonly System.Action<int, Vector4> setVectorMaterial;
        private readonly System.Action<int, Vector4[]> setVectorArrayMaterial;
        private readonly System.Action<int, Color> setColorMaterial;
        private readonly System.Action<int, Color[]> setColorArrayMaterial;
        private readonly System.Action<int, Texture> setTextureMaterial;

        private readonly System.Action<int, int> setIntBlock;
        private readonly System.Action<int, float> setFloatBlock;
        private readonly System.Action<int, float[]> setFloatArrayBlock;
        private readonly System.Action<int, Vector4> setVectorBlock;
        private readonly System.Action<int, Vector4[]> setVectorArrayBlock;
        private readonly System.Action<int, Color> setColorBlock;
        private readonly System.Action<int, Color[]> setColorArrayBlock;
        private readonly System.Action<int, Texture> setTextureBlock;

        private readonly System.Action<int, int> setIntCompute;
        private readonly System.Action<int, float> setFloatCompute;
        private readonly System.Action<int, float[]> setFloatArrayCompute;
        private readonly System.Action<int, Vector4> setVectorCompute;
        private readonly System.Action<int, Vector4[]> setVectorArrayCompute;
        private readonly System.Action<int, Color> setColorCompute;
        private readonly System.Action<int, Color[]> setColorArrayCompute;
        private readonly System.Action<int, Texture> setTextureCompute;

        private readonly System.Action<int, int> setIntGlobal;
        private readonly System.Action<int, float> setFloatGlobal;
        private readonly System.Action<int, float[]> setFloatArrayGlobal;
        private readonly System.Action<int, Vector4> setVectorGlobal;
        private readonly System.Action<int, Vector4[]> setVectorArrayGlobal;
        private readonly System.Action<int, Color> setColorGlobal;
        private readonly System.Action<int, Color[]> setColorArrayGlobal;
        private readonly System.Action<int, Texture> setTextureGlobal;

        private System.Action<int, int> setInt;
        private System.Action<int, float> setFloat;
        private System.Action<int, float[]> setFloatArray;
        private System.Action<int, Vector4> setVector;
        private System.Action<int, Vector4[]> setVectorArray;
        private System.Action<int, Color> setColor;
        private System.Action<int, Color[]> setColorArray;
        private System.Action<int, Texture> setTexture;

        private static readonly Vector4[] tmpVectorArray = new Vector4[4];
        private static readonly float[] tmpFloatArray = new float[4];
        private static readonly float[] tmpFloatArray2 = new float[8];

        private void SetIntMaterial(int id, int value)
        {
            mat.SetInt(id, value);
        }

        private void SetFloatMaterial(int id, float value)
        {
            mat.SetFloat(id, value);
        }

        private void SetFloatArrayMaterial(int id, float[] value)
        {
            mat.SetFloatArray(id, value);
        }

        private void SetVectorMaterial(int id, Vector4 value)
        {
            mat.SetVector(id, value);
        }

        private void SetVectorArrayMaterial(int id, Vector4[] value)
        {
            mat.SetVectorArray(id, value);
        }

        private void SetColorMaterial(int id, Color value)
        {
            mat.SetColor(id, value);
        }

        private void SetColorArrayMaterial(int id, Color[] value)
        {
            mat.SetColorArray(id, value);
        }

        private void SetTextureMaterial(int id, Texture value)
        {
            mat.SetTexture(id, value);
        }

        private void SetIntBlock(int id, int value)
        {

#if UNITY_2019_1_OR_NEWER

            block.SetInt(id, value);

#else

            block.SetFloat(id, value);

#endif

        }

        private void SetFloatBlock(int id, float value)
        {
            block.SetFloat(id, value);
        }

        private void SetFloatArrayBlock(int id, float[] value)
        {
            block.SetFloatArray(id, value);
        }

        private void SetVectorBlock(int id, Vector4 value)
        {
            block.SetVector(id, value);
        }

        private void SetVectorArrayBlock(int id, Vector4[] value)
        {
            block.SetVectorArray(id, value);
        }

        private void SetColorBlock(int id, Color value)
        {
            block.SetColor(id, value);
        }

        private void SetColorArrayBlock(int id, Color[] value)
        {
            for (int i = 0; i < value.Length && i < tmpVectorArray.Length; i++)
            {
                tmpVectorArray[i] = value[i];
            }
            block.SetVectorArray(id, tmpVectorArray);
        }

        private void SetTextureBlock(int id, Texture value)
        {
            block.SetTexture(id, value);
        }

        private void SetIntCompute(int id, int value)
        {
            compute.SetInt(id, value);
        }

        private void SetFloatCompute(int id, float value)
        {
            compute.SetFloat(id, value);
        }

        private void SetFloatArrayCompute(int id, float[] value)
        {
            compute.SetFloats(id, value);
        }

        private void SetVectorCompute(int id, Vector4 value)
        {
            compute.SetVector(id, value);
        }

        private void SetVectorArrayCompute(int id, Vector4[] value)
        {
            compute.SetVectorArray(id, value);
        }

        private void SetColorCompute(int id, Color value)
        {
            compute.SetVector(id, value);
        }

        private void SetColorArrayCompute(int id, Color[] value)
        {
            for (int i = 0; i < value.Length && i < tmpVectorArray.Length; i++)
            {
                tmpVectorArray[i] = value[i];
            }
            compute.SetVectorArray(id, tmpVectorArray);
        }

        private void SetTextureCompute(int id, Texture value)
        {
            // null ref exception but only for compute shaders... weird.
            if (value != null)
            {
                compute.SetTexture(0, id, value);
            }
        }

        private void SetIntGlobal(int id, int value)
        {
            Shader.SetGlobalInt(id, value);
        }

        private void SetFloatGlobal(int id, float value)
        {
            Shader.SetGlobalFloat(id, value);
        }

        private void SetFloatArrayGlobal(int id, float[] value)
        {
            Shader.SetGlobalFloatArray(id, value);
        }

        private void SetVectorGlobal(int id, Vector4 value)
        {
            Shader.SetGlobalVector(id, value);
        }

        private void SetVectorArrayGlobal(int id, Vector4[] value)
        {
            Shader.SetGlobalVectorArray(id, value);
        }

        private void SetColorGlobal(int id, Color value)
        {
            Shader.SetGlobalColor(id, value);
        }

        private void SetColorArrayGlobal(int id, Color[] value)
        {
            for (int i = 0; i < value.Length && i < tmpVectorArray.Length; i++)
            {
                tmpVectorArray[i] = value[i];
            }
            Shader.SetGlobalVectorArray(id, tmpVectorArray);
        }

        private void SetTextureGlobal(int id, Texture value)
        {
            Shader.SetGlobalTexture(id, value);
        }

        public static readonly WeatherMakerShaderPropertiesScript Global = new WeatherMakerShaderPropertiesScript();

        public WeatherMakerShaderPropertiesScript(object material = null)
        {
            setIntMaterial = SetIntMaterial;
            setFloatMaterial = SetFloatMaterial;
            setFloatArrayMaterial = SetFloatArrayMaterial;
            setVectorMaterial = SetVectorMaterial;
            setVectorArrayMaterial = SetVectorArrayMaterial;
            setColorMaterial = SetColorMaterial;
            setColorArrayMaterial = SetColorArrayMaterial;
            setTextureMaterial = SetTextureMaterial;

            setIntBlock = SetIntBlock;
            setFloatBlock = SetFloatBlock;
            setFloatArrayBlock = SetFloatArrayBlock;
            setVectorBlock = SetVectorBlock;
            setVectorArrayBlock = SetVectorArrayBlock;
            setColorBlock = SetColorBlock;
            setColorArrayBlock = SetColorArrayBlock;
            setTextureBlock = SetTextureBlock;

            setIntCompute = SetIntCompute;
            setFloatCompute = SetFloatCompute;
            setFloatArrayCompute = SetFloatArrayCompute;
            setVectorCompute = SetVectorCompute;
            setVectorArrayCompute = SetVectorArrayCompute;
            setColorCompute = SetColorCompute;
            setColorArrayCompute = SetColorArrayCompute;
            setTextureCompute = SetTextureCompute;

            setIntGlobal = SetIntGlobal;
            setFloatGlobal = SetFloatGlobal;
            setFloatArrayGlobal = SetFloatArrayGlobal;
            setVectorGlobal = SetVectorGlobal;
            setVectorArrayGlobal = SetVectorArrayGlobal;
            setColorGlobal = SetColorGlobal;
            setColorArrayGlobal = SetColorArrayGlobal;
            setTextureGlobal = SetTextureGlobal;

            Update(material);
        }

        public void Update(object material)
        {
            // If we have already initialized with this same material, don't re-create the actions
            if (material == lastMaterial)
            {
                return;
            }

            mat = material as Material;
            block = material as MaterialPropertyBlock;
            compute = material as ComputeShader;
            if (mat == null && block == null && compute == null && material != null)
            {
                Debug.LogError("Invalid material parameter, must be Material or MaterialPropertyBlock or ComputeShader or null");
                return;
            }
            lastMaterial = material;

            if (mat != null)
            {
                setInt = setIntMaterial;
                setFloat = setFloatMaterial;
                setFloatArray = setFloatArrayMaterial;
                setVector = setVectorMaterial;
                setVectorArray = setVectorArrayMaterial;
                setColor = setColorMaterial;
                setColorArray = setColorArrayMaterial;
                setTexture = setTextureMaterial;
            }
            else if (block != null)
            {
                setInt = setIntBlock;
                setFloat = setFloatBlock;
                setFloatArray = setFloatArrayBlock;
                setVector = setVectorBlock;
                setVectorArray = setVectorArrayBlock;
                setColor = setColorBlock;
                setColorArray = setColorArrayBlock;
                setTexture = setTextureBlock;
            }
            else if (compute != null)
            {
                setInt = setIntCompute;
                setFloat = setFloatCompute;
                setFloatArray = setFloatArrayCompute;
                setVector = setVectorCompute;
                setVectorArray = setVectorArrayCompute;
                setColor = setColorCompute;
                setColorArray = setColorArrayCompute;
                setTexture = setTextureCompute;
            }
            else
            {
                setInt = setIntGlobal;
                setFloat = setFloatGlobal;
                setFloatArray = setFloatArrayGlobal;
                setVector = setVectorGlobal;
                setVectorArray = setVectorArrayGlobal;
                setColor = setColorGlobal;
                setColorArray = setColorArrayGlobal;
                setTexture = setTextureGlobal;
            }
        }

        public void SetInt(int name, int value)
        {
            setInt(name, value);
        }

        public void SetFloat(int name, float value)
        {
            setFloat(name, value);
        }

        public void SetFloatArray(int name, float[] value)
        {
            setFloatArray(name, value);
        }

        public void SetVector(int name, Vector4 value)
        {
            setVector(name, value);
        }

        public void SetVectorArray(int name, Vector4[] value)
        {
            setVectorArray(name, value);
        }

        public void SetColorArray(int name, Color[] value)
        {
            setColorArray(name, value);
        }

        public void SetTexture(int name, Texture value)
        {
            setTexture(name, value);
        }

        public void SetColor(int name, Color c)
        {
            setColor(name, c);
        }

        public void SetColorArray(int name, Color c1, Color c2, Color c3, Color c4)
        {
            tmpVectorArray[0] = c1;
            tmpVectorArray[1] = c2;
            tmpVectorArray[2] = c3;
            tmpVectorArray[3] = c4;
            SetVectorArray(name, tmpVectorArray);
        }

        public void SetFloatArray(int name, float f1, float f2, float f3, float f4)
        {
            tmpFloatArray[0] = f1;
            tmpFloatArray[1] = f2;
            tmpFloatArray[2] = f3;
            tmpFloatArray[3] = f4;
            SetFloatArray(name, tmpFloatArray);
        }

        public void SetFloatArrayRotation(int name, float f1, float f2, float f3, float f4)
        {
            tmpFloatArray2[0] = Mathf.Cos(f1 * Mathf.Deg2Rad);
            tmpFloatArray2[1] = Mathf.Cos(f2 * Mathf.Deg2Rad);
            tmpFloatArray2[2] = Mathf.Cos(f3 * Mathf.Deg2Rad);
            tmpFloatArray2[3] = Mathf.Cos(f4 * Mathf.Deg2Rad);
            tmpFloatArray2[4] = Mathf.Sin(f1 * Mathf.Deg2Rad);
            tmpFloatArray2[5] = Mathf.Sin(f2 * Mathf.Deg2Rad);
            tmpFloatArray2[6] = Mathf.Sin(f3 * Mathf.Deg2Rad);
            tmpFloatArray2[7] = Mathf.Sin(f4 * Mathf.Deg2Rad);
            SetFloatArray(name, tmpFloatArray2);
        }

        public void SetVectorArray(int name, Vector4 v1, Vector4 v2, Vector4 v3, Vector4 v4)
        {
            tmpVectorArray[0] = v1;
            tmpVectorArray[1] = v2;
            tmpVectorArray[2] = v3;
            tmpVectorArray[3] = v4;
            SetVectorArray(name, tmpVectorArray);
        }
    }
}
