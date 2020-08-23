﻿using System.Collections.Generic;
using System.Linq;
using Plugins.GeometricVision.Utilities;
using Unity.Collections;
using UnityEngine;
using static Plugins.GeometricVision.GeometryDataModels.Boolean;

namespace Plugins.GeometricVision.Interfaces.Implementations
{
    /// <summary>
    /// Class that is responsible for seeing objects and geometry.
    /// It checks, if object is inside visibility area and filters out unwanted objects and geometry.
    ///
    /// Usage: Add to objects you want it to. The component will handle the rest. Component has list of geometry types.
    /// These are used to see certain type of objects and clicking the targeting option from the inspector UI the user can
    /// add option to find the closest element of this type.
    /// </summary>
    [DisallowMultipleComponent]
    public class GeometryVisionEye : MonoBehaviour, IGeoEye
    {
        public string Id { get; set; }
        [SerializeField] private bool debugMode;
        [SerializeField] private bool hideEdgesOutsideFieldOfView = true;
        [SerializeField] private int lastCount = 0;
        [SerializeField] private List<GeometryDataModels.GeoInfo> seenGeoInfos = new List<GeometryDataModels.GeoInfo>();

        [SerializeField] public HashSet<Transform> seenTransforms;

        [SerializeField, Tooltip(" Geometry is extracted from collider instead of renderers mesh")]
        private bool targetColliderMeshes;

        public GeometryVision GeoVision { get; set; }

        public GeometryVisionHead Head { get; set; }

        void Reset()
        {
            Initialize();
        }

        // Start is called before the first frame update
        void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            seenGeoInfos = new List<GeometryDataModels.GeoInfo>();
            seenTransforms = new HashSet<Transform>();
        }

        public NativeArray<GeometryDataModels.Edge> GetSeenEdges()
        {
            List<GeometryDataModels.Edge> seenEdges = new List<GeometryDataModels.Edge>();
            int visibleEdgeCount = 0;
            foreach (var geo in SeenGeoInfos)
            {
                foreach (var edge1 in geo.edges.Where(edge => edge.isVisible == True))
                {
                    seenEdges.Add(edge1);
                    visibleEdgeCount += 1;
                }
            }

            return new NativeArray<GeometryDataModels.Edge>(seenEdges.ToArray(), Allocator.Temp);
        }


        /// <summary>
        /// Updates visibility of the objects in the eye and processor/manager
        /// </summary>
        public void UpdateVisibility()
        {
            seenTransforms = UpdateTransformVisibility(Head.GetProcessor<GeometryVisionProcessor>().GetAllObjects(), seenTransforms);
            SeenGeoInfos = UpdateRenderedMeshVisibility(GeoVision.Planes, Head.GeoMemory.GeoInfos);
        }


        /// <summary>
        /// Update GameObject/transform visibility. Object that does not have Mesh or renderer in it
        /// </summary>
        private HashSet<Transform> UpdateTransformVisibility(List<Transform> listToCheck,
            HashSet<Transform> seenTransforms)
        {
            seenTransforms = new HashSet<Transform>();

            seenTransforms = GetObjectsInsideFrustum(seenTransforms, listToCheck);


            return seenTransforms;
        }

        /// <summary>
        /// Checks all the object that contain render component if they are visible by testing their bounding box
        /// Hides Edges, vertices, geometryObject outside th frustum
        /// </summary>
        /// <param name="planes"></param>
        /// <param name="allGeoInfos"></param>
        private List<GeometryDataModels.GeoInfo> UpdateRenderedMeshVisibility(Plane[] planes,
            List<GeometryDataModels.GeoInfo> allGeoInfos)
        {
            int geoCount = allGeoInfos.Count;
            var newSeenGeometriesList = new List<GeometryDataModels.GeoInfo>();

            UpdateSeenGeometryObjects();
            
            // Updates object collection containing geometry and data related to seen object. Usage is to internally update seen geometry objects by checking objects renderer bounds
            // against eyes/cameras frustum
            void UpdateSeenGeometryObjects()
            {
                for (var i = 0; i < geoCount; i++)
                {
                    var geInfo = allGeoInfos[i];

                    if (GeometryUtility.TestPlanesAABB(GeoVision.Planes, allGeoInfos[i].renderer.bounds) &&
                        hideEdgesOutsideFieldOfView)
                    {
                        newSeenGeometriesList.Add(geInfo);
                    }
                }
            }
            foreach (var geometryType in GeoVision.TargetingInstructions)
            {
                if (geometryType.GeometryType == GeometryType.Lines && geometryType.Enabled)
                {
                    MeshUtilities.UpdateEdgesVisibilityParallel(planes, newSeenGeometriesList);
                }
            }

            return newSeenGeometriesList;
        }


        private HashSet<Transform> GetObjectsInsideFrustum(HashSet<Transform> seenTransforms,
            List<Transform> allTransforms)
        {
            foreach (var transform in allTransforms)
            {
                if (MeshUtilities.IsInsideFrustum(transform.position, GeoVision.Planes))
                {
                    seenTransforms.Add(transform);
                    lastCount = seenTransforms.Count;
                }
            }

            return seenTransforms;
        }

        public List<GeometryDataModels.GeoInfo> SeenGeoInfos
        {
            get { return seenGeoInfos; }
            set { seenGeoInfos = value; }
        }

        public bool DebugMode
        {
            get { return debugMode; }
            set { debugMode = value; }
        }


        public bool TargetColliderMeshes
        {
            get { return targetColliderMeshes; }
            set { targetColliderMeshes = value; }
        }
    }
}