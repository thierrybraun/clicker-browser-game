namespace API
{
    [System.Serializable]
    public struct City
    {
        public long id;
        public int width, height;
        public Field[] fields;
        public int tickDuration;
        public Currency currency;
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

    public enum BuildingType
    {
        None, House, Applefarm, Fishingboat, Lumberjack, Mine
    }

    public enum ResourceType
    {
        None, Apples, Fish, Forest, Ore
    }
}