using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(AutomaticHorizontalSize))]
    public class AutomaticVerticalSizeEditor : UnityEditor.Editor {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        
            if (GUILayout.Button("Recalc Size"))
            {
                ((AutomaticHorizontalSize)target).AdjustSize();
            }
        }
    }
}
