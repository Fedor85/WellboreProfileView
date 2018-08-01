namespace WellboreProfileView.ToolBox
{
    public class DisplayPageRegion
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public DisplayPageRegion(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}