using UnityEngine;

/// <summary>
/// Handles and (Hopefully) simplifies the editting of particle systems.
/// </summary>
public class ParticleHandler : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;

    [SerializeField] float duration;
    [SerializeField] float lifeTime;
    [SerializeField] float gravityModifier;

    [SerializeField] ParticleSystem.MinMaxGradient startColor;
    [SerializeField] ParticleSystem.MinMaxGradient colorOverLifetime;

    ParticleSystem.ShapeModule shapeModule;
    ParticleSystem.CollisionModule collisionModule;
    ParticleSystem.EmissionModule emissionModule;
    ParticleSystem.MainModule mainModule;


    void Awake()
    {
        mainModule = particleSystem.main;
        mainModule.duration = duration;
        mainModule.startLifetime = lifeTime;
        mainModule.startColor = startColor;
        mainModule.gravityModifier = gravityModifier;

        //ParticleSystem.ColorOverLifetimeModule _colorOverLifetimeModule = particleSystem.colorOverLifetime;
        //_colorOverLifetimeModule.color = colorOverLifetime;

        shapeModule = particleSystem.shape;
        emissionModule = particleSystem.emission;
        
        //foreach(GameObject _game in GameObject.FindGameObjectsWithTag("Platform"))
        //{
        //    collisionModule.AddPlane(_game.transform);
        //}
    }

    /// <summary>
    /// Set the position and rotaion of the particle system.
    /// For example if an enemy is slashed at the front, the player would expect blood to come from the front of the enemy.
    /// </summary>
    /// <param name="pos">The Position of the Particle Emission</param>
    /// <param name="rot">The Direction the particles will be emmitted in.</param>
    public void SetParticlePosAndRot(Vector2 pos, Vector2 rot)
    {
        shapeModule.position = new Vector3(pos.x, pos.y, 0);
        shapeModule.rotation = rot;
    }

    public void PlayParticle()
    {
        particleSystem.Emit(Random.Range(5, 10));
    }

    internal void Explode()
    {
        shapeModule.shapeType = ParticleSystemShapeType.Circle;
        mainModule.gravityModifier = 0;
        emissionModule.rateOverTime = 500;
        Emit(true);
    }

    internal void Emit(bool v)
    {
        if (v)
            particleSystem.Play();
        else
        {
            particleSystem.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
