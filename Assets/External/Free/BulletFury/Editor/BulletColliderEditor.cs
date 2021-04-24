using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BulletFury.Editor
{
    [CustomEditor(typeof(BulletCollider))]
    public class BulletColliderEditor : UnityEditor.Editor
    {
        private SerializedProperty _hitByBulletsList;
        private SerializedProperty _radius;
        private SerializedProperty _onCollide;
        private SerializedProperty _shape;
        private SerializedProperty _bounds;
        private SerializedProperty _offset;
        
        private void OnEnable()
        {
            _hitByBulletsList = serializedObject.FindProperty("hitByBullets");
            _radius = serializedObject.FindProperty("radius");
            _onCollide = serializedObject.FindProperty("OnCollide");
            _shape = serializedObject.FindProperty("shape");
            _bounds = serializedObject.FindProperty("size");
            _offset = serializedObject.FindProperty("offset");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var collider = (BulletCollider) target;
                        
            var managers = FindObjectsOfType<BulletManager>();

            EditorGUILayout.LabelField("Bullet managers in scene", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            var idx = 0;
            var isGroup = false;
            foreach (var manager in managers)
            {
                if (idx % 2 == 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    isGroup = true;
                }
                
                if (GUILayout.Button($"{(collider.HitByBullets.Contains(manager) ? "Remove" : "Add")} {manager.name} {(collider.HitByBullets.Contains(manager) ? "from" : "to")} bullets"))
                {
                    if (!collider.HitByBullets.Contains(manager))
                    {
                        _hitByBulletsList.InsertArrayElementAtIndex(_hitByBulletsList.arraySize);
                        _hitByBulletsList.GetArrayElementAtIndex(_hitByBulletsList.arraySize - 1).objectReferenceValue = manager;
                    }
                    else
                    {
                        _hitByBulletsList.DeleteArrayElementAtIndex(collider.HitByBullets.IndexOf(manager));
                    }
                }

                ++idx;
                if (idx % 2 == 0)
                {
                    EditorGUILayout.EndHorizontal();
                    isGroup = false;
                }
            }
            
            if (isGroup)
                EditorGUILayout.EndHorizontal();

            for (int i = 0; i < _hitByBulletsList.arraySize; i++)
            {
                if (_hitByBulletsList.GetArrayElementAtIndex(i).objectReferenceValue == null)
                    _hitByBulletsList.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.PropertyField(_hitByBulletsList);

            EditorGUILayout.PropertyField(_shape);
            EditorGUILayout.PropertyField((ColliderShape) _shape.enumValueIndex == ColliderShape.Sphere
                ? _radius
                : _bounds);
            
            EditorGUILayout.PropertyField(_offset);
            EditorGUILayout.PropertyField(_onCollide);
            serializedObject.ApplyModifiedProperties();
        }
    }
}