namespace Demo_Console.Algorithm.Array;

public class StackQueue
{
    // https://leetcode.com/problems/make-the-string-great
    public static string MakeGood(string s)
    {
        var stack = new Stack<char>();

        foreach (var c in s)
        {
            if (stack.TryPeek(out var targetChar) && char.ToUpperInvariant(targetChar) == char.ToUpperInvariant(c) && targetChar != c)
                stack.Pop();
            else
                stack.Push(c);
        }

        var result = stack.ToArray();
        System.Array.Reverse(result);
        return new string(result);
    }
    
    // https://leetcode.com/problems/next-greater-element-i
    public static int[] NextGreaterElement(int[] nums1, int[] nums2)
    {
        var nextGreater = new int[nums2.Length];
        var st = new Stack<int>();
        for (var i = 0; i < nums2.Length; i++)
        {
            nextGreater[i] = -1;
            while (st.TryPeek(out var peek) && nums2[peek] < nums2[i])
                nextGreater[st.Pop()] = nums2[i];
            st.Push(i);
        }
        
        var dict = new Dictionary<int, int>();
        for (var i = 0; i < nums2.Length; i++)
        {
            dict.Add(nums2[i], i);
        }
        
        var result = new int[nums1.Length];
        for (var i = 0; i < nums1.Length; i++)
        {
            result[i] = nextGreater[dict[nums1[i]]];
        }
        
        return result;
    }
}

// https://leetcode.com/problems/online-stock-span
public class StockSpanner {
    public Stack<(int price, int span)> Stack { get; set; }
    
    public StockSpanner() {
        Stack = new Stack<(int price, int span)>();
    }
    
    public int Next(int price)
    {
        var span = 1;
        while (Stack.TryPeek(out (int price, int span) peek) && peek.price <= price) span += Stack.Pop().span;
        
        Stack.Push((price, span));

        return span;
    }
}