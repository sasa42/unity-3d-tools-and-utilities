﻿using System.Collections.Generic;
using GeometricVision;
using UnityEngine;

namespace Plugins.GeometricVision.Interfaces.Implementations
{
    public class GeometryVertexTargeting : IGeoTargeting
    {

        public List<GeometryDataModels.Target> GetTargets(Vector3 rayLocation, Vector3 rayDirection, List<GeometryDataModels.GeoInfo> targets)
        {
            throw new System.NotImplementedException();
        }

        public GeometryType TargetedType
        {
            get
            {
                return GeometryType.Vertices;
            }
        }

        public bool IsForEntities()
        {
            return false;
        }
    }
}
