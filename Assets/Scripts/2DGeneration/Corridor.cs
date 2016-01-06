using UnityEngine;

public enum Direction{
	North, East, South, West,
}

public class Corridor {

	public int startXPos;
	public int startYPos;
	public int corridorLength;
	public Direction direction;

	public int EndPositionX{
		get{
			if(direction == Direction.North || direction == Direction.South)
				return startXPos;
			if(direction == Direction.East)
				return startXPos + corridorLength - 1;
			return startXPos - corridorLength + 1;
		}
	}

	public int EndPositionY{
		get{
			if(direction == Direction.East || direction == Direction.West)
				return startYPos;
			if(direction == Direction.North)
				return startYPos + corridorLength - 1;
			return startYPos - corridorLength + 1;
		}
	}

	public void SetupCorridor(Room room, IntRange length, IntRange roomWidth, IntRange roomHeight, int columns, int rows, bool firstCorridor){

		direction = (Direction)Random.Range(0,4);

		Direction oppositeDirection = (Direction)(((int)room.enteringCorridor + 2) % 4);

		if(!firstCorridor && direction == oppositeDirection){
			int directionInt = (int)direction;
			directionInt++;
			directionInt = directionInt % 4;
			direction = (Direction)directionInt;
		}

		corridorLength = length.Random;

		int maxLength = length.iMAX;

		switch(direction){
			case Direction.North:
				startXPos = Random.Range (room.xPos + 1, room.xPos + room.roomWidth - 1);
				startYPos = room.yPos + room.roomHeight;
				maxLength = rows - startYPos - roomHeight.iMIN;
				break;
			case Direction.East:
				startXPos = room.xPos + room.roomWidth;
				startYPos = Random.Range (room.yPos + 1, room.yPos + room.roomHeight - 1);
				maxLength = columns - startXPos - roomHeight.iMIN;
				break;
			case Direction.South:
				startXPos = Random.Range (room.xPos + 1, room.xPos + room.roomWidth - 1);
				startYPos = room.yPos;
				maxLength = startYPos - roomHeight.iMIN;
				break;
			case Direction.West:
				startXPos = room.xPos;
				startYPos = Random.Range (room.yPos + 1, room.yPos + room.roomHeight - 1);
				maxLength = startXPos - roomHeight.iMIN;
				break;
		}
		corridorLength = Mathf.Clamp(corridorLength,1,maxLength);
	}

}
