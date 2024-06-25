using System;

namespace TreeLib;

public enum Side {Left, Right}

class Node<T> where T: IComparable
{
    public T Data{get; set;}

    public Node<T> Ancestor{get; set;}

    public Node<T> LeftDescendant{get; set;}

    public Node<T> RightDescendant{get; set;}

    public Side? NodeSide
    {
        get => Ancestor is null ? null : Ancestor.LeftDescendant == this ? Side.Left : Side.Right;

        private set{}
    }

    public override string ToString() => Data.ToString();
}

class BinaryTree<T> where T: IComparable
{
    public Node<T> Root{get; set;}

    private Node<T> Add(Node<T> node, Node<T> current_node = null)
    {
        if(Root is null)
        {
            node.Ancestor = null;
            return Root = node;
        }
        current_node = current_node ?? Root;
        node.Ancestor = current_node;
        var result = node.Data.CompareTo(current_node.Data);
        if(result == 0)
            return current_node;
        else
        {
            if(result < 0)
            {
                if(current_node.LeftDescendant is null)
                    return current_node.LeftDescendant = node;
                else return Add(node, current_node.LeftDescendant);
            }
            else
            {
                if(current_node.RightDescendant is null)
                    return current_node.RightDescendant = node;
                else return Add(node, current_node.RightDescendant);
            }
        }
    }

    public Node<T> Add(T Data) => Add(new Node<T>(){Data = Data});

    public void Remove(T data) => Remove(Find(data));

    public void Remove(Node<T> node)
    {
        if(node is null) return;
        var current_node_side = node.NodeSide;
        //1. у узла нет потомков
        if(node.LeftDescendant is null && node.RightDescendant is null)
        {
            if(current_node_side == Side.Left)
                node.Ancestor.LeftDescendant = node.RightDescendant;
            else node.Ancestor.RightDescendant = node.RightDescendant;
            node.RightDescendant.Ancestor = node.Ancestor;
        }
        //2. у узла нет левого потомка
        else if(node.LeftDescendant is null)
        {
            if(current_node_side == Side.Left)
                node.Ancestor.LeftDescendant = node.RightDescendant;
            else node.Ancestor.RightDescendant = node.RightDescendant;
            node.LeftDescendant.Ancestor = node.Ancestor;
        }
        //3. у узла нет правого потомка
        else if(node.RightDescendant is null)
        {
            if(current_node_side == Side.Left)
                node.Ancestor.LeftDescendant = node.LeftDescendant;
            else node.Ancestor.RightDescendant = node.LeftDescendant;
            node.LeftDescendant.Ancestor = node.Ancestor;
        }
        //3. у узла имеются оба потомка
        else
        {
            switch(current_node_side)
            {
                case Side.Left:
                    node.Ancestor.LeftDescendant = node.RightDescendant;
                    node.RightDescendant.Ancestor = node.Ancestor;
                    Add(node.LeftDescendant, node.RightDescendant);
                    break;
                case Side.Right:
                    node.Ancestor.RightDescendant = node.RightDescendant;
                    node.RightDescendant.Ancestor = node.Ancestor;
                    Add(node.LeftDescendant, node.RightDescendant);
                    break;
                default:
                    var buf_l = node.LeftDescendant;
                    var buf_rl = node.RightDescendant.LeftDescendant;
                    var buf_rr = node.RightDescendant.RightDescendant;
                    node.Data = node.RightDescendant.Data;
                    node.RightDescendant = buf_rr;
                    node.LeftDescendant = buf_rl;
                    Add(buf_l, node);
                    break;  
            }
        }
    }

    public Node<T> Find(T data, Node<T> start_node = null)
    {
        start_node = start_node ?? Root;
        var result = data.CompareTo(start_node.Data);
        if(result == 0)
            return start_node;
        else
        {
            if(result < 0)
            {
                if(start_node.LeftDescendant is null)
                    return null;
                else return Find(data, start_node.LeftDescendant);
            }
            else
            {
                if(start_node.RightDescendant is null)
                    return null;
                else return Find(data, start_node.RightDescendant);
            }
        }
    }

    public void PrintTree(Node<T> startNode, string indent = "", Side? side = null)
    {
        if (startNode != null)
        {
            var nodeSide = side == null ? "+" : side == Side.Left ? "L" : "R";
            Console.WriteLine($"{indent} [{nodeSide}]- {startNode.Data}");
            indent += new string(' ', 3);
            //рекурсивный вызов для левой и правой веток
            PrintTree(startNode.LeftDescendant, indent, Side.Left);
            PrintTree(startNode.RightDescendant, indent, Side.Right);
        }
    }

    public void PrintTree() => PrintTree(Root);
}

class Program
{
    static void Main(string[] args)
    {
        var binaryTree = new BinaryTree<int>();
        
        binaryTree.Add(8);
        binaryTree.Add(3);
        binaryTree.Add(10);
        binaryTree.Add(1);
        binaryTree.Add(6);
        binaryTree.Add(4);
        binaryTree.Add(7);
        binaryTree.Add(14);
        binaryTree.Add(16);

        binaryTree.PrintTree();

        Console.WriteLine(new string('-', 40));
        binaryTree.Remove(3);
        binaryTree.PrintTree();

        Console.WriteLine(new string('-', 40));
        binaryTree.Remove(8);
        binaryTree.PrintTree();

        Console.ReadLine();
    }
}