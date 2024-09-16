using UnityEngine;

public class StarFieldGenerator : MonoBehaviour
{
    public GameObject starPrefab;
    public int starCount = 100;
    public float fieldRadius = 30f;

    void Start()
    {
        GenerateStarField();
    }

    void GenerateStarField()
    {
        for (int i = 0; i < starCount; i++)
        {
            Vector3 position = Random.insideUnitSphere * fieldRadius;
            Instantiate(starPrefab, position, Quaternion.identity, transform);
        }
    }
}
