﻿using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.ITypes.IElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IModifiers
{
    public static partial class ISubdividor
    {
        public static IMesh CatmullClark(IMesh mesh)
        {
            IMesh sMesh = new IMesh();

            //Old vertices
            foreach (int vK in mesh.Vertices.VerticesKeys)
            {
                sMesh.Vertices.AddVertex(vK, new ITopologicVertex(ComputesCatmullClarkVertexPosition(mesh, vK)));
            }

            // Subidvision
            int key = mesh.Vertices.FindNextKey();
            int[] hf;
            IElement element_sibling;
            int elementID_sibling, halfFacetID_sibling;
            Boolean visited;
            Dictionary<int, int[]> eVertex = new Dictionary<int, int[]>();
            Dictionary<int, int> fVertex = new Dictionary<int, int>();

            IVector3D pos;

            int count = mesh.Elements.ElementsKeys.Count;

            // Vertices
            foreach (int elementID in mesh.Elements.ElementsKeys)
            {
                IElement e = mesh.Elements.GetElementWithKey(elementID);

                //Add face vertex
                pos = ComputeAveragePosition(e.Vertices, mesh);
                sMesh.Vertices.AddVertex(key, new ITopologicVertex(pos.X, pos.Y, pos.Z, key));
                fVertex.Add(elementID, key);
                key++;

                if (!e.Visited)
                {
                    if (e.TopologicDimension == 2)
                    {
                        if (!eVertex.ContainsKey(elementID)) eVertex.Add(elementID, new int[e.HalfFacetsCount]);

                        for (int halfFacetID = 1; halfFacetID <= e.HalfFacetsCount; halfFacetID++)
                        {
                            e.GetHalfFacet(halfFacetID, out hf);
                            visited = e.IsHalfFacetVisited(halfFacetID);

                            if (!visited)
                            {
                                e.RegisterHalfFacetVisit(halfFacetID);
                                e.GetHalfFacet(halfFacetID, out hf);
                                pos = ComputeAveragePosition(hf, mesh);
                                sMesh.Vertices.AddVertex(key, new ITopologicVertex(pos.X, pos.Y, pos.Z, key));
                                eVertex[elementID][halfFacetID - 1] = key;

                                if (!e.IsNakedSiblingHalfFacet(halfFacetID))
                                {
                                    while (!visited)
                                    {
                                        e.RegisterHalfFacetVisit(halfFacetID);

                                        //Collect information of siblings
                                        elementID_sibling = e.GetSiblingElementID(halfFacetID);
                                        halfFacetID_sibling = e.GetSiblingHalfFacetID(halfFacetID);
                                        element_sibling = mesh.Elements.GetElementWithKey(elementID_sibling);

                                        visited = element_sibling.IsHalfFacetVisited(halfFacetID_sibling);

                                        halfFacetID = halfFacetID_sibling;
                                        e = element_sibling;

                                        if (!eVertex.ContainsKey(elementID_sibling)) eVertex.Add(elementID_sibling, new int[e.HalfFacetsCount]);
                                        eVertex[elementID_sibling][halfFacetID - 1] = key;
                                    }
                                }

                                key++;
                            }
                        }
                    }
                }
            }
            mesh.Elements.CleanVisits();

            //Faces
            int prev;
            int[] data;
            int elementKey = sMesh.Elements.FindNextKey();
            foreach (int elementID in mesh.Elements.ElementsKeys)
            {
                IElement e = mesh.Elements.GetElementWithKey(elementID);

                if (e.TopologicDimension == 2)
                {
                    int[] eV = eVertex[elementID];
                    for (int i = 0; i < e.Vertices.Length; i++)
                    {
                        prev = i - 1;
                        if (prev < 0) prev = e.Vertices.Length - 1;

                        data = new int[] { e.Vertices[i], eV[prev], fVertex[elementID], eV[i] };
                        ISurfaceElement face = new ISurfaceElement(data);
                        sMesh.Elements.AddElement(elementKey, face);
                        elementKey++;
                    }
                }
            }

            // Edge Vertex
            foreach (int eK in eVertex.Keys)
            {
                IElement e = mesh.Elements.GetElementWithKey(eK);
                for (int i = 1; i <= e.HalfFacetsCount; i++)
                {
                    int[] hf1;
                    e.GetHalfFacet(i, out hf1);
                    ITopologicVertex v1 = sMesh.Vertices.GetVertexWithKey(eVertex[eK][i - 1]);
                    v1.Position = ComputeCatmullClarkEdgeVertexPosition(hf1, mesh);
                    sMesh.Vertices.SetVertex(v1.Key,v1);
                }
            }

            //Build Mesh
            sMesh.BuildTopology();

            return sMesh;
        }

        internal static IVector3D ComputeCatmullClarkEdgeVertexPosition(int[] keys, IMesh m)
        {
            IVector3D v = new IVector3D();
            if (!m.Topology.IsNakedEdge(keys[0], keys[1]))
            {
                // Edge ends average
                for (int i = 0; i < keys.Length; i++) v += m.Vertices.GetVertexWithKey(keys[i]).Position;

                //Face centers average
                int[] incidentE = m.Topology.GetEdgeIncidentElements(keys[0], keys[1]);
                IElement e;
                foreach (int eKey in incidentE)
                {
                    e = m.Elements.GetElementWithKey(eKey);
                    v += ComputeAveragePosition(e.Vertices, m);
                }
                v /= (incidentE.Length + keys.Length);
            }
            else
            {
                v = ComputeAveragePosition(keys, m);
            }

            return v;
        }

        internal static IVector3D ComputesCatmullClarkVertexPosition(IMesh iM, int vKey)
        {
            ITopologicVertex v = iM.Vertices.GetVertexWithKey(vKey);
            int[] vN = iM.Topology.GetVertexAdjacentVertices(vKey);
            int[] eN = iM.Topology.GetVertexIncidentElements(vKey);
            IVector3D barycenter = new IVector3D();

            if (!iM.Topology.IsNakedVertex(vKey))
            {
                int n = vN.Length;

                IVector3D P = v.Position;
                P *= (n - 3);

                IVector3D R = new IVector3D();
                foreach (int nKey in vN) R += ComputeAveragePosition(new[] { vKey, nKey }, iM);
                R /= vN.Length;
                R *= 2;

                IVector3D F = new IVector3D();

                foreach (int nKey in eN) F += ComputeAveragePosition(iM.Elements.GetElementWithKey(nKey).Vertices, iM);
                F /= eN.Length;

                barycenter = F + R + P;
                barycenter /= n;
            }
            else
            {
                barycenter = new IVector3D();
                int count = 0;
                foreach (int nKey in vN)
                {
                    if (iM.Topology.IsNakedVertex(nKey))
                    {
                        barycenter += ComputeAveragePosition(new[] { vKey, nKey }, iM);
                        count++;
                    }
                }
                barycenter /= count;
            }

            return barycenter;
        }
    }
}