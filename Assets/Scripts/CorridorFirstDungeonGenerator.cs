using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int corridorLength = 14, corridorCount = 5;
    [SerializeField]
    [Range(0.1f, 1f)]
    private float roomPercent;


    protected override void RunProceduralGeneration()
    {
        CorridorFirstDungeonGeneration();
    }

    // 복도우선 생성
    private void CorridorFirstDungeonGeneration()
    {
        HashSet<Vector2Int> floorPosition = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        CreateCorridors(floorPosition, potentialRoomPositions);

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);
        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPosition);

        CreateRoomsAtDeadEnd(deadEnds, roomPositions);

        floorPosition.UnionWith(roomPositions);

        tilemapVisualizer.PaintFloorTiles(floorPosition);
        WallGenerator.CreateWalls(floorPosition, tilemapVisualizer);
    }

    private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (var position in deadEnds)
        {
            if (roomFloors.Contains(position) == false)
            {
                var room = RunRandomWalk(randomWalkParameters, position);
                roomFloors.UnionWith(room);
            }
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPosition)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();

        foreach (var position in floorPosition)
        {
            int neighboursCount = 0;

            foreach (var direction in Direction2D.cardinalDirectionList)
            {
                if (floorPosition.Contains(position + direction))
                    ++neighboursCount;
            }

            if (neighboursCount == 1)
                deadEnds.Add(position);
        }

        return deadEnds;
    }


    // 즉, 추출한 위치중에서 Count만큼 골라내고, 골라낸 위치에서 절차 맵을 생성 및 반환.
    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();

        // 생성할 방의 갯수를 복도갯수의 특정 비율로 설정.
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

        // potentialRoomPositions의 각 위치를 랜덤하게 순서를 섞어놓은거를,
        // roomToCreateCount만큼만 List로 변환.
        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        // roomsToCreate내 위치마다 절차 맵 생성진행
        foreach (var roomPosition in roomsToCreate)
        {
            // roomPosition부터 절차 맵 생성진행
            var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition);

            // 합치기.
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPosition, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);

        // 만들려는 복도의 횟수만큼 반복.
        for (int i = 0; i < corridorCount; i++)
        {
            // 현 위치에서 특정방향으로 corridorLength만큼 걸어본다.
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            
            // 마지막으로 걸었던 위치를 currentPosition으로 갱신.
            currentPosition = corridor[corridor.Count - 1];

            // 모든 복도위치는 예비 방이 될수 있음.
            potentialRoomPositions.Add(currentPosition);

            // 걸어본 경로 corridor와 floorPostion을 병합시킴.
            floorPosition.UnionWith(corridor);
        }
    }
}
