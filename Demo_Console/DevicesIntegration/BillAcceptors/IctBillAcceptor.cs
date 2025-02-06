using System.IO.Ports;

namespace Demo_Console.DevicesIntegration.BillAcceptors;

/// <summary>
/// Design for ICT <i>TK77</i>. Model <i>TK77TH30LSPFN</i>
/// </summary>
public class IctBillAcceptor
{
    public const string ConsoleDescription =
        "Bill acceptor operation: 1. Open Connection, 2. Reset, 3. Close Connection, 4. Exit";
    
    public static readonly Dictionary<string, string> ErrorCodes = new()
    {
        {"20", "Error bill - Motor Failure"},
        {"21", "Error bill - Checksum Error"},
        {"22", "Error bill - Bill Jam"},
        {"23", "Error bill - Bill Remove"},
        {"24", "Error bill - Stacker Open"},
        {"25", "Error bill - Sensor Problem"},
        {"27", "Error bill - Bill Fish"},
        {"28", "Error bill - Stacker Problem"},
        {"29", "Error bill - Bill Reject"}
    };  

    private static bool _isTerminated;

    public const string StandardPort = "COM3";
    public const int StandardBaudRate = 9600;
    public const int StandardDataBit = 8;

    private static SerialPort? _serialPort;
    public static decimal DepositAmount { get; private set; }
    private static decimal _amount;
    private static List<string> _readDataBuffer = new(64);
    public static bool IsConnectionOpen() => _serialPort?.IsOpen ?? false;

    /// <summary>
    /// Open Connection with Bill Acceptor device
    /// </summary>
    /// <returns>Result for operation and the reason</returns>
    public static async ValueTask<(bool, string)> OpenConnection()
    {
        if (IsConnectionOpen()) return (false, "Connection is already established!");

        try
        {
            if (_serialPort != null)
            {
                ResetState();
                await Task.Delay(100);
            }

            _serialPort = new SerialPort(StandardPort, StandardBaudRate, Parity.Even, StandardDataBit)
            {
                ReadTimeout = 100000
            };
            _serialPort.DataReceived += SerialPortDataReceived;
            if (!_serialPort.IsOpen) _serialPort.Open();

            byte[] dataToSend = [0x02]; // for powerup
            _serialPort.Write(dataToSend, 0, dataToSend.Length); // In case SerialPort is already open, can send byte `0x3E`, but not sure what for?

            return (true, "Connection established!");
        }
        catch (Exception ex)
        {
            // Logging
            return (false, ex.Message);
        }
    }

    /// <summary>
    /// Reset Bill Acceptor, <i>in case data is not being received.</i>
    /// </summary>
    /// <returns></returns>
    public static (bool, string) Reset()
    {
        if (!IsConnectionOpen()) return (false, "Connection is not initialized yet!");

        try
        {
            _amount = 0;
            _readDataBuffer.Clear();
            
            byte[] dataToSend = [0x30];
            _serialPort!.Write(dataToSend, 0, dataToSend.Length);

            return (true, "Reset done!");
        }
        catch (Exception ex)
        {
            // Logging
            return (false, ex.Message);
        }
    }
    
    /// <summary>
    /// Close Connection with Bill Acceptor!
    /// </summary>
    /// <returns></returns>
    public static async ValueTask<(bool, string)> CloseConnection()
    {
        if (!IsConnectionOpen()) return (false, "There's no current connection to close!");

        try
        {
            if (_serialPort != null)
            {
                byte[] dataToSend = [0x5E];
                _serialPort.Write(dataToSend, 0, dataToSend.Length);

                await Task.Delay(100);
                ResetState();
                
                return (true, "Connection closed!");
            }
            else
            {
                return (false, "There's no port open to close!");
            }
        }
        catch (Exception ex)
        {
            // Logging
            return (false, ex.Message);
        }
    }
    
