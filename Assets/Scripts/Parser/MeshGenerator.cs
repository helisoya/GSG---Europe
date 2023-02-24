using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EarcutNet;

public class MeshGenerator
{
    private enum Facing { Up, Forward, Right };


    public static Mesh GenerateMesh(Vector3[] vecs)
    {
        Mesh mesh = new Mesh();

        Vector2[] array2D = new Vector2[vecs.Length];
        Vector3[] array = new Vector3[vecs.Length];
        Vector2[] uvs = new Vector2[vecs.Length];

        for (int i = 0; i < vecs.Length; i++)
        {
            array[i] = vecs[i];
            array2D[i] = new Vector2(vecs[i].x, vecs[i].z);
        }

        double[] values = new double[array2D.Length * 2];
        int j = 0;
        foreach (Vector2 vec in array2D)
        {
            values[j] = vec.x;
            values[j + 1] = vec.y;
            j += 2;
        }

        int[] holes = new int[0];


        mesh.vertices = array;


        List<int> res = Earcut.Tessellate(values, holes);
        res.Reverse();
        mesh.triangles = res.ToArray();


        for (int index = 0; index < res.Count; index += 3)
        {
            Vector3 v1 = vecs[res[index]];
            Vector3 v2 = vecs[res[index + 1]];
            Vector3 v3 = vecs[res[index + 2]];

            Vector3 normal = Vector3.Cross(v3 - v1, v2 - v1);

            Quaternion rotation = Quaternion.Inverse(Quaternion.LookRotation(normal));

            uvs[res[index]] = (Vector2)(rotation * v1) * 0.1f;
            uvs[res[index + 1]] = (Vector2)(rotation * v2) * 0.1f;
            uvs[res[index + 2]] = (Vector2)(rotation * v3) * 0.1f;
        }

        mesh.uv = uvs;

        return mesh;
    }
}
