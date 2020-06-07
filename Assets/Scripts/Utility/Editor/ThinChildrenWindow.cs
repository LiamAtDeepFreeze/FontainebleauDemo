using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Utility.Editor
{
    public class ThinChildrenWindow : EditorWindow
    {
        private GameObject _selectedObject;
        private int _minChildCount = 10;
        private float _reductionMultiplier = 0.25f;

        private int _childCount;
        private int _finalChildCount;
        private List<RegisteredChild> allChildren = new List<RegisteredChild>();

        private Vector2 _scrollPosChildList = Vector2.zero;

        [MenuItem("Tools/Children Thinner")]
        public static void Initialize()
        {
            var window = GetWindow<ThinChildrenWindow>();
            window.titleContent = new GUIContent("Thin Children");
            window.Show();
        }

        private void OnEnable()
        {
            if (_selectedObject == null 
                && Selection.activeGameObject != null 
                && Selection.activeGameObject.transform.childCount > _minChildCount)
            {
                _selectedObject = Selection.activeGameObject;
                RefreshState();
            }
        }

        private void OnSelectionChange()
        {
            if (Selection.activeGameObject == null)
            {
                return;
            }
            
            //Only update the state for large parents
            if (_selectedObject != Selection.activeGameObject && Selection.activeGameObject.transform.childCount > _minChildCount)
            {
                RestoreState();  
                ClearSelection();
                _selectedObject = Selection.activeGameObject;
                Refresh();
            }
        }

        private void OnGUI()
        {
            if (_selectedObject != null)
            {
                if (_childCount != _selectedObject.transform.childCount)
                {
                    Refresh();
                }
                
                GUILayout.Label("Active Selection", EditorStyles.boldLabel);
                GUILayout.Label(_selectedObject.name);
                
                GUILayout.Space(9f);
                
                GUILayout.Label("Reduction Multiplier", EditorStyles.boldLabel);
                EditorGUI.BeginChangeCheck();
                _reductionMultiplier = EditorGUILayout.Slider(_reductionMultiplier, 0.001f, 1f);
                if (EditorGUI.EndChangeCheck())
                {
                    RefreshState();
                }

                GUILayout.Space(9f);
                
                GUILayout.Label($"Child Count: {_childCount.ToString()} -> {_finalChildCount.ToString()}", EditorStyles.boldLabel);
                
                GUILayout.Space(9f);
                
                DrawChildList();
                
                GUILayout.Space(9f);
                
                GUILayout.FlexibleSpace();
                DrawFooter();
            }
        }

        private void DrawChildList()
        {
            GUILayout.Label("Child List", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            _scrollPosChildList = EditorGUILayout.BeginScrollView(_scrollPosChildList);
            foreach (var registeredChild in allChildren)
            {
                DrawChild(registeredChild);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawChild(RegisteredChild child)
        {
            EditorGUILayout.BeginHorizontal("box");
            GUI.enabled = child.GameObject.activeSelf;
            EditorGUILayout.LabelField(child.Name);
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        private void DrawFooter()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh"))
            {
                Refresh();
            }
            if (GUILayout.Button("Reduce"))
            {
                Reduce();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void Refresh()
        {
            if (_selectedObject == null)
            {
                _selectedObject = Selection.activeGameObject;
            }

            if (_selectedObject == null)
            {
                return;
            }
            
            //Reset the collection
            allChildren.Clear();

            //Add all of the current children to the collection
            _childCount = _selectedObject.transform.childCount;
            for (var i = 0; i < _childCount; i++)
            {
                allChildren.Add(new RegisteredChild(_selectedObject.transform.GetChild(i).gameObject));
            }
            
            //Refresh the override states of the objects
            RefreshState();
        }

        private void RefreshState()
        {
            if (allChildren.Count == 0)
            {
                RefreshState();
            }
            
            _finalChildCount = Mathf.RoundToInt(_childCount * _reductionMultiplier);
            var skipCount = _childCount / _finalChildCount;
            var currentCount = 0;
            for (var i = 0; i < allChildren.Count; i++)
            {
                if (currentCount < skipCount)
                {
                    allChildren[i].GameObject.SetActive(false);
                    currentCount++;
                    continue;
                }

                allChildren[i].GameObject.SetActive(true);
                currentCount = 0;
            }
        }

        private void RestoreState()
        {
            foreach (var child in allChildren)
            {
                //This is being dumb so force-setting active
                //child.GameObject.SetActive(child.OriginalState);
                
                child.GameObject.SetActive(true);
            }
            
            allChildren.Clear();
        }

        private void Reduce()
        {
            if (_selectedObject == null)
            {
                return;
            }

            //Cache the child list before deletion
            var cachedList = allChildren;
            
            //Delete all children that are deactivated and their original state was active
            for (var i = 0; i < cachedList.Count; i++)
            {
                if (cachedList[i].GameObject.activeSelf == false && cachedList[i].OriginalState)
                {
                    DestroyImmediate(allChildren[i].GameObject);
                    allChildren.RemoveAt(i);
                }
            }

            foreach (var child in allChildren)
            {
                child.GameObject.SetActive(child.OriginalState);
            }

            //Deselect the active object so that the state doesn't refresh
            ClearSelection();
        }

        private void ClearSelection()
        {
            allChildren.Clear();
            Selection.activeGameObject = null;
        }

        [Serializable]
        private class RegisteredChild
        {
            public string Name => GameObject.name;
            public GameObject GameObject;
            public bool OriginalState;

            public RegisteredChild(GameObject gameObject)
            {
                GameObject = gameObject;
                OriginalState = gameObject.activeSelf;
            }
        }
    }
}
