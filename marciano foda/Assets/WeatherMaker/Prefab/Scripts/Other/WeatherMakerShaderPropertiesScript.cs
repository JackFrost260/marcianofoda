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

        private System.Action<int, int> setInt;
        private System.Action<int, float> setFloat;
        private System.Action<int, Vector4> setVector;
        private System.Action<int, Vector4[]> setVectorArray;
        private System.Action<int, Texture> setTexture;

        public static readonly WeatherMakerShaderPropertiesScript Global = new WeatherMakerShaderPropertiesScript();

        public WeatherMakerShaderPropertiesScript(object material = null)
        {
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
                setInt = mat.SetInt;
                setFloat = mat.SetFloat;
                setVector = mat.SetVector;
                setVectorArray = mat.SetVectorArray;
                setTexture = mat.SetTexture;
            }
            else if (block != null)
            {
                setInt = (name, value) => block.SetFloat(name, value);
                setFloat = block.SetFloat;
                setVector = block.SetVector;
                setVectorArray = block.SetVectorArray;
                setTexture = block.SetTexture;
            }
            else if (compute != null)
            {
                setInt = compute.SetInt;
                setFloat = compute.SetFloat;
                setVector = compute.SetVector;
                setVectorArray = compute.SetVectorArray;
                setTexture = (name, value) => compute.SetTexture(0, name, value);
            }
            else
            {
                setInt = Shader.SetGlobalInt;
                setFloat = Shader.SetGlobalFloat;
                setVector = Shader.SetGlobalVector;
                setVectorArray = Shader.SetGlobalVectorArray;
                setTexture = Shader.SetGlobalTexture;
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

        public void SetVector(int name, Vector4 value)
        {
            setVector(name, value);
        }

        public void SetVectorArray(int name, Vector4[] value)
        {
            setVectorArray(name, value);
        }

        public void SetTexture(int name, Texture value)
        {
            setTexture(name, value);
        }
    }
}
