/*
 * Created by SharpDevelop.
 * User: princ
 * Date: 27/07/2024
 * Time: 15:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

public class Vector
{
	public float[,] rotMatrix(float theta, int axis)
	{
		float[,] matrix;
		switch(axis)
		{
			case 0:
			{
				matrix = new float[,]
				{{1,0,0},{0,(float)Math.Cos(theta),-(float)Math.Sin(theta)},{0,(float)Math.Sin(theta),(float)Math.Cos(theta)}};
				break;
			}
			case 1:
			{
				matrix = new float[,]
				{{(float)Math.Cos(theta),0,(float)Math.Sin(theta)},{0,1,0},{-(float)Math.Sin(theta),0,(float)Math.Cos(theta)}};
				break;
			}
			case 2:
			{
				matrix = new float[,]
				{{(float)Math.Cos(theta),-(float)Math.Sin(theta),0},{(float)Math.Sin(theta),(float)Math.Cos(theta),0},{0,0,1}};
				break;
			}
			default:
			{
				matrix = new float[,]
				{{1,0,0},{0,1,0},{0,0,1}};
				break;
			}
		}
		return matrix;
	}
	public Tuple<float,float,float> matVecMult(Tuple<float,float,float> point, float[,] matrix)
	{
		float X = point.Item1;
		float Y = point.Item2;
		float Z = point.Item3;
		float Xtrans = (matrix[0,0]*X)+(matrix[0,1]*Y)+(matrix[0,2]*Z);
		float Ytrans = (matrix[1,0]*X)+(matrix[1,1]*Y)+(matrix[1,2]*Z);
		float Ztrans = (matrix[2,0]*X)+(matrix[2,1]*Y)+(matrix[2,2]*Z);
		var output = new Tuple<float,float,float>(Xtrans,Ytrans,Ztrans);
		return output;
	}
	public float vecDot(Tuple<float,float,float> A, Tuple<float,float,float> B)
	{
		float AX = A.Item1;
		float AY = A.Item2;
		float AZ = A.Item3;
		float BX = B.Item1;
		float BY = B.Item2;
		float BZ = B.Item3;
		var output = AX*BX+AY*BY+AZ+BZ;
		return output;
	}
	public Tuple<float,float,float> vecCross(Tuple<float,float,float> A, Tuple<float,float,float> B)
	{
		float AX = A.Item1;
		float AY = A.Item2;
		float AZ = A.Item3;
		float BX = B.Item1;
		float BY = B.Item2;
		float BZ = B.Item3;
		float Xtrans = AY*BZ-BY*AZ;
		float Ytrans = AZ*BX-BZ*AX;
		float Ztrans = AX*BY-BX*AY;
		var output = new Tuple<float,float,float>(Xtrans,Ytrans,Ztrans);
		return output;
	}
	public Tuple<float,float,float> unit(Tuple<float,float,float> point)
	{
		float X = point.Item1;
		float Y = point.Item2;
		float Z = point.Item3;
		float Xtrans;
		float Ytrans;
		float Ztrans;
		float L = length(point);
		if(L == 0)
		{
			Xtrans = 0;
			Ytrans = 0;
			Ztrans = 0;
		}
		else
		{
			Xtrans = X/L;
			Ytrans = Y/L;
			Ztrans = Z/L;
		}
		var output = new Tuple<float,float,float>(Xtrans,Ytrans,Ztrans);
		return output;
	}
	public float length(Tuple<float,float,float> point)
	{
		float X = point.Item1;
		float Y = point.Item2;
		float Z = point.Item3;
		float L = (float)(Math.Sqrt(((X * X))+((Y * Y))+((Z * Z))));
		return L;
	}
	public Tuple<float,float,float> vecAdd(Tuple<float,float,float> A, Tuple<float,float,float> B)
	{
		float AX = A.Item1;
		float AY = A.Item2;
		float AZ = A.Item3;
		float BX = B.Item1;
		float BY = B.Item2;
		float BZ = B.Item3;
		float Xtrans = AX+BX;
		float Ytrans = AY+BY;
		float Ztrans = AZ+BZ;
		var output = new Tuple<float,float,float>(Xtrans,Ytrans,Ztrans);
		return output;
	}
	public Tuple<float,float,float> vecSub(Tuple<float,float,float> A, Tuple<float,float,float> B)
	{
		float AX = A.Item1;
		float AY = A.Item2;
		float AZ = A.Item3;
		float BX = B.Item1;
		float BY = B.Item2;
		float BZ = B.Item3;
		float Xtrans = AX-BX;
		float Ytrans = AY-BY;
		float Ztrans = AZ-BZ;
		var output = new Tuple<float,float,float>(Xtrans,Ytrans,Ztrans);
		return output;
	}
	public Tuple<float,float,float> centroid(Tuple<float,float,float> A, Tuple<float,float,float> B, Tuple<float,float,float> C)
	{
		float AX = A.Item1;
		float AY = A.Item2;
		float AZ = A.Item3;
		float BX = B.Item1;
		float BY = B.Item2;
		float BZ = B.Item3;
		float CX = C.Item1;
		float CY = C.Item2;
		float CZ = C.Item3;
		float Xtrans = (AX+BX+CX)/3;
		float Ytrans = (AY+BY+CY)/3;
		float Ztrans = (AZ+BZ+CZ)/3;
		var output = new Tuple<float,float,float>(Xtrans,Ytrans,Ztrans);
		return output;
	}
}