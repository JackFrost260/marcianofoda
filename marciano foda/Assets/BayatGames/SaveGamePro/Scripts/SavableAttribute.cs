using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BayatGames.SaveGamePro
{

    /// <summary>
    /// Mark a field or property as savable.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Field,
        Inherited = false,
        AllowMultiple = false)]
    public sealed class SavableAttribute : Attribute
    {

        public SavableAttribute()
        {
        }

    }

}