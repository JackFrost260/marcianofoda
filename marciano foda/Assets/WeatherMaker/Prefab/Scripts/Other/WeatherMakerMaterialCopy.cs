using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    public class WeatherMakerMaterialCopy : IDisposable
    {
        private Material source;
        private Material copy;

        public void Update(Material source)
        {
            if (source != this.source)
            {
                Dispose();
                if (source == null)
                {
                    copy = null;
                }
                else
                {
                    copy = new Material(source);
                    copy.name += " (Clone)";
                }
                this.source = source;
            }
        }

        /// <summary>
        /// Dispose of copy, can be called multiple times
        /// </summary>
        public void Dispose()
        {
            if (copy != null)
            {
                GameObject.DestroyImmediate(copy);
                copy = null;
            }
            source = null;
        }

        public static implicit operator Material(WeatherMakerMaterialCopy copy)
        {
            return copy.copy;
        }

        public Material Original
        {
            get { return source; }
        }

        public Material Copy
        {
            get { return copy; }
        }
    }
}
