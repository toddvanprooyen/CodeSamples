using System;

namespace VanProoyen.CodeSamples.Triangles.Core
{
    /// <summary>
    /// This represents a point in 2 or 3 dimensional space
    /// 
    /// Sure, we could just use System.Drawing.Point, 
    /// but why load that whole assembly into memory just for that one simple model? 
    /// we can build our own super light weight model in just a few lines.
    /// 
    /// if we had any other need to use the System.Drawing assembly, then it would be a different matter
    /// 
    /// Keeping this as a struct instead of a full class will keep it as a value type 
    /// and on the stack instead of in the heap
    /// </summary>
    public struct Vertex
    {
        // using short instead of int to keep the memory footprint as small as possible.
        //using fields instead of properties removes overhead of underlying getters and setters 
        public short X;
        public short Y;
       
        public Vertex(short x, short y)
        {
            this.X = x;
            this.Y = y;
        }
        public Vertex(int x, int y)
            : this((short)x, (short)y) { }

        //overriding Equals and GetHashCode allow for easier evaluations and comparisons

        public override bool Equals(object obj)
        {
            bool matched = false;
            if (obj is Vertex)
            {
                Vertex compare = (Vertex)obj;//unbox

                matched = (this.X == compare.X && this.Y == compare.Y);
            }
            return matched;
        }
        public override int GetHashCode()
        {
            return (this.X.GetHashCode() + (this.Y.GetHashCode() * 1000));
        }
        public override string ToString()
        {
            return string.Format("({0}, {1})", this.X, this.Y);
        }

        /// <summary>
        /// Manual Json Serialization
        /// Typically, we would use a Json serialization library like Newtonsoft;
        /// but these use reflection and are therefore slow
        /// we're looking for high performance and this is a very simple model 
        /// so we're going to manually serialize to Json
        /// </summary>
        /// <returns>Json representation of Vertex</returns>
        public string Serialize()
        {
            System.Text.StringBuilder json = new System.Text.StringBuilder();
            json.Append("{ \"Vertex\": { \"X\": ");
            json.Append(this.X.ToString());
            json.Append(", \"Y\": ");
            json.Append(this.Y.ToString());
            json.Append(" } }");
            return json.ToString();
        }

        public static Vertex Deserialize(string json)
        {
            short x = 0;
            short y = 0;
            //parse json

            //get X

            //get Y

            

            return new Vertex(x, y);
        }
    }

}
