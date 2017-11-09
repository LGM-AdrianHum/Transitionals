//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/DoorTransition.cs
// Name     : Adrian Hum - adrianhum 
// Created  : 2017-09-23-11:00 AM
// Modified : 2017-11-10-7:45 AM

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Transitionals.Controls;

namespace Transitionals.Transitions
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    [ComVisible(false)]
    public class DoorTransition : Transition3D
    {
        static DoorTransition()
        {
            AcceptsNullContentProperty.OverrideMetadata(typeof(DoorTransition),
                new FrameworkPropertyMetadata(NullContentSupport.New));
        }

        /// <summary>
        /// </summary>
        public DoorTransition()
        {
            Duration = new Duration(TimeSpan.FromSeconds(0.5));
            FieldOfView = 40;
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
            var clone = CreateBrush(oldContent);

            var size = transitionElement.RenderSize;
            var leftDoor = CreateMesh(new Point3D(),
                new Vector3D(size.Width / 2, 0, 0),
                new Vector3D(0, size.Height, 0),
                1,
                1,
                new Rect(0, 0, 0.5, 1));

            var leftDoorGeometry = new GeometryModel3D
            {
                Geometry = leftDoor,
                Material = new DiffuseMaterial(clone)
            };

            var leftRotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            leftDoorGeometry.Transform = new RotateTransform3D(leftRotation);

            var rightDoorGeometry = new GeometryModel3D();
            var rightDoor = CreateMesh(new Point3D(size.Width / 2, 0, 0),
                new Vector3D(size.Width / 2, 0, 0),
                new Vector3D(0, size.Height, 0),
                1,
                1,
                new Rect(0.5, 0, 0.5, 1));

            rightDoorGeometry.Geometry = rightDoor;
            rightDoorGeometry.Material = new DiffuseMaterial(clone);

            var rightRotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            rightDoorGeometry.Transform = new RotateTransform3D(rightRotation, size.Width, 0, 0);


            var doors = new Model3DGroup();
            doors.Children.Add(leftDoorGeometry);
            doors.Children.Add(rightDoorGeometry);

            var model = new ModelVisual3D {Content = doors};

            // Replace old content in visual tree with new 3d model
            transitionElement.HideContent(oldContent);
            viewport.Children.Add(model);

            var da = new DoubleAnimation(90 - 0.5 * FieldOfView, Duration);
            leftRotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);

            da = new DoubleAnimation(-(90 - 0.5 * FieldOfView), Duration);
            rightRotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);

            da = new DoubleAnimation(0, Duration);
            da.Completed += delegate { EndTransition(transitionElement, oldContent, newContent); };
            clone.BeginAnimation(Brush.OpacityProperty, da);
        }
    }
}