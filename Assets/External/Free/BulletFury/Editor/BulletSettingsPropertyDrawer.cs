using BulletFury.Data;
using UnityEditor;
using UnityEngine;

namespace BulletFury.Editor
{
    [CustomPropertyDrawer(typeof(BulletSettings))]
    public class BulletSettingsPropertyDrawer : PropertyDrawer
    {
        private const float height = 18f;
        private int numFields = 5;

        private bool _expandColor;
        private bool _expandRotation;
        private bool _expandSize;
        private bool _expandVelocity;
        private bool _expandForce;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var currentPos = position;
            currentPos.height = height;

            numFields = 0;
            ++numFields;
            EditorGUI.LabelField(currentPos, label, EditorStyles.boldLabel);

            EditorGUI.PropertyField(currentPos, property);
            if (property.objectReferenceValue == null)
            {
                if (GUILayout.Button("Create New"))
                {
                    var obj = ScriptableObject.CreateInstance<BulletSettings>();
                    AssetDatabase.CreateAsset(obj, "Assets/New Bullet Settings.asset");
                    property.objectReferenceValue = obj;
                    EditorGUIUtility.PingObject(obj);
                }
                return;
            }

            //currentPos.y += height;
            //++numFields;
            var serializedObject = new SerializedObject(property.objectReferenceValue);
            serializedObject.Update();
            serializedObject.FindProperty("isExpanded").boolValue = EditorGUI.Foldout(currentPos, serializedObject.FindProperty("isExpanded").boolValue, "");

            if (!serializedObject.FindProperty("isExpanded").boolValue)
            {
                serializedObject.ApplyModifiedProperties();
                property.objectReferenceValue = serializedObject.targetObject;
                return;
            }
            
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
                

            EditorGUI.indentLevel--;
            AddLabel("Visual Settings", ref currentPos);
            EditorGUI.indentLevel++;

            AddRelativeProperty(ref serializedObject, "mesh", ref currentPos);

            AddRelativeProperty(ref serializedObject, "material", ref currentPos);
            
            AddRelativeProperty(ref serializedObject, "plane", ref currentPos);

            AddRelativeProperty(ref serializedObject, "startColor", ref currentPos);

            AddRelativeProperty(ref serializedObject, "size", ref currentPos);

            EditorGUI.indentLevel--;
            AddLabel("Behaviour", ref currentPos);
            EditorGUI.indentLevel++;

            AddRelativeProperty(ref serializedObject, "waitToStart", ref currentPos);
                
            if (serializedObject.FindProperty("waitToStart").boolValue)
                AddRelativeProperty(ref serializedObject, "timeToPlayWhileWaiting", ref currentPos);

            AddRelativeProperty(ref serializedObject, "lifetime", ref currentPos);

            AddRelativeProperty(ref serializedObject, "speed", ref currentPos);
            
            AddRelativeProperty(ref serializedObject, "inheritSpeedFromTransform", ref currentPos);

            EditorGUI.indentLevel--;
            AddLabel("Time-based Properties", ref currentPos);
            EditorGUI.indentLevel++;
                
            currentPos.y += height;
            ++numFields;
            currentPos.x += height;
            _expandColor = EditorGUI.Foldout(currentPos, _expandColor, "Color Over Lifetime");
            currentPos.x -= height;
                
            var checkboxPos = currentPos;
            checkboxPos.x = position.x - height;
            checkboxPos.width = height * 2;
                
            EditorGUI.PropertyField(checkboxPos, serializedObject.FindProperty("useColorOverTime"), GUIContent.none,
                true);

            var enabled = serializedObject.FindProperty("useColorOverTime").boolValue;
            if (_expandColor)
            {
                EditorGUI.BeginDisabledGroup(!enabled);
                AddRelativeProperty(ref serializedObject, "colorOverTime", ref currentPos);
                EditorGUI.EndDisabledGroup();
            }

