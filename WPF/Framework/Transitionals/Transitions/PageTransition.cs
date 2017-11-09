//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/PageTransition.cs
// Name     : Adrian Hum - adrianhum 
// Created  : 2017-09-23-11:00 AM
// Modified : 2017-11-10-7:45 AM

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Transitionals.Controls;

namespace Transitionals.Transitions
{
    /// <inheritdoc />
    /// <summary>
    ///     Cloth physics with extra constraints to the sides of the pages
    /// </summary>
    [ComVisible(false)]
    public class PageTransition : Transition3D
    {
        /// <inheritdoc />
        public PageTransition()
        {
            Duration = new Duration(TimeSpan.FromSeconds(2));
            FieldOfView = 10;
            ClipToBounds = true;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="transitionElement"></param>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        /// <param name="viewport"></param>
        protected override void BeginTransition3D(TransitionElement transitionElement, ContentPresenter oldContent,
            ContentPresenter newContent, Viewport3D viewport)
        {
            int xparticles = 10, yparticles = 10;
            var size = transitionElement.RenderSize;

            if (size.Width > size.Height)
                yparticles = (int) (xparticles * size.Height / size.Width);
            else
                xparticles = (int) (yparticles * size.Width / size.Height);

            var mesh = CreateMesh(new Point3D(), new Vector3D(size.Width, 0, 0), new Vector3D(0, size.Height, 0),
                xparticles - 1, yparticles - 1, new Rect(0, 0, 1, 1));
            var cloneBrush = CreateBrush(oldContent);
            Material clone = new DiffuseMaterial(cloneBrush);


            double ustep = size.Width / (xparticles - 1), vstep = size.Height / (yparticles - 1);

            var points = mesh.Positions;


            var oldPoints = points.Clone();

            const double timeStep = 1.0 / 30.0;
            var timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(timeStep)};
            double time = 0;
            var duration = Duration.HasTimeSpan ? Duration.TimeSpan.TotalSeconds : 2;
            timer.Tick += delegate
            {
                time = time + timeStep;
                //var mousePos = Mouse.GetPosition(viewport);
                //var mousePos3D = new Point3D(mousePos.X, mousePos.Y, -10);

                // Cloth physics based on work of Thomas Jakobsen http://www.ioi.dk/~thomas
                for (var i = 0; i < oldPoints.Count; i++)
                {
                    var currentPoint = points[i];
                    var newPoint = currentPoint + 0.9 * (currentPoint - oldPoints[i]);

                    if (newPoint.Y > size.Height)
                        newPoint.Y = size.Height;

                    oldPoints[i] = newPoint;
                }

                //for (int j = 0; j < 5; j++)
                //for (int i = oldPoints.Count - 1; i > 0 ; i--)
                for (var a = yparticles - 1; a >= 0; a--)
                for (var b = xparticles - 1; b >= 0; b--)
                {
                    var i = b * yparticles + a;
                    // constrain with point to the left
                    if (i > yparticles)
                        Constrain(oldPoints, i, i - yparticles, ustep);
                    // constrain with point to the top
                    if (i % yparticles != 0)
                        Constrain(oldPoints, i, i - 1, vstep);

                    // constrain the sides 
                    if (a == 0)
                        oldPoints[i] = new Point3D(oldPoints[i].X, 0, oldPoints[i].Z);
                    if (a == yparticles - 1)
                        oldPoints[i] = new Point3D(oldPoints[i].X, size.Height, oldPoints[i].Z);

                    if (b == 0)
                        oldPoints[i] = new Point3D(0, a * size.Height / (yparticles - 1), 0);

                    if (b != xparticles - 1) continue;
                    var angle = time / duration * Math.PI / (0.8 + 0.5 * (yparticles - (double) a) / yparticles);
                    oldPoints[i] = new Point3D(size.Width * Math.Cos(angle), a * size.Height / (yparticles - 1),
                        -size.Width * Math.Sin(angle));
                }

                if (time > duration - 0)
                {
                    timer.Stop();
                    EndTransition(transitionElement, oldContent, newContent);
                }

                // Swap position arrays
                mesh.Positions = oldPoints;
                oldPoints = points;
                points = mesh.Positions;
            };
            timer.Start();


            var geo = new GeometryModel3D(mesh, clone) {BackMaterial = clone};
            var model = new ModelVisual3D {Content = geo};

            // Replace old content in visual tree with new 3d model
            transitionElement.HideContent(oldContent);
            viewport.Children.Add(model);
        }

        private static void Constrain(Point3DCollection points, int i1, int i2, double length)
        {
            Point3D p1 = points[i1], p2 = points[i2];
            var delta = p2 - p1;
            var deltalength = delta.Length;
            var diff = (deltalength - length) / deltalength;
            p1 += delta * 0.5 * diff;
            p2 -= delta * 0.5 * diff;

            points[i1] = p1;
            points[i2] = p2;
        }
    }
}