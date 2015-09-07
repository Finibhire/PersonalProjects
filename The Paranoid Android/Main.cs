using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace The_Paranoid_Android
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        struct ElevatorData
        {
            public int Floor, Pos;
            
            public ElevatorData(int Floor, int Pos)
            {
                this.Floor = Floor;
                this.Pos = Pos;
            }

            public int GetHash()
            {
                return (Floor << 16) & Pos;
            }
        }

        private void btnMap_Click(object sender, EventArgs ea)
        {
            string[] inputs = { "1", "", "", "", "", "", "", "", "", "", "", ""};
            inputs = Console.ReadLine().Split(' ');
            int nbFloors = int.Parse(inputs[0]); // number of floors
            int width = int.Parse(inputs[1]); // width of the area
            int nbRounds = int.Parse(inputs[2]); // maximum number of rounds
            int exitFloor = int.Parse(inputs[3]); // floor on which the exit is found
            int exitPos = int.Parse(inputs[4]); // position of the exit on its floor
            int nbTotalClones = int.Parse(inputs[5]); // number of generated clones
            int nbAdditionalElevators = int.Parse(inputs[6]); // ignore (always zero)
            int nbElevators = int.Parse(inputs[7]); // number of elevators
            int[][] elevators = new int[nbFloors][];

            List<int>[] eleList = new List<int>[nbFloors];
            for (int i = 0; i < nbFloors; i++)
            {
                eleList[i] = new List<int>();
            }

            for (int i = 0; i < nbElevators; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int elevatorFloor = int.Parse(inputs[0]); // floor on which this elevator is found
                int elevatorPos = int.Parse(inputs[1]); // position of the elevator on its floor
                //ElevatorData e = new ElevatorData(int.Parse(inputs[0]), int.Parse(inputs[1]));

                eleList[elevatorFloor].Add(elevatorPos);
            }


            // game loop
            while (true)
            {
                inputs = Console.ReadLine().Split(' ');
                int cloneFloor = int.Parse(inputs[0]); // floor of the leading clone
                int clonePos = int.Parse(inputs[1]); // position of the leading clone on its floor
                string direction = inputs[2]; // direction of the leading clone: LEFT or RIGHT


                // Write an action using Console.WriteLine()
                // To debug: Console.Error.WriteLine("Debug messages...");

                Console.WriteLine("WAIT"); // action: WAIT or BLOCK
            }
        }

        //private void Map(List<int>[] elevators, int CreateCount)
        //{
        //    SortedList<int, Elevator>[] CombinedEle = new SortedList<int, Elevator>[elevators.Length];  // sorted by position, array index = bottomfloor
        //    // create the additional elevators that could be "created"
        //    // creating an elevator directly under a pre-existing elevator allows the android to approach the pre-existing
        //    // elevator from either the left or right side thus reducing the possible paths we have to calculate for in
        //    // comparison to creating an elevator to the left or right side of a pre-exiting elevator.

        //    int topFloor = elevators.Length - 1;
        //    CombinedEle[topFloor] = new SortedList<int, Elevator>();
        //    for (int i = 0; i < elevators[topFloor].Count; i++)
        //    {
        //        CombinedEle[topFloor].Add(elevators[topFloor][i], new Elevator(elevators[topFloor][i], topFloor, -1));
        //    }

        //    for (int i = elevators.Length - 2; i > 0; i--)
        //    {
        //        CombinedEle[i] = new SortedList<int, Elevator>();
        //        for (int j = 0; i < elevators[i].Count; i++)  // add existing elevators
        //        {
        //            Elevator n = new Elevator(elevators[i][j], i, i + 1);
        //            try
        //            {
        //                Elevator aboveEle = CombinedEle[i + 1][elevators[i][j]];
        //            }
        //            catch
        //            {
                    
        //            }
        //            CombinedEle[i].Add(elevators[i][j], new Elevator(elevators[i][j], i, i + 1));
        //        }
        //        for (int j = CombinedEle[i + 1].Count - 1; j >= 0; j--) // extend elevators from previous level
        //        {
        //            int pos = CombinedEle[i + 1][j].Position;
        //            if (CombinedEle[i + 1].BinarySearch()
        //            {
        //                Elevator ele = CombinedEle
        //            }
        //            CombinedEle[i].Add(elevators[i][j], new Elevator(elevators[i][j], topFloor, -1));
        //        }
        //        //CombinedEle[i].Sort();
        //    }
        //}


        //private void Map(int[][] elevators, int eleCreates, int turnsUsed, bool movingRight, int level, int pos, int levelWidth)
        //{
        //    int minPos = 1, maxPos = levelWidth - 1;

        //    for (int i = 0; i < elevators[level].Length; i++)
        //    {
        //        if (elevators[level][i] > minPos && elevators[level][i] < pos)
        //        {
        //            minPos = elevators[level][i];
        //        }
        //        else if (elevators[level][i] < maxPos )
        //    }
        //}

        private void button1_Click(object sender, EventArgs ea)
        {
            double i = 0.02d / 12d; // cost of living inflation
            double ci = i;          // current inflation vs base
            double r = 0.03d / 12d; // savings interest rate
            double p = 300000d;     // principle
            double x = 1500d;       // monthly living eXpenses

            double s = p;           // current savings

            for (int y = 1; y < 51; y++)
            {
                for (int yy = 1; yy < 13; yy++)
                {
                    ci *= i + 1d;
                    s -= x * (1d + ci);
                    s += s * r;
                }
                tbOut.Text += y.ToString() + ":  " + s.ToString("F1") + Environment.NewLine;
            }
        }
    }
}
