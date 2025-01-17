namespace Demo_Console.Algorithm.Array;

public class MultiDimensional
{
    public static int[,] Matrix = {
        {1, 2, 3, 4},
        {5, 6, 7, 8},
        {9, 10, 11, 12}
    };

    public static void RowMajorTraverse(int[,] matrix)
    {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);
        var row = 0;
        var col = 0;
        var output = new int[rows * cols];
        var index = 0;

        while (index < rows * cols)
        {
            if(row == rows) break;
            
            output[index++] = matrix[row, col];

            if (col < cols - 1)
            {
                col++;
            }
            else
            {
                col = 0;
                row++;
            }
        }
        
        Console.WriteLine(string.Join(',', output));
    }
    
    public static void ColMajorTraverse(int[,] matrix)
    {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);
        var row = 0;
        var col = 0;
        var output = new int[rows * cols];
        var index = 0;

        while (index < rows * cols)
        {
            if(col == cols) break;
            
            output[index++] = matrix[row, col];

            if (row < rows - 1)
            {
                row++;
            }
            else
            {
                row = 0;
                col++;
            }
        }
        
        Console.WriteLine(string.Join(',', output));
    }
    
    public static void ColumnTraverse(int[,] matrix) {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);
        var direction = "up";
        var row = rows - 1;
        var col = cols - 1;
        var output = new int[rows * cols];
        var index = 0;

        while (index < rows * cols) {
            output[index++] = matrix[row, col];

            if (direction == "up") {
                if (row - 1 < 0) {
                    direction = "down";
                    col -= 1;
                } else {
                    row -= 1;
                }
            } else {
                if (row + 1 == rows) {
                    direction = "up";
                    col -= 1;
                } else {
                    row += 1;
                }
            }
        }
        
        Console.WriteLine(string.Join(',', output));
    }
}