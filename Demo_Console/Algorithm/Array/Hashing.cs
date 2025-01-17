using System.Text;

namespace Demo_Console.Algorithm.Array;

public class Hashing
{
    // https://leetcode.com/problems/longest-substring-without-repeating-characters
    public static int LengthOfLongestSubstring(string s)
    {
        int left = 0, maxLength = 0;
        var chars = new HashSet<char>();

        for (var right = 0; right < s.Length; right++)
        {
            while (chars.Contains(s[right]))
            {
                chars.Remove(s[left++]);
            }
            
            chars.Add(s[right]);
            maxLength = Math.Max(maxLength, right - left + 1);
        }
        
        return 1;
    }
    
    // https://leetcode.com/problems/maximum-number-of-balloons
    private static readonly string TargetBalloonString = "balloon";
    public static int MaxNumberOfBalloons(string s)
    {
        var dict = new Dictionary<char, int>();
        var result = int.MaxValue;
        
        for (var i = 0; i < s.Length; i++)
        {
            if (TargetBalloonString.Contains(s[i])) dict[s[i]] = dict.GetValueOrDefault(s[i], 0) + 1;
        }

        if (dict.Count != 5) return 0;

        dict['o'] /= 2;
        dict['l'] /= 2;

        foreach (var pair in dict)
        {
            result = Math.Min(result, pair.Value);
        }
        
        return result;
    }
    
    // https://leetcode.com/problems/jewels-and-stones
    public static int NumJewelsInStones(string jewels, string stones)
    {
        if (string.IsNullOrEmpty(jewels) || string.IsNullOrEmpty(stones)) return 0;
        var dict = new Dictionary<char, int>(256);
        var result = 0;

        for (var i = 0; i < stones.Length; i++)
        {
            dict[stones[i]] = dict.GetValueOrDefault(stones[i], 0) + 1;
        }

        for (var i = 0; i < jewels.Length; i++)
        {
            result += dict.GetValueOrDefault(jewels[i], 0);
        }

        return result;
    }

    // https://leetcode.com/problems/remove-duplicate-letters
    // Don't even know what's exicographical order
    public static string RemoveDuplicateLetters(string s)
    {
        var ss = new SortedSet<char>();
        for (var i = 0; i < s.Length; i++)
        {
            ss.Add(s[i]);
        }

        var result = new StringBuilder(ss.Count);
        foreach (var c in ss) result.Append(c);
        return result.ToString();
    }
}