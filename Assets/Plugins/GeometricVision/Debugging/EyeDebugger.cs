﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeometricVision;
using UnityEditor.Graphs;
using UnityEngine;
using static GeometricVision.GeometryDataModels.Boolean;
using Plane = GeometricVision.GeometryDataModels.Plane;

public class EyeDebugger 
{
    List<GameObject> debugPlanes = new List<GameObject>();
    public UnityEngine.Plane[] _planes;
    private int amountOfSeenEdges = 0;
    public UnityEngine.Plane[] Planes
    {
        get { return _planes; }
        set { _planes = value; }
    }
    
    public struct Root
    {
        public Vector3 positiom;
        public Quaternion rotation;
        public Vector3 direction;
        public Transform rootObject;
        public GameObject[] debugPlanes;
        public UnityEngine.Plane[] Planes;
    }
    public List<GameObject> DebugPlanes
    {
        get { return debugPlanes; }
        set { debugPlanes = value; }
    }

    public int AmountOfSeenEdges
    {
        get { return amountOfSeenEdges; }
        set { amountOfSeenEdges = value; }
    }

    internal void CreateDebugPlanes(float fieldOfView, Vector3[] frustumCornersNear, Vector3[] frustumCornersFar, Camera camera, UnityEngine.Plane[] planes)
    {
        _planes = planes;
        camera.fieldOfView = fieldOfView;
        debugPlanes.Add(CreateUnityPlane(Plane.near, frustumCornersNear.Take(2).ToArray(),frustumCornersNear.Skip(2).ToArray(), Color.clear, camera));
        debugPlanes.Add(CreateUnityPlane(Plane.far, frustumCornersFar.Take(2).ToArray(),frustumCornersFar.Skip(2).ToArray(), Color.clear, camera));
        debugPlanes.Add(CreateUnityPlane(Plane.left, frustumCornersFar.Take(2).ToArray(),frustumCornersNear.Take(2).ToArray(), Color.green, camera));
        debugPlanes.Add(CreateUnityPlane(Plane.right,frustumCornersFar.Skip(2).Take(2).ToArray(), frustumCornersNear.Skip(2).Take(2).ToArray(), Color.black,camera));
        Vector3[] nearPoints = {frustumCornersNear.Take(1).First(), frustumCornersNear.Skip(3).Take(1).First()};
        Vector3[] farPoints = {frustumCornersFar.Take(1).First(), frustumCornersFar.Skip(3).Take(1).First()};
        debugPlanes.Add(CreateUnityPlane(Plane.down, nearPoints, farPoints, Color.blue, camera));
        debugPlanes.Add(CreateUnityPlane(Plane.up, frustumCornersFar.Skip(1).Take(2).ToArray(),frustumCornersNear.Skip(1).Take(2).ToArray(), Color.red, camera));
    }
        
    private GameObject CreateUnityPlane(Plane planeType, Vector3[] frustumCornersFar, Vector3[] frustumCornersNear, Color color, Camera camera)
    {
        var normal = Planes[(int) planeType].normal;
        var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        var plane2 = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = planeType.ToString();
        plane2.name = planeType.ToString();
        plane.transform.position = -normal * Planes[(int) planeType].distance;
        plane2.transform.position = -normal * Planes[(int) planeType].distance;
        plane.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
        Material debugMaterial = new Material(Shader.Find("Diffuse"));
        color.a = 0.1f;
        debugMaterial.SetColor(Shader.PropertyToID("_Color"), (color));
        var mesh = CreateMeshPlaneFromCorners(frustumCornersFar, frustumCornersNear);
        plane2.GetComponent<MeshFilter>().mesh = mesh;
        for (int i = 0; i < frustumCornersFar.Length; i++)
        {
            var wsCornerClose = camera.transform.TransformVector(frustumCornersNear[i]);
            var wsCornerFar = camera.transform.TransformVector(frustumCornersFar[i]);
            //     Debug.Log(wsCornerFar);
            //     Debug.DrawRay(wsCornerClose, wsCornerFar, color, 11111);
        }

// plane.transform.GetComponent<MeshFilter>().mesh.vertices = frustumCorners;
        return plane;
    }
    private Mesh CreateMeshPlaneFromCorners(Vector3[] frustumCornersFar, Vector3[] frustumCornersNear)
    {
        var mesh = new Mesh();
        var verts = new Vector3[6];
        verts[0] = frustumCornersFar[0];
        verts[1] = frustumCornersFar[1];
        verts[2] = frustumCornersNear[1];
        verts[3] = frustumCornersNear[0];
        verts[4] = frustumCornersNear[1];
        verts[5] = frustumCornersFar[1];
        var tris = new int[6];
        tris[0] = 0;
        tris[1] = 1;
        tris[2] = 2;
        tris[3] = 3;
        tris[4] = 4;
        tris[5] = 5;
        mesh.vertices = verts;
        mesh.triangles = tris;
        return mesh;
    }
    
    private void DrawEdgesOnAllObjects(List<GeometryDataModels.GeoInfo> geoInfos, Action<GeometryDataModels.GeoInfo> draw)
    {
        if (geoInfos.Count != 0)
        {
            foreach (var geoItem in geoInfos)
            {
                draw(geoItem);
            }
        }
    }

    private void DrawEdges(GeometryDataModels.GeoInfo geoItem)
    {
        foreach (var edge in geoItem.edges)
        {
            if (edge.isVisible == True)
            {
                UnityEngine.Debug.DrawLine(edge.firstVertex, edge.secondVertex, Color.blue, 1);
                amountOfSeenEdges++;
            }
        }
    }
    
    public void Debug(Camera camera, List<GeometryDataModels.GeoInfo> geoInfos, bool geometryOnly)
    {
        if (geometryOnly == false)
        {
            RefreshFrustumCorners(camera);
            if (DebugPlanes.Count == 0)
            {
                CreateDebugPlanes(25, _frustumCornersNear, _frustumCornersFar, camera, _planes);
            }
        }
        amountOfSeenEdges = 0;
        DrawEdgesOnAllObjects(geoInfos, DrawEdges);
        
    }
    private void RefreshFrustumCorners(Camera camera)
    {
        camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane,
            Camera.MonoOrStereoscopicEye.Mono, _frustumCornersFar);
        camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.nearClipPlane,
            Camera.MonoOrStereoscopicEye.Mono, _frustumCornersNear);
    }
    
    Vector3[] _frustumCornersFar = new Vector3[4];
    Vector3[] _frustumCornersNear = new Vector3[4];
}
