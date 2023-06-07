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

    // 해당 BSP 알고리즘에서는 트리가 아닌 큐를 이용하여 구역을 나눈다.
    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();   // 트리 대신 큐를 사용하여 나눈 구역을 관리한다.
        List<BoundsInt> roomsList =  new List<BoundsInt>();     // 최종적으로 분할한 모든 방을 관리하는 List

        // 최초 구역을 큐에 전달.
        roomsQueue.Enqueue(spaceToSplit);

        // 큐가 비어질 때까지 진행
        while (roomsQueue.Count > 0) 
        {
            var room = roomsQueue.Dequeue();

            // 모든 방의 사이즈가 (minWidth, minHeight)의 2배 미만까지 도달할 때까지 큐를 진행
            if (room.size.y >= minHeight && room.size.x >= minWidth) 
            {
                // 수평분할을 먼저할지, 수직분할을 먼저 할지 50% 확률로 결정.
                if (Random.value < 0.5f)
                {
                    // 현재 Room.y 사이즈가 minWidth * 2 이상일 경우
                    if (room.size.y >= minHeight * 2)
                    {
                        // 수평분할 시작
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }

                    // 현재 Room.x 사이즈가 minHeight * 2 이상일 경우
                    else if (room.size.x >= minWidth * 2)
                    {
                        // 수직분할 시작
                        SplitVertically(minWidth, roomsQueue, room);
                    }

                    // 방이 minWidth, minHeight에 도달했다면 반환 List에 추가
                    else if (room.size.x >= minWidth && room.size.y >= minHeight)
                    {
                        roomsList.Add(room);
                    }
                }
                else
                {

                    // 현재 Room.x 사이즈가 minHeight * 2 이상일 경우
                    if (room.size.x >= minWidth * 2)
                    {
                        // 수직분할 시작
                        SplitVertically(minWidth, roomsQueue, room);
                    }

                    // 현재 Room.y 사이즈가 minWidth * 2 이상일 경우
                    else if (room.size.y >= minHeight * 2)
                    {
                        // 수평분할 시작
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }

                    // 방이 minWidth, minHeight에 도달했다면 반환 List에 추가
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
        // 나눌 x 높이를 랜덤결정.
        var xSplit = Random.Range(1, room.size.x);

        // 사각형의 방을 두개로 분할했을때,
        // room1 : 좌측 방
        // room2 : 우측 방
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z), 
            new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));

        // 분할한 방을 모두 큐에 삽입.
        roomQueue.Enqueue(room1);
        roomQueue.Enqueue(room2);
    }

    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomQueue, BoundsInt room)
    {
        // 나눌 y 높이를 랜덤결정.
        var ySplit = Random.Range(1, room.size.y);

        // 사각형의 방을 두개로 분할했을때,
        // room1 : 하단 방
        // room2 : 상단 방
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
            new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));

        // 분할한 방을 모두 큐에 삽입.
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

    // 상,하,좌,우 방향을 랜덤으로 반환.
    public static Vector2Int GetRandomCardinalDirection2D()
    {
        return cardinalDirectionList[Random.Range(0, cardinalDirectionList.Count)];
    }
}