using System;
using System.IO;
using System.Linq;

public class import
{
	private float[,] vert;
	private int[,] tri;
	public float objScl = 0.1f;
	private float [,] material;
	private float[,] output;
	private string obj;
	public float bgR = 0, bgG = 0, bgB = 0;
	private int vCount = 0, mCount = 0, fCount = 0;
	public float[,] import_obj(string filename = "")
	{
		if(filename != "")
		{
			//read in .obj file
			try
			{
				read_file(filename);
			}
			catch (Exception)
			{
				debug_cube();
			}
		}
		else
		{
			debug_cube();
		}
		output = new float[(tri.Length)/4,15];
		for(int i = 0; i < (tri.Length)/4; i++)
		{
			output[i,0] = vert[tri[i,0]-1,0];
			output[i,1] = vert[tri[i,0]-1,1];
			output[i,2] = vert[tri[i,0]-1,2];
			output[i,3] = vert[tri[i,1]-1,0];
			output[i,4] = vert[tri[i,1]-1,1];
			output[i,5] = vert[tri[i,1]-1,2];
			output[i,6] = vert[tri[i,2]-1,0];
			output[i,7] = vert[tri[i,2]-1,1];
			output[i,8] = vert[tri[i,2]-1,2];
			if(material.Length != 0 && tri[i,3] >= 0)
			{
				output[i,9] = material[tri[i,3],0]; //red
				output[i,10] = material[tri[i,3],1]; //green
				output[i,11] = material[tri[i,3],2]; //blue
				output[i,12] = material[tri[i,3],3]; //mirror
				output[i,13] = material[tri[i,3],4]; //glow
				output[i,14] = material[tri[i,3],5]; //opacity
			}
			else
			{
				output[i,9] = 0.95f; //red
				output[i,10] = 0.95f; //green
				output[i,11] = 0.95f; //blue
				output[i,12] = 0; //mirror
				output[i,13] = 0; //glow
				output[i,14] = 1; //opacity
			}
		}
		return output;
	}
	private void read_file(string filename)
	{
		obj = File.ReadAllText(filename);
		string[] result = obj.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
		for(int i = 0; i < result.Length; i++)
		{
			string X = (char.ToString(result[i][0]) + char.ToString(result[i][1]));
			if(X == "v ")
			{
				vCount++;
			}
			if(X == "m ")
			{
				mCount++;
			}
			if(X == "f ")
			{
				fCount++;
			}
		}
		vert = new float[vCount,3];
		tri = new int[fCount,4];
		material = new float[mCount,6];
		int vRow = 0, mRow = -1, fRow = 0;
		for(int l = 0; l < result.Length; l++)
		{
			string X = (char.ToString(result[l][0]) + char.ToString(result[l][1]));
			if(X == "b ")
			{
				string[] parseB = result[l].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				bgR = (float)Convert.ToDouble(parseB[1]);
				bgG = (float)Convert.ToDouble(parseB[2]);
				bgB = (float)Convert.ToDouble(parseB[3]);
			}
			if(X == "v ")
			{
				string[] parseV = result[l].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				vert[vRow,0] = (float)(Convert.ToDouble(parseV[1])*objScl);
				vert[vRow,1] = -(float)(Convert.ToDouble(parseV[2])*objScl);
				vert[vRow,2] = -(float)(Convert.ToDouble(parseV[3])*objScl);
				vRow++;
			}
			if(X == "m ")
			{
				mRow++;
				string[] parseM = result[l].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				material[mRow,0] = (float)(Convert.ToDouble(parseM[1]));
				material[mRow,1] = (float)(Convert.ToDouble(parseM[2]));
				material[mRow,2] = (float)(Convert.ToDouble(parseM[3]));
				material[mRow,3] = (float)(Convert.ToDouble(parseM[4]));
				material[mRow,4] = (float)(Convert.ToDouble(parseM[5]));
				material[mRow,5] = (float)(Convert.ToDouble(parseM[6]));
			}
			if(X == "f ")
			{
				string[] parseF = result[l].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				tri[fRow,0] = Convert.ToInt32(parseF[1]);
				tri[fRow,1] = Convert.ToInt32(parseF[2]);
				tri[fRow,2] = Convert.ToInt32(parseF[3]);
				tri[fRow,3] = mRow;
				fRow++;
			}
		}
	}
	private void debug_cube()
	{
		float size = 0.08f;
		//debug cube
		vert = new float[,]
		{
			{ size, size, size},
			{-size, size, size},
			{ size,-size, size},
			{-size,-size, size},
			{ size, size,-size},
			{-size, size,-size},
			{ size,-size,-size},
			{-size,-size,-size},
		};
		material = new float[,] {
			{1,0,0,0,0,1},
			{1,1,0,0,0,1},
			{1,0.5f,0,0,0,1},
			{1,1,1,0,0,1},
			{0,1,0,0,0,1},
			{0,0,1,0,0,1}
		};
		tri = new int[,]
		{
			{1,3,7,0},//1
			{1,7,5,0},//2
			{5,7,8,1},//3
			{5,8,6,1},//4
			{2,6,8,2},//5
			{2,8,4,2},//6
			{1,2,4,3},//7
			{1,4,3,3},//8
			{3,4,8,4},//9
			{3,8,7,4},//10
			{1,5,6,5},//11
			{1,6,2,5},//12
		};
	}
}