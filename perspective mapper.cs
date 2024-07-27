using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using plotter_2D;

public class projection
{
	private int TriangleBounds = 3;
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
	private float ambient = 1f/32f;
	private float background = 0.5f;
	private float scale = 2;
	public float fov = 45;
	public import import = new import();
	public Vector Vector = new Vector();
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
		faces = faces.OrderByDescending(entry => entry[6]).ToArray();
		for(int i = 0; i < count; i++)
		{
			drawTri(i,fill);
		}
		vector.render.update();
	}
	private void projectTri(int face = 0)
	{
		var position =  new Tuple<float,float,float>(camX,camY,camZ);
		//defining tuples for ease of passing data between functions
		var point1 = new Tuple<float,float,float>(tris[face,0],tris[face,1],tris[face,2]);
		var point2 = new Tuple<float,float,float>(tris[face,3],tris[face,4],tris[face,5]);
		var point3 = new Tuple<float,float,float>(tris[face,6],tris[face,7],tris[face,8]);
		//transforming the 3d space into a 2d projection
		var transformA = rotate(rotate(rotate(point1,5),4),3);
		var transformB = rotate(rotate(rotate(point2,5),4),3);
		var transformC = rotate(rotate(rotate(point3,5),4),3);
		var camA = rotate(rotate(rotate(Vector.vecSub(transformA,position),2),1),0);
		var camB = rotate(rotate(rotate(Vector.vecSub(transformB,position),2),1),0);
		var camC  =rotate(rotate(rotate(Vector.vecSub(transformC,position),2),1),0);
		var projA = perspective(camA);
		var projB = perspective(camB);
		var projC = perspective(camC);
		//calculating the normal vector
		var camAB = Vector.vecSub(projB,projA);
		var camAC = Vector.vecSub(projC,projA);
		var camN = Vector.unit(Vector.vecCross(camAB,camAC));
		var lumAB = Vector.vecSub(transformB,transformA);
		var lumAC = Vector.vecSub(transformC,transformA);
		var lumN = Vector.unit(Vector.vecCross(lumAB,lumAC));
		var luma = Vector.unit(new Tuple<float,float,float>(-2,5,3));
		float D = Vector.vecDot(lumN,luma);
		var direction = Vector.vecSub(Vector.centroid(camA,camB,camC),position);
		float distance = Vector.centroid(camA,camB,camC).Item3;
		float K = Vector.vecDot(Vector.unit(direction),camN);
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
		float R1 = tris[Convert.ToInt32(faces[face][9]),9];
		float G1 = tris[Convert.ToInt32(faces[face][9]),10];
		float B1 = tris[Convert.ToInt32(faces[face][9]),11];
		float Ra = ((import.bgR*(1-colorMult)*background)+ambient*R1)/2f;
		float Ga = ((import.bgG*(1-colorMult)*background)+ambient*G1)/2f;
		float Ba = ((import.bgB*(1-colorMult)*background)+ambient*B1)/2f;
		float R2 = (((R1*colorMult)+Ra));
		float G2 = (((G1*colorMult)+Ga));
		float B2 = (((B1*colorMult)+Ba));
		if(colorMult < tris[Convert.ToInt32(faces[face][9]),13])
		{
			colorMult = tris[Convert.ToInt32(faces[face][9]),13];
			R2 = (R1*colorMult);
			G2 = (G1*colorMult);
			B2 = (B1*colorMult);
		}
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
		if(faces[face][7] < 1)
		{
			//checking if triangle is within screen bounds
			if(faces[face][6] > nearClip && faces[face][6] < farClip && Math.Abs(faces[face][0]) < TriangleBounds && Math.Abs(faces[face][1]) < TriangleBounds && Math.Abs(faces[face][2]) < TriangleBounds && Math.Abs(faces[face][3]) < TriangleBounds && Math.Abs(faces[face][4]) < TriangleBounds && Math.Abs(faces[face][5]) < TriangleBounds)
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
		var output = Vector.matVecMult(point,Vector.rotMatrix(angle,axis%3));
		return output;
	}
}