using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPosition, TilemapVisualizer tilemapVisualizer)
    {
        // 상하좌우 벽을 판단.
        var basicWallPosition = FindWallsInDirections(floorPosition, Direction2D.cardinalDirectionList);
        
        // 대각선의 벽을 판단.
        var cornerWallPosition = FindWallsInDirections(floorPosition, Direction2D.diagonalDirectionList);

        // 일반 벽과 코너 벽을 배치하는 함수 수행.
        CreateBasicWall(tilemapVisualizer, basicWallPosition, floorPosition);
        CreateCornerWalls(tilemapVisualizer, cornerWallPosition, floorPosition);
    }

    // 바닥좌표를 기준으로 코너 벽과 상하좌우벽을 판단.
    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPosition, List<Vector2Int> directionsList)
    {
        // 벽타일 좌표를 담는 Hashset.
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

       
        foreach (var Position in floorPosition)
        {
            // Direction2D.cardinalDirectionList와 Direction2D.diagonalDirectionList를 이용하여 
            // 바닥타일에서 상하좌우, 대각선에 존재하는 벽 좌표를 판단.
            foreach (var direction in directionsList)
            {
                // 현재 바닥타일에서 주어진 방향만큼 더한 위치를 계산.
                var neighbourPosition = Position + direction;

                // 바닥 타일좌표가 아닌경우, 벽 타일로 판단.
                if (floorPosition.Contains(neighbourPosition) == false)
                {
                    // 벽타일 좌표를 추가.
                    wallPositions.Add(neighbourPosition);
                }
            }
        }

        return wallPositions;
    }

    private static void CreateCornerWalls(TilemapVisualizer tilemapVisualizer, 
        HashSet<Vector2Int> cornerWallPosition, HashSet<Vector2Int> floorPosition)
    {
        // 모든 벽타일에 대해서 수행.
        foreach (var position in cornerWallPosition)
        {
            // 벽정보를 이진수로 담기위한 문자열
            string neighboursBinaryType = "";

            foreach (var direction in Direction2D.eightDirectionList)
            {
                // 현재 벽타일에서 Direction2D.eightDirectionList의 요소만큼 더한 위치를 계산.
                var neighbourPosition = position + direction;

                // 해당 위치가 바닥타일이라면,
                if (floorPosition.Contains(neighbourPosition))
                {
                    // 이진수 1을 추가
                    neighboursBinaryType += "1";
                }
                // 벽타일이라면,
                else
                {
                    // 이진수 0을 추가
                    neighboursBinaryType += "0";
                }
            }
            // 현재 벽타일의 위치와 이진수 문자열을 전달.
            // 사전에 설정한 이진수 값을 비교하여 방향에 맞는 벽타일을 배치.
            tilemapVisualizer.PaintSingleCornerWall(position, neighboursBinaryType);
        }
    }

    // 상하좌우 벽을 판단하는 함수
    private static void CreateBasicWall(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicWallPosition, 
        HashSet<Vector2Int> floorPosition)
    {
        // 모든 벽타일에 대해서 수행.
        foreach (var position in basicWallPosition)
        {
            // 벽정보를 이진수로 담기위한 문자열
            string neighboursBinaryType = "";

            foreach (var direction in Direction2D.cardinalDirectionList)
            {
                // 현재 벽타일에서 Direction2D.cardinalDirectionList의 요소만큼 더한 위치를 계산.
                var neighbourPosition = position + direction;

                // 해당 위치가 바닥타일이라면,
                if (floorPosition.Contains(neighbourPosition))
                {
                    // 이진수 1을 추가
                    neighboursBinaryType += "1";
                }
                // 벽타일이라면,
                else
                {
                    // 이진수 0을 추가
                    neighboursBinaryType += "0";
                }
            }

            // 현재 벽타일의 위치와 이진수 문자열을 전달.
            // 사전에 설정한 이진수 값을 비교하여 방향에 맞는 벽타일을 배치.
            tilemapVisualizer.PaintSingleBasicWall(position, neighboursBinaryType);
        }
    }

}
