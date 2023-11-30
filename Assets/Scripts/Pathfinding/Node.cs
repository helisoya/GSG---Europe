using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Node
{
	private Province m_province;

	public Vector3 position
	{
		get
		{
			return m_province.center;
		}
	}

	public Province province
	{
		get
		{
			return m_province;
		}
	}

	public Node(Province prov)
	{
		m_province = prov;
	}

	private List<Node> m_Connections = new List<Node>();


	public List<Node> connections
	{
		get
		{
			return m_Connections;
		}
	}

	public Node this[int index]
	{
		get
		{
			return m_Connections[index];
		}
	}

	void OnValidate()
	{

		// Removing duplicate elements
		m_Connections = m_Connections.Distinct().ToList();
	}

}
