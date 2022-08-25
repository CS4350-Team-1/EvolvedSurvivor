using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

public class CombatAbility : MonoBehaviour
{
    [SerializeField] private bool isValidAbility;
    [SerializeField] private MMObjectPooler objectPool;

    [SerializeField] private float abilityCooldown;
    [SerializeField] private float size;
    [SerializeField] private float speed;
    [SerializeField] private float count;
    [SerializeField] private float damage;
    [SerializeField] private string[] statusEffects;
    [SerializeField] private string attackType;

    private float remainingCooldown;

    private void Start()
    {
        remainingCooldown = abilityCooldown;
    }

    private void Update()
    {
        if (isValidAbility) FireAbility();
    }
    
    private void FireAbility()
    { 
        remainingCooldown -= Time.deltaTime;

        if (remainingCooldown <= 0f)
        {
            GameObject nextGameObject = objectPool.GetPooledGameObject();
            nextGameObject.transform.position = gameObject.transform.position;
            nextGameObject.SetActive(true);

            Projectile projectile = nextGameObject.GetComponent<Projectile>();
            SetRandomDirection(projectile);

            remainingCooldown = abilityCooldown;
        }
    }

    private void SetRandomDirection(Projectile projectile)
    {
        projectile.SetDirection(
            new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f),
            transform.rotation);
    }

    public void SetNewStats(CombatAbility newAbility) {
        this.abilityCooldown = newAbility.abilityCooldown;
        this.size = newAbility.size;
        this.speed = newAbility.speed;
        this.count = newAbility.count;
        this.damage = newAbility.damage;
        this.statusEffects = newAbility.statusEffects;
        this.attackType = newAbility.attackType;
    }
}
