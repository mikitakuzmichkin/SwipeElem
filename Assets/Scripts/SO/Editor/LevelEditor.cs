using System;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

namespace SO.Editor
{
    [CustomEditor(typeof(Level))]
    public class LevelEditor : UnityEditor.Editor
    {
        private Level _level;
        private bool _showBoard;
        
        public void OnEnable()
        {
            _level = (Level)target;
            if (_level.IsMapNull() && _level.Rows * _level.Columns != 0)
            {
                _level.CreateMap();
            }
        }
        
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            _level.Rows = EditorGUILayout.IntField("Rows",_level.Rows);
            _level.Columns = EditorGUILayout.IntField("Columns",_level.Columns);
            if (EditorGUI.EndChangeCheck())
            {
                if(_level.Rows * _level.Columns != 0)
                {
                    _level.ResizeMap();
                }
            }
            CreateDoubleArray( _level.Rows, _level.Columns, ref _showBoard, "Level", false);
            EditorUtility.SetDirty(_level);
            base.OnInspectorGUI();
        }
        
        public void CreateDoubleArray(int rows, int columns, ref bool showBoard, string boardName = "Board", bool reverseArray = true)
        {
            showBoard = EditorGUILayout.Foldout(showBoard, boardName);
            if (showBoard && (rows * columns != 0))
            {

                EditorGUI.indentLevel = 0;

                GUIStyle tableStyle = new GUIStyle("box");
                tableStyle.padding = new RectOffset(10, 10, 10, 10);
                tableStyle.margin.left = 32;

                GUIStyle headerColumnStyle = new GUIStyle();
                headerColumnStyle.fixedWidth = 35;

                GUIStyle columnStyle = new GUIStyle();
                GUILayoutOption[] options = null;
                // if (typeof(T) == typeof(bool))
                // {
                    columnStyle.fixedWidth = 25;
                    options = new[] {GUILayout.Width(25)};
                // }
                // else
                // {
                //     columnStyle.fixedWidth = 65;
                // }

                GUIStyle rowStyle = new GUIStyle();
                rowStyle.fixedHeight = 25;

                GUIStyle rowHeaderStyle = new GUIStyle();
                rowHeaderStyle.fixedWidth = columnStyle.fixedWidth - 1;

                GUIStyle columnHeaderStyle = new GUIStyle();
                columnHeaderStyle.fixedWidth = 30;
                columnHeaderStyle.fixedHeight = 25.5f;

                GUIStyle columnLabelStyle = new GUIStyle();
                columnLabelStyle.fixedWidth = rowHeaderStyle.fixedWidth - 6;
                columnLabelStyle.alignment = TextAnchor.MiddleCenter;
                columnLabelStyle.fontStyle = FontStyle.Bold;

                GUIStyle cornerLabelStyle = new GUIStyle();
                cornerLabelStyle.fixedWidth = 42;
                cornerLabelStyle.alignment = TextAnchor.MiddleRight;
                cornerLabelStyle.fontStyle = FontStyle.BoldAndItalic;
                cornerLabelStyle.fontSize = 14;
                cornerLabelStyle.padding.top = -5;

                GUIStyle rowLabelStyle = new GUIStyle();
                rowLabelStyle.fixedWidth = 25;
                rowLabelStyle.alignment = TextAnchor.MiddleRight;
                rowLabelStyle.fontStyle = FontStyle.Bold;

                GUIStyle enumStyle = new GUIStyle("popup");

                EditorGUILayout.BeginHorizontal(tableStyle);
                for (int x = -1; x < (reverseArray ? rows : columns); x++)
                {
                    EditorGUILayout.BeginVertical((x == -1) ? headerColumnStyle : columnStyle);
                    for (int y = -1; y < (reverseArray ? columns : rows); y++)
                    {
                        if (x == -1 && y == -1)
                        {
                            EditorGUILayout.BeginVertical(rowHeaderStyle);
                            EditorGUILayout.LabelField("[X,Y]", cornerLabelStyle);
                            EditorGUILayout.EndHorizontal();
                        }
                        else if (x == -1)
                        {
                            EditorGUILayout.BeginVertical(columnHeaderStyle);
                            EditorGUILayout.LabelField(y.ToString(), rowLabelStyle);
                            EditorGUILayout.EndHorizontal();
                        }
                        else if (y == -1)
                        {
                            EditorGUILayout.BeginVertical(rowHeaderStyle);
                            EditorGUILayout.LabelField(x.ToString(), columnLabelStyle);
                            EditorGUILayout.EndHorizontal();
                        }

                        Color originalBackgroundColor = GUI.backgroundColor;
                        if (x >= 0 && y >= 0)
                        {
                            EditorGUILayout.BeginHorizontal(rowStyle);
                            int i, j;
                            if (reverseArray)
                            {
                                i = x;
                                j = y;
                            }
                            else
                            {
                                i = y;
                                j = x;
                            }

                            GUI.backgroundColor = _level._Colors[_level[i, j]];
                            if (GUILayout.Button(""))
                            {
                                _level[i, j]++;
                                if (_level[i, j] >= _level._Colors.Length)
                                {
                                    _level[i, j] = 0;
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }

                        GUI.backgroundColor = originalBackgroundColor;
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}