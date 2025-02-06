using System.IO.Ports;

namespace Demo_Console.DevicesIntegration.BillAcceptors;

/// <summary>
/// Design for TOP <i>TP70P3B-VN60Jil</i>
/// </summary>
public static class TopBillAcceptor
{
    public const string ConsoleDescription = "Bill acceptor operation: 1. Open Connection, 2. Reset, 3. Close Connection, 4. Exit";
    private static bool _isTerminated;
    
    public const string StandardPort = "COM3";
    public const int StandardBaudRate = 9600;
    public const int StandardDataBit = 8;

    private static SerialPort? _serialPort;
    public static decimal DepositAmount { get; private set; }
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

            _serialPort = new SerialPort(StandardPort, StandardBaudRate, Parity.Even, StandardDataBit, StopBits.One);
            _serialPort.DataReceived += SerialPortDataReceived;
            if (!_serialPort.IsOpen) _serialPort.Open();

            byte[]
                dataToSend = [0x02]; // In case SerialPort is already open, can send byte `0x3E`, but not sure what for?
            _serialPort.Write(dataToSend, 0, dataToSend.Length);

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
    }

    private static void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        // Đọc dữ liệu từ cổng COM
        var serialPort = (SerialPort)sender;
        var data = serialPort.ReadExisting();
        decimal money = 0;
        if (data.Contains("@"))
        {
            money = 10000;
        }
        else if (data.Contains("A"))
        {
            money = 20000;
        }
        else if (data.Contains("B"))
        {
            money = 50000;
        }
        //else
        //if (data.Contains("C"))
        //{
        //    money = "100000";
        //}
        //else
        //if (data.Contains("D"))
        //{
        //    money = "200000";
        //}
        //else
        //if (data.Contains("E"))
        //{
        //    money = "500000";
        //}

        if (money == 0) return;
        
        DepositAmount += money;
        // Experiment
        Console.WriteLine($"--> Data received: {data}, Deposit amount: {money}, Total amount: {DepositAmount}");

        byte[] dataToSend = [0x02];
        serialPort.Write(dataToSend, 0, dataToSend.Length);
    }
}