using System;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.IO;

namespace renderer_3d
{
	class Program
	{
		static int width = 1280;
		static int height = 960;
		static projection projection = new projection(width,height);
		static bool faces = true;
		static bool rotObj = false;
		static float step = 0.025f;
		static Window w = new Window();
		static Application app = new Application();
		static float angleX = 0.0f, angleY = 0.0f, angleZ = 0.0f, rotateX = 0.0f, rotateY = 0.0f, rotateZ = 0.0f;
		static float posX = 0.0f, posY = 0.0f, posZ = -0.75f;
		static string obj = "";
		[STAThread]
		public static void Main(string[] args)
		{
			if(args.Length >= 1)
			{
				obj = args[0];
			}
			else
			{
				obj = "";
			}
			w.Width = width;
			w.MinWidth = width;
			w.MaxWidth = width;
			w.Height = height;
			w.MinHeight = height;
			w.MaxHeight = height;
			w.KeyDown += new KeyEventHandler(w_KeyDown);
			w.Content = projection.vector.render.i;
			w.Show();
			project();
			app.Run();
		}
		private static void project()
		{
			projection.rotX = angleX;
			projection.rotY = angleY;
			projection.rotZ = angleZ;
			projection.camX = posX;
			projection.camY = posY;
			projection.camZ = posZ;
			projection.angX = rotateX;
			projection.angY = rotateY;
			projection.angZ = rotateZ;
			projection.drawObj(obj, faces);
		}
		private static void w_KeyDown(object sender, KeyEventArgs e)
		{
			angleX = (angleX + 720)%360;
			angleY = (angleY + 720)%360;
			angleZ = (angleZ + 720)%360;
			rotateX = (rotateX + 720)%360;
			rotateY = (rotateY + 720)%360;
			rotateZ = (rotateZ + 720)%360;
			switch (e.Key)
			{
				case Key.Up:
					if(rotObj)
					{
						rotateX -= 2.5f;
					}
					else
					{
						angleX -= 2.5f;
					}
					break;
				case Key.Down:
					if(rotObj)
					{
						rotateX += 2.5f;
					}
					else
					{
						angleX += 2.5f;
					}
					break;
				case Key.Left:
					if(rotObj)
					{
						rotateY += 2.5f;
					}
					else
					{
						angleY += 2.5f;
					}
					break;
				case Key.Right:
					if(rotObj)
					{
						rotateY -= 2.5f;
					}
					else
					{
						angleY -= 2.5f;
					}
					break;
				case Key.D:
					posZ = (float)(posZ+(Math.Sin(angleY*(Math.PI/180))*step));
					posX = (float)(posX+(Math.Cos(angleY*(Math.PI/180))*step));
					break;
				case Key.A:
					posZ = (float)(posZ+(Math.Sin(angleY*(Math.PI/180))*-step));
					posX = (float)(posX+(Math.Cos(angleY*(Math.PI/180))*-step));
					break;
				case Key.S:
					posZ = (float)(posZ+(Math.Sin((angleY-90)*(Math.PI/180))*step));
					posX = (float)(posX+(Math.Cos((angleY-90)*(Math.PI/180))*step));
					break;
				case Key.W:
					posZ = (float)(posZ+(Math.Sin((angleY-90)*(Math.PI/180))*-step));
					posX = (float)(posX+(Math.Cos((angleY-90)*(Math.PI/180))*-step));
					break;
				case Key.Space:
					posY -= step;
					break;
				case Key.LeftShift:
					posY += step;
					break;
				case Key.Tab:
					faces = !faces;
					break;
				case Key.Z:
					rotObj = !rotObj;
					break;
				case Key.PageUp:
					step *=2;
					break;
				case Key.PageDown:
					step /=2;
					break;
			}
			project();
		}
	}
}