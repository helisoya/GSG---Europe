using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A graphPath is the path that needs to be taken to go from a point A to a point B
/// </summary>
public class GraphPath
{


	private List<Province> m_Nodes = new List<Province>();
	private int m_Length = 0;


	public List<Province> nodes
	{
		get
		{
			return m_Nodes;
		}
	}

	public int length
	{
		get
		{
			return m_Length;
		}
	}

	public GraphPath()
	{
		m_Length = 0;
		m_Nodes = new List<Province>();
	}

	/// <summary>
	/// Computes the path length
	/// </summary>
	public void Bake()
	{
		m_Length = nodes.Count;
	}


}
