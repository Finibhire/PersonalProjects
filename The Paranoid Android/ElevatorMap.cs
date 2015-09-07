using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Paranoid_Android
{
    public class ElevatorMap
    {
        private readonly int levels;
        private readonly int maxCreateCount;
        private readonly Elevator[][] elevators;

        private bool finalized;
        private SortedDictionary<int, Elevator>[] setupElevators;


        public bool Finalized
        {
            get
            {
                return finalized;
            }
        }
        public int Levels
        {
            get {
                return levels;
            }
        }
        public int MaxCreateCount
        {
            get{
                return maxCreateCount;
            }
        }

        public ElevatorMap(int Levels, int MaxCreateCount)
        {
            this.finalized = false;
            this.levels = Levels;
            this.maxCreateCount = MaxCreateCount;

            elevators = new Elevator[Levels][];

            setupElevators = new SortedDictionary<int, Elevator>[Levels];
            for (int i = 0; i < Levels; i++)
			{
                setupElevators[i] = new SortedDictionary<int, Elevator>();
			}
        }

        public void AddElevator(int levelIndex, int position)
        {
            setupElevators[levelIndex][position] = new Elevator(position, false);
            int ghostCreateCount = maxCreateCount;
            if (levelIndex - maxCreateCount < 0) 
                ghostCreateCount = levelIndex;
            for (int i = levelIndex - ghostCreateCount; i < levelIndex; i++)
			{
			    if (!setupElevators[i].ContainsKey(position))
                {
                    setupElevators[i].Add(position, new Elevator(position, true));
                }
			}
        }

        public void FinalizeMap()
        {
            if (!finalized)
            {
                for (int i = 0; i < levels; i++)
                {
                    elevators[i] = setupElevators[i].Values.ToArray();
                }
                setupElevators = null;  // for garbage collection
                finalized = true;
            }
        }

        /// <summary>
        /// Returns all the points that can be traveled to from the point given.  If the point given is the 
        /// entrance/location of an elevator the only point that can be traveled to is the corresponding point on the 
        /// level direclty above the point supplied.
        /// </summary>
        public ReadOnlyCollection<MapPosition> GetConnectedPositions(int levelIndex, int position)
        {
            ReadOnlyCollection<MapPosition> rtn;

            if (elevators[levelIndex].Length > 0)
            {
                int idx;
                Elevator baseEle = null;
                if (levelIndex > 0)
                {
                    int baseIdx = elevators[levelIndex - 1].BinarySearch(position);
                    if (baseIdx >= 0)
                    {
                        baseEle = elevators[levelIndex - 1][baseIdx];
                        if (baseEle.ConnectedPositions != null)
                        {
                            return baseEle.ConnectedPositions;
                        }
                    }
                }
                
                List<MapPosition> posList = new List<MapPosition>();
                idx = elevators[levelIndex].BinarySearch(position);
                if (idx >= 0)  // there is an elevator at this location so one can only go up at this point
                {
                    if (levelIndex < levels - 1)  // going up past the last level doesn't work so well
                    {
                        posList.Add(new MapPosition(levelIndex, position, elevators[levelIndex][idx].Ghost));
                    }
                    rtn = new ReadOnlyCollection<MapPosition>(posList);
                    if (baseEle != null)
                    {
                        baseEle.ConnectedPositions = rtn;
                    }
                    return rtn;
                }
                else
                {
                    idx = ~idx;
                    int i = idx - 1;
                    while (i >= 0)
                    {
                        posList.Add(new MapPosition(levelIndex, elevators[levelIndex][i].Position, elevators[levelIndex][i].Ghost));
                        if (!elevators[levelIndex][i].Ghost)
                        {
                            break;
                        }
                    }
                    i = idx;
                    while (i < elevators[levelIndex].Length)
                    {
                        posList.Add(new MapPosition(levelIndex, elevators[levelIndex][i].Position, elevators[levelIndex][i].Ghost));
                        if (!elevators[levelIndex][i].Ghost)
                        {
                            break;
                        }
                    }

                    posList.Sort((x, y) => x.Position - y.Position);
                    rtn = new ReadOnlyCollection<MapPosition>(posList);
                    if (baseEle != null)
                    {
                        baseEle.ConnectedPositions = rtn;
                    }
                    return rtn;
                }
            }
            else  // this level is a dead end apparently
            {
                return new ReadOnlyCollection<MapPosition>(new MapPosition[] { });
            }
        }

        public struct MapPosition
        {
            public readonly int LevelIndex;
            public readonly int Position;
            public readonly bool CreateElevatorRequired;

            public MapPosition(int LevelIndex, int Position, bool CreateElevatorRequired = false)
            {
                this.LevelIndex = LevelIndex;
                this.Position = Position;
                this.CreateElevatorRequired = CreateElevatorRequired;
            }
        }

        private class Elevator : IComparable<Elevator>, IComparable<int>
        {
            public int Position;
            public bool Ghost;
            public ReadOnlyCollection<MapPosition> ConnectedPositions;

            public Elevator(int position, bool ghost)
            {
                this.Position = position;
                this.Ghost = ghost;
            }

            public int CompareTo(Elevator other)
            {
                return Position.CompareTo(other.Position);
            }
            public int CompareTo(int otherPos)
            {
                return Position.CompareTo(otherPos);
            }
        }
    }

    /// <summary>
    /// Provides BinarySearch Capabilities to IList<> that only require a key and a Comparison or a key and appropriate
    /// IComparable<> interface on the IList<> generic type.
    /// 
    /// I wrote these because prior to this, I needed to create an entirely new object every time I wanted to lookup the 
    /// sorting key in a sorted array/list.  Thus I would need to process the constructor and allocate a lot of extra memory
    /// for all the parts of the object that I didn't actually need.
    /// </summary>
    public static class BinarySearchExtentions
    {
        /// <summary>
        /// Represents the method that compares two objects of different types.
        /// </summary>
        /// <typeparam name="T1">The type of the first object to compare.</typeparam>
        /// <typeparam name="T2">The type of the second object to compare.</typeparam>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Less than 0     = x is less than y.
        /// 0               = x equals y.
        /// Greater than 0  = x is greater than y.
        /// </returns>
        public delegate int Comparison<in T1, in T2>(T1 x, T2 y);

        /// <summary>
        /// Searches the entire sorted List<listType> for an element using the supplied comparison function and returns the 
        /// zero-based index of the element.
        /// </summary>
        /// <returns>
        /// The zero-based index of item in the sorted List<listType>, if item is found; otherwise, a negative number that is the 
        /// bitwise complement of the index of the next element that is larger than item or, if there is no larger element, 
        /// the bitwise complement of Count.
        /// </returns>
        public static int BinarySearch<listType, keyType>(this IList<listType> list, keyType key, Comparison<listType, keyType> compare)
        {
            return PrivateBinarySearch<listType, keyType>(list, key, 0, list.Count - 1, compare);
        }
        public static int BinarySearch<listType, keyType>(this IList<listType> list, keyType key, int startIndex, Comparison<listType, keyType> compare)
        {
            return PrivateBinarySearch<listType, keyType>(list, key, startIndex, startIndex + list.Count - 1, compare);
        }
        public static int BinarySearch<listType, keyType>(this IList<listType> list, keyType key, int startIndex, int length, Comparison<listType, keyType> compare)
        {
            return PrivateBinarySearch<listType, keyType>(list, key, startIndex, startIndex + length - 1, compare);
        }
        private static int PrivateBinarySearch<listType, keyType>(IList<listType> list, keyType key, int imin, int imax, Comparison<listType, keyType> compare)
        {
            while (imin < imax)
            {
                int imid = (imin + imax) / 2;

                if (compare(list[imid], key) < 0)
                {
                    imin = imid + 1;
                }
                else
                {
                    imax = imid;
                }
            }

            int r = compare(list[imin], key);
            if (r == 0)
            {
                return imin;
            }
            else if (r < 0)
            {
                return ~imin;
            }
            return ~(imin + 1);
        }

        /// <summary>
        /// Searches the entire sorted List<listType> for an element using the IComparable<keyType> interface supplied by listType
        /// and returns the zero-based index of the element.
        /// </summary>
        /// <returns>
        /// The zero-based index of item in the sorted List<listType>, if item is found; otherwise, a negative number that is the 
        /// bitwise complement of the index of the next element that is larger than item or, if there is no larger element, 
        /// the bitwise complement of Count.
        /// </returns>
        public static int BinarySearch<listType, keyType>(this IList<listType> list, keyType key, int startIndex = 0)
            where listType : IComparable<keyType>
        {
            return PrivateBinarySearch<listType, keyType>(list, key, startIndex, startIndex + list.Count - 1);
        }
        public static int BinarySearch<listType, keyType>(this IList<listType> list, keyType key, int startIndex, int length)
            where listType : IComparable<keyType>
        {
            return PrivateBinarySearch<listType, keyType>(list, key, startIndex, startIndex + length - 1);
        }
        private static int PrivateBinarySearch<listType, keyType>(IList<listType> list, keyType key, int imin, int imax)
            where listType : IComparable<keyType>
        {
            while (imin < imax)
            {
                int imid = (imin + imax) / 2;

                if (list[imid].CompareTo(key) < 0)
                {
                    imin = imid + 1;
                }
                else
                {
                    imax = imid;
                }
            }

            int r = list[imin].CompareTo(key);
            if (r == 0)
            {
                return imin;
            }
            else if (r < 0)
            {
                return ~imin;
            }
            return ~(imin + 1);
        }
    }
}
