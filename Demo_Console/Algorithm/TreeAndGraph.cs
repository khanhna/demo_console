namespace Demo_Console.Algorithm;

public class TreeNode
{
    public int val;
    public TreeNode left;
    public TreeNode right;

    public TreeNode(int val = 0, TreeNode left = null, TreeNode right = null)
    {
        this.val = val;
        this.left = left;
        this.right = right;
    }
}

public class TreeAndGraph
{
    // https://leetcode.com/problems/minimum-depth-of-binary-tree/
    public int DFSMinDepth(TreeNode? root)
    {
        if (root == null) return 0;

        var leftMinDepth = DFSMinDepth(root.left);
        var rightMinDepth = DFSMinDepth(root.right);

        if (leftMinDepth == 0 || rightMinDepth == 0) return Math.Max(leftMinDepth, rightMinDepth) + 1;

        return Math.Min(leftMinDepth, rightMinDepth) + 1;
    }

    // https://leetcode.com/problems/minimum-depth-of-binary-tree/
    public int BFSMinDepth(TreeNode? root)
    {
        if (root == null) return 0;
        
        var queue = new Queue<TreeNode?>();
        queue.Enqueue(root);

        var depth = 0;

        while (queue.Count > 0)
        {
            var size = queue.Count;

            for (var i = 0; i < size; i++)
            {
                var current = queue.Dequeue();
                
                if(current?.left != null) queue.Enqueue(current.left);
                if(current?.right != null) queue.Enqueue(current.right);

                if (current?.left == null && current?.right == null) return depth + 1;
            }
            
            depth++;
        }

        return 0;
    }

    // https://leetcode.com/problems/balanced-binary-tree/
    public bool CheckIfTreeIsBalanced(TreeNode? root)
    {
        if(root == null) return true;

        if (Math.Abs(GetTreeHeight(root.left) - GetTreeHeight(root.right)) <= 1)
            return CheckIfTreeIsBalanced(root.left) && CheckIfTreeIsBalanced(root.right);
        else return false;
    }

    private int GetTreeHeight(TreeNode? root) =>
        root == null ? 0 : Math.Max(GetTreeHeight(root.left), GetTreeHeight(root.right)) + 1;
    
    // https://leetcode.com/problems/deepest-leaves-sum/description/
    public int DeepestLeavesSum(TreeNode? root)
    {
        if (root == null) return 0;
        
        var queue = new Queue<TreeNode?>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var isNextLevelExist = false;
            var size = queue.Count;
            var currentLevelNodes = new TreeNode[size];

            for (var i = 0; i < size; i++)
            {
                var current = queue.Dequeue();
                currentLevelNodes[i] = current!;
                
                if(current?.left != null)
                {
                    queue.Enqueue(current.left);
                    isNextLevelExist = true;
                }
                
                if(current?.right != null)
                {
                    queue.Enqueue(current.right);
                    isNextLevelExist = true;
                }
            }
            
            if(!isNextLevelExist) return currentLevelNodes.Sum(x => x.val);
        }

        return 0;
    }

    // https://leetcode.com/problems/binary-tree-zigzag-level-order-traversal
    public IList<IList<int>> ZigzagLevelOrder(TreeNode? root)
    {
        if (root == null) return new List<IList<int>>();
        
        var queue = new Queue<TreeNode?>();
        queue.Enqueue(root);
        var isLeftToRightDirection = true;
        var result = new List<IList<int>>();

        while (queue.Count > 0)
        {
            var subResult = new List<int>();
            var size = queue.Count;

            for (var i = 0; i < size; i++)
            {
                var current = queue.Dequeue();
                subResult.Add(current!.val);
                
                if(current?.left != null) queue.Enqueue(current.left);    
                if(current?.right != null) queue.Enqueue(current.right);
            }

            if(isLeftToRightDirection) result.Add(subResult);
            else
            {
                subResult.Reverse();
                result.Add(subResult);
            }
            
            isLeftToRightDirection = !isLeftToRightDirection;
        }

        return result;
    }
}