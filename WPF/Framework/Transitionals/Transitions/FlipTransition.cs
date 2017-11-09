//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/FlipTransition.cs
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
    public class FlipTransition : Transition3D
    {
        /// <summary>
        /// </summary>
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(RotateDirection), typeof(FlipTransition),
                new UIPropertyMetadata(RotateDirection.Left));

        static FlipTransition()
        {
            AcceptsNullContentProperty.OverrideMetadata(typeof(FlipTransition),
                new FrameworkPropertyMetadata(NullContentSupport.None));
        }

        /// <inheritdoc />
        public FlipTransition()
        {
            Duration = new Duration(TimeSpan.FromSeconds(0.5));
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
        /// <param name="transitionElement"></param>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        /// <param name="viewport"></param>
        protected override void BeginTransition3D(TransitionElement transitionElement, ContentPresenter oldContent,
            ContentPresenter newContent, Viewport3D viewport)
        {
            var size = transitionElement.RenderSize;

            // Create a rectangle
            var mesh = CreateMesh(new Point3D(),
                new Vector3D(size.Width, 0, 0),
                new Vector3D(0, size.Height, 0),
                1,
                1,
                new Rect(0, 0, 1, 1));

            var geometry = new GeometryModel3D {Geometry = mesh};
            var clone = new VisualBrush(oldContent);
            geometry.Material = new DiffuseMaterial(clone);

            var model = new ModelVisual3D {Content = geometry};

            // Replace old content in visual tree with new 3d model
            transitionElement.HideContent(oldContent);
            viewport.Children.Add(model);

            Vector3D axis;
            var center = new Point3D();
            switch (Direction)
            {
                case RotateDirection.Left:
                    axis = new Vector3D(0, 1, 0);
                    break;
                case RotateDirection.Right:
                    axis = new Vector3D(0, -1, 0);
                    center = new Point3D(size.Width, 0, 0);
                    break;
                case RotateDirection.Up:
                    axis = new Vector3D(-1, 0, 0);
                    break;
                default:
                    axis = new Vector3D(1, 0, 0);
                    center = new Point3D(0, size.Height, 0);
                    break;
            }
            var rotation = new AxisAngleRotation3D(axis, 0);
            model.Transform = new RotateTransform3D(rotation, center);

            var da = new DoubleAnimation(0, Duration);
            clone.BeginAnimation(Brush.OpacityProperty, da);

            da = new DoubleAnimation(90, Duration);
            da.Completed += delegate { EndTransition(transitionElement, oldContent, newContent); };
            rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);
        }
    }
}