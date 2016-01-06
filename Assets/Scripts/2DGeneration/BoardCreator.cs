using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BoardCreator : MonoBehaviour {

	public enum TileType{
		RoomWall, Floor, None, TopRightCorner, TopLeftCorner, BottomLeftCorner, BottomRightCorner, TopWall, BottomWall, RightWall, LeftWall,
	}

	public int columns = 150;
	public int rows = 150;
	[HideInInspector] public IntRange numRooms;
	public int ItemsPerRoom = 3;
	public IntRange roomWidth = new IntRange (3,10);
	public IntRange roomHeight = new IntRange (3,10);
	public IntRange corridorLength = new IntRange (6,10);
	public GameObject blankTile;
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] outerWallTiles;
	public GameObject[] enemies;
	public GameObject food;
	public GameObject drink;
	public GameObject player;
	public GameObject exit;
	
	//Dev Purposes Only
	public bool SpawnEnemies;

	private TileType[][] tiles;
	private Room[] rooms;
	private Corridor[] corridors;
	private GameObject boardHolder;
	
	// Use this for initialization
	void Awake () {

		boardHolder = new GameObject("BoardHolder");

		SetUpNumRooms(GameManager.instance.level);
		SetUpTilesArray ();

		CreateRoomsAndCorridors();

		SetTilesValuesForRooms();
		SetTileValuesForCorridors();

		InstantiateTiles();
	}

	void SetUpNumRooms(int level){
		numRooms = new IntRange(level * 2 + 1, (level * 2) + 3);
		if(numRooms.iMAX > 10 || numRooms.iMIN > 13){
			numRooms = new IntRange(10,13);
		}
	}

	void SetUpTilesArray (){
		tiles = new TileType[columns][];
		for(int i = 0; i < tiles.Length; i++){
			tiles[i] = new TileType[rows];
			for(int j = 0; j < tiles[i].Length; j++){
				tiles[i][j] = TileType.None;
			}
		}
	}

	void CreateRoomsAndCorridors(){

		rooms = new Room[numRooms.Random];
		corridors = new Corridor[rooms.Length - 1];

		rooms[0] = new Room();
		corridors[0] = new Corridor();
		rooms[0].SetupRoom(roomWidth, roomHeight, columns, rows);

		//Place Player in First Room
		PlaceItemsInRoom(player, rooms[0], true);

	
		corridors[0].SetupCorridor(rooms[0], corridorLength, roomWidth, roomHeight, columns, rows, true);
		for(int i = 1; i < rooms.Length; i++){

			rooms[i] = new Room();
			rooms[i].SetupRoom(roomWidth, roomHeight, columns, rows, corridors[i-1]);
			//TODO: Fix Room Overlapping
//			while(!CheckAreaClear(rooms[i])){
//				rooms[i].enteringCorridor.direction +=1;
//				rooms[i].SetupRoom(roomWidth, roomHeight, columns, rows, corridors[i-1]);
//			}

			PlaceItemsInRoom(food, rooms[i], false);
			PlaceItemsInRoom(drink, rooms[i], false);
			if(SpawnEnemies)
				PlaceItemsInRoom(enemies[Random.Range(0,enemies.Length)], rooms[i], false);

			if(i < corridors.Length){
				corridors[i] = new Corridor();
				corridors[i].SetupCorridor(rooms[i], corridorLength, roomWidth, roomHeight, columns, rows, false);
			}

		}
		//Place Exit in Last Room
		PlaceItemsInRoom(exit, rooms[rooms.Length - 1], true);
	}

	//TODO: Check area is clear before placing a room
	bool CheckAreaClear(Room room){
		for(int x = 0; x <= room.roomWidth; x++){
			for(int y = 0; y <= room.roomHeight; y++){
				if(tiles[room.xPos + x][room.yPos + y] != TileType.None){
					return false;
				}
			}
		}
		return true;
	}

	void SetTilesValuesForRooms(){
		for(int i = 0; i < rooms.Length; i++){

			Room currentRoom = rooms[i];

			for(int j = 0; j <= currentRoom.roomWidth; j++){
				int XCoord = currentRoom.xPos + j;

				for(int k = 0; k <= currentRoom.roomHeight; k++){
					int YCoord = currentRoom.yPos + k;
					tiles[XCoord][YCoord] = TileType.Floor;

					if(YCoord == currentRoom.yPos){
						tiles[XCoord][YCoord] = TileType.BottomWall;
						if(XCoord == currentRoom.xPos + currentRoom.roomWidth)
							tiles[XCoord][YCoord] = TileType.BottomRightCorner;
						if(XCoord == currentRoom.xPos)
							tiles[XCoord][YCoord] = TileType.BottomLeftCorner;
					} 
					if(YCoord == currentRoom.yPos + currentRoom.roomHeight){
						tiles[XCoord][YCoord] = TileType.TopWall;
						if(XCoord == currentRoom.xPos)
							tiles[XCoord][YCoord] = TileType.TopLeftCorner;
						
						if(XCoord == currentRoom.xPos + currentRoom.roomWidth)
							tiles[XCoord][YCoord] = TileType.TopRightCorner;
					}
					if(XCoord == currentRoom.xPos)
						tiles[XCoord][YCoord] = TileType.LeftWall;
					if(XCoord == currentRoom.xPos + currentRoom.roomWidth)
						tiles[XCoord][YCoord] = TileType.RightWall;
				}
			}
		}
	}
	
	void SetTileValuesForCorridors(){
		for(int i = 0; i < corridors.Length; i++){

			Corridor currentCorridor = corridors[i];

			for(int j = 0; j < currentCorridor.corridorLength; j++){

				int XCoord = currentCorridor.startXPos;
				int YCoord = currentCorridor.startYPos;

				switch (currentCorridor.direction){
				case Direction.North:
					YCoord += j;
					for(int x = -1; x <= 1; x++){
						if(YCoord != currentCorridor.startYPos || YCoord != currentCorridor.startYPos + currentCorridor.corridorLength){
							if(x == -1)
								tiles[XCoord + x][YCoord] = TileType.LeftWall;
							if(x == 1)
								tiles[XCoord + x][YCoord] = TileType.RightWall;
						}
					}

					break;
				case Direction.East:
					XCoord += j;
					for(int y = -1; y <= 1; y++){
						if(XCoord != currentCorridor.startXPos || XCoord != currentCorridor.startXPos + currentCorridor.corridorLength -1){
							if(y == -1)
								tiles[XCoord][YCoord + y] = TileType.TopWall;
							if(y == 1)
								tiles[XCoord][YCoord + y] = TileType.BottomWall;
						}
					}
					break;
				case Direction.South:
					YCoord -= j;
					for(int x = -1; x <= 1; x++){
						if(YCoord != currentCorridor.startYPos || YCoord != currentCorridor.startYPos + currentCorridor.corridorLength -1){
							if(x == -1)
								tiles[YCoord + x][YCoord] = TileType.LeftWall;
							if(x == 1)
								tiles[XCoord + x][YCoord] = TileType.RightWall;
						}
					}
					break;
				case Direction.West:
					XCoord -= j;
					for(int y = -1; y <= 1; y++){
						if(XCoord != currentCorridor.startXPos || XCoord != currentCorridor.startXPos + currentCorridor.corridorLength -1){
							if(y == -1)
								tiles[XCoord][YCoord + y] = TileType.TopWall;
							if(y == 1)
								tiles[XCoord][YCoord + y] = TileType.BottomWall;
						}
					}
					break;
				}
				tiles[XCoord][YCoord] = TileType.Floor;
			} 
		}
	}

	void InstantiateTiles(){
		for (int i = 0; i < tiles.Length; i++) {
			for (int j = 0; j < tiles[i].Length; j++) {
				switch(tiles[i][j]){
					case TileType.Floor:
						InstantiateFromArray(floorTiles,i,j);
					break;

					case TileType.RoomWall:
						InstantiateFromArray(outerWallTiles,i,j);
					break;

					case TileType.BottomRightCorner:
						GameObject brc = Instantiate(outerWallTiles[5], new Vector3(i,j), Quaternion.identity) as GameObject;
						brc.transform.parent = boardHolder.transform;
					break;

					case TileType.BottomWall:
						GameObject bc = Instantiate(outerWallTiles[1], new Vector3(i,j), Quaternion.identity) as GameObject;
						bc.transform.parent = boardHolder.transform;
					break;

					case TileType.LeftWall:
						GameObject rc = Instantiate(outerWallTiles[4], new Vector3(i,j), Quaternion.identity) as GameObject;
						rc.transform.parent = boardHolder.transform;
					break;

					case TileType.RightWall:
						GameObject lc = Instantiate(outerWallTiles[7], new Vector3(i,j), Quaternion.identity) as GameObject;
						lc.transform.parent = boardHolder.transform;
					break;

					case TileType.BottomLeftCorner:
						GameObject blc = Instantiate(outerWallTiles[6], new Vector3(i,j), Quaternion.identity) as GameObject;
						blc.transform.parent = boardHolder.transform;
					break;

					case TileType.TopRightCorner:
						GameObject trc = Instantiate(outerWallTiles[2], new Vector3(i,j), Quaternion.identity) as GameObject;
						trc.transform.parent = boardHolder.transform;
					break;

					case TileType.TopLeftCorner:
						GameObject tlc = Instantiate(outerWallTiles[0], new Vector3(i,j), Quaternion.identity) as GameObject;
						tlc.transform.parent = boardHolder.transform;
					break;

					case TileType.TopWall:
						GameObject top = Instantiate(outerWallTiles[1], new Vector3(i,j), Quaternion.identity) as GameObject;
						top.transform.parent = boardHolder.transform;
					break;

					case TileType.None:
						GameObject none = Instantiate(blankTile, new Vector3(i,j), Quaternion.identity) as GameObject;
						none.transform.parent = boardHolder.transform;
					break;
				}
			}
		}
	}

	public void PlaceItemsInRoom(GameObject item, Room currentRoom, bool forcePlaceSingle){
		int NumPlaced = Random.Range(0 , 3);

		int xCoord = Random.Range(1, currentRoom.roomWidth); 
		int yCoord = Random.Range(1, currentRoom.roomHeight); 

		if(forcePlaceSingle){
			AddObjectToBoard(item, new Vector3(currentRoom.xPos + xCoord, currentRoom.yPos + yCoord));
		} else {
			for(int x  = 0; x <= NumPlaced; x++){
				if(NumPlaced == 0)
					return;
//				while(tiles[currentRoom.xPos + xCoord][currentRoom.yPos + yCoord] != TileType.Floor){
//					xCoord = Random.Range(1, currentRoom.roomWidth);
//					yCoord = Random.Range(1, currentRoom.roomHeight);
//				}
			
				AddObjectToBoard(item, new Vector3(currentRoom.xPos + xCoord, currentRoom.yPos + yCoord));
			}
		}
	}

	void AddObjectToBoard(GameObject obj, Vector3 loc){
		GameObject instance = Instantiate(obj, loc, Quaternion.identity) as GameObject;
		instance.transform.parent = boardHolder.transform;
	}

	void InstantiateFromArray(GameObject[] prefabs, float xCoord, float yCoord){
		int randomIndex = Random.Range(0, prefabs.Length);

		Vector3 position = new Vector3(xCoord, yCoord, 0f);
		GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;

		tileInstance.transform.parent = boardHolder.transform;
	}
}
