using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanProoyen.CodeSamples.Triangles.Core;

namespace VanProoyen.CodeSamples.Triangles.API.Controllers
{
    [Route("api/Triangles/Address")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        // GET: api/Triangle/Address/[1 1 2 2 1 2]
        [HttpGet("{verteces}", Name = "GetAddress")]
        public string Get(int[] verteces)
        {
            //ideally any web api or similar project is a thin wrapper around a core implementation library - making the solution more portable
            // here we're referencing the VanProoyen.CodeSamples.Triangles.Core library
            Triangle triangle = new Triangle(verteces[0], verteces[1], verteces[2], verteces[3], verteces[4], verteces[5]);
            return triangle.Address;
            // we could just do this, but putting several operations in one line does make debugging a bit more difficult
            //return new Triangle(Ax, Ay, Bx, By, Cx, Cy).Address;

            //alternatively, we could use the simple implementation code that follows
            //return GetAddress(verteces);
        }

        //code for a very simple implementation example follows

        private string GetAddress(int[] verteces)
        {
            //simple input validation
            if (verteces.Length != 6)
            {
                return "Invalid Input. Expected input is 3 verteces composed of an array of six integer values";
            }
            List<Vertex> vertecesList = new List<Vertex>()
            {
                new Vertex(verteces[0], verteces[1]),
                new Vertex(verteces[2], verteces[3]),
                new Vertex(verteces[4], verteces[5])
            };

            vertecesList = (from vertex in vertecesList
                            orderby vertex.X, vertex.Y
                            select vertex).ToList();
            Vertex top = vertecesList[0];
            Vertex opposite = vertecesList[1];
            Vertex bottom = vertecesList[2];
            
            char row = (char)(bottom.Y + 64);//ASCII Alpha character offset
            int col = bottom.X * 2;

            //determine if this is the top triangle or the bottom triangle
            // if bottom, subtract 1 from column
            if (opposite.Y == bottom.Y)
            {
                col--;
            }
            return string.Format("{0}{1}", row.ToString(), col.ToString());
        }
    }
    
    internal class Vertex
    {
        internal int X;
        internal int Y;
        internal Vertex(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}