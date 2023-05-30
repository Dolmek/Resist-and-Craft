using System.Collections.Generic;
using UnityEngine;

public class Triangulator
{
    public static int[] Triangulate(List<Vector2> contour)
    {
        List<int> triangles = new List<int>();

        // Triangulate the contour using ear-clipping algorithm
        int vertexCount = contour.Count;
        if (vertexCount < 3)
        {
            return triangles.ToArray();
        }

        int[] vertexIndices = new int[vertexCount];

        // Determine the orientation of the contour (clockwise or counterclockwise)
        float area = CalculateArea(contour);
        bool clockwise = (area > 0f);

        // Initialize the vertex indices
        if (clockwise)
        {
            for (int i = 0; i < vertexCount; i++)
            {
                vertexIndices[i] = i;
            }
        }
        else
        {
            for (int i = 0; i < vertexCount; i++)
            {
                vertexIndices[i] = (vertexCount - 1) - i;
            }
        }

        int vertexIndex = vertexCount - 1;
        int iterations = vertexCount * vertexCount; // Avoid infinite loop

        while (vertexCount > 2 && iterations > 0)
        {
            int previousIndex = GetPreviousIndex(vertexIndex, vertexCount);
            int nextIndex = GetNextIndex(vertexIndex, vertexCount);

            int previousVertexIndex = vertexIndices[previousIndex];
            int currentVertexIndex = vertexIndices[vertexIndex];
            int nextVertexIndex = vertexIndices[nextIndex];

            Vector2 previousVertex = contour[previousVertexIndex];
            Vector2 currentVertex = contour[currentVertexIndex];
            Vector2 nextVertex = contour[nextVertexIndex];

            bool isEar = IsEar(previousVertex, currentVertex, nextVertex, contour, vertexIndices, vertexCount);

            if (isEar)
            {
                triangles.Add(previousVertexIndex);
                triangles.Add(currentVertexIndex);
                triangles.Add(nextVertexIndex);

                vertexCount--;
                vertexIndices = UpdateVertexIndices(vertexIndices, vertexIndex, vertexCount);

                iterations = vertexCount * vertexCount;
            }

            vertexIndex = GetNextIndex(vertexIndex, vertexCount);
            iterations--;
        }

        triangles.Reverse(); // Reverse the triangles to match Unity's winding order

        return triangles.ToArray();
    }

    private static float CalculateArea(List<Vector2> contour)
    {
        int vertexCount = contour.Count;
        float area = 0f;

        for (int i = 0; i < vertexCount; i++)
        {
            Vector2 vertex1 = contour[i];
            Vector2 vertex2 = contour[(i + 1) % vertexCount];
            area += (vertex1.x * vertex2.y) - (vertex2.x * vertex1.y);
        }

        return 0.5f * area;
    }

    private static bool IsEar(Vector2 previousVertex, Vector2 currentVertex, Vector2 nextVertex, List<Vector2> contour, int[] vertexIndices, int vertexCount)
    {
        bool isEar = true;

        for (int i = 0; i < vertexCount; i++)
        {
            int vertexIndex = vertexIndices[i];

            if (vertexIndex != previousVertex && vertexIndex != currentVertex && vertexIndex != nextVertex)
            {
                Vector2 vertex = contour[vertexIndex];

                if (IsPointInsideTriangle(previousVertex, currentVertex, nextVertex, vertex))
                {
                    isEar = false;
                    break;
                }
            }
        }

        return isEar;
    }

    private static bool IsPointInsideTriangle(Vector2 vertex1, Vector2 vertex2, Vector2 vertex3, Vector2 point)
    {
        float denominator = ((vertex2.y - vertex3.y) * (vertex1.x - vertex3.x)) + ((vertex3.x - vertex2.x) * (vertex1.y - vertex3.y));

        float a = ((vertex2.y - vertex3.y) * (point.x - vertex3.x)) + ((vertex3.x - vertex2.x) * (point.y - vertex3.y)) / denominator;
        float b = ((vertex3.y - vertex1.y) * (point.x - vertex3.x)) + ((vertex1.x - vertex3.x) * (point.y - vertex3.y)) / denominator;
        float c = 1f - a - b;

        return (a >= 0f && a <= 1f && b >= 0f && b <= 1f && c >= 0f && c <= 1f);
    }

    private static int[] UpdateVertexIndices(int[] vertexIndices, int vertexIndexToRemove, int vertexCount)
    {
        int[] updatedIndices = new int[vertexCount];

        for (int i = 0, j = 0; i < vertexCount + 1; i++, j++)
        {
            if (j == vertexIndexToRemove)
            {
                j++;
            }

            updatedIndices[i] = vertexIndices[j];
        }

        return updatedIndices;
    }

    private static int GetPreviousIndex(int currentIndex, int vertexCount)
    {
        return (currentIndex + vertexCount - 1) % vertexCount;
    }

    private static int GetNextIndex(int currentIndex, int vertexCount)
    {
        return (currentIndex + 1) % vertexCount;
    }
}
