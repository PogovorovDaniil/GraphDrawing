using System;
using System.Collections.Generic;
using System.Numerics;

namespace GraphDrawing.Models
{
    internal class Node
    {
        private static int nodeNumber = 0;

        internal readonly int Id;
        private List<Node> links;
        internal Vector2 Position { get; set; }

        internal Node()
        {
            Id = nodeNumber++;
            links = new List<Node>();
            Position = new Vector2(Random.Shared.NextSingle(), Random.Shared.NextSingle());
        }

        internal bool ContainsLinks(Node node) => links.Contains(node);
        internal void AddLink(Node node)
        {
            if (links.Contains(node) || node.links.Contains(node)) throw new ArgumentException();
            links.Add(node);
            node.links.Add(this);
        }
    }
}
