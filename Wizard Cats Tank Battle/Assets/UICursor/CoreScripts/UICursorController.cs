using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cooldhands
{
    public interface IUICursor
    {
        Vector2 CursorPosition { get; set; }
    }
    public class UICursorController
    {
        /// <summary>
        /// <para>Return the current UICursor.</para>
        /// </summary>
        public static IUICursor current { get; set; }
    }
}
