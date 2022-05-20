using System.Collections.Generic;

public class WeatherData {
    public Dictionary<string, Dictionary<string, Times>> locations = new Dictionary<string, Dictionary<string, Times>> ();
    public Units unit;
}

public class Units {
    public string air_pressure_at_sea_level;
    public string air_temperature;
    public string cloud_area_fraction;
    public string precipitation_amount;
    public string relative_humidity;
    public string wind_from_direction;
    public string wind_speed;
}

public class Times {
    public string air_pressure_at_sea_level;
    public string air_temperature;
    public string cloud_area_fraction;
    public string precipitation_amount;
    public string relative_humidity;
    public string wind_from_direction;
    public string wind_speed;
    public string next_12_hours;
    public string next_1_hours;
    public string next_6_hours;
}
