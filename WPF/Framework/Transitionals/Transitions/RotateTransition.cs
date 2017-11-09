//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/RotateTransition.cs
// Name     : Adrian Hum - adrianhum 
// Created  : 2017-09-23-11:00 AM
// Modified : 2017-11-10-7:45 AM

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Transitionals.Controls;

namespace Transitionals.Transitions
{
    /// <summary>
    /// </summary>
    public enum RotateDirection
    {
        /// <summary>
        /// </summary>
        Up,

        /// <summary>
        /// </summary>
        Down,

        /// <summary>
        /// </summary>
        Left,

        /// <summary>
        /// </summary>
        Right
    }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    [ComVisible(false)]
    public class RotateTransition : Transition3D
    {
        /// <summary>
        /// </summary>
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(RotateTransition), new UIPropertyMetadata(90.0),
                IsAngleValid);

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(RotateDirection), typeof(RotateTransition),
                new UIPropertyMetadata(RotateDirection.Left));

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty ContainedProperty =
            DependencyProperty.Register("Contained", typeof(bool), typeof(RotateTransition),
                new UIPropertyMetadata(false));

        static RotateTransition()
        {
            AcceptsNullContentProperty.OverrideMetadata(typeof(RotateTransition),
                new FrameworkPropertyMetadata(NullContentSupport.Both));
        }

        /// <inheritdoc />
        public RotateTransition()
        {
            Duration = new Duration(TimeSpan.FromSeconds(0.75));
            Angle = 90;
            FieldOfView = 40;
        }

        /// <summary>
        /// </summary>
        public double Angle
        {
            get { return (double) GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        /// <summary>
        /// </summary>
        public RotateDirection Direction
        {
            get { return (RotateDirection) GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        /// <summary>
        /// </summary>
        public bool Contained
        {
            get { return (bool) GetValue(ContainedProperty); }
            set { SetValue(ContainedProperty, value); }
        }

        private static bool IsAngleValid(object value)
        {
            var angle = (double) value;
            return angle >= 0 && angle < 180;
        }

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

            var origin = new Point3D(); // origin of 2nd face
            Vector3D u = new Vector3D(), v = new Vector3D(); // u & v vectors of 2nd face

            var angle = Angle;
            Point3D rotationCenter;
            Vector3D rotationAxis;
            var direction = Direction;

            TranslateTransform3D translation = null;
            var angleRads = Angle * Math.PI / 180;
            if (direction == RotateDirection.Left || direction == RotateDirection.Right)
            {
                if (Contained)
                {
                    rotationCenter = new Point3D(direction == RotateDirection.Left ? size.Width : 0, 0, 0);
                    translation = new TranslateTransform3D();
                    var x = new DoubleAnimation(direction == RotateDirection.Left ? -size.Width : size.Width, Duration);
                    translation.BeginAnimation(TranslateTransform3D.OffsetXProperty, x);
                }
                else
                {
                    rotationCenter = new Point3D(size.Width / 2, 0,
                        size.Width / 2 * Math.Tan(angle / 2 * Math.PI / 180));
                }

                rotationAxis = new Vector3D(0, 1, 0);

                if (direction == RotateDirection.Left)
                {
                    u.X = -size.Width * Math.Cos(angleRads);
                    u.Z = size.Width * Math.Sin(angleRads);

                    origin.X = size.Width;
                }
                else
                {
                    u.X = -size.Width * Math.Cos(angleRads);
                    u.Z = -size.Width * Math.Sin(angleRads);

                    origin.X = -u.X;
                    origin.Z = -u.Z;
                }
                v.Y = size.Height;
            }
            else
            {
                if (Contained)
                {
                    rotationCenter = new Point3D(0, direction == RotateDirection.Up ? size.Height : 0, 0);
                    translation = new TranslateTransform3D();
                    var y = new DoubleAnimation(direction == RotateDirection.Up ? -size.Height : size.Height, Duration);
                    translation.BeginAnimation(TranslateTransform3D.OffsetYProperty, y);
                }
                else
                {
                    rotationCenter = new Point3D(0, size.Height / 2,
                        size.Height / 2 * Math.Tan(angle / 2 * Math.PI / 180));
                }

                rotationAxis = new Vector3D(1, 0, 0);

                if (direction == RotateDirection.Up)
                {
                    v.Y = -size.Height * Math.Cos(angleRads);
                    v.Z = size.Height * Math.Sin(angleRads);

                    origin.Y = size.Height;
                }
                else
                {
                    v.Y = -size.Height * Math.Cos(angleRads);
                    v.Z = -size.Height * Math.Sin(angleRads);

                    origin.Y = -v.Y;
                    origin.Z = -v.Z;
                }
                u.X = size.Width;
            }

            var endAngle = 180 - angle;
            if (direction == RotateDirection.Right || direction == RotateDirection.Up)
                endAngle = -endAngle;

            ModelVisual3D m1, m2;
            viewport.Children.Add(m1 = MakeSide(oldContent, new Point3D(), new Vector3D(size.Width, 0, 0),
                new Vector3D(0, size.Height, 0), endAngle, rotationCenter, rotationAxis, null));
            viewport.Children.Add(m2 = MakeSide(newContent, origin, u, v, endAngle, rotationCenter, rotationAxis,
                delegate { EndTransition(transitionElement, oldContent, newContent); }));

            m1.Transform = m2.Transform = translation;

            // Replace old and new content in visual tree with new 3d models
            transitionElement.HideContent(oldContent);
            transitionElement.HideContent(newContent);
        }

        private ModelVisual3D MakeSide(ContentPresenter content, Point3D origin, Vector3D u, Vector3D v,
            double endAngle, Point3D rotationCenter, Vector3D rotationAxis, EventHandler onCompleted)
        {
            var sideMesh = CreateMesh(origin, u, v, 1, 1, new Rect(0, 0, 1, 1));

            var sideModel = new GeometryModel3D();
            sideModel.Geometry = sideMesh;

            var clone = CreateBrush(content);
            sideModel.Material = new DiffuseMaterial(clone);

            var rotation = new AxisAngleRotation3D(rotationAxis, 0);
            sideModel.Transform = new RotateTransform3D(rotation, rotationCenter);


            var da = new DoubleAnimation(endAngle, Duration);
            if (onCompleted != null)
                da.Completed += onCompleted;

            rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);

            var side = new ModelVisual3D();
            side.Content = sideModel;
            return side;
        }
    }
}