namespace API
{
    [System.Serializable]
    public struct City
    {
        public long id;
        public int width, height;
        public Field[] fields;
        public int tickDuration;
        public int food, wood, metal;
    }

    [System.Serializable]
    public struct Field
    {
        public int x, y;
        public FieldType fieldType;
        public ResourceType resourceType;
        public BuildingType buildingType;
        public int buildingLevel;
    }

    [System.Serializable]
    public struct ResourceStash
    {
        public int X, Y;
        public int Food, Wood, Metal;

        public ResourceCollection ToResourceCollection()
        {
            return new ResourceCollection
            {
                Food = Food,
                Wood = Wood,
                Metal = Metal
            };
        }
    }

    public enum BuildingType
    {
        None, House, Applefarm, Fishingboat, Lumberjack, Mine
    }

    public enum ResourceType
    {
        None, Apples, Fish, Forest, Ore
    }
}