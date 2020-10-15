﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Iguana.IguanaMesh.IUtils;

namespace Iguana.IguanaMesh.IWrappers
{
    public static partial class IguanaGmsh
    {
        public static partial class Model
        {
            /// <summary>
            /// Add a new model, with name `name', and set it as the current model.
            /// </summary>
            /// <param name="name"> Name of the model </param>
            public static void Add(string name)
            {
                IWrappers.GmshModelAdd(name, ref _ierr);
            }

            /// <summary>
            /// Remove Model
            /// </summary>
            public static void Remove()
            {
                IWrappers.GmshModelRemove(ref _ierr);
            }

            /// <summary>
            /// Add a physical group of dimension `dim', grouping the model entities with tags `tags'. 
            /// Return the tag of the physical group, equal to `tag' if `tag' is positive, or a new tag if `tag' < 0.
            /// </summary>
            /// <param name="dim"></param>
            /// <param name="tags"></param>
            /// <param name="tag"></param>
            /// <returns></returns>
            public static int AddPhysicalGroup(int dim, int[] tags, int tag = -1)
            {
                return IWrappers.GmshModelAddPhysicalGroup(dim, tags, tags.Length, tag, ref _ierr);
            }

            /// <summary>
            /// Set the name of the physical group of dimension `dim' and tag `tag'.
            /// </summary>
            /// <param name="dim"></param>
            /// <param name="tag"></param>
            /// <param name="name"></param>
            public static void SetPhysicalName(int dim, int tag, string name)
            {
                IWrappers.GmshModelSetPhysicalName(dim, tag, name, ref _ierr);
            }

            /// <summary>
            /// Get all the entities in the current model. If `dim' is >= 0, return only 
            /// the entities of the specified dimension(e.g.points if `dim' == 0). The
            /// entities are returned as a vector of (dim, tag) integer pairs.
            /// </summary>
            /// <param name="dimTags"></param>
            /// <param name="dim"></param>
            public static void GetEntities(out Tuple<int, int>[] dimTags, int dim = -1)
            {
                IntPtr dimTags_parse;
                long dimTags_n;
                IWrappers.GmshModelGetEntities(out dimTags_parse, out dimTags_n, dim, ref _ierr);

                dimTags = null;

                // Tags
                if (dimTags_n > 0)
                {
                    var temp = new int[dimTags_n];
                    Marshal.Copy(dimTags_parse, temp, 0, (int)dimTags_n);

                    dimTags = IHelpers.GraftIntTupleArray(temp);
                }

                // Delete unmanaged allocated memory
                IWrappers.GmshFree(dimTags_parse);
            }


            /// <summary>
            /// Get the boundary of the model entities `dimTags'. Return in `outDimTags' 
            /// the boundary of the individual entities(if `combined' is false) or the
            /// boundary of the combined geometrical shape formed by all input entities (if
            /// `combined' is true). Return tags multiplied by the sign of the boundary
            /// entity if `oriented' is true. Apply the boundary operator recursively down
            /// to dimension 0 (i.e.to points) if `recursive' is true. 
            /// </summary>
            public static void GetBoundary(Tuple<int, int>[] dimTags, out Tuple<int, int>[] outDimTags, bool combined = false, bool oriented = false, bool recursive = false)
            {
                int[] dimTags_flatten = IHelpers.FlattenIntTupleArray(dimTags);
                IntPtr outDimTags_parse;
                long outDimTags_n;
                IWrappers.GmshModelGetBoundary(dimTags_flatten, dimTags_flatten.LongLength, out outDimTags_parse, out outDimTags_n, Convert.ToInt32(combined), Convert.ToInt32(oriented), Convert.ToInt32(recursive), ref _ierr);

                outDimTags = null;
                if (outDimTags_n > 0)
                {
                    var temp = new int[outDimTags_n];
                    Marshal.Copy(outDimTags_parse, temp, 0, (int)outDimTags_n);
                    outDimTags = IHelpers.GraftIntTupleArray(temp);
                }

                IWrappers.GmshFree(outDimTags_parse);
            }

