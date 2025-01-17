namespace Demo_Console.Algorithm;

public class ListNode
{
    public int Val { get; set; }
    public ListNode? Next { get; set; }

    public ListNode(int val = 0, ListNode? next = null)
    {
        Val = val;
        Next = next;
    }
}

public class LinkedListAlgo
{
    // https://leetcode.com/problems/reverse-linked-list-ii/description/
    public static ListNode ReverseBetween(ListNode head, int left, int right) {
        if (head == null || left == right) return head;

        var dummy = new ListNode();
        dummy.Next = head;
        var leftOutterNode = dummy;

        for(int i = 0; i < left - 1; i++)
        {
            leftOutterNode = leftOutterNode.Next;
        }

        ListNode? initialNode = leftOutterNode.Next;
        ListNode? prev = initialNode;
        ListNode? current = prev.Next;
        ListNode? next = null;

        for(int i = 0; i < right - left; i++)
        {
            next = current.Next;
            current.Next = prev;
            prev = current;
            current = next;
        }

        leftOutterNode.Next = prev;
        initialNode.Next = current;

        return dummy.Next;
    }
}