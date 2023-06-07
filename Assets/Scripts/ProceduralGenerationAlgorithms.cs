using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;

public static class ProceduralGenerationAlgorithms 
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPosition);

        var previousPosition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection2D();
            path.Add(newPosition);
            previousPosition = newPosition;
        }

        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        var direction = Direction2D.GetRandomCardinalDirection2D();
        var currentPosition = startPosition;
        corridor.Add(currentPosition);

        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }

        return corridor;
    }

    // �ش� BSP �˰����򿡼��� Ʈ���� �ƴ� ť�� �̿��Ͽ� ������ ������.
    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();   // Ʈ�� ��� ť�� ����Ͽ� ���� ������ �����Ѵ�.
        List<BoundsInt> roomsList =  new List<BoundsInt>();     // ���������� ������ ��� ���� �����ϴ� List

        // ���� ������ ť�� ����.
        roomsQueue.Enqueue(spaceToSplit);

        // ť�� ����� ������ ����
        while (roomsQueue.Count > 0) 
        {
            var room = roomsQueue.Dequeue();

            // ��� ���� ����� (minWidth, minHeight)�� 2�� �̸����� ������ ������ ť�� ����
            if (room.size.y >= minHeight && room.size.x >= minWidth) 
            {
                // ��������� ��������, ���������� ���� ���� 50% Ȯ���� ����.
                if (Random.value < 0.5f)
                {
                    // ���� Room.y ����� minWidth * 2 �̻��� ���
                    if (room.size.y >= minHeight * 2)
                    {
                        // ������� ����
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }

                    // ���� Room.x ����� minHeight * 2 �̻��� ���
                    else if (room.size.x >= minWidth * 2)
                    {
                        // �������� ����
                        SplitVertically(minWidth, roomsQueue, room);
                    }

                    // ���� minWidth, minHeight�� �����ߴٸ� ��ȯ List�� �߰�
                    else if (room.size.x >= minWidth && room.size.y >= minHeight)
                    {
                        roomsList.Add(room);
                    }
                }
                else
                {

                    // ���� Room.x ����� minHeight * 2 �̻��� ���
                    if (room.size.x >= minWidth * 2)
                    {
                        // �������� ����
                        SplitVertically(minWidth, roomsQueue, room);
                    }

                    // ���� Room.y ����� minWidth * 2 �̻��� ���
                    else if (room.size.y >= minHeight * 2)
                    {
                        // ������� ����
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }

                    // ���� minWidth, minHeight�� �����ߴٸ� ��ȯ List�� �߰�
                    else if (room.size.x >= minWidth && room.size.y >= minHeight)
                    {
                        roomsList.Add(room);
                    }
                }
            }

        }

        return roomsList;
    }

    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomQueue, BoundsInt room)
    {
        // ���� x ���̸� ��������.
        var xSplit = Random.Range(1, room.size.x);

        // �簢���� ���� �ΰ��� ����������,
        // room1 : ���� ��
        // room2 : ���� ��
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z), 
            new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));

        // ������ ���� ��� ť�� ����.
        roomQueue.Enqueue(room1);
        roomQueue.Enqueue(room2);
    }

    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomQueue, BoundsInt room)
    {
        // ���� y ���̸� ��������.
        var ySplit = Random.Range(1, room.size.y);

        // �簢���� ���� �ΰ��� ����������,
        // room1 : �ϴ� ��
        // room2 : ��� ��
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
            new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));

        // ������ ���� ��� ť�� ����.
        roomQueue.Enqueue(room1);
        roomQueue.Enqueue(room2);
    }
}

public static class Direction2D
{
    public static List<Vector2Int> cardinalDirectionList = new List<Vector2Int>()
    {
        new Vector2Int(0, 1),  // Up
        new Vector2Int(1, 0),  // Right
        new Vector2Int(0, -1), // Down  
        new Vector2Int(-1, 0)  // Left
    };
    
    public static List<Vector2Int> diagonalDirectionList = new List<Vector2Int>()
    {
        new Vector2Int(1, 1),  // Up-Right
        new Vector2Int(1, -1),  // Down-Right
        new Vector2Int(-1, -1), // Down-Left
        new Vector2Int(-1, 1)  // Up-Left
    };

    public static List<Vector2Int> eightDirectionList = new List<Vector2Int>()
    {

        new Vector2Int(0, 1),  // Up
        new Vector2Int(1, 1),  // Up-Right
        new Vector2Int(1, 0),  // Right
        new Vector2Int(1, -1),  // Down-Right
        new Vector2Int(0, -1), // Down  
        new Vector2Int(-1, -1), // Down-Left
        new Vector2Int(-1, 0),  // Left
        new Vector2Int(-1, 1)  // Up-Left
    };

    // ��,��,��,�� ������ �������� ��ȯ.
    public static Vector2Int GetRandomCardinalDirection2D()
    {
        return cardinalDirectionList[Random.Range(0, cardinalDirectionList.Count)];
    }
}