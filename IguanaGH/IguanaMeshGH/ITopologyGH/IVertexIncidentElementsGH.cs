﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ITopologyGH
{
    public class IVertexIncidentElementsGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IVertexIncidentElementsGH class.
        /// </summary>
        public IVertexIncidentElementsGH()
          : base("Vertex Incident Elements", "VIE",
              "Retrieve the incident elements of a given vertex.",
              "Iguana", "Topology")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "The Iguana mesh to explore.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Vertex Key", "iK", "The key of the vertex.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Elements Keys", "ID", "Incident elements keys.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            int key = -1;
            DA.GetData(0, ref mesh);
            DA.GetData(1, ref key);

            List<int> eIdx = mesh.Topology.GetVertexIncidentElements(key);

            DA.SetDataList(0, eIdx);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3079adac-6a4d-45f7-aaec-1071aea475b1"); }
        }
    }
}