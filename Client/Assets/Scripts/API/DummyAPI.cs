using Model;
using System;

public class DummyAPI : IAPI
{
    public void GetCity(long id, Action<City> callback)
    {
        int width = 10, height = 10;
        var city = new City();
        Field[] fields = new Field[10 * 10];
        var random = new Random((int)id);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                var types = (FieldType[])Enum.GetValues(typeof(FieldType));

                var field = new Field();
                field.x = j;
                field.y = i;
                field.fieldType = types[random.Next() % types.Length];
                fields[i * height + j] = field;
            }
        }
        city.width = width;
        city.height = height;
        city.fields = fields;
        callback(city);
    }
}
