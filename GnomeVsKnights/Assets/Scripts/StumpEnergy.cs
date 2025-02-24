using UnityEngine;

public class StumpEnergy : EnergyGnome
{
    protected float UpTime = 1.5f;
    protected float HorizontalMovement;

    protected override void Start()
    {
        fallYPos = transform.position.y;
        HorizontalMovement = Random.Range(-1f, 1f);
    }

    protected override void Update()
    {
        if (UpTime > 0)
        {
            transform.position += new Vector3(HorizontalMovement * Time.deltaTime, fallSpeed * Time.deltaTime, 0);
            transform.Rotate(0f, 0f, 150f * Time.deltaTime);
            UpTime -= Time.deltaTime;
            return;
        }
        base.Update();
    }
}
