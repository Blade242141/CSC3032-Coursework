public class RawData {
    string ip;
    string type;
    string continent;
    string continent_code;
    string country;
    string country_code;
    string region;
    double latitude;
    double longitude;
    string timezone_name;
    string timezone_gmt;

    #region Set
    public void SetIp(string _ip) {
        ip = _ip;
    }
    public void SetIpType(string _type) {
        type = _type;
    }
    public void SetContinent(string _continent) {
        continent = _continent;
    }
    public void SetContinent_code(string _continent_code) {
        continent_code = _continent_code;
    }
    public void SetCountry(string _country) {
        country = _country;
    }
    public void SetCountry_code(string _country_code) {
        country_code = _country_code;
    }
    public void SetRegion(string _region) {
        region = _region;
    }
    public void SetLatitude(double _latitude) {
        latitude = _latitude;
    }
    public void SetLongitude(double _longitude) {
        longitude = _longitude;
    }
    public void SetTimezone_name(string _timezone_name) {
        timezone_name = _timezone_name;
    }
    public void SetTimezone_gmt(string _timezone_gmt) {
        timezone_gmt = _timezone_gmt;
    }
    #endregion

    #region Get
    public string GetIp() {
        return ip;
    }
    public string GetIpType() {
        return type;
    }
    public string GetContinent() {
        return continent;
    }
    public string GetContinent_code() {
        return continent_code;
    }
    public string GetCountry() {
        return country;
    }
    public string GetCountry_code() {
        return country_code;
    }
    public string GetRegion() {
        return region;
    }
    public double GetLatitude() {
        return latitude;
    }
    public double GetLongitude() {
        return longitude;
    }
    public string GetTimezone_name() {
        return timezone_name;
    }
    public string GetTimezone_gmt() {
        return timezone_gmt;
    }
    #endregion
}
