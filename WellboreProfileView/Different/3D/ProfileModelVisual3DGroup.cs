using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace WellboreProfileView
{
    public class ProfileModelVisual3DGroup
    {
        private ModelVisual3D gridModelVisual3D { get; set; }

        private ModelVisual3D trajectoryModelVisual3D { get; set; }

        public Model3DGroup GridModel3DGroup
        {
            get { return (Model3DGroup)gridModelVisual3D.Content; }
        }

        public Model3DGroup TrajectoryModel3DGroup
        {
            get { return (Model3DGroup)trajectoryModelVisual3D.Content; }
        }


        public List<ModelVisual3D> TextModelVisual3D { get; set; }

        public ProfileModelVisual3DGroup()
        {
            gridModelVisual3D = new ModelVisual3D();
            gridModelVisual3D.Content = new Model3DGroup();
            gridModelVisual3D.Transform = new Transform3DGroup();

            trajectoryModelVisual3D = new ModelVisual3D();
            trajectoryModelVisual3D.Content = new Model3DGroup();
            trajectoryModelVisual3D.Transform = new Transform3DGroup();

            TextModelVisual3D = new List<ModelVisual3D>();
        }

        public void AddTransform3DGroup(Transform3DGroup transform3DGroup)
        {
            Transform3DGroup gridTransform3DGroup = (Transform3DGroup)gridModelVisual3D.Transform;
            Transform3DGroup trajectoryTransform3DGroup = (Transform3DGroup)trajectoryModelVisual3D.Transform;
            List<Transform3DGroup> depthsTransform3DGroup = new List<Transform3DGroup>();
            foreach (ModelVisual3D depthModelVisual3D in TextModelVisual3D)
                depthsTransform3DGroup.Add((Transform3DGroup)depthModelVisual3D.Transform);

            foreach (Transform3D transform3D in transform3DGroup.Children)
            {
                gridTransform3DGroup.Children.Add(transform3D);
                trajectoryTransform3DGroup.Children.Add(transform3D);
                foreach (Transform3DGroup depthTransform3DGroup in depthsTransform3DGroup)
                    depthTransform3DGroup.Children.Add(transform3D);
            }
        }

        public void AddToViewport3D(Viewport3D viewport3D)
        {
            viewport3D.Children.Add(gridModelVisual3D);
            viewport3D.Children.Add(trajectoryModelVisual3D);
            foreach (ModelVisual3D depthModelVisual3D in TextModelVisual3D)
            {
                viewport3D.Children.Add(depthModelVisual3D);
            }
        }

        public void AddLight(Light light)
        {
            GridModel3DGroup.Children.Add(light);
            TrajectoryModel3DGroup.Children.Add(light);
        }
    }
}