            currentPos.y += height;
            ++numFields;
            currentPos.x += height;
            _expandRotation = EditorGUI.Foldout(currentPos, _expandRotation, "Rotation Over Lifetime");
            currentPos.x -= height;
            checkboxPos.y = currentPos.y;
            EditorGUI.PropertyField(checkboxPos, serializedObject.FindProperty("useRotationOverTime"), GUIContent.none,
                true);
            enabled = serializedObject.FindProperty("useRotationOverTime").boolValue;
            if (_expandRotation)
            {
                EditorGUI.BeginDisabledGroup(!enabled);
                AddRelativeProperty(ref serializedObject, "trackObject", ref currentPos);
                if (serializedObject.FindProperty("trackObject").boolValue)
                {
                    currentPos.y += height * 1.25f;
                    ++numFields;
                    serializedObject.FindProperty("trackedObjectTag").stringValue = EditorGUI.TagField(currentPos,
                        serializedObject.FindProperty("trackedObjectTag").stringValue);
                    
                    AddRelativeProperty(ref serializedObject, "turnSpeed" ,ref currentPos);
                }
                else
                {
                    AddRelativeProperty(ref serializedObject, "angularVelocity", ref currentPos);
                    AddRelativeProperty(ref serializedObject, "rotationOverTime", ref currentPos);
                }
                
                
                
                EditorGUI.EndDisabledGroup();
            }

            currentPos.y += height;
            ++numFields;
            currentPos.x += height;
            _expandSize = EditorGUI.Foldout(currentPos, _expandSize, "Size Over Lifetime");
            currentPos.x -= height;
            checkboxPos.y = currentPos.y;
            EditorGUI.PropertyField(checkboxPos, serializedObject.FindProperty("useSizeOverTime"), GUIContent.none,
                true);
            enabled = serializedObject.FindProperty("useSizeOverTime").boolValue;
            if (_expandSize)
            {
                EditorGUI.BeginDisabledGroup(!enabled);
                AddRelativeProperty(ref serializedObject, "sizeOverTime", ref currentPos);
                EditorGUI.EndDisabledGroup();
            }

            currentPos.y += height;
            ++numFields;
            currentPos.x += height;
            _expandVelocity = EditorGUI.Foldout(currentPos, _expandVelocity, "Velocity Over Lifetime");
            currentPos.x -= height;
            checkboxPos.y = currentPos.y;
            EditorGUI.PropertyField(checkboxPos, serializedObject.FindProperty("useVelocityOverTime"), GUIContent.none,
                true);
            enabled = serializedObject.FindProperty("useVelocityOverTime").boolValue;
            if (_expandVelocity)
            {
                EditorGUI.BeginDisabledGroup(!enabled);
                AddRelativeProperty(ref serializedObject, "velocitySpace", ref currentPos);
                AddRelativeProperty(ref serializedObject, "scaleWithSpeed", ref currentPos);

                AddRelativeCurve(ref serializedObject, "velocityOverTimeX", Color.red, ref currentPos);
                AddRelativeCurve(ref serializedObject, "velocityOverTimeY", Color.green, ref currentPos);
                AddRelativeCurve(ref serializedObject, "velocityOverTimeZ", Color.yellow, ref currentPos);

                EditorGUI.EndDisabledGroup();
            }

            currentPos.y += height;
            ++numFields;
            currentPos.x += height;
            _expandForce = EditorGUI.Foldout(currentPos, _expandForce, "Force Over Lifetime");
            currentPos.x -= height;
            checkboxPos.y = currentPos.y;
            EditorGUI.PropertyField(checkboxPos, serializedObject.FindProperty("useForceOverTime"), GUIContent.none,
                true);
            enabled = serializedObject.FindProperty("useForceOverTime").boolValue;
            if (_expandForce)
            {
                EditorGUI.BeginDisabledGroup(!enabled);
                AddRelativeProperty(ref serializedObject, "forceSpace", ref currentPos);

                AddRelativeCurve(ref serializedObject, "forceOverTimeX", Color.red, ref currentPos);
                AddRelativeCurve(ref serializedObject, "forceOverTimeY", Color.green, ref currentPos);
                AddRelativeCurve(ref serializedObject, "forceOverTimeZ", Color.yellow, ref currentPos);

                EditorGUI.EndDisabledGroup();
            }

            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
            property.objectReferenceValue = serializedObject.targetObject;
        }

        private void AddRelativeProperty(ref SerializedObject property, string name, ref Rect currentPos)
        {
            currentPos.y += height * 1.25f;
            ++numFields;
            EditorGUI.PropertyField(currentPos, property.FindProperty(name), true);
        }

        private void AddRelativeCurve(ref SerializedObject property, string name, Color color, ref Rect currentPos)
        {
            currentPos.y += height * 1.25f;
            ++numFields;
            EditorGUI.CurveField(currentPos, property.FindProperty(name), color, new Rect(0, -1, 1, 2));
        }

        private void AddLabel(string label, ref Rect currentPos)
        {
            currentPos.y += height * 1.25f;
            ++numFields;
            EditorGUI.LabelField(currentPos, label, EditorStyles.boldLabel);
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return height * numFields * 1.25f;
        }
    }
}