//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/ExplosionTransition.cs
// Name     : Adrian Hum - adrianhum 
// Created  : 2017-09-23-11:00 AM
// Modified : 2017-11-10-7:45 AM

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Transitionals.Controls;

namespace Transitionals.Transitions
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    [ComVisible(false)]
    public class ExplosionTransition : Transition3D
    {
        /// <summary>
        /// </summary>
        public static readonly DependencyProperty MouseAsCenterProperty =
            DependencyProperty.Register("UseMouseLocation", typeof(bool), typeof(ExplosionTransition),
                new UIPropertyMetadata(false));

        static ExplosionTransition()
        {
            AcceptsNullContentProperty.OverrideMetadata(typeof(ExplosionTransition),
                new FrameworkPropertyMetadata(NullContentSupport.New));
            ClipToBoundsProperty.OverrideMetadata(typeof(ExplosionTransition), new FrameworkPropertyMetadata(true));
        }

        /// <summary>
        /// </summary>
        public ExplosionTransition()
        {
            Duration = new Duration(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// </summary>
        [DefaultValue(false)]
        public bool MouseAsCenter
        {
            get { return (bool) GetValue(MouseAsCenterProperty); }
            set { SetValue(MouseAsCenterProperty, value); }
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
            var size = transitionElement.RenderSize;

            Point3D centerPoint;
            if (MouseAsCenter)
            {
                var mouse2D = Mouse.GetPosition(transitionElement);
                centerPoint = new Point3D(mouse2D.X, mouse2D.Y, 0.5 * size.Width);
            }
            else
            {
                centerPoint = new Point3D(0.5 * size.Width, 0.5 * size.Height, 0.5 * size.Width);
            }

            int xparticles = 10, yparticles = 10;

            if (size.Width > size.Height)
                yparticles = (int) (xparticles * size.Height / size.Width);
            else
                xparticles = (int) (yparticles * size.Width / size.Height);

            double sx = 1.0 / xparticles, sy = 1.0 / yparticles;
            var u = new Vector3D(size.Width * sx, 0, 0);
            var v = new Vector3D(0, size.Height * sy, 0);
            var cloneBrush = CreateBrush(oldContent);
            Material clone = new DiffuseMaterial(cloneBrush);

            var velocities = new Vector3D[xparticles * yparticles];
            var angularVelocities = new Vector3D[xparticles * yparticles];
            var centers = new Point3D[xparticles * yparticles];

            var positions = new Point3DCollection(4 * xparticles * yparticles);
            var textures = new PointCollection(4 * xparticles * yparticles);
            var triangles = new Int32Collection(6 * xparticles * yparticles);
            var n = 0;
            for (var i = 0; i < xparticles; i++)
            for (var j = 0; j < yparticles; j++)
            {
                var topleft = (Point3D) (i * u + j * v);
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

                var f0 = positions[n] - centerPoint;
                var f1 = positions[n + 1] - centerPoint;
                var f2 = positions[n + 2] - centerPoint;
                var f3 = positions[n + 3] - centerPoint;

                f0 = f0 / f0.LengthSquared;
                f1 = f1 / f1.LengthSquared;
                f2 = f2 / f2.LengthSquared;
                f3 = f3 / f3.LengthSquared;

                velocities[n / 4] = 2 * size.Width * (f0 + f1 + f2 + f3);

                var center = centers[n / 4] = (Point3D) ((i + 0.5) * u + (j + 0.5) * v);
                angularVelocities[n / 4] = 200 * (Vector3D.CrossProduct(f0, positions[n] - center) +
                                                  Vector3D.CrossProduct(f1, positions[n + 1] - center) +
                                                  Vector3D.CrossProduct(f2, positions[n + 2] - center) +
                                                  Vector3D.CrossProduct(f3, positions[n + 3] - center));

                n += 4;
            }

            var mesh = new MeshGeometry3D
            {
                Positions = positions,
                TextureCoordinates = textures,
                TriangleIndices = triangles
            };

            var geometryModel = new GeometryModel3D(mesh, clone) {BackMaterial = clone};
            var model = new ModelVisual3D {Content = geometryModel};

            // Replace old content in visual tree with new 3d model
            transitionElement.HideContent(oldContent);
            viewport.Children.Add(model);

            var timer = new DispatcherTimer();
            var t = 0;
            var opacityDelta = 1.0 / (Duration.TimeSpan.Seconds * 60.0);
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
                var axisAngle = new AxisAngleRotation3D();
                var rotation = new RotateTransform3D(axisAngle, new Point3D());
                for (var i = 0; i < positions.Count; i += 4)
                {
                    var velocity = velocities[i / 4];

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