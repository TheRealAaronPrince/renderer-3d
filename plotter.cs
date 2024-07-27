using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using graphics;
using System;

namespace plotter_2D
{
	public class Plotter
	{
		public int Pwidth;
		public int Pheight;
		public render render;
		public Plotter(int w = 960, int h = 540)
		{
			Pwidth = w;
			Pheight = h;
			render = new render(Pwidth,Pheight);
		}
		public void line(long x1, long y1, long x2, long y2, byte R = 255, byte G = 255, byte B = 255, int many = 0)
		{
			long dx = x2 - x1;
			long dy = y2 - y1;
			long x1a, x2a, y1a, y2a;
			if(x1 <= x2)
			{
				x1a = x1;
				x2a = x2;
			}
			else
			{
				x1a = x2;
				x2a = x1;
			}
			if(y1 <= y2)
			{
				y1a = y1;
				y2a = y2;
			}
			else
			{
				y1a = y2;
				y2a = y1;
			}
			if(System.Math.Abs(dy) < System.Math.Abs(dx))
			{
				for(long xa = x1a; xa < x2a; xa++)
				{
					long ya = y1 + (dy * (xa - x1)) / dx;
					if(xa >= 0 && ya >= 0 && xa < render.width && ya < render.height)
					{
						long xb = Convert.ToInt64(xa);
						long yb = Convert.ToInt64(ya);
						render.pixelBuffer[(((yb * render.width) + xb) * 4) + 0] = B;
						render.pixelBuffer[(((yb * render.width) + xb) * 4) + 1] = G;
						render.pixelBuffer[(((yb * render.width) + xb) * 4) + 2] = R;
						render.pixelBuffer[(((yb * render.width) + xb) * 4) + 3] = 255;
					}
				}
			}
			else
			{
				for(long ya = y1a; ya < y2a; ya++)
				{
					long xa = x1 + (dx * (ya - y1)) / dy;
					if(xa >= 0 && ya >= 0 && xa < render.width && ya < render.height)
					{
						long xb = Convert.ToInt64(xa);
						long yb = Convert.ToInt64(ya);
						render.pixelBuffer[(((yb * render.width) + xb) * 4) + 0] = B;
						render.pixelBuffer[(((yb * render.width) + xb) * 4) + 1] = G;
						render.pixelBuffer[(((yb * render.width) + xb) * 4) + 2] = R;
						render.pixelBuffer[(((yb * render.width) + xb) * 4) + 3] = 255;
					}
				}
			}
			if(many == 0)
			{
				render.update();
			}
		}
		public void triangle(long x1, long y1, long x2, long y2, long x3, long y3, byte R = 255, byte G = 255, byte B = 255, float O = 1)
		{
			// Deltas
			long Dx12 = x1 - x2;
			long Dx23 = x2 - x3;
			long Dx31 = x3 - x1;
			long Dy12 = y1 - y2;
			long Dy23 = y2 - y3;
			long Dy31 = y3 - y1;
			// Bounding rectangle
			long[] xVal = new long[] {x1,x2,x3};
			long[] yVal = new long[] {y1,y2,y3};
			long minx = xVal.Min();
			long maxx = xVal.Max();
			long miny = yVal.Min();
			long maxy = yVal.Max();
			// Constant part of half-edge functions
			long C1 = Dy12 * x1 - Dx12 * y1;
			long C2 = Dy23 * x2 - Dx23 * y2;
			long C3 = Dy31 * x3 - Dx31 * y3;
			long Cy1 = C1 + Dx12 * miny - Dy12 * minx;
			long Cy2 = C2 + Dx23 * miny - Dy23 * minx;
			long Cy3 = C3 + Dx31 * miny - Dy31 * minx;
			long w = maxx - minx;
			long h = maxy - miny;
			long area = w*h;
			long Cx1 = 0;
			long Cx2 = 0;
			long Cx3 = 0;
			unsafe
			{
				fixed(byte *pointer = render.pixelBuffer)
				{
					for(long i = 0; i < area; i++)
					{
						long relx = i%w;
						long rely = i/w;
						long x = relx + minx;
						long y = rely + miny;
						// Start value for horizontal scan
						if(relx == 0)
						{
							Cx1 = Cy1;
							Cx2 = Cy2;
							Cx3 = Cy3;	
						}
						if(x >= 0 && y >= 0 && x < Pwidth && y < Pheight && Cx1 >= 0 && Cx2 >= 0 && Cx3 >= 0)
						{
							byte *index = pointer +  (((y * Pwidth) + x) * 4);
							*(index + 0) = (byte)((*(index + 0)*(1-O))+(B*O));
							*(index + 1) = (byte)((*(index + 1)*(1-O))+(G*O));
							*(index + 2) = (byte)((*(index + 2)*(1-O))+(R*O));
						}
						Cx1 -= Dy12;
						Cx2 -= Dy23;
						Cx3 -= Dy31;
						if(relx == w-1)
						{
							Cy1 += Dx12;
							Cy2 += Dx23;
							Cy3 += Dx31;
						}
					}
				}
			};
		}
	}
}