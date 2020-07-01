﻿using System;
using System.Collections;
using System.Collections.Generic;
using Plugins.GeometricVision;
using Plugins.GeometricVision.Interfaces.Implementations;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEngine.Serialization;

namespace GeometricVision
{
    public class GeometryTargeting : MonoBehaviour
    {
        [SerializeField] private bool debugMode;
        private GeometryVisionEye eye;
        [SerializeField]private List<IGeoTargeting> targetingPrograms =new List<IGeoTargeting>();
        [SerializeField] private int targetingSystemsCount = 0;

        public List<IGeoTargeting> TargetingPrograms
        {
            get { return targetingPrograms; }
            set { targetingPrograms = value; }
        }

        private void Reset()
        {
            InitializeTargeting();
        }

        void Start()
        {
            InitializeTargeting();
        }
        void OnValidate () {

            if (this.GetComponent<GeometryVision>()== null)
            {
                // The really important part, using the library
                EditorCoroutineUtility.StartCoroutine(DestroyThis(), this);
            }
        }


        IEnumerator DestroyThis()
        {
            RemoveAddedComponents();
            yield return new WaitForEndOfFrame();
        }

        private void RemoveAddedComponents()
        {
            if (this.GetComponent<Camera>() != null)
            {
                DestroyImmediate(this.GetComponent<Camera>());
            }
            
            if (this.GetComponent<GeometryVisionEye>() != null)
            {
                DestroyImmediate(GetComponent<GeometryVisionEye>());
            }

            if (this.GetComponent<GeometryTargeting>() != null)
            {
                DestroyImmediate(GetComponent<GeometryTargeting>());
            }
        }
        /// <summary>
        /// Initializes the geometry vision eye, which handles setting up the geometry vision plugin.
        /// </summary>
        private void InitializeTargeting()
        {
            if (TargetingPrograms == null)
            {
                TargetingPrograms = new List<IGeoTargeting>();
            }

            if (GetComponent<GeometryVisionEye>() == null)
            {
                gameObject.AddComponent<GeometryVisionEye>();
                eye = GetComponent<GeometryVisionEye>();
            }

            /*targetingPrograms = new IGeoTargeting[eye.GeometryTypes.Length];
             
              foreach (var targetTypes in eye.GeometryTypes)
              {
                  
              }*/
        }

        public void AddTarget(VisionTarget geometryContainer)
        {
            if (geometryContainer.GeometryType == GeometryType.Objects)
            {
                IGeoTargeting targetingSystem = new GeometryObjectTargeting();
                geometryContainer.TargetingSystem = targetingSystem;
                TargetingPrograms.Add(geometryContainer.TargetingSystem);
            }

            if(TargetingPrograms != null){
                targetingSystemsCount = TargetingPrograms.Count;
            }
        }

        public void RemoveTarget(VisionTarget geometryType)
        {
            if (geometryType.TargetingSystem != null)
            {
                TargetingPrograms.Remove(geometryType.TargetingSystem);
                targetingSystemsCount = TargetingPrograms.Count;
            }
        }
    }
}