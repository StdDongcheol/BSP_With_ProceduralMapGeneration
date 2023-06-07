using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;   // 방의 최소 가로 및 세로 길이
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;  // 전체 던전 가로 및 세로 길이
    [SerializeField]
    [Range(0, 10)]
    private int offset = 1;                             // 각 방의 간격을 조절하기 위한 offset.
    [SerializeField]
    private bool randomWalkRooms = false;

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        // BSP를 시작할 위치, 생성할 전체 던전 크기, 최소 방 가로 및 세로길이를 변수로 넘긴다.
        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, 
            new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        if(randomWalkRooms)
            floor = CreateRoomsRandomly(roomsList);
        
        else
            floor = CreateSimpleRooms(roomsList);


        // 모든 방의 좌표를 정수로 변환.
        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            // 현재 좌표특성상 실수라서, 정수로 변환해야 하기 때문에
            // float to Int를 반올림으로변환.
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        // 각 방을 연결하는 복도의 좌표를 반환
        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);

        // 복도타일도 합치기
        floor.UnionWith(corridors);

        // 타일 맵에 각 타일좌표를 전달하여 바닥 타일을 생성.
        tilemapVisualizer.PaintFloorTiles(floor);

        // 벽타일 생성
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);

            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) &&
                    position.x <= (roomBounds.xMax - offset) && 
                    position.y >= (roomBounds.yMin + offset) && 
                    position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                }
            }
        }

        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();

        // 리스트 중에서 랜덤 방을 선택.
        var currentRoomCenter = roomCenters[UnityEngine.Random.Range(0, roomCenters.Count)];
        
        // 방을 리스트에서 제거.(중복탐색방지)
        roomCenters.Remove(currentRoomCenter);

        while(roomCenters.Count > 0)
        {
            // 현 위치에서 가장 가까운 방을 탐색.
            // (그냥 랜덤한 방 하나로 시작해서 제일 가까운 방으로 연결을 반복하는 논리)
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);

            //Debug.DrawLine(new Vector3Int(currentRoomCenter.x, currentRoomCenter.y, 0), new Vector3Int(closest.x, closest.y, 0), Color.red, 1f);
            
            // 가장가까운 방을 리스트에서 제거.(중복탐색방지)
            roomCenters.Remove(closest);

            // 현위치부터 가장 가까운 방까지의 이동좌표탐색
            HashSet<Vector2Int> newCorridor = CreateCoriddor(currentRoomCenter, closest);

            // 현위치를 가장 가까운방으로 갱신.
            currentRoomCenter = closest;

            // 이동좌표를 병합.
            corridors.UnionWith(newCorridor);
        }

        // 기록한 모든 이동좌표를 반환.
        return corridors;
    }

    private HashSet<Vector2Int> CreateCoriddor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;

        corridor.Add(position);

        // 현좌표부터 도착좌표까지의 Y좌표를 기록
        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if(destination.y < position.y) 
            {
                position += Vector2Int.down;
            }

            corridor.Add(position);
        }

        // 현좌표부터 도착좌표까지의 X좌표를 기록
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }

            corridor.Add(position);
        }

        // 이동한 좌표모음을 반환.
        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;

        // 초기값은 float 최대값.
        float dist = float.MaxValue;

        // 각 방마다 직선거리를 비교해서 가장 짧은 방을 선별.
        foreach (var position in roomCenters)
        {
            float currentDist = Vector2.Distance(position, currentRoomCenter);

            if(currentDist < dist)
            {
                dist = currentDist;
                closest = position;
            }
        }

        // 가장 가까운 방의 center좌표 반환.
        return closest;
    }

    // 타일 좌표를 기록하여 방을 생성.
    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        // 모든 방에 대해서 foreach
        foreach (var room in roomsList)
        {
            // 각 방에 대한 offset을 일종의 Padding 값으로 인식.
            // 방의 최소, 최대 좌표 위치에서 offset만큼 방의 사이즈(간격)을 좁히는 것.
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    // 해당 방의 맨좌측하단부터 (좌표 + offset)값을 기록({0,1}, {0,2}, {0,3},...)
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);

                    // 타일 좌표 기록.
                    floor.Add(position);
                }
            }
        }

        return floor;
    }
}
