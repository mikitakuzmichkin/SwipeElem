using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class DoubleArrayForEditor<T> 
{
    public delegate T ToFieldDelegate(T value, params GUILayoutOption[] options);

    private Dictionary<Type, ToFieldDelegate> _types;
    private Dictionary<Type, ToFieldDelegate> TypesToField
    {
        get
        {
            if (_types == null)
            {
                _types = new Dictionary<Type, ToFieldDelegate>();
                _types.Add(typeof(int), (value, options) => (T) (object) EditorGUILayout.IntField((int) (object) (value), options));
                _types.Add(typeof(string), (value, options) => (T) (object) EditorGUILayout.TextField((string) (object) (value)));
                _types.Add(typeof(float), (value, options) => (T) (object) EditorGUILayout.FloatField((float) (object) (value)));
                _types.Add(typeof(bool), (value, options) => (T) (object) EditorGUILayout.Toggle((bool) (object) (value)));
            }
    
            return _types; 
        }
    }

    public void CreateDoubleArray(T[,] array, int rows, int columns, ref bool showBoard, string boardName = "Board", bool reverseArray = true)
    {
        showBoard = EditorGUILayout.Foldout(showBoard, boardName);
        if (showBoard)
        {

            EditorGUI.indentLevel = 0;

            GUIStyle tableStyle = new GUIStyle("box");
            tableStyle.padding = new RectOffset(10, 10, 10, 10);
            tableStyle.margin.left = 32;

            GUIStyle headerColumnStyle = new GUIStyle();
            headerColumnStyle.fixedWidth = 35;

            GUIStyle columnStyle = new GUIStyle();
            GUILayoutOption[] options = null;
            if (typeof(T) == typeof(bool))
            {
                columnStyle.fixedWidth = 25;
                options = new[] {GUILayout.Width(25)};
            }
            else
            {
                columnStyle.fixedWidth = 65;
            }

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
                        if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
                        {
                            array[i, j] = (T)(object)ToFieldClass(array[i, j] as UnityEngine.Object);
                        }
                        else
                        {
                            array[i, j] = ToFieldStruct(array[i, j], options);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndHorizontal();
            

        }
    }
    
     public void CreateDoubleArray(T[] array, int rows, int columns, ref bool showBoard, string boardName = "Board", bool reverseArray = true)
    {
        showBoard = EditorGUILayout.Foldout(showBoard, boardName);
        if (showBoard)
        {

            EditorGUI.indentLevel = 0;

            GUIStyle tableStyle = new GUIStyle("box");
            tableStyle.padding = new RectOffset(10, 10, 10, 10);
            tableStyle.margin.left = 32;

            GUIStyle headerColumnStyle = new GUIStyle();
            headerColumnStyle.fixedWidth = 35;

            GUIStyle columnStyle = new GUIStyle();
            GUILayoutOption[] options = null;
            if (typeof(T) == typeof(bool))
            {
                columnStyle.fixedWidth = 25;
                options = new[] {GUILayout.Width(25)};
            }
            else
            {
                columnStyle.fixedWidth = 65;
            }

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

            int ind;
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

                    if (x >= 0 && y >= 0)
                    {
                        EditorGUILayout.BeginHorizontal(rowStyle);
                        int i, j;
                        if (reverseArray)
                        {
                            ind = x * columns + y;
                        }
                        else
                        {
                            ind = y * columns + x;
                        }
                        if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
                        {
                            array[ind] = (T)(object)ToFieldClass(array[ind] as UnityEngine.Object);
                        }
                        else
                        {
                            array[ind] = ToFieldStruct(array[ind], options);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    
                    
                    
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndHorizontal();
            

        }
    }

    public S ToFieldClass<S>(S value) where S: UnityEngine.Object
    {
        return (S)EditorGUILayout.ObjectField(value, typeof(T), true);
    }
    
    public T ToFieldStruct(T value, params GUILayoutOption[] options)
    {
        return TypesToField[typeof(T)](value, options);
    }


}
