namespace Demo_Console.Algorithm.Array;

public class SingleDimensional
{
    // https://leetcode.com/problems/max-consecutive-ones-iii/description/
    public static int LongestOnes(int[] nums, int k)
    {
        int left = 0, right = 0, currentZero = 0, maxOnes = 0;

        while (right < nums.Length)
        {
            if (nums[right] == 0) currentZero++;

            while (currentZero > k)
            {
                if (nums[left] == 0) currentZero--;
                left++;
            }
            
            maxOnes = Math.Max(maxOnes, right - left + 1);
            right++;
        }
        
        return maxOnes;
    } 
}