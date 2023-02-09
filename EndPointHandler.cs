internal class EndPointHandler : IEndPointHandler
{
    private DataPoint[] _dataPoints;

    public EndPointHandler(PFMAssignment.ISensorDataReader dataReader)
    {
        _dataPoints = dataReader.ReadDataPoints();
    }

    public DataPoint[] GetData()
    {
        return _dataPoints;
    }
}