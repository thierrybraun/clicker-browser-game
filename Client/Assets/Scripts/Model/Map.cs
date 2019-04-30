namespace Model
{
    [System.Serializable]
    public struct City
    {
        public int width, height;
        public Field[] fields;
        public Building[] buildings;        
    }

    [System.Serializable]
    public struct Field
    {
        public int x, y;
        public FieldType fieldType;
        public ResourceType? resourceType;
    }

    [System.Serializable]
    public struct Building
    {
        public int x, y;
        public BuildingType buildingType;
    }
}