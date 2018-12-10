using System;
using System.Collections.Generic;
using System.Linq;
namespace VanProoyen.CodeSamples.Triangles.Core
{

    /// <summary>
    ///  for this example, we are going to prioritize performance over code readability/maintainability
    ///  we will minimize memory utilization by using shorts instead of ints 
    ///  and we will use primitive evaluations instead of Linq, Lamba statements or other advanced language features
    ///  to prevent spinning up secondary threads and to generally keep the cyclomatic complexity to a minimum
    ///  
    ///  in a great many business cases, developer time is much more valuable than processor time; 
    ///  so we often build out code with poor performance in favor of improved code maintainability or hitting productivity milestones
    ///  however there are cases where a piece of code is going to be exercised
    /// </summary>
    public class Triangle
    {
        #region Properties and Fields

        private const short ASCIIoffset = 64;
        private bool populated;//not nullable and therefore false by default

        /// <summary>
        /// note that verteces values are not scaled here - scaling (multiplying by 10px) will be handled on the client in the presentation layer
        /// </summary>
        public List<Vertex> Verteces
        {
            get
            {
                
                if (_verteces is null)
                {
                    _verteces = new List<Vertex>();
                    if (populated)
                    { // if the triangle has already been constructed,
                      // then we need to calculated the verteces from the address
                        findVerteces();
                    } 
                    
                }
                return _verteces;
            }
            private set
            {
                _verteces = value;
            }
        }
        public string Address
        {
            get
            {
                return getAddress(Row, Column);
            }
        }

        private List<Vertex> _verteces;

        private Vertex TopVertex { get; set; }

        private Vertex BottomVertex { get; set; }
        
        public Vertex OppositeVertex { get; set; }
        
        
        internal TriangleType TriangleType
        {
            get
            {
                TriangleType result = TriangleType.Undefined;
                // if we have 3 Verteces, figure out if this is top or bottom from verteces
                if (Verteces.Count == 3)
                {
                    if (OppositeVertex.Y == TopVertex.Y)
                    {
                        result = TriangleType.Top;
                    }
                    else if (OppositeVertex.Y == BottomVertex.Y)
                    {
                        result = TriangleType.Bottom;
                    }
                }
                // if we don't have Verteces, figure out from Row and Column values 
                else if (Row > 0 && Column > 0)
                {
                    result = (TriangleType)(Column % 2);// if column ordinal (1 based ordinal system) is even (divisable by 2), then triangle is Top, if odd then Bottom
                }
                return result;  
            }

        }
        private TriangleType _triangleType;
        
        private short Row { get; set; }
        private short Column { get; set; }

        #endregion

        #region Constructors

        #region Address & Row Column Constructors

        public Triangle(string Address)
        {
            short row = 0;
            short col = 0;
            //typically we would call a get row method and a separate get column method
            // but that would end up duplicating most of the work we need to do here
            // so instead, we're using the out keywords to get two values out of one method
            // to minimize cyclomatic complexity of the operation 
            // and to show a use case for little used language feature
            parseAddress(Address, out row, out col);
            Row = row;
            Column = col;

            populated = true;

        }
        public Triangle(short row, short column)
        {
            Row = row;
            Column = column;

            populated = true;
        }

        #endregion
        
        #region Verteces Constructors
        public Triangle(Vertex a, Vertex b, Vertex c)
        {
            Verteces.Add(a);
            Verteces.Add(b);
            Verteces.Add(c);
            populated = true;
            //figure out which vertex is top, bottom and opposite (as in opposite the hypotunuse)
            sortVerteces();
        }
        public Triangle(short Ax, short Ay, short Bx, short By, short Cx, short Cy)
         : this(new Vertex(Ax, Ay), new Vertex(Bx, By), new Vertex(Cx, Cy)) { }

        public Triangle(int Ax, int Ay, int Bx, int By, int Cx, int Cy)
            : this((short)Ax, (short)Ay, (short)Bx, (short)By, (short)Cx, (short)Cy) { }
        #endregion

