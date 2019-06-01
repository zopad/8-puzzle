using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

	public class Piece {
		private int position ; //0-8, 0: top left, 8: bottom right
		public GameObject go;

		public Piece(GameObject go){
			this.go = go;
			this.position = pieceCounter++;
		}

		public int getPos(){
			return position;
		}

		public void setPos(int newPos){
			if(newPos > 8 || newPos < 0){
				Debug.LogError("Te tényleg ennyire hülye vagy? " + newPos);
			} else {
				this.position = newPos;
			}
		}

	}
	public float AnimSpeed = 1f;
	public GameObject[] tiles;
	private Piece[] pieces;
	private static int pieceCounter = 0;
	private int emptyPos;		
	//there is exactly 1 empty pos in this game, we can
	//use it to check if a piece is movable ;) WeSmart

	private enum DIRECTION{NONE = 0, RIGHT = 1, DOWN = 3, LEFT = -1, UP = -3};

	// Use this for initialization
	void Start () {
		initPieces();
		int steps = Random.Range(8, 19);
		randomizePieces(steps);
	}
	
	// Update is called once per frame
	void Update () {
		checkClick();
	}

	private void initPieces() {
		pieces = new Piece[8];

		foreach(GameObject obj in tiles) {
			Piece piece = new Piece(obj);
			pieces[piece.getPos()] = piece;
		}
		emptyPos = 8;
	}

	private void randomizePieces(int backsteps){
		for(int i = 0; i < backsteps; i++) {
				foreach(Piece piece in pieces) {
					DIRECTION dir = getMoveDir(piece);
					if(dir != DIRECTION.NONE){    
						movePiece(piece, 1, dir);
					}
				}
		}
	}

	private void movePiece(Piece toMove, float time, DIRECTION direction)
    {
        Vector3 screenPositionToAnimate = getResultPosition(toMove, direction);
        toMove.go.transform.position = Vector3.MoveTowards(toMove.go.transform.position, 
          screenPositionToAnimate , AnimSpeed/time);

		Debug.Log("Switching " + emptyPos + " and " + toMove.getPos());
		int tempPos = toMove.getPos();
		toMove.setPos(emptyPos);
		emptyPos = tempPos;
	}

    private void checkClick()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

            //check if a piece was hit
            if(Physics.Raycast (ray, out hit))
            {
                string name = hit.collider.gameObject.name;
				string pieceNum = name.Split('_')[1];
				int num = int.Parse(pieceNum) - 1;
				Piece toMove = pieces[num];
				DIRECTION dir = getMoveDir(toMove);
				if(dir != DIRECTION.NONE) {
					movePiece(toMove, 1, dir);
				}
            }
        }
    }

	private Vector3 getResultPosition(Piece toMove, DIRECTION direction) {
		Vector3 oldPos = toMove.go.transform.position;
		switch(direction){
			case DIRECTION.RIGHT:
				oldPos.x += 10;
				break;
			case DIRECTION.LEFT:
				oldPos.x -= 10;
				break;
			case DIRECTION.UP:
				oldPos.z += 10;
				break;
			case DIRECTION.DOWN:
				oldPos.z -= 10;
				break;
			default:
				break;
		}
		return oldPos;
	}

	private DIRECTION getMoveDir(Piece piece){
		int pos = piece.getPos();
		if(pos == 2 && emptyPos == 3 || (pos == 3 && emptyPos == 2)) {	//next line is not neighbour
			return DIRECTION.NONE;
		}
		if(pos == 5 && emptyPos == 6 || (pos == 6 && emptyPos == 5)) {
			return DIRECTION.NONE;
		}
		if(isLegal(pos+1) && pos + 1 == emptyPos){
			return DIRECTION.RIGHT;
		}
		if(isLegal(pos-1) && pos - 1 == emptyPos){
			return DIRECTION.LEFT;
		}
		if(isLegal(pos+3) && pos + 3 == emptyPos){
			return DIRECTION.DOWN;
		}
		if(isLegal(pos-3) && pos - 3 == emptyPos){
			return DIRECTION.UP;
		}
		return DIRECTION.NONE;
	}

	private bool isLegal(int pos){
		return (pos >= 0 && pos <= 8);
	}
}
