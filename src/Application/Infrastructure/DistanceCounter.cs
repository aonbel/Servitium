using Domain.Entities.Core;

namespace Application.Infrastructure;

public static class DistanceCounter
{
    private static readonly double EarthRadius = 6371;

    public static double GetDistance(Coordinates coordinates1, Coordinates coordinates2)
    {
        return GetDistance(coordinates1.Latitude, coordinates1.Longitude, coordinates2.Latitude, coordinates2.Longitude);
    }
    
    public static double GetDistance(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        var deltaLatitude = latitude1 - latitude2;
        var deltaLongitude = longitude1 - longitude2;
        
        var a = Math.Pow(Math.Sin(deltaLatitude / 2), 2) + Math.Cos(latitude1) * Math.Cos(latitude2) * Math.Pow(Math.Sin(deltaLongitude / 2), 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var distance = EarthRadius * c;
        
        return distance;
    }
}