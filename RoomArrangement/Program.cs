﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAF;
using GAF.Operators;
using static System.Math;

namespace RoomArrangement
{
	class Program
	{
		static void Main(string[] args)
		{
			// Create the Rooms
			var NumOfRooms = 3;

			for (int i = 0; i < NumOfRooms; i++)
			{
				var r = new Room();
			}

			// This is just to test the code with three numbers
			Database.PairRooms(0, 1);
			Database.PairRooms(0, 2);
			Database.PairRooms(1, 2);

			var population = new Population(100, 9 * NumOfRooms, false, false);

			//create the genetic operators 
			var elite = new Elite(5);
			var crossover = new Crossover(0.85, true)
			{
				CrossoverType = CrossoverType.SinglePoint
			};
			var mutation = new BinaryMutate(0.08, true);

			//create the GA itself 
			var ga = new GeneticAlgorithm(population, GACompanions.CalculateFitness);

			//add the operators to the ga process pipeline 
			ga.Operators.Add(elite);
			ga.Operators.Add(crossover);
			ga.Operators.Add(mutation);

			// sunbscribe to Termination
			ga.OnRunComplete += ga_OnRunComplete;
			ga.OnGenerationComplete += ga_OnGenerationComplete;

			//run the GA 
			Console.WriteLine("Starting the GA");
			ga.Run(GACompanions.Terminate);

			Console.ReadKey();
		}

		private static void ga_OnGenerationComplete(object sender, GaEventArgs e)
		{
			var c = e.Population.GetTop(1)[0];
			Console.WriteLine("Fitness is {0}", c.Fitness);
		}

		// subscribing to the event of Completion
		static void ga_OnRunComplete(object sender, GaEventArgs e)
		{
			var c = e.Population.GetTop(1)[0];

			// copying some code from the Evaluate function. Is some refactoring in order?
			for (int i = 0; i < c.Count; i += 9)
			{
				int x = Convert.ToInt32(c.ToBinaryString(i, 4), 2);
				int y = Convert.ToInt32(c.ToBinaryString(i + 4, 4), 2);
				int oTemp = Convert.ToInt32(c.ToBinaryString(i + 8, 1), 2);

				bool o = Convert.ToBoolean(oTemp);

				var j = i / 9;

				Database.List[j].Adjust(x, y, o);
			}

			foreach (Room r in Database.List)
				Console.WriteLine("{0}'s coordinates are {1}. Its dimensions are {2}", r.Name, r.Anchor.ToString(), r.Space.ToString());

			Console.WriteLine("The GA is Done");
			Console.WriteLine("Fitness is {0}", c.Fitness);

			DrawSolution();
		}


		// Needs rework
		private static void DrawSolution()
		{
			var rooms = new Dictionary<Point, Rectangle>();

			foreach (Room r in Database.List)
			{
				rooms.Add(r.Anchor, r.Space);
			}

			var roomCounter = 0;
			var recXStart = 21;
			var inRectangle = false;
			var recXCount = 0;
			var currentRec = new Rectangle();
			var currentPnt = new Point();

			// Y loop
			for (int y = 0; y < 20; y++)
			{
				// X loop
				for (int x = 0; x < 20; x++)
				{
					var testPt = new Point(x, y);
					if (rooms.ContainsKey(testPt))
					{
						inRectangle = true;
						roomCounter++;
						recXStart = x;
						currentRec = rooms[testPt];
						currentPnt = testPt;
					}

					if (recXStart == x)
						inRectangle = true;

					if (inRectangle)
					{
						Console.Write("|_");
						recXCount++;
						if (recXCount >= currentRec.XDimension)
						{
							inRectangle = false;
						}
					}
					else
					{
						Console.Write(". ");
						recXCount = 0;
					}
				}
				Console.Write("\n");
			}
		}
	}
}
