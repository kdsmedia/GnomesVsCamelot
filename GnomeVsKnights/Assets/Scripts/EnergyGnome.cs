using UnityEngine;
using UnityEngine.UIElements;

public class EnergyGnome : MonoBehaviour
{
    private float fallYPos;
    private float fallSpeed = .2f;
    void Start()
    {
        transform.position = new Vector3(Random.Range(-5.3f, 5.3f), 7, -0.1f);
        fallYPos = Random.Range(-3, 3);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y >= fallYPos)
        {
            transform.position -= new Vector3(0, fallSpeed * Time.fixedDeltaTime, 0);
            transform.Rotate(0f, 0f, 80f * Time.fixedDeltaTime);
        }
    }
}
