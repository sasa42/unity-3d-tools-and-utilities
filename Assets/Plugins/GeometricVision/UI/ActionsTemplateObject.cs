﻿using System.IO;
using System.Security.Permissions;
using UnityEditor;
using UnityEngine;

namespace Plugins.GeometricVision.UI
{
    
    [CustomEditor(typeof(ActionsTemplateObject))]
    public class ActionsTemplateDrawer : UnityEditor.Editor
    {
        private Texture headerTexture;
        private float textureBottomSpaceExtension = 50f;
        private float ratioMultiplier;
        
        public override void OnInspectorGUI()
        {
            if (headerTexture == null)
            {
                headerTexture = LoadPNG(Application.dataPath+"/Plugins/GeometricVision/UI/Images/GeoVisionTargeting.png");

                Texture2D LoadPNG(string filePath) {
 
                    Texture2D texture2D = null;
                    byte[] fileData;
 
                    if (File.Exists(filePath))     {
                        fileData = File.ReadAllBytes(filePath);
                        texture2D = new Texture2D(2, 2);

                        texture2D.LoadImage(fileData); 


                    }
                    return texture2D;
                }
            }
            
            DrawTexture();
            DrawDefaultInspector ();

            void DrawTexture()
            {
                GUILayout.Label("Geometric vision actions template");
                ratioMultiplier = (float) headerTexture.height / (float) headerTexture.width;
                EditorGUI.DrawPreviewTexture(
                    new Rect(25, 60, EditorGUIUtility.currentViewWidth, EditorGUIUtility.currentViewWidth * ratioMultiplier),
                    headerTexture);
                GUILayout.Space(EditorGUIUtility.currentViewWidth * ratioMultiplier + textureBottomSpaceExtension);
            }
        }
    }
    
    [CreateAssetMenu(fileName = "Actions", menuName = "ScriptableObjects/ActionsForTargeting", order = 1)]
    public class ActionsTemplateObject : ScriptableObject
    {
        [Header("Hand effect Settings")] [SerializeField]
        private bool startActionEnabled;

        [SerializeField] private float startDelay = 0;
        [SerializeField] private float startDuration = 0;
        [SerializeField] private GameObject startActionObject;

        [Header("Between target and hand effect Settings")] [SerializeField]
        private bool actionEnabled = true;

        [SerializeField] private float delay = 0;
        [SerializeField] private float duration = 0;
        [SerializeField] private GameObject actionObject;

        [Header("Target effect Settings")] [SerializeField]
        private bool endActionEnabled = true;

        [SerializeField] private float endDelay = 0;
        [SerializeField] private float endDuration = 0;
        [SerializeField] private GameObject endActionObject;



        public float StartDelay
        {
            get { return startDelay; }
            set { startDelay = value; }
        }

        public bool StartActionEnabled
        {
            get { return startActionEnabled; }
            set { startActionEnabled = value; }
        }

        public float StartDuration
        {
            get { return startDuration; }
            set { startDuration = value; }
        }

        public GameObject StartActionObject
        {
            get { return startActionObject; }
            set { startActionObject = value; }
        }

        public bool ActionEnabled
        {
            get { return actionEnabled; }
            set { actionEnabled = value; }
        }

        public bool EndActionEnabled
        {
            get { return endActionEnabled; }
            set { endActionEnabled = value; }
        }

        public float Delay
        {
            get { return delay; }
            set { delay = value; }
        }

        public float Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        public GameObject ActionObject
        {
            get { return actionObject; }
            set { actionObject = value; }
        }

        public float EndDelay
        {
            get { return endDelay; }
            set { endDelay = value; }
        }

        public float EndDuration
        {
            get { return endDuration; }
            set { endDuration = value; }
        }

        public GameObject EndActionObject
        {
            get { return endActionObject; }
            set { endActionObject = value; }
        }
    }
}