        #endregion

        #region Address to Verteces Methods

        #region Methods called by Constructors with Address 
        private void parseAddress(string address, out short row, out short column)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new FormatException("Address cannot be empty");
            }

            char rowCharacter = address.ToCharArray()[0];
            string colText = address.Substring(1);
            row = translateAlphaToOrd(rowCharacter);
            if (!short.TryParse(colText, out column))
            {
                throw new FormatException("Address suffix must be a number");
            }
        }


        private short translateAlphaToOrd(char alpha)
        {
            if (char.IsLetter(alpha))
            {
                alpha = char.ToUpper(alpha);
                return (short)(((short)alpha) - ASCIIoffset);
            }
            else
            {
                throw new IndexOutOfRangeException("Address Row Character must be a letter to be translated to a Row Ordinal.");
            }
        }
        #endregion
        
        #region Methods Called by Verteces Getter
        private void findVerteces()
        {
            // to get here, we've created a triangle using either a cell address or a row and column
            TopVertex = new Vertex(((Column / 2) - 1), Row - 1);
            BottomVertex = new Vertex((Column / 2), Row);
            if (TriangleType == TriangleType.Top)
            {
                OppositeVertex = new Vertex((Column / 2), Row - 1);
            }
            else if (TriangleType == TriangleType.Bottom)
            {
                OppositeVertex = new Vertex(((Column / 2) - 1), Row);
            }
            Verteces.Add(TopVertex);
            Verteces.Add(OppositeVertex);
            Verteces.Add(BottomVertex);
        }



        #endregion

        #endregion

        #region Verteces to Address Methods

        #region Methods called by Constructors with Verteces
        private void sortVerteces()
        {// we know that at this point there will be exactly 3 verteces in our collection
            if (_verteces.Count != 3)
            {
                throw new Exception("A Triangle can only have 3 Verteces.");
            }
            //we could just do this in Linq - but that would not be performant - and too easy - but much easier to follow
            // see sortVertecesWithLinq method below for a Linq example

            // instead we are going to implement a quicksort algorithm and apply it first to the Vertex X and then to the Vertex Y
            // this prioritizes the sort on Y similar to a T-SQL clause like "Order By Y asc, X asc
            _verteces = quickSortOnX(_verteces.ToArray()).ToList();
            _verteces = quickSortOnY(_verteces.ToArray()).ToList();


            TopVertex = _verteces[0];
            OppositeVertex = _verteces[1];
            BottomVertex = _verteces[2];

        }

        private Vertex[] quickSortOnX(Vertex[] originalList)
        {
            Vertex[] sorted = new Vertex[originalList.Length];
            Vertex[] lessThan = new Vertex[originalList.Length];
            Vertex[] same = new Vertex[originalList.Length];
            Vertex[] greaterThan = new Vertex[originalList.Length];
            //first we evaluate whether there are enough elements to sort
            if (originalList.Length <= 1)
            {
                return originalList;
            }
            else
            {
                // now we evaluate each element and segment the elements into groups
                int sortedPosition = 0;
                int lessThanPosition = 0;
                int samePosition = 0;
                int greaterThanPosition = 0;
                int len = originalList.Length;

                Vertex pivot = originalList[0];
                same[samePosition] = pivot;
                samePosition++;
                for (int index = 1; index < len; index++)
                {
                    Vertex evaluated = originalList[index];
                    if (pivot.X > evaluated.X)
                    {
                        lessThan[lessThanPosition] = evaluated;
                        lessThanPosition++;
                    }
                    else if (pivot.X == evaluated.X)
                    {
                        same[samePosition] = evaluated;
                        samePosition++;
                    }
                    else //if pivot.X < evaluated.X -- this is the only remaining possibility so no need for another comparison
                    {
                        greaterThan[greaterThanPosition] = evaluated;
                        greaterThanPosition++;
                    }
                }
                // recursive call to sort our less than and greater than collections
                lessThan = quickSortOnX(lessThan);
                greaterThan = quickSortOnX(greaterThan);

                //now we take the segmented collections and put them back together in a sorted collection
                for (int index = 0; index < lessThanPosition; index++)
                {
                    sorted[sortedPosition] = lessThan[index];
                    sortedPosition++;
                }
                for (int index = 0; index < samePosition; index++)
                {
                    sorted[sortedPosition] = same[index];
                    sortedPosition++;
                }
                for (int index = 0; index < greaterThanPosition; index++)
                {
                    sorted[sortedPosition] = greaterThan[index];
                    sortedPosition++;
                }
                
                return sorted;
            }
        }

        private Vertex[] quickSortOnY(Vertex[] originalList)
        {
            Vertex[] sorted = new Vertex[originalList.Length];
            Vertex[] lessThan = new Vertex[originalList.Length];
            Vertex[] same = new Vertex[originalList.Length];
            Vertex[] greaterThan = new Vertex[originalList.Length];

            if (originalList.Length <= 1)
            {
                return originalList;
            }
            else
            {
                int sortedPosition = 0;
                int lessThanPosition = 0;
                int samePosition = 0;
                int greaterThanPosition = 0;
                int len = originalList.Length;

                Vertex pivot = originalList[0];
                same[samePosition] = pivot;
                samePosition++;
                for (int index = 1; index < len; index++)
                {
                    Vertex evaluated = originalList[index];
                    if (pivot.Y > evaluated.Y)
                    {
                        lessThan[lessThanPosition] = evaluated;
                        lessThanPosition++;
                    }
                    else if (pivot.Y == evaluated.Y)
                    {
                        same[samePosition] = evaluated;
                        samePosition++;
                    }
                    else //if pivot.X < evaluated.X -- this is the only remaining possibility so no need for another comparison
                    {
                        greaterThan[greaterThanPosition] = evaluated;
                        greaterThanPosition++;
                    }
                }
                // recursive
                lessThan = quickSortOnX(lessThan);
                greaterThan = quickSortOnX(greaterThan);
                for (int index = 0; index < lessThanPosition; index++)
                {
                    sorted[sortedPosition] = lessThan[index];
                    sortedPosition++;
                }
                for (int index = 0; index < samePosition; index++)
                {
                    sorted[sortedPosition] = same[index];
                    sortedPosition++;
                }
                for (int index = 0; index < greaterThanPosition; index++)
                {
                    sorted[sortedPosition] = greaterThan[index];
                    sortedPosition++;
                }

                return sorted;
            }
        }


        private void sortVertecesWithLinq()
        {// we know that at this point there will be exactly 3 verteces in our collection
            if (_verteces.Count != 3)
            {
                throw new Exception("A Triangle can only have 3 Verteces.");
            }
            _verteces = (from vertex in _verteces
                         orderby vertex.Y ascending,
                                 vertex.X ascending
                         select vertex).ToList();
            //ToList() acts like an await , insuring that the linq child thread is finished working before this main thread continues

            TopVertex = _verteces[0];
            OppositeVertex = _verteces[1];
            BottomVertex = _verteces[2];
        }
        #endregion

        #region Methods called by Address Getter
        private string findAddress()
        {
            Row = BottomVertex.Y;
            Column = (short)(BottomVertex.X * 2);
            if (TriangleType == TriangleType.Bottom)
            {
                Column--;
            }
            return getAddress(Row, Column);
        }

        private string getAddress(short row, short column)
        {
            //translate row ordinal to alpha representation 
            //- once we have more than 27 columns we blow out or range
            char rowAlpha = translateOrdinalToAlpha(row);

            return string.Format("{0}{1}", rowAlpha, column);
        }
        private char translateOrdinalToAlpha(short ord)
        {
            if (ord > 0 && ord < 28)
            {
                return (char)(ord + ASCIIoffset);
            }
            else
            {
                throw new IndexOutOfRangeException("Row ordinal value must be between 0 and 28 to be translated to Alpha Character.");
            }
        }

        #endregion

        #endregion



    }
}
