using UnityEngine;
using UnityEngine.UIElements;

public class EnergyGnome : MonoBehaviour
{
    private float fallYPos;
    private float fallSpeed = .8f;
    private int energyValue = 25;

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
            transform.position -= new Vector3(0, fallSpeed * Time.deltaTime, 0);
            transform.Rotate(0f, 0f, 150f * Time.deltaTime);
        }
    }

    public void OnMouseOver()
    {
        Destroy(gameObject);
        GameManager.Instance.AddEnergy(energyValue);
    }
}
