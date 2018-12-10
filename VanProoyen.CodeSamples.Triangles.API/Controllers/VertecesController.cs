using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanProoyen.CodeSamples.Triangles.Core;

namespace VanProoyen.CodeSamples.Triangles.API.Controllers
{
    [Route("api/Triangles/Verteces")]
    [ApiController]
    public class VertecesController : ControllerBase
    {
        [HttpGet("{address}", Name = "GetVerteces")]
        public int[] Get(string address)
        {

            //ideally any web api or similar project is a thin wrapper around a core implementation library - making the solution more portable
            // here we're referencing the VanProoyen.CodeSamples.Triangles.Core library

            Triangle triangle = new Triangle(address);
            var vertices = triangle.Verteces.ToArray();
            return new int[]
            {
                vertices[0].X,
                vertices[0].Y,
                vertices[1].X,
                vertices[1].Y,
                vertices[2].X,
                vertices[2].Y,
            };

            //alternatively, we could use the simple implementation code that follows
            
            //    return GetVerteces(address);
            
        }

        //code for a very simple implementation example follows


        private int[] GetVerteces(string address)
        {
            //validate input
            if(!(address.Length == 2 || address.Length == 3))
            {
                throw new Exception("Input Error. Address should be composed of 2 or 3 characters, where the first character is a letter and the remaining character(s) represent a number.");
            }

            string rowRaw = address.ToUpper().Substring(0, 1);
            string colRaw = address.Substring(1);
            int row = ((int)rowRaw.ToCharArray()[0]) - 64;//ASCII offset
            bool isBottom = false;
            if (row < 1 || row > 26)
            {
                throw new Exception("Input Error. Address should be composed of 2 or 3 characters, where the first character is a letter and the remaining character(s) represent a number.");
            }
            int col = 0;
            if (int.TryParse(colRaw, out col))
            {
                isBottom = (col % 2 == 1);//if the column number is odd, the triangle is on the bottom otherwise it's on the top
                col = col / 2;
            }
            else
            {
                throw new Exception("Input Error. Address should be composed of 2 or 3 characters, where the first character is a letter and the remaining character(s) represent a number.");
            }
            int oppositeX = col;
            int oppositeY = row - 1;
            if (isBottom)
            {
                oppositeX = col - 1;
                oppositeY = row;
            }
            return new int[]
            {
                col - 1, //top X
                row - 1, //top Y
                oppositeX,
                oppositeY,
                col, //bottom x
                row, //bottom y
            };
        }

    }
}