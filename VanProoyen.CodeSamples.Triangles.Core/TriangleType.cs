using System;


namespace VanProoyen.CodeSamples.Triangles.Core
{
   /// <summary>
   /// this simple enumeration allows us to logically segment a cell (intersection of row and column) 
   /// into a top triangle and a bottom triangle
   /// </summary>
    internal enum TriangleType
    {
        Top = 0,
        Bottom = 1,
        Undefined = 2//schrodinger's triangle
    }
}
