using UnityEngine;

public class ParticleAttractor : MonoBehaviour
{
    [SerializeField] private float delayTime = 1;
    [SerializeField] private float speed = 50f;
    [SerializeField] private Transform playerTransform;

    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    private float timer = 0;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }


    void Update()
    {
        timer += Time.deltaTime;
        if (timer > delayTime)
        {
            int numParticlesAlive = ps.GetParticles(particles);
            Vector3 targetPos = playerTransform.position;

            for (int i = 0; i < numParticlesAlive; i++)
            {
                particles[i].position = Vector3.MoveTowards(particles[i].position, targetPos, speed * Time.deltaTime);

                if (Vector3.Distance(particles[i].position, targetPos) < 0.3f)
                {
                    particles[i].remainingLifetime = 0f;
                    if (numParticlesAlive < 5f)
                    {
                        Destroy(gameObject, 5f);
                    }

                }
            }
            ps.SetParticles(particles, numParticlesAlive);
        }
    }

    void OnEnable()
    {
        timer = 0;
    }

    public void SetTarget(Transform transform)
    {
        playerTransform = transform;
    }
}
