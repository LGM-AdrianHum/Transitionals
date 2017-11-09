//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/Transition3D.cs
// Name     : Adrian Hum - adrianhum 
// Created  : 2017-09-23-11:00 AM
// Modified : 2017-11-10-7:22 AM

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Transitionals.Controls;

namespace Transitionals
{
    // Base class for 3D transitions
    /// <summary>
    /// </summary>
    [ComVisible(false)]
    public abstract class Transition3D : Transition
    {
        /// <summary>
        /// </summary>
        public static readonly DependencyProperty FieldOfViewProperty =
            DependencyProperty.Register("FieldOfView", typeof(double), typeof(Transition3D),
                new UIPropertyMetadata(20.0));

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty LightProperty;

        static Transition3D()
        {
            var defaultLight = new Model3DGroup();

            var direction = new Vector3D(1, 1, 1);
            direction.Normalize();
            byte ambient = 108; // 108 is minimum for directional to be < 256 (for direction = [1,1,1])
            var directional = (byte) Math.Min((255 - ambient) / Vector3D.DotProduct(direction, new Vector3D(0, 0, 1)),
                255);

            defaultLight.Children.Add(new AmbientLight(Color.FromRgb(ambient, ambient, ambient)));
            defaultLight.Children.Add(new DirectionalLight(Color.FromRgb(directional, directional, directional),
                direction));
            defaultLight.Freeze();
            LightProperty = DependencyProperty.Register("Light", typeof(Model3D), typeof(Transition3D),
                new UIPropertyMetadata(defaultLight));
        }

        /// <summary>
        /// </summary>
        public double FieldOfView
        {
            get { return (double) GetValue(FieldOfViewProperty); }
            set { SetValue(FieldOfViewProperty, value); }
        }


        /// <summary>
        /// </summary>
        public Model3D Light
        {
            get { return (Model3D) GetValue(LightProperty); }
            set { SetValue(LightProperty, value); }
        }

        /// <summary>
        ///     Setup the Viewport 3D
        /// </summary>
        /// <param name="transitionElement"></param>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        protected internal sealed override void BeginTransition(TransitionElement transitionElement,
            ContentPresenter oldContent, ContentPresenter newContent)
        {
            var viewport = new Viewport3D
            {
                IsHitTestVisible = false,
                Camera = CreateCamera(transitionElement, FieldOfView),
                ClipToBounds = false
            };

            var light = new ModelVisual3D {Content = Light};
            viewport.Children.Add(light);

            transitionElement.Children.Add(viewport);
            BeginTransition3D(transitionElement, oldContent, newContent, viewport);
        }

        /// <summary>
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="fieldOfView"></param>
        /// <returns></returns>
        protected virtual Camera CreateCamera(UIElement uiElement, double fieldOfView)
        {
            var size = uiElement.RenderSize;
            return new PerspectiveCamera(
                new Point3D(size.Width / 2, size.Height / 2,
                    -size.Width / Math.Tan(fieldOfView / 2 * Math.PI / 180) / 2),
                new Vector3D(0, 0, 1),
                new Vector3D(0, -1, 0),
                fieldOfView);
        }


        /// <summary>
        /// </summary>
        /// <param name="transitionElement"></param>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        /// <param name="viewport"></param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "3#viewport")]
        protected virtual void BeginTransition3D(TransitionElement transitionElement, ContentPresenter oldContent,
            ContentPresenter newContent, Viewport3D viewport)
        {
            EndTransition(transitionElement, oldContent, newContent);
        }

        // Generates a flat mesh starting at origin with sides equal to vector1 and vector2 vectors
        /// <summary>
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <param name="steps1"></param>
        /// <param name="steps2"></param>
        /// <param name="textureBounds"></param>
        /// <returns></returns>
        public static MeshGeometry3D CreateMesh(Point3D origin, Vector3D vector1, Vector3D vector2, int steps1,
            int steps2, Rect textureBounds)
        {
            vector1 = 1.0 / steps1 * vector1;
            vector2 = 1.0 / steps2 * vector2;

            var mesh = new MeshGeometry3D();

            for (var i = 0; i <= steps1; i++)
            for (var j = 0; j <= steps2; j++)
            {
                mesh.Positions.Add(origin + i * vector1 + j * vector2);

                mesh.TextureCoordinates.Add(new Point(textureBounds.X + textureBounds.Width * i / steps1,
                    textureBounds.Y + textureBounds.Height * j / steps2));
                if (i <= 0 || j <= 0) continue;
                mesh.TriangleIndices.Add((i - 1) * (steps2 + 1) + (j - 1));
                mesh.TriangleIndices.Add((i - 0) * (steps2 + 1) + (j - 0));
                mesh.TriangleIndices.Add((i - 0) * (steps2 + 1) + (j - 1));

                mesh.TriangleIndices.Add((i - 1) * (steps2 + 1) + (j - 1));
                mesh.TriangleIndices.Add((i - 1) * (steps2 + 1) + (j - 0));
                mesh.TriangleIndices.Add((i - 0) * (steps2 + 1) + (j - 0));
            }
            return mesh;
        }
    }
}