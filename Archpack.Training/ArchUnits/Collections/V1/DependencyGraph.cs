using System.Collections.Generic;

namespace Archpack.Training.ArchUnits.Collections.V1
{
    public class DependencyGraph<T>
    {

        private Dictionary<T, Node<T>> nodes;

        public DependencyGraph()
        {
            nodes = new Dictionary<T, Node<T>>();
        }

        public void AddNode(T val)
        {
            if (!nodes.ContainsKey(val))
            {
                nodes.Add(val, new Node<T>(val));
            }
        }

        public void AddEdge(T from, T to)
        {
            if (!nodes.ContainsKey(from))
            {
                AddNode(from);
            }

            if (!nodes.ContainsKey(to))
            {
                AddNode(to);
            }

            Node<T> from_node = nodes[from];
            Node<T> to_node = nodes[to];

            from_node.edges.Add(to_node);
        }

        public IEnumerable<T> TopologicalSort()
        {
            foreach (Node<T> node in nodes.Values)
            {
                node.marked = false;
            }

            List<T> list = new List<T>();
            foreach (Node<T> node in nodes.Values)
            {
                if (!node.marked)
                {
                    Visit(node, list);
                }
            }

            return list;
        }

        public bool HasNode(T val)
        {
            return nodes.ContainsKey(val);
        }

        private void Visit(Node<T> node, List<T> list)
        {
            node.marked = true;
            foreach (Node<T> adj in node.edges)
            {
                if (!adj.marked)
                {
                    Visit(adj, list);
                }
            }

            list.Insert(0, node.value);
        }

        private class Node<TNode>
        {
            public Node(TNode val)
            {
                this.value = val;
                this.edges = new List<Node<TNode>>();
            }

            public TNode value;
            public List<Node<TNode>> edges;
            public bool marked;
        }

    }
}