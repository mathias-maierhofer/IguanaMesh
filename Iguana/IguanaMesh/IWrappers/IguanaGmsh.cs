﻿using System;
using System.Runtime.InteropServices;

namespace Iguana.IguanaMesh.IWrappers
{
    public static partial class IguanaGmsh
    {
        internal static int _ierr = 0;

        /// <summary>
        /// Initialize IguanaGmsh. This must be called before any call to the other functions in the API.
        /// </summary>
        public static void Initialize()
        {
            IntPtr argv = IntPtr.Zero;
            IWrappers.GmshInitialize(0, ref argv, Convert.ToInt32(true), ref _ierr);
            Marshal.FreeCoTaskMem(argv);
        }

        /// <summary>
        /// Free a gmsh pointer.
        /// </summary>
        /// <param name="ptr"> Pointer</param>
        public static void Free(IntPtr ptr)
        {
            IWrappers.GmshFree(ptr);
        }

        /// <summary>
        /// Finalize gmsh.
        /// </summary>
        public static void FinalizeGmsh()
        {
            IWrappers.GmshFinalize(ref _ierr);
        }

        /// <summary>
        /// Open a file. Handling of the file depends on its extension and/or its contents: opening a file with model data will create a new model.
        /// </summary>
        /// <param name="fileName"></param>
        public static void Open(string fileName)
        {
            IWrappers.GmshOpen(fileName, ref _ierr);
        }

        /// <summary>
        /// Write a file. The export format is determined by the file extension.
        /// </summary>
        /// <param name="fileName"> Name of the file </param>
        public static void Write(string fileName)
        {
            IWrappers.GmshWrite(fileName, ref _ierr);
        }

        /// <summary>
        /// Clear all loaded models and post-processing data, and add a new empty model.
        /// </summary>
        public static void Clear()
        {
            IWrappers.GmshClear(ref _ierr);
        }

        /// <summary>
        /// Merge a file. Equivalent to the `File->Merge' menu in the Gmsh app.
        /// Handling of the file depends on its extension and/or its contents.Merging
        /// a file with model data will add the data to the current model.
        /// </summary>
        /// <param name="fileName"></param>
        public static void Merge(string fileName) {
            IWrappers.GmshMerge(fileName, ref _ierr);
        }


        public static partial class Option
        {

            /// <summary>
            /// Set a numerical option to `value'. `name' is of the form "category.option" or "category[num].option". 
            /// Available categories and options are listed in the IguanaGmsh reference manual.
            /// By default IguanaGmsh will not print out any messages: in order to output messages on the terminal, just set the "General.Terminal" option to 1:
            /// </summary>
            /// <param name="name"></param>
            /// <param name="value"></param>
            public static void SetNumber(string name, double value)
            {
                IWrappers.GmshOptionSetNumber(name, value, ref _ierr);
            }

            /// <summary>
            /// Get the `value' of a numerical option. 
            /// </summary>
            /// <param name="name"> `name' is of the form "category.option" or "category[num].option". Available categories and options are listed in the IguanaGmsh reference manual. </param>
            /// <returns></returns>
            public static double GetNumber(string name)
            {
                IntPtr ptr = new IntPtr(0);
                IWrappers.GmshOptionGetNumber(name, ptr, ref _ierr);
                double val = IWrappers.IntPtrToDouble(ptr);
                Marshal.FreeCoTaskMem(ptr);
                return val;
            }

            /// <summary>
            /// Set a string option to `value'. `name' is of the form "category.option" or
            /// "category[num].option". Available categories and options are listed in the Gmsh reference manual.
            /// </summary>
            public static void SetString(string name, string value) {
                IWrappers.GmshOptionSetString(name, value, ref _ierr);
            }

            /// <summary>
            /// Get the `value' of a string option. `name' is of the form "category.option"
            /// or "category[num].option". Available categories and options are listed in
            /// the Gmsh reference manual.
            /// </summary>
            public static void GetString(string name, out string value) {
                IntPtr vP;
                IWrappers.GmshOptionGetString(name, out vP, ref _ierr);
                value = Marshal.PtrToStringAnsi(vP);

                IguanaGmsh.Free(vP);
            }

            /// <summary>
            ///  Set a color option to the RGBA value (`r', `g', `b', `a'), where where `r',
            ///  `g', `b' and `a' should be integers between 0 and 255. `name' is of the
            ///  form "category.option" or "category[num].option". Available categories and
            ///  options are listed in the Gmsh reference manual, with the "Color." middle string removed.
            /// </summary>
            public static void SetColor(int[] name, int r, int g, int b, int a) {
                IWrappers.GmshOptionSetColor(name, r, g, b, a, ref _ierr);
            }

            /// <summary>
            /// Get the `r', `g', `b', `a' value of a color option. `name' is of the form
            /// "category.option" or "category[num].option". Available categories and
            /// options are listed in the Gmsh reference manual, with the "Color." middle
            /// string removed.
            /// </summary>
            public static void GetColor(string name, out int r, out int g, out int b, out int a) {
                IWrappers.GmshOptionGetColor(name, out r, out g, out b, out a, ref _ierr);
            }
        }
    }
}