using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// A graph used for the Djikstra Algorithm (Unit movements)
/// </summary>
public class Graph
{

	private List<Province> m_Nodes = new List<Province>();

	public List<Province> nodes
	{
		get
		{
			return m_Nodes;
		}
	}

	public void Clear()
	{
		m_Nodes.Clear();
	}

	public Graph()
	{
		m_Nodes = new List<Province>();
	}

	/// <summary>
	/// Find the shortest path between two provinces
	/// </summary>
	/// <param name="start">Start province</param>
	/// <param name="end">End province</param>
	/// <param name="canTraverse">Country that can be traversed</param>
	/// <param name="canTraverseSea">Can traverse sea ?</param>
	/// <returns>The path to take to get to the end point from the start point</returns>
	public GraphPath GetShortestPath(Province start, Province end, List<Pays> canTraverse, bool canTraverseSea)
	{

		// We don't accept null arguments
		if (start == null || end == null)
		{
			return new GraphPath();
		}

		// The final path
		GraphPath path = new GraphPath();

		// If the start and end are same node, we can return the start node
		if (start == end)
		{
			path.nodes.Add(start);
			return path;
		}

		// The list of unvisited nodes
		List<Province> unvisited = new List<Province>();

		// Previous nodes in optimal path from source
		Dictionary<Province, Province> previous = new Dictionary<Province, Province>();

		// The calculated distances, set all to Infinity at start, except the start Node
		Dictionary<Province, float> distances = new Dictionary<Province, float>();

		for (int i = 0; i < m_Nodes.Count; i++)
		{
			Province node = m_Nodes[i];

			if (node == null ||
			(!canTraverseSea && node.type == ProvinceType.SEA) ||
			(node.type != ProvinceType.SEA && !canTraverse.Contains(node.owner))
			) continue;

			unvisited.Add(node);

			// Setting the node distance to Infinity
			distances.Add(node, float.MaxValue);
		}

		// Set the starting Node distance to zero
		distances[start] = 0f;
		while (unvisited.Count != 0)
		{

			// Ordering the unvisited list by distance, smallest distance at start and largest at end
			unvisited = unvisited.OrderBy(node => distances[node]).ToList();

			// Getting the Node with smallest distance
			Province current = unvisited[0];

			// Remove the current node from unvisisted list
			unvisited.Remove(current);

			// When the current node is equal to the end node, then we can break and return the path
			if (current == end)
			{

				// Construct the shortest path
				while (previous.ContainsKey(current))
				{

					// Insert the node onto the final result
					path.nodes.Insert(0, current);

					// Traverse from start to end
					current = previous[current];
				}

				// Insert the source onto the final result
				path.nodes.Insert(0, current);
				break;
			}

			// Looping through the Node connections (neighbors) and where the connection (neighbor) is available at unvisited list
			for (int i = 0; i < current.adjacencies.Length; i++)
			{
				Province neighbor = current.adjacencies[i];

				if ((neighbor.type == ProvinceType.SEA && canTraverseSea) || canTraverse.Contains(neighbor.owner))
				{
					// Getting the distance between the current node and the connection (neighbor)

					// The distance from start node to this connection (neighbor) of current node
					float alt = distances[current] + 1;

					// A shorter path to the connection (neighbor) has been found
					if (alt < distances[neighbor])
					{
						distances[neighbor] = alt;
						previous[neighbor] = current;
					}
				}
			}
		}

		if (path.nodes.Count == 1 && path.nodes[0] == end)
		{
			path.nodes.Clear();
		}

		path.Bake();
		return path;
	}

}
