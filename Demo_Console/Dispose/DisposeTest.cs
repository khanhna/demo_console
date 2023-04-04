namespace Demo_Console.Dispose;

public static class DisposeTest
{
    public static List<StreamReader> Placeholder = new();
    public const string FilePath = "D:\\Workspace\\NetPractice\\Demo_Console\\test_file.docx";

    public static void ReadLine()
    {
        var keepRunning = true;
        while (keepRunning)
        {
            var input = Console.ReadLine();
            if (input == "exit")
            {
                keepRunning = false;
            }
            
            if(input == "test") TestFilePath();
            
            if(input == "1") ReadFileKeepReference();
            if(input == "2") ReadFileWithoutKeepReference();
        }
        
        Console.WriteLine("DisposeTest Finish!");
    }

    public static void ReadFileKeepReference()
    {
        for (int i = 0; i < 10; i++)
        {
            var file = new StreamReader(FilePath);
            int counter = 0; 
            
            while (file.ReadLine() != null) {
                counter++;  
            }  
            // file.Close();
            Console.WriteLine($"{i} - File has {counter} lines.");
            Placeholder.Add(file);
        }
        Console.WriteLine($"Placeholder have {Placeholder.Count} references!");
    }
    
    public static void ReadFileWithoutKeepReference()
    {
        for (int i = 0; i < 10; i++)
        {
            var file = new StreamReader(FilePath);
            int counter = 0;
            
            while (file.ReadLine() != null) {
                counter++;  
            }  
            // file.Close();
            Console.WriteLine($"{i} - File has {counter} lines.");
        }
    }

    public static void TestFilePath()
    {
        using var file = new StreamReader(FilePath);
        int counter = 0;
            
        while (file.ReadLine() != null) {
            counter++;  
        }  
        file.Close();
        Console.WriteLine($"File has {counter} lines.");
    }
}