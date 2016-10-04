#region License Revision: 0 Last Revised: 3/29/2006 8:21 AM
/******************************************************************************
Copyright (c) Microsoft Corporation.  All rights reserved.


This file is licensed under the Microsoft Public License (Ms-PL). A copy of the Ms-PL should accompany this file. 
If it does not, you can obtain a copy from: 

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx
******************************************************************************/
#endregion // License
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Input;
using System.Windows.Threading;
using Transitionals.Controls;
using System.ComponentModel;

namespace Transitionals.Transitions
{
    [System.Runtime.InteropServices.ComVisible(false)]
    public class ExplosionTransition : Transition3D
    {
        static ExplosionTransition()
        {
            AcceptsNullContentProperty.OverrideMetadata(typeof(ExplosionTransition), new FrameworkPropertyMetadata(NullContentSupport.New));
            ClipToBoundsProperty.OverrideMetadata(typeof(ExplosionTransition), new FrameworkPropertyMetadata(true));
        }

        public ExplosionTransition()
        {
            this.Duration = new Duration(TimeSpan.FromSeconds(1));
        }

        [DefaultValue(false)]
        public bool MouseAsCenter
        {
            get { return (bool)GetValue(MouseAsCenterProperty); }
            set { SetValue(MouseAsCenterProperty, value); }
        }

        public static readonly DependencyProperty MouseAsCenterProperty =
            DependencyProperty.Register("UseMouseLocation", typeof(bool), typeof(ExplosionTransition), new UIPropertyMetadata(false));

        protected override void BeginTransition3D(TransitionElement transitionElement, ContentPresenter oldContent, ContentPresenter newContent, Viewport3D viewport)
        {
            Size size = transitionElement.RenderSize;

            Point3D centerPoint;
            if (MouseAsCenter)
            {
                Point mouse2D = Mouse.GetPosition(transitionElement);
                centerPoint = new Point3D(mouse2D.X, mouse2D.Y, 0.5 * size.Width);
            }
            else
            {
                centerPoint = new Point3D(0.5 * size.Width, 0.5 * size.Height, 0.5 * size.Width);
            }

            int xparticles = 10, yparticles = 10;

            if (size.Width > size.Height)
            {
                yparticles = (int)(xparticles * size.Height / size.Width);
            }
            else
            {
                xparticles = (int)(yparticles * size.Width / size.Height);
            }

            double sx = 1.0 / xparticles, sy = 1.0 / yparticles;
            Vector3D u = new Vector3D(size.Width * sx, 0, 0);
            Vector3D v = new Vector3D(0, size.Height * sy, 0);
            Brush cloneBrush = CreateBrush(oldContent);
            Material clone = new DiffuseMaterial(cloneBrush);

            Vector3D[] velocities = new Vector3D[xparticles * yparticles];
            Vector3D[] angularVelocities = new Vector3D[xparticles * yparticles];
            Point3D[] centers = new Point3D[xparticles * yparticles];

            Point3DCollection positions = new Point3DCollection(4 * xparticles * yparticles);
            PointCollection textures = new PointCollection(4 * xparticles * yparticles);
            Int32Collection triangles = new Int32Collection(6 * xparticles * yparticles);
            int n = 0;
            for (int i = 0; i < xparticles; i++)
            {
                for (int j = 0; j < yparticles; j++)
                {
                    Point3D topleft = (Point3D)(i * u + j * v);
                    positions.Add(topleft);
                    positions.Add(topleft + u);
                    positions.Add(topleft + u + v);
                    positions.Add(topleft + v);

                    textures.Add(new Point(i * sx, j * sy));
                    textures.Add(new Point((i + 1) * sx, j * sy));
                    textures.Add(new Point((i + 1) * sx, (j + 1) * sy));
                    textures.Add(new Point(i * sx, (j + 1) * sy));


                    triangles.Add(n);
                    triangles.Add(n + 2);
                    triangles.Add(n + 1);

                    triangles.Add(n);
                    triangles.Add(n + 3);
                    triangles.Add(n + 2);

                    Vector3D f0 = positions[n] - centerPoint;
                    Vector3D f1 = positions[n + 1] - centerPoint;
                    Vector3D f2 = positions[n + 2] - centerPoint;
                    Vector3D f3 = positions[n + 3] - centerPoint;

                    f0 = f0 / f0.LengthSquared;
                    f1 = f1 / f1.LengthSquared;
                    f2 = f2 / f2.LengthSquared;
                    f3 = f3 / f3.LengthSquared;

                    velocities[n / 4] = 2 * size.Width * (f0 + f1 + f2 + f3);

                    Point3D center = centers[n / 4] = (Point3D)((i + 0.5) * u + (j + 0.5) * v);
                    angularVelocities[n / 4] = 200 * (Vector3D.CrossProduct(f0, positions[n] - center) +
                        Vector3D.CrossProduct(f1, positions[n + 1] - center) +
                        Vector3D.CrossProduct(f2, positions[n + 2] - center) +
                        Vector3D.CrossProduct(f3, positions[n + 3] - center));

                    n += 4;
                }
            }

            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = positions;
            mesh.TextureCoordinates = textures;
            mesh.TriangleIndices = triangles;

            GeometryModel3D geometryModel = new GeometryModel3D(mesh, clone);
            geometryModel.BackMaterial = clone;
            ModelVisual3D model = new ModelVisual3D();
            model.Content = geometryModel;

            // Replace old content in visual tree with new 3d model
            transitionElement.HideContent(oldContent);
            viewport.Children.Add(model);

            DispatcherTimer timer = new DispatcherTimer();
            int t = 0;
            double opacityDelta = 1.0 / (Duration.TimeSpan.Seconds * 60.0);
            timer.Interval = TimeSpan.FromSeconds(1.0 / 60.0);
            timer.Tick += delegate
            {
                t++;
                cloneBrush.Opacity = 1 - t * opacityDelta;
                if (cloneBrush.Opacity < opacityDelta)
                {
                    timer.Stop();
                    EndTransition(transitionElement, oldContent, newContent);
                    return;
                }
                mesh.Positions = null;
                AxisAngleRotation3D axisAngle = new AxisAngleRotation3D();
                RotateTransform3D rotation = new RotateTransform3D(axisAngle, new Point3D());
                for (int i = 0; i < positions.Count; i += 4)
                {
                    Vector3D velocity = velocities[i / 4];

                    axisAngle.Axis = angularVelocities[i / 4];
                    axisAngle.Angle = angularVelocities[i / 4].Length;
                    rotation.CenterX = centers[i / 4].X;
                    rotation.CenterY = centers[i / 4].Y;
                    rotation.CenterZ = centers[i / 4].Z;

                    positions[i] = rotation.Transform(positions[i]) + velocity;
                    positions[i + 1] = rotation.Transform(positions[i + 1]) + velocity;
                    positions[i + 2] = rotation.Transform(positions[i + 2]) + velocity;
                    positions[i + 3] = rotation.Transform(positions[i + 3]) + velocity;

                    centers[i / 4] += velocity;
                }
                mesh.Positions = positions;
            };
            timer.Start();            
        }
    }
}