    /// <summary>
    /// Experiment console operation
    /// </summary>
    public static async ValueTask ConsoleExecution()
    {
        while (!_isTerminated)
        {
            Console.WriteLine(ConsoleDescription);
            var option = Console.ReadLine();
            if (string.IsNullOrEmpty(option) || !int.TryParse(option, out var choice) || choice < 1 || choice > 4)
            {
                Console.WriteLine("Invalid choice, please try again!");
                continue;
            }

            switch (choice)
            {
                case 1:
                    var (_, openConnResult) = await OpenConnection();
                    Console.WriteLine($"--> {openConnResult}");
                    // Console.WriteLine("--> Option 1 received");
                    break;
                case 2:
                    var (_, resetConnResult) = Reset();
                    Console.WriteLine($"--> {resetConnResult}");
                    // Console.WriteLine("--> Option 2 received");
                    break;
                case 3:
                    var (_, closeConnResult) = await CloseConnection();
                    Console.WriteLine($"--> {closeConnResult}");
                    // Console.WriteLine("--> Option 3 received");
                    break;
                case 4:
                    _isTerminated = true;
                    var (_, result) = await CloseConnection();
                    Console.WriteLine($"--> {result}");
                    // Console.WriteLine("--> Option 4 received");
                    break;
            }
        }
    }
    
    private static void ResetState()
    {
        if (_serialPort != null)
        {
            _serialPort.Dispose();
            _serialPort = null;
        }
        
        DepositAmount = 0;
        _amount = 0;
        _readDataBuffer.Clear();
    }

    private static void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var data = ((SerialPort)sender).ReadByte();
        var hexData = data.ToString("X");
        if (hexData is "0" or "E0" or "CO") return;

        _readDataBuffer.Add(hexData);

        if (_readDataBuffer.Count > 0)
        {
            switch (_readDataBuffer[0])
            {
                case "10":
                    DepositAmount += _amount;
                    Console.WriteLine($"--> Received amount: {_amount}, Total deposit amount: {DepositAmount}");
                    _readDataBuffer.Clear();
                    break;
                case "20":
                case "21":
                case "22":
                case "23":
                case "24":
                case "25":
                case "27":
                case "28":
                    Console.WriteLine(ErrorCodes[_readDataBuffer[0]]);
                    Reset();
                    break;
                case "29":
                    Console.WriteLine(ErrorCodes[_readDataBuffer[0]]);
                    break;
                case "3E":
                case "8F":
                    _readDataBuffer.Clear();
                    return;
            }
        }

        Console.WriteLine($"--> Current reading data buffer: {string.Join(", ", _readDataBuffer)}");

        if (_readDataBuffer.Count > 1)
        {
            byte[] dataToSend = [0x02];
            if (_readDataBuffer[0] == "80" && _readDataBuffer[1] == "8F")
            {
                _serialPort?.Write(dataToSend, 0, dataToSend.Length);
                _readDataBuffer.Clear();
                return;
            }

            if (!int.TryParse(_readDataBuffer[1], out var billValueIdx))
            {
                throw new InvalidOperationException($"Cannot parse bill value index from {_readDataBuffer[1]}!");
            }

            if (_readDataBuffer[0] == "81" && billValueIdx is >= 41 and <= 44)
            {
                var billValue = CurrencyFaceValues.VND[billValueIdx % 40];
                _amount = billValue;
                Console.WriteLine($"--> Received bill value: {billValue}");
                _serialPort?.Write(dataToSend, 0, dataToSend.Length);
                _readDataBuffer.Clear();
                return;
            }

            if (_readDataBuffer[0] is not "80" and "81" || (_readDataBuffer[0] == "81" && billValueIdx is 40 or 45))
            {
                _readDataBuffer.Clear();
                return;
            }
        }
    }
}

public static class CurrencyFaceValues
{
    public static readonly int[] VND = { 5000, 10000, 20000, 50000, 100000, 200000, 500000 };
    public static readonly int[] HHR = { 100, 500, 1000, 2000, 5000, 10000 };
    public static readonly int[] THB = { 1, 10, 100, 1000 };
    public static readonly int[] PHP = { 20, 50, 100, 200, 500, 1000 };
    public static readonly int[] MYR = { 5, 10, 20, 50, 100, 200, 1000, 5000, 10000 };
    public static readonly int[] KRW = { 1000, 5000, 10000 };
    public static readonly int[] CNY = { 1, 5, 10, 20, 50, 100 };
    public static readonly int[] USD = { 1, 2, 5, 10, 20, 50, 100 };
}