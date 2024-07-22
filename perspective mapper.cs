using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using plotter_2D;

public class projection
{
	public Plotter vector;
	private float[,] tris;
	private bool loaded = false;
	public float[][] faces;
	private int count;
	public float camX = 0, camY = 0, camZ = 0;
	public float rotX = 0, rotY = 0, rotZ = 0;
	public float angX = 0, angY = 0, angZ = 0;
	private float aspect = 0;
	private float nearClip = 0.001f, farClip = 25f;
	private float ambient = 0.25f;
	private int background = 6;
	private float scale = 2;
	public float fov = 45;
	public import import = new import();
	public projection(int w, int h)
	{
		 vector = new Plotter(w,h);
	}
	public void drawObj(string obj = "", bool fill = true)
	{
		if(!loaded)
		{
			tris = import.import_obj(obj);
			count = tris.Length/15;
			faces = new float[count][];
			loaded = true;
		}
		vector.render.clearImg(Convert.ToInt32(import.bgR*255),Convert.ToInt32(import.bgG*255),Convert.ToInt32(import.bgB*255));
		Parallel.For(0,count,w => { projectTri(w);});
		faces = faces.OrderByDescending(entry => entry[9]).ToArray();
		for(int i = 0; i < count; i++)
		{
			drawTri(i,fill);
		}
		vector.render.update();
	}
	private void projectTri(int face = 0)
	{
		//defining tuples for ease of passing data between functions
		var point1 = new Tuple<float,float,float>(tris[face,0],tris[face,1],tris[face,2]);
		var point2 = new Tuple<float,float,float>(tris[face,3],tris[face,4],tris[face,5]);
		var point3 = new Tuple<float,float,float>(tris[face,6],tris[face,7],tris[face,8]);
		//transforming the 3d space into a 2d projection
		var transformA = (rotate(rotate(rotate(translate(rotate(rotate(rotate(point1,5),4),3)),2),1),0));
		var transformB = (rotate(rotate(rotate(translate(rotate(rotate(rotate(point2,5),4),3)),2),1),0));
		var transformC = (rotate(rotate(rotate(translate(rotate(rotate(rotate(point3,5),4),3)),2),1),0));
		var projA = perspective(transformA);
		var projB = perspective(transformB);
		var projC = perspective(transformC);
		//calculating the normal vector
		var vecAB = vecSub(transformB,transformA);
		var vecAC = vecSub(transformC,transformA);
		var vecN = unit(vecCross(vecAB,vecAC));
		var luma = unit(new Tuple<float,float,float>(-3,5,3));
		float D = vecDot(vecN,luma);
		var position =  new Tuple<float,float,float>(camX,camY,camZ);
		var direction = vecSub(centroid(projA,projB,projC),position);
		float distance = centroid(projA,projB,projC).Item3;
		float K = vecDot(unit(direction),vecN);
		faces[face] = new float[] {
			projA.Item1,
			projA.Item2,
			projB.Item1,
			projB.Item2,
			projC.Item1,
			projC.Item2,
			distance,
			K,
			D,
			face};
	}
	private void drawTri(int face = 0,bool fill = true)
	{
		if(faces[face][8] > 0)
		{
			faces[face][8] = 0;
		}
		//rescalling from a normalized space to screen space
			long[] scrn1 = {0,0};
			long[] scrn2 = {0,0};
			long[] scrn3 = {0,0};
		try
		{
			scrn1 = new long[] {Convert.ToInt64((faces[face][0]+(scale/2))*(vector.Pwidth/scale)),Convert.ToInt64((faces[face][1]+(scale/2))*(vector.Pheight/scale))};
			scrn2 = new long[] {Convert.ToInt64((faces[face][2]+(scale/2))*(vector.Pwidth/scale)),Convert.ToInt64((faces[face][3]+(scale/2))*(vector.Pheight/scale))};
			scrn3 = new long[] {Convert.ToInt64((faces[face][4]+(scale/2))*(vector.Pwidth/scale)),Convert.ToInt64((faces[face][5]+(scale/2))*(vector.Pheight/scale))};
		}
		catch
		{
			scrn1 = new long[] {0,0};
			scrn2 = new long[] {0,0};
			scrn3 = new long[] {0,0};
		}
		//coloring
		float colorMult = (Math.Abs(faces[face][8]));
		if(colorMult < tris[Convert.ToInt32(faces[face][9]),13])
		{
			colorMult = tris[Convert.ToInt32(faces[face][9]),13];
		}
		float R1 = tris[Convert.ToInt32(faces[face][9]),9];
		float G1 = tris[Convert.ToInt32(faces[face][9]),10];
		float B1 = tris[Convert.ToInt32(faces[face][9]),11];
		float R2 = (R1*colorMult)+((((import.bgR*background)+R1)/(background+1))*ambient);
		float G2 = (G1*colorMult)+((((import.bgG*background)+G1)/(background+1))*ambient);
		float B2 = (B1*colorMult)+((((import.bgB*background)+B1)/(background+1))*ambient);
		if(R2 > 1)
		{
			R2 = 1;
		}
		if(G2 > 1)
		{
			G2 = 1;
		}
		if(B2 > 1)
		{
			B2 = 1;
		}
		float Rm = metalic(R2,tris[Convert.ToInt32(faces[face][9]),12]);
		float Gm = metalic(G2,tris[Convert.ToInt32(faces[face][9]),12]);
		float Bm = metalic(B2,tris[Convert.ToInt32(faces[face][9]),12]);
		byte faceR = Convert.ToByte(Rm*255);
		byte faceG = Convert.ToByte(Gm*255);
		byte faceB = Convert.ToByte(Bm*255);
		byte edgeR = Convert.ToByte(255-faceR);
		byte edgeG = Convert.ToByte(255-faceG);
		byte edgeB = Convert.ToByte(255-faceB);
		//backface culling
		if(faces[face][7] <= 1.1)
		{
			//checking if triangle is within screen bounds
			if(Math.Abs(faces[face][0]) < 2 && Math.Abs(faces[face][1]) < 2 && Math.Abs(faces[face][2]) < 2 && Math.Abs(faces[face][3]) < 2 && Math.Abs(faces[face][4]) < 2 && Math.Abs(faces[face][5]) < 2 && faces[face][6] > nearClip && faces[face][6] < farClip)
			{
				if(fill)
				{
					//face fill
					vector.triangle(scrn1[0],scrn1[1],scrn2[0],scrn2[1],scrn3[0],scrn3[1],faceR,faceG,faceB,tris[Convert.ToInt32(faces[face][9]),14]);
				}
				if(!fill)
				{
					//draw edges
					vector.line(scrn1[0],scrn1[1],scrn2[0],scrn2[1],edgeR,edgeG,edgeB,1);
					vector.line(scrn2[0],scrn2[1],scrn3[0],scrn3[1],edgeR,edgeG,edgeB,1);
					vector.line(scrn3[0],scrn3[1],scrn1[0],scrn1[1],edgeR,edgeG,edgeB,1);
				}
			}
		}
	}
	private float metalic(float x, float k = 0)
	{
		float g = 2*x-(1+(k/2));
		float y = (float)Math.Abs((g+k*g)/(2*(-k+(2*k*Math.Abs(g))+1))+0.5);
		return y;
	}
	private Tuple<float,float,float> perspective(Tuple<float,float,float> point)
	{
		float X = point.Item1;
		float Y = point.Item2;
		float Z = point.Item3;
		aspect = (float)(Convert.ToDouble(vector.Pheight)/Convert.ToDouble(vector.Pwidth));
		float angle = (float)(fov * (Math.PI/180));
		float projectConst = (float)(1/(Math.Tan(angle/2)));
		float Xtrans = 0;
		float Ytrans = 0;
		float Xtemp = ((X)*aspect*projectConst);
		float Ytemp = ((Y)*projectConst);
		Xtrans = Xtemp/(Z);
		Ytrans = Ytemp/(Z);
		var output = new Tuple<float,float,float>(Xtrans,Ytrans,Z);
		return output;
	}
	private float degToRad(float deg)
	{
		return (deg/180f)*(float)Math.PI;
	}
	private Tuple<float,float,float> rotate(Tuple<float,float,float> point, int axis)
	{
		float angle;
		switch (axis)
		{
			case 0:
				angle = degToRad(rotX);
				break;
			case 1:
				angle = degToRad(rotY);
				break;
			case 2:
				angle = degToRad(rotZ);
				break;
			case 3:
				angle = degToRad(angX);
				break;
			case 4:
				angle = degToRad(angY);
				break;
			case 5:
				angle = degToRad(angZ);
				break;
			default:
				angle = 0f;
				break;
		}
		var output = matMult(point,rotMatrix(angle,axis%3));
		return output;
	}
	private Tuple<float,float,float> rotobj(Tuple<float,float,float> point, int axis)
	{
		float X = point.Item1;
		float Y = point.Item2;
		float Z = point.Item3;
		float angleX = (float)(angX * (Math.PI/180));
		float angleY = (float)(angY * (Math.PI/180));
		float angleZ = (float)(angZ * (Math.PI/180));
		float Xtrans = X;
		float Ytrans = Y;
		float Ztrans = Z;
		switch (axis)
		{
			case 0:
				Ytrans = (float)(Y*Math.Cos(angleX) - Z*Math.Sin(angleX));
				Ztrans = (float)(Y*Math.Sin(angleX) + Z*Math.Cos(angleX));
				break;
			case 1:
				Xtrans = (float)(X*Math.Cos(angleY) + Z*Math.Sin(angleY));
				Ztrans = (float)(Z*Math.Cos(angleY) - X*Math.Sin(angleY));
				break;
			case 2:
				Xtrans = (float)(X*Math.Cos(angleZ) - Y*Math.Sin(angleZ));
				Ytrans = (float)(X*Math.Sin(angleZ) + Y*Math.Cos(angleZ));
				break;
			default:
				
				break;
		}
		var output = new Tuple<float,float,float>(Xtrans,Ytrans,Ztrans);
		return output;
	}
	private Tuple<float,float,float> translate(Tuple<float,float,float> point)
	{
		float X = point.Item1;
		float Y = point.Item2;
		float Z = point.Item3;
		float Xtrans = X-camX;
		float Ytrans = Y-camY;
		float Ztrans = Z-camZ;
		var output = new Tuple<float,float,float>(Xtrans,Ytrans,Ztrans);
		return output;
	}
	private float[,] rotMatrix(float theta, int axis)
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
	private Tuple<float,float,float> matMult(Tuple<float,float,float> point, float[,] matrix)
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
	private float vecDot(Tuple<float,float,float> A, Tuple<float,float,float> B)
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
	private Tuple<float,float,float> vecCross(Tuple<float,float,float> A, Tuple<float,float,float> B)
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
	private Tuple<float,float,float> unit(Tuple<float,float,float> point)
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
	private float length(Tuple<float,float,float> point)
	{
		float X = point.Item1;
		float Y = point.Item2;
		float Z = point.Item3;
		float L = (float)(Math.Sqrt(((X * X))+((Y * Y))+((Z * Z))));
		return L;
	}
	private Tuple<float,float,float> vecAdd(Tuple<float,float,float> A, Tuple<float,float,float> B)
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
	private Tuple<float,float,float> vecSub(Tuple<float,float,float> A, Tuple<float,float,float> B)
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
	private Tuple<float,float,float> centroid(Tuple<float,float,float> A, Tuple<float,float,float> B, Tuple<float,float,float> C)
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