using UnityEngine;

public class GameObjectGrassDrawer : AbstractGrassDrawer
{
	[SerializeField] private GameObject grassPrefab;
	private GameObject[,] grassEntities;

	public override void Init(Vector2[,] grassEntities, Vector2 fieldSize)
	{
		this.grassEntities = new GameObject[grassEntities.GetLength(0), grassEntities.GetLength(1)];
		for (var i = 0; i < grassEntities.GetLength(0); i++)
		{
			for (var j = 0; j < grassEntities.GetLength(1); j++)
			{
				this.grassEntities[i, j] = Instantiate(grassPrefab,
					new Vector3(grassEntities[i, j].x, transform.position.y, grassEntities[i, j].y), Quaternion.identity);
			}
		}
	}

	public override void UpdatePositions(Vector2Int bottomLeftCameraCell, Vector2Int topRightCameraCell)
	{
		for (var i = 0; i < grassEntities.GetLength(0); i++)
		{
			for (var j = 0; j < grassEntities.GetLength(1); j++)
			{
				grassEntities[i, j].SetActive(i >= bottomLeftCameraCell.x && i < topRightCameraCell.x && j >= bottomLeftCameraCell.y &&
												  j < topRightCameraCell.y);
			}
		}
	}
}