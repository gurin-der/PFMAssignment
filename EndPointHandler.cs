using System.Globalization;

internal class EndPointHandler : IEndPointHandler
{
    private List<DataPoint> _allDataPoints;
    private IDictionary<int, IList<DataPoint>> _dataPoints = new Dictionary<int, IList<DataPoint>>();

    public EndPointHandler(PFMAssignment.ISensorDataReader dataReader)
    {
        _allDataPoints = dataReader.ReadDataPoints();
        foreach(var dp in _allDataPoints)
        {
            if(!_dataPoints.TryGetValue(dp.SensorId, out var dps))
            {
                _dataPoints[dp.SensorId] = dps = new List<DataPoint>();
            }
            dps.Add(dp);
        }
    }

    List<DataPoint> IEndPointHandler.GetData() => _allDataPoints;

    List<DataPoint> IEndPointHandler.GetHourlyData()
    {
        var hourlyDataPoints = new List<DataPoint>();
        foreach(var dps in _dataPoints)
        {
            var r = dps.Value
                .GroupBy(dp => new DateTime(dp.DateFrom.Year, dp.DateFrom.Month, dp.DateFrom.Day, dp.DateFrom.Hour, 0, 0))
                .Select(g => new DataPoint
                {
                    SensorId = g.First().SensorId,
                    DateFrom = new DateTime
                        (g.First().DateFrom.Year, g.First().DateFrom.Month, g.First().DateFrom.Day, g.First().DateFrom.Hour, 0, 0),
                    DateTo = new DateTime
                        (g.First().DateFrom.Year, g.First().DateFrom.Month, g.First().DateFrom.Day, g.First().DateFrom.Hour, 0, 0).AddHours(1),
                    Count = g.Sum(g => g.Count)
                });
            hourlyDataPoints.AddRange(r);
        }
        return hourlyDataPoints;
    }

    List<DataPoint> IEndPointHandler.GetDailyData()
    {
        var dailyDataPoints = new List<DataPoint>();
        foreach (var dps in _dataPoints)
        {
            var r = dps.Value
                .GroupBy(dp => new DateTime(dp.DateFrom.Year, dp.DateFrom.Month, dp.DateFrom.Day))
                .Select(g => new DataPoint
                {
                    SensorId = g.First().SensorId,
                    DateFrom = new DateTime
                        (g.First().DateFrom.Year, g.First().DateFrom.Month, g.First().DateFrom.Day),
                    DateTo = new DateTime
                        (g.First().DateFrom.Year, g.First().DateFrom.Month, g.First().DateFrom.Day).AddDays(1),
                    Count = g.Sum(g => g.Count)
                });
            dailyDataPoints.AddRange(r);
        }
        return dailyDataPoints;
    }

    List<DataPoint> IEndPointHandler.GetWeeklyData()
    {
        var weeklyDataPoints = new List<DataPoint>();
        foreach (var dps in _dataPoints)
        {
            var r = dps.Value
                .GroupBy(dp => ISOWeek.ToDateTime(dp.DateFrom.Year, ISOWeek.GetWeekOfYear(dp.DateFrom), DayOfWeek.Monday))
                .Select(g => new DataPoint
                {
                    SensorId = g.First().SensorId,
                    DateFrom = ISOWeek.ToDateTime(g.First().DateFrom.Year, ISOWeek.GetWeekOfYear(g.Key), DayOfWeek.Monday),
                    DateTo = ISOWeek.ToDateTime(g.First().DateFrom.Year, ISOWeek.GetWeekOfYear(g.Key), DayOfWeek.Monday).AddDays(7),
                    Count = g.Sum(g => g.Count)
                });
            weeklyDataPoints.AddRange(r);
        }
        return weeklyDataPoints;
    }
}