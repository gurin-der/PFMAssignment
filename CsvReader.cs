namespace PFMAssignment
{
    public class CsvReader : ISensorDataReader
    {
        private readonly string _datafile;
        private readonly bool _isHeader;

        private int _indexOfSensorId = 0;
        private int _indexOfDateFrom = 1;
        private int _indexOfDateTo = 2;
        private int _indexOfCount = 3;


        internal CsvReader(string datafile, bool isHeader)
        {
            if(!File.Exists(datafile))
            {
                throw new FileNotFoundException($"File doesn't exist: {datafile}");
            }

            _datafile = datafile;
            _isHeader = isHeader;
        }

        List<DataPoint> ISensorDataReader.ReadDataPoints()
        {
            var lines = File.ReadAllLines( _datafile );
            var startIdx = 0;
            if(_isHeader)
            {
                ReadHeader(lines);
                startIdx = 1;
            }

            var dataPoints = new List<DataPoint>();
            for(var idx = startIdx; idx < lines.Length; idx++)
            {
                try
                {
                    dataPoints.Add(GetDataPoint(lines[idx]));
                }
                catch
                {
                    Console.WriteLine($"Ignore invalid line: {lines[idx]}");
                }
            }

            return dataPoints;
        }

        private DataPoint GetDataPoint(string v)
        {
            var data = v.Split(',');
            return new DataPoint
            {
                SensorId = int.Parse(data[_indexOfSensorId]),
                DateFrom = DateTime.Parse(data[_indexOfDateFrom]),
                DateTo = DateTime.Parse(data[_indexOfDateTo]),
                Count = int.Parse(data[_indexOfCount]),
            };
        }

        private void ReadHeader(string[] lines)
        {
            if (lines.Length < 1)
                throw new InvalidDataException($"Invalid data in {_datafile}");

            var headers = lines[0].Split(',');
            if (headers.Length != 4)
            {
                throw new InvalidDataException($"Data file {_datafile} must have 4 columns");
            }

            _indexOfSensorId = Array.IndexOf(headers, nameof(DataPoint.SensorId));
            if (_indexOfSensorId < 0)
            {
                throw new InvalidDataException($"Invalid header in {_datafile}, missing {nameof(DataPoint.SensorId)}");
            }
            _indexOfDateFrom = Array.IndexOf(headers, nameof(DataPoint.DateFrom));
            if (_indexOfDateFrom < 0)
            {
                throw new InvalidDataException($"Invalid header in {_datafile}, missing {nameof(DataPoint.DateFrom)}");
            }
            _indexOfDateTo = Array.IndexOf(headers, nameof(DataPoint.DateTo));
            if (_indexOfDateTo < 0)
            {
                throw new InvalidDataException($"Invalid header in {_datafile}, missing {nameof(DataPoint.DateTo)}");
            }
            _indexOfCount = Array.IndexOf(headers, nameof(DataPoint.Count));
            if (_indexOfCount < 0)
            {
                throw new InvalidDataException($"Invalid header in {_datafile}, missing {nameof(DataPoint.Count)}");
            }
        }
    }
}
