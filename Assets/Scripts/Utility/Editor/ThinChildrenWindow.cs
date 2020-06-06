using System;
using UnityEditor;
using UnityEngine;

namespace Utility.Editor
{
    public class ThinChildrenWindow : EditorWindow
    {
        private GameObject _selectedObject;
        private int _childCount;

        [MenuItem("Window/Tools/Children Thinner")]
        public static void Initialize()
        {
            var window = GetWindow<ThinChildrenWindow>();
            window.titleContent = new GUIContent("Thin Children");
            window.Show();
        }

        private void OnEnable()
        {
            
        }

        private void OnSelectionChange()
        {
            
        }

        private void OnGUI()
        {
            
        }
    }
}
