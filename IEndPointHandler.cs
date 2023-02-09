internal interface IEndPointHandler
{
    List<DataPoint> GetData();
    List<DataPoint> GetDailyData();
    List<DataPoint> GetHourlyData();
    List<DataPoint> GetWeeklyData();
}