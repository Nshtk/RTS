using UnityEngine;

public class Projectile : MobileObject
{
	public DynamicObject owner;
	public float velocity = 1;
	public int damage = 1;
	private float range = 1;

	protected override void Update()
	{
		if(HitSomething())
		{
			InflictDamage();
			Destroy(gameObject);
		}
		if(range>0)
		{
			float positionChange = Time.deltaTime * velocity;
			range -= positionChange;
			transform.position += (positionChange * transform.forward);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void SetRange(float range)
	{
		this.range = range;
	}

	public void SetTarget(DynamicObject target)
	{
		this.target = target;
	}

	private bool HitSomething()
	{
		/*if(target && target.GetSelectionBounds().Contains(transform.position))
			return true;*/
		return false;
	}

	private void InflictDamage()
	{
		/*if(target)
			target.substractHealth(damage);*/
	}
}
