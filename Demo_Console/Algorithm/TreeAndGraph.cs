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

    // https://leetcode.com/problems/find-if-path-exists-in-graph
    public bool ValidPath(int n, int[][] edges, int source, int destination)
    {
        if (source == destination) return true;
        if (edges.Length == 0) return false;
        
        var graph = new Dictionary<int, List<int>>();
        
        foreach (var edge in edges)
        {
            if (!graph.ContainsKey(edge[0])) graph[edge[0]] = new List<int>();
            if (!graph.ContainsKey(edge[1])) graph[edge[1]] = new List<int>();
            
            graph[edge[0]].Add(edge[1]);
            graph[edge[1]].Add(edge[0]);
        }
        
        var visited = new HashSet<int>();
        var traversalStack = new Stack<int>(n);
        traversalStack.Push(source);

        while (traversalStack.Count > 0)
        {
            var node = traversalStack.Pop();
            if(node == destination) return true;
            
            foreach (var neighbor in graph[node])
            {
                if (visited.Add(neighbor)) traversalStack.Push(neighbor);
            }
        }

        return false;
    }
    
    // https://leetcode.com/problems/max-area-of-island
    public int MaxAreaOfIsland(int[][] grid)
    {
        if (grid == null || grid.Length == 0) return 0;

        var result = 0;
        var r = grid.Length;
        var c = grid[0].Length;
        var visited = new HashSet<(int i, int j)>();

        for (var i = 0; i < r; i++)
        {
            for (var j = 0; j < c; j++)
            {
                if (grid[i][j] == 1) result = Math.Max(result, DFS(i, j));
            }
        }

        int DFS(int i, int j)
        {
            if (i < 0 || i > r - 1) return 0;
            if (j < 0 || j > c - 1) return 0;
            if (grid[i][j] == 0) return 0;

            visited.Add((i, j));
            return DFS(i, j - 1) + DFS(i, j + 1) + DFS(i - 1, j) + DFS(i + 1, j) + 1;
        }
        
        return result;
    }
    
    
}