            /// <summary>
            /// Add a discrete model entity (defined by a mesh) of dimension `dim' in the
            /// current model.Return the tag of the new discrete entity, equal to `tag' if
            /// `tag' is positive, or a new tag if `tag' < 0. `boundary' specifies the tags
            /// of the entities on the boundary of the discrete entity, if any.Specifying
            /// `boundary' allows Gmsh to construct the topology of the overall model.
            /// </summary>
            public static int AddDiscreteEntity(int dim, int tag, int[] boundary = default)
            {
                if (boundary == default) boundary = new int[0];
                return IWrappers.GmshModelAddDiscreteEntity(dim, tag, boundary, boundary.LongLength, ref _ierr);
            }

            /// <summary>
            /// Remove the entities `dimTags' of the current model. If `recursive' is true,
            /// remove all the entities on their boundaries, down to dimension 0.
            /// </summary>
            public static void RemoveEntities(Tuple<int, int>[] dimTags, bool recursive)
            {
                var arr = IHelpers.FlattenIntTupleArray(dimTags);
                IWrappers.GmshModelRemoveEntities(arr, arr.LongLength, Convert.ToInt32(recursive), ref _ierr);
            }

            /// <summary>
            /// Set the `x', `y', `z' coordinates of a geometrical point.
            /// </summary>
            public static void SetCoordinates(int tag, double x, double y, double z)
            {
                IWrappers.GmshModelSetCoordinates(tag, x, y, z, ref _ierr);
            }


            /// <summary>
            /// Evaluate the parametrization of the entity of dimension `dim' and tag `tag'
            /// at the parametric coordinates `parametricCoord'. Only valid for `dim' equal
            /// to 0 (with empty `parametricCoord'), 1 (with `parametricCoord' containing
            /// parametric coordinates on the curve) or 2 (with `parametricCoord'
            /// containing pairs of u, v parametric coordinates on the surface,
            /// concatenated: [p1u, p1v, p2u, ...]). Return triplets of x, y, z coordinates
            /// in `coord', concatenated: [p1x, p1y, p1z, p2x, ...].
            /// </summary>
            public static void GetValue(int dim, int tag, out double[] coord, double[] parametricCoord = default)
            {
                IntPtr coord_parse;
                long coord_n;
                if (parametricCoord == default) parametricCoord = new double[0];
                IWrappers.GmshModelGetValue(dim, tag, parametricCoord, parametricCoord.LongLength, out coord_parse, out coord_n, ref _ierr);

                coord = new double[0];
                if (coord_n > 0)
                {
                    coord = new double[coord_n];
                    Marshal.Copy(coord_parse, coord, 0, (int)coord_n);
                }

                IguanaGmsh.Free(coord_parse);
            }

            /// <summary>
            /// Evaluate the derivative of the parametrization of the entity of dimension
            /// `dim' and tag `tag' at the parametric coordinates `parametricCoord'. Only
            /// valid for `dim' equal to 1 (with `parametricCoord' containing parametric
            /// coordinates on the curve) or 2 (with `parametricCoord' containing pairs of
            /// u, v parametric coordinates on the surface, concatenated: [p1u, p1v, p2u,
            /// ...]). For `dim' equal to 1 return the x, y, z components of the derivative
            /// with respect to u[d1ux, d1uy, d1uz, d2ux, ...]; for `dim' equal to 2
            /// return the x, y, z components of the derivate with respect to u and v:
            /// [d1ux, d1uy, d1uz, d1vx, d1vy, d1vz, d2ux, ...].
            /// </summary>
            public static void GetDerivatives(int dim, int tag, double[] parametricCoord, out double[] derivatives)
            {
                IntPtr dP;
                long derivatives_n;

                IWrappers.GmshModelGetDerivative(dim, tag, parametricCoord, parametricCoord.LongLength, out dP, out derivatives_n, ref _ierr);

                derivatives = new double[derivatives_n];
                Marshal.Copy(dP, derivatives, 0, (int)derivatives_n);

                IguanaGmsh.Free(dP);
            }

            /// <summary>
            /// Evaluate the (maximum) curvature of the entity of dimension `dim' and tag
            /// `tag' at the parametric coordinates `parametricCoord'. Only valid for `dim'
            /// equal to 1 (with `parametricCoord' containing parametric coordinates on the
            /// curve) or 2 (with `parametricCoord' containing pairs of u, v parametric
            /// coordinates on the surface, concatenated: [p1u, p1v, p2u, ...]).
            /// </summary>
            public static void GetCurvature(int dim, int tag, double[] parametricCoord, out double[] curvatures)
            {
                IntPtr cP;
                long curvatures_n;
                IWrappers.GmshModelGetCurvature(dim, tag, parametricCoord, parametricCoord.LongLength, out cP, out curvatures_n, ref _ierr);

                curvatures = new double[curvatures_n];
                Marshal.Copy(cP, curvatures, 0, (int)curvatures_n);

                IguanaGmsh.Free(cP);
            }

            /// <summary>
            /// Evaluate the principal curvatures of the surface with tag `tag' at the
            /// parametric coordinates `parametricCoord', as well as their respective
            /// directions. `parametricCoord' are given by pair of u and v coordinates,
            /// concatenated: [p1u, p1v, p2u, ...].
            /// </summary>
            public static void GetPrincipalCurvatures(int tag, double[] parametricCoord, out double[] curvatureMax, out double[] curvatureMin, out double[] directionMax, out double[] directionMin)
            {
                IntPtr cMax, cMin, dMax, dMin;
                long directionMin_n, directionMax_n, curvatureMin_n, curvatureMax_n;

                IWrappers.GmshModelGetPrincipalCurvatures(tag, parametricCoord, parametricCoord.LongLength, out cMax, out curvatureMax_n, out cMin, out curvatureMin_n, out dMax, out directionMax_n, out dMin, out directionMin_n, ref _ierr);

                curvatureMax = new double[curvatureMax_n];
                curvatureMin = new double[curvatureMin_n];
                directionMax = new double[directionMax_n];
                directionMin = new double[directionMin_n];

                Marshal.Copy(cMax, curvatureMax, 0, (int)curvatureMax_n);
                Marshal.Copy(cMin, curvatureMin, 0, (int)curvatureMin_n);
                Marshal.Copy(dMax, directionMax, 0, (int)directionMax_n);
                Marshal.Copy(dMin, directionMin, 0, (int)directionMin_n);

                IguanaGmsh.Free(cMax);
                IguanaGmsh.Free(cMin);
                IguanaGmsh.Free(dMax);
                IguanaGmsh.Free(dMin);

            }

            /// <summary>
            /// Get the normal to the surface with tag `tag' at the parametric coordinates
            /// `parametricCoord'. `parametricCoord' are given by pairs of u and v
            /// coordinates, concatenated: [p1u, p1v, p2u, ...]. `normals' are returned as
            /// triplets of x, y, z components, concatenated: [n1x, n1y, n1z, n2x, ...].
            /// </summary>
            public static void GetNormal(int tag, double[] parametricCoord, out double[] normals)
            {
                IntPtr nP;
                long normals_n;
                IWrappers.GmshModelGetNormal(tag, parametricCoord, parametricCoord.LongLength, out nP, out normals_n, ref _ierr);

                normals = new double[0];
                if (normals_n > 0)
                {
                    normals = new double[normals_n];
                    Marshal.Copy(nP, normals, 0, (int)normals_n);
                }

                IguanaGmsh.Free(nP);
            }

            /// <summary>
            /// Set the color of the model entities `dimTags' to the RGBA value (`r', `g',
            /// `b', `a'), where `r', `g', `b' and `a' should be integers between 0 and
            /// 255. Apply the color setting recursively if `recursive' is true.
            /// </summary>
            public static void SetColor(Tuple<int, int>[] dimTags, int r, int g, int b, int a = 255, bool recursive = false)
            {
                var arr = IHelpers.FlattenIntTupleArray(dimTags);
                IWrappers.GmshModelSetColor(arr, arr.LongLength, r, g, b, a, Convert.ToInt32(recursive), ref _ierr);
            }

            /// <summary>
            /// Get the color of the model entity of dimension `dim' and tag `tag'.
            /// </summary>
            public static void GetColor(int dim, int tag, out int r, out int g, out int b, out int a)
            {
                IWrappers.GmshModelGetColor(dim, tag, out r, out g, out b, out a, ref _ierr);
            }

            /// <summary>
            /// Get the name of the current model.
            /// </summary>
            public static void GetCurrent(out string name)
            {
                IntPtr nP;
                IWrappers.GmshModelGetCurrent(out nP, ref _ierr);
                name = Marshal.PtrToStringAnsi(nP);

                IguanaGmsh.Free(nP);
            }

            /// <summary>
            /// Set the current model to the model with name `name'. If several models have
            /// the same name, select the one that was added first.
            /// </summary>
            public static void SetCurrent(string name)
            {
                IWrappers.GmshModelSetCurrent(name, ref _ierr);
            }

            /// <summary>
            /// Set the name of the entity of dimension `dim' and tag `tag'.
            /// </summary>
            public static void SetEntityName(int dim, int tag, string name)
            {
                IWrappers.GmshModelSetEntityName(dim, tag, name, ref _ierr);
            }

            /// <summary>
            /// Get the name of the entity of dimension `dim' and tag `tag'.
            /// </summary>
            public static void GetEntityName(int dim, int tag, out string name)
            {
                IntPtr nP;
                IWrappers.GmshModelGetEntityName(dim, tag, out nP, ref _ierr);

                name = Marshal.PtrToStringAnsi(nP);

                IguanaGmsh.Free(nP);
            }

            /// <summary>
            /// Get all the physical groups in the current model. If `dim' is >= 0, return
            /// only the entities of the specified dimension(e.g.physical points if `dim'
            /// == 0). The entities are returned as a vector of(dim, tag) integer pairs.
            /// </summary>
            /// <param name="dim"></param>
            /// <param name="dimTags"></param>
            public static void GetPhysicalGroups(int dim, out Tuple<int, int>[] dimTags)
            {
                IntPtr dtP;
                long dimTags_n;
                IWrappers.GmshModelGetPhysicalGroups(out dtP, out dimTags_n, dim, ref _ierr);

                dimTags = new Tuple<int, int>[0];
                if (dimTags_n > 0)
                {
                    var temp = new int[dimTags_n];
                    ; Marshal.Copy(dtP, temp, 0, (int)dimTags_n);
                    dimTags = IHelpers.GraftIntTupleArray(temp);
                }

                IguanaGmsh.Free(dtP);
            }

            /// <summary>
            /// Get the tags of the model entities making up the physical group of dimension `dim' and tag `tag'.
            /// </summary>
            public static void GetEntitiesForPhysicalGroup(int dim, int tag, out int[] tags)
            {
                IntPtr tP;
                long tags_n;
                IWrappers.GmshModelGetEntitiesForPhysicalGroup(dim, tag, out tP, out tags_n, ref _ierr);

                tags = new int[0];
                if (tags_n > 0)
                {
                    tags = new int[tags_n];
                    Marshal.Copy(tP, tags, 0, (int)tags_n);
                }

                IguanaGmsh.Free(tP);
            }

            /// <summary>
            /// Get the tags of the physical groups (if any) to which the model entity of dimension `dim' and tag `tag' belongs.
            /// </summary>
            public static void GetPhysicalGroupsForEntity(int dim, int tag, out int[] physicalTags)
            {
                IntPtr ptP;
                long physicalTags_n;
                IWrappers.GmshModelGetPhysicalGroupsForEntity(dim, tag, out ptP, out physicalTags_n, ref _ierr);

                physicalTags = new int[0];
                if (physicalTags_n > 0)
                {
                    physicalTags = new int[physicalTags_n];
                    Marshal.Copy(ptP, physicalTags, 0, (int)physicalTags_n);
                }

                IguanaGmsh.Free(ptP);
            }

            /// <summary>
            /// Get the bounding box (`xmin', `ymin', `zmin'), (`xmax', `ymax', `zmax') of
            /// the model entity of dimension `dim' and tag `tag'. If `dim' and `tag' are
            /// negative, get the bounding box of the whole model.
            /// </summary>
            public static void GetBoundingBox(int dim, int tag, out double xmin, out double ymin, out double zmin, out double xmax, out double ymax, out double zmax)
            {
                IWrappers.GmshModelGetBoundingBox(dim, tag, out xmin, out ymin, out zmin, out xmax, out ymax, out zmax, ref _ierr);
            }

            /// <summary>
            /// Get the geometrical dimension of the current model.
            /// </summary>
            /// <returns></returns>
            public static int GetDimension()
            {
                return IWrappers.GmshModelGetDimension(ref _ierr);
            }

            /// <summary>
            /// Get the name of the physical group of dimension `dim' and tag `tag'.
            /// </summary>
            public static void GetPhysicalName(int dim, int tag, out string name)
            {
                IntPtr nP;
                IWrappers.GmshModelGetPhysicalName(dim, tag, out nP, ref _ierr);

                name = Marshal.PtrToStringAnsi(nP);

                IguanaGmsh.Free(nP);
            }

            /// <summary>
            /// Get the model entities in the bounding box defined by the two points
            /// (`xmin', `ymin', `zmin') and (`xmax', `ymax', `zmax'). If `dim' is >= 0,
            /// return only the entities of the specified dimension(e.g.points if `dim'== 0).
            /// </summary>
            public static void GetEntitiesInBoundingBox(double xmin, double ymin, double zmin, double xmax, double ymax, double zmax, out int[] tags, int dim = 0)
            {
                IntPtr tP;
                long tags_n;
                IWrappers.GmshModelGetEntitiesInBoundingBox(xmin, ymin, zmin, xmax, ymax, zmax, out tP, out tags_n, dim, ref _ierr);

                tags = new int[tags_n];
                Marshal.Copy(tP, tags, 0, (int)tags_n);

                IguanaGmsh.Free(tP);
            }

            /// <summary>
            /// Remove the entity name `name' from the current model.
            /// </summary>
            public static void RemoveEntityName(string name)
            {
                IWrappers.GmshModelRemoveEntityName(name, ref _ierr);
            }

            /// <summary>
            /// Remove the physical groups `dimTags' of the current model. If `dimTags' is empty, remove all groups.
            /// </summary>
            public static void RemovePhysicalGroups(Tuple<int,int>[] dimTags) {
                var arr = IHelpers.FlattenIntTupleArray(dimTags);
                IWrappers.GmshModelRemovePhysicalGroups(arr, arr.LongLength, ref _ierr);
            }

            /// <summary>
            /// Remove the physical name `name' from the current model.
            /// </summary>
            public static void RemovePhysicalName(string name) {
                IWrappers.GmshModelRemovePhysicalName(name, ref _ierr);
            }

            /// <summary>
            /// Get the type of the entity of dimension `dim' and tag `tag'.
            /// </summary>
            public static void GetType(int dim, int tag, out string entityType) {
                IntPtr etP;
                IWrappers.GmshModelGetType(dim, tag, out etP, ref _ierr);

                entityType = Marshal.PtrToStringAnsi(etP);

                IguanaGmsh.Free(etP);
            }

        }
    }
}