﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DevionGames.CharacterSystem
{
    [System.Serializable]
    public class CharacterSystemInspector
    {
        private CharacterDatabase m_Database;
        private List<ICollectionEditor> m_ChildEditors;

        [SerializeField]
        private int toolbarIndex;

        private string[] toolbarNames
        {
            get
            {
                string[] items = new string[m_ChildEditors.Count];
                for (int i = 0; i < m_ChildEditors.Count; i++)
                {
                    items[i] = m_ChildEditors[i].ToolbarName;
                }
                return items;
            }
        }

        public void OnEnable()
        {
            this.m_Database = AssetDatabase.LoadAssetAtPath<CharacterDatabase>(EditorPrefs.GetString("CharacterDatabasePath"));
            if (this.m_Database == null)
            {
                string[] guids = AssetDatabase.FindAssets("t:CharacterDatabase");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    this.m_Database = AssetDatabase.LoadAssetAtPath<CharacterDatabase>(path);
                }
            }
            ResetChildEditors();

        }

        public void OnDisable()
        {
            if (this.m_Database != null)
            {
                EditorPrefs.SetString("CharacterDatabasePath", AssetDatabase.GetAssetPath(this.m_Database));
            }
            if (m_ChildEditors != null)
            {
                for (int i = 0; i < m_ChildEditors.Count; i++)
                {
                    m_ChildEditors[i].OnDisable();
                }
            }
        }

        public void OnDestroy()
        {
            if (m_ChildEditors != null)
            {
                for (int i = 0; i < m_ChildEditors.Count; i++)
                {
                    m_ChildEditors[i].OnDestroy();
                }
            }
        }

        public void OnGUI(Rect position)
        {

            DoToolbar();

            if (m_ChildEditors != null)
            {
                m_ChildEditors[toolbarIndex].OnGUI(new Rect(0f, 30f, position.width, position.height - 30f));
            }
        }

        private void DoToolbar()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            SelectDatabaseButton();

            if (this.m_ChildEditors != null)
                toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarNames, GUILayout.MinWidth(200));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void SelectDatabaseButton()
        {
            GUIStyle buttonStyle = EditorStyles.objectField;
            GUIContent buttonContent = new GUIContent(this.m_Database != null ? this.m_Database.name : "Null");
            Rect buttonRect = GUILayoutUtility.GetRect(180f, 18f);
            if (GUI.Button(buttonRect, buttonContent, buttonStyle))
            {
                ObjectPickerWindow.ShowWindow(buttonRect, typeof(CharacterDatabase),
                    (UnityEngine.Object obj) => {
                        this.m_Database = obj as CharacterDatabase;
                        ResetChildEditors();
                    },
                    () => {
                        CharacterDatabase db = EditorTools.CreateAsset<CharacterDatabase>(true);
                        if (db != null)
                        {
                            this.m_Database = db;
                            ResetChildEditors();
                        }
                    });
            }
        }

        private void ResetChildEditors()
        {

            if (this.m_Database != null)
            {
                this.m_ChildEditors = new List<ICollectionEditor>();
                this.m_ChildEditors.Add(new CharacterCollectionEditor(this.m_Database, this.m_Database.items, new List<string>()));
                this.m_ChildEditors.Add(new Configuration.CharacterSettingsEditor(this.m_Database, this.m_Database.settings));

                for (int i = 0; i < this.m_ChildEditors.Count; i++)
                {
                    this.m_ChildEditors[i].OnEnable();
                }
            }
        }
    }
}