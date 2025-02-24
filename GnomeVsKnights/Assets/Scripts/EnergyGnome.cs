using UnityEngine;
using UnityEngine.UIElements;

public class EnergyGnome : MonoBehaviour
{
    protected float fallYPos;
    protected float fallSpeed = .8f;
    protected int energyValue = 25;

    protected virtual void Start()
    {
        transform.position = new Vector3(Random.Range(-5.3f, 5.3f), 7, -0.1f);
        fallYPos = Random.Range(-3, 3);
    }

    // Update is called once per frame
    protected virtual void Update()
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
