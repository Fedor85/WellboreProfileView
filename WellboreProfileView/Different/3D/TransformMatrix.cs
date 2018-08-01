using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace WellboreProfileView
{
    public class TransformMatrix
    {
        private double scaleFactor = 1.1;

        private Viewport3D viewport3D;

        private Point mouseStartPoint;

        private bool mouseDown;
        public TransformMatrix(FeedbackViewport3D viewport3D)
        {
            this.viewport3D = viewport3D;
            viewport3D.SubstrateViewport3D.MouseMove += Viewport3DMouseMove;
            viewport3D.SubstrateViewport3D.MouseUp += Viewport3DMouseUp;
            viewport3D.SubstrateViewport3D.MouseDown += Viewport3DMouseDown;
            viewport3D.SubstrateViewport3D.MouseLeave += SubstrateViewport3DMouseLeave;
            viewport3D.SubstrateViewport3D.MouseWheel += SubstrateViewport3DMouseWheel;
        }

        private void SubstrateViewport3DMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Vector3D vector3D = e.Delta > 0 ? new Vector3D(scaleFactor, scaleFactor, scaleFactor) :
                                     new Vector3D(1 / scaleFactor, 1 / scaleFactor, 1 / scaleFactor);
            SetScale(vector3D);
        }

        private void SubstrateViewport3DMouseLeave(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void Viewport3DMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                mouseStartPoint = e.GetPosition(viewport3D);
                mouseDown = true;
            }
        }

        private void Viewport3DMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                mouseDown = false;
        }

        private void Viewport3DMouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseDown)
                return;

            Point currentPoint = e.GetPosition(viewport3D);
            double width = viewport3D.ActualWidth;
            double height = viewport3D.ActualHeight;

            double aY = 180 * (currentPoint.X - mouseStartPoint.X) / width;
            double aX = 180 * (currentPoint.Y - mouseStartPoint.Y) / height;
            SetRotate(new Quaternion(new Vector3D(1, 0, 0), aX), new Quaternion(new Vector3D(0, 1, 0), aY));
            mouseStartPoint = currentPoint;
        }

        private void SetRotate(Quaternion quaternionX, Quaternion quaternionY)
        {
            foreach (MatrixTransform3D matrixTransform3D in GetMatrixTransform3Ds())
            {
                Matrix3D currentMatrix3D = matrixTransform3D.Matrix;
                currentMatrix3D.Rotate(quaternionX);
                currentMatrix3D.Rotate(quaternionY);
                matrixTransform3D.Matrix = currentMatrix3D;
            }
        }

        private void SetScale(Vector3D vector3D)
        {
            foreach (MatrixTransform3D matrixTransform3D in GetMatrixTransform3Ds())
            {
                Matrix3D currentMatrix3D = matrixTransform3D.Matrix;
                currentMatrix3D.Scale(vector3D);
                matrixTransform3D.Matrix = currentMatrix3D;
            }
        }

        private List<MatrixTransform3D> GetMatrixTransform3Ds()
        {
            List<MatrixTransform3D> matrixTransform3Ds = new List<MatrixTransform3D>();

            List<ModelVisual3D> visuals3D = new List<ModelVisual3D>();
            foreach (Visual3D visual3D in viewport3D.Children)
            {
                ModelVisual3D modelVisual3D = visual3D as ModelVisual3D;
                if (modelVisual3D != null)
                    visuals3D.Add(modelVisual3D);
            }

            foreach (ModelVisual3D visual3D in visuals3D)
            {
                Transform3DGroup transform3DGroup = visual3D.Transform as Transform3DGroup;
                if (transform3DGroup == null)
                    continue;

                MatrixTransform3D matrixTransform3D = null;
                foreach (Transform3D transform3D in transform3DGroup.Children)
                {
                    if (transform3D is MatrixTransform3D)
                    {
                        matrixTransform3D = (MatrixTransform3D)transform3D;
                        break;
                    }
                }

                if (matrixTransform3D == null)
                {
                    matrixTransform3D = new MatrixTransform3D();
                    transform3DGroup.Children.Add(new MatrixTransform3D());
                }

                if (!matrixTransform3Ds.Contains(matrixTransform3D))
                    matrixTransform3Ds.Add(matrixTransform3D);
            }
            return matrixTransform3Ds;
        }
    }
}
