using GraphDrawing.Models;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace GraphDrawing.Controllers
{
    internal class GraphController : IDisposable
    {
        private List<Node> nodes;
        private bool disposed;
        private object lockObject;
        private Task loopTask;
        public GraphController()
        {
            disposed = false;
            lockObject = new object();
            nodes = new List<Node>();

            #region Fill nodes
            for (int i = 0; i < 4; i++)
            {
                nodes.Add(new Node());
                for (int j = 0; j < i; j++) nodes[i].AddLink(nodes[j]);
            }
            for (int i = 0; i < 4; i++)
            {
                nodes.Add(new Node());
                nodes[i].AddLink(nodes[i + 4]);
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    nodes.Add(new Node());
                    nodes[i + 4].AddLink(nodes[8 + i * 4 + j]);
                }
            }
            #endregion

            loopTask = Task.Run(() =>
            {
                while (!disposed) Update();
            });
        }

        private void Update()
        {
            lock (lockObject)
            {
                const float delta = 0.01f;
                const float forceRepulsion = 0.1f;
                const float forceCommunication = 0.1f;

                foreach (var node1 in nodes)
                {
                    foreach (var node2 in nodes)
                    {
                        if (node1 == node2) continue;

                        float distance = (node1.Position - node2.Position).Length();
                        Vector2 communicationVector = Vector2.Normalize(node2.Position - node1.Position);
                        node1.Position -= forceRepulsion * communicationVector * delta / (distance * distance);
                        if (node1.ContainsLinks(node2))
                        {
                            node1.Position += forceCommunication * communicationVector * delta * distance;
                        }
                    }
                }

                float minX = float.MaxValue;
                float minY = float.MaxValue;
                float maxX = float.MinValue;
                float maxY = float.MinValue;
                foreach (var node in nodes)
                {
                    if (node.Position.X < minX) minX = node.Position.X;
                    if (node.Position.Y < minY) minY = node.Position.Y;
                    if (node.Position.X > maxX) maxX = node.Position.X;
                    if (node.Position.Y > maxY) maxY = node.Position.Y;
                }

                float dx = (minX + maxX) / 2;
                float dy = (minY + maxY) / 2;
                foreach (var node in nodes)
                {
                    node.Position -= new Vector2(dx, dy);
                }
            }
        }

        public List<Vector2> GetNodes()
        {
            lock (lockObject)
            {
                float zoomX = float.MaxValue;
                float zoomY = float.MaxValue;
                foreach (var node in nodes)
                {
                    if (node.Position.X < zoomX) zoomX = node.Position.X;
                    if (node.Position.Y < zoomY) zoomY = node.Position.Y;
                }
                List<Vector2> result = new List<Vector2>();
                foreach (var node in nodes)
                {
                    result.Add(new Vector2(node.Position.X / zoomX, node.Position.Y / zoomY));
                }
                return result;
            }
        }

        public List<(Vector2, Vector2)> GetLinks()
        {
            List<Vector2> nodePositions = GetNodes();
            List<(Vector2, Vector2)> result = new List<(Vector2, Vector2)>();
            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (nodes[i].ContainsLinks(nodes[j]))
                    {
                        result.Add((nodePositions[i], nodePositions[j]));
                    }
                }
            }
            return result;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            disposed = true;
            loopTask.Wait();
        }

        ~GraphController() => Dispose();
    }
}
