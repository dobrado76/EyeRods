using UnityEngine;

[ExecuteAlways]
public class Rod : MonoBehaviour
{
    public float size = 0.5f;
    public float moveSpeed = 1f;
    public float frequency = 5f;
    public float distMin = 1f;
    public float distMax = 10f;
    public Vector3 _startPosition;
    public Material material;

    private float _time;
    private float timeOffset = 0;
    private float _size = -1;
    private float _directionChangeTime;
    private Vector2 _direction = Vector2.right;
    private float _dist;

    private float xMax = 10.25f;
    private float yMax = 6.7f;

    private MeshRenderer mr;
    private MaterialPropertyBlock _propBlock;

    internal float minIntensity = 0.1f;
    internal float maxIntensity = 1.0f;

    void Start()
    {
        _directionChangeTime = 1 / frequency; // Initial direction change interval.
        _propBlock = new MaterialPropertyBlock();
    }

    private void OnValidate()
    {
        if (size < 0.001f)
            size = 0.001f;
        if (mr == null)
            mr = transform.Find("Plane").GetComponent<MeshRenderer>();
        if (_propBlock == null)
            _propBlock = new MaterialPropertyBlock();
    }

    public void Init()
    {
        _startPosition = new Vector3(Random.Range(-xMax, xMax), Random.Range(-yMax, yMax));
        timeOffset = Random.value * 1000000f;
        _dist = Random.Range(distMin, distMax);

        // Clone the material
        if (mr == null)
            mr = transform.Find("Plane").GetComponent<MeshRenderer>();
        mr.sharedMaterial = Instantiate(mr.sharedMaterial);
    }

    void Update()
    {
        if (_startPosition == Vector3.zero)
        { 
            _startPosition = new Vector3(Random.Range(-xMax, xMax), Random.Range(-yMax, yMax));
        }

        if (_size != size)
        {
            _size = size;
            transform.localScale = new Vector3(size, size, size);
        }

        _time += Time.deltaTime;

        // Change direction at each peak based on frequency.
        if (_time >= _directionChangeTime)
        {
            _directionChangeTime = 1 / frequency; // Update direction change interval.
            _direction = new Vector2(Mathf.Cos(Random.value * 2 * Mathf.PI), Mathf.Sin(Random.value * 2 * Mathf.PI)); // Random new direction.
            _dist = Random.Range(distMin, distMax);
            timeOffset = Random.value * 10000000f;
            _time = timeOffset; // Reset time for the next cycle.
        }

        // Calculate the current amplitude of the oscillation.
        float currentAmplitude = Mathf.Lerp(_dist, distMax, Mathf.Sin((Time.time+timeOffset) * moveSpeed) * 0.5f + 0.5f);

        // Calculate the current oscillation position.
        float oscillation = Mathf.Sin(_time * Mathf.PI * 2 * frequency) * currentAmplitude;

        // Apply the oscillation in the current direction while keeping the center position constant.
        transform.position = _startPosition + (Vector3)(_direction * oscillation);

        /////////////////////////////////////////////////////////////////////////////////////
        // Update the material transparency based on the current oscillation amplitude.

        // Get the current value of the material properties
        mr.GetPropertyBlock(_propBlock);

        // Modulate intensity by adjusting the alpha value of the color
        //float v = Mathf.Lerp(minIntensity, maxIntensity, oscillation);

        //float currentAmplitudeNormalized = Mathf.Max(oscillation, minIntensity); // Normalize and ensure minimum value to avoid log(0)
        //float logIntensity = Mathf.Log10(currentAmplitudeNormalized) * -1f; // Invert since log(x) < 0 for 0 < x < 1
        //float v = Mathf.Lerp(minIntensity, maxIntensity, logIntensity); // Map logarithmic intensity to alpha range
        //v = Mathf.Clamp(v, minIntensity, maxIntensity);

        float currentAmplitudeNormalized = 1f - Mathf.Clamp(currentAmplitude / distMax, 0f, 1f);
        // Apply an exponential function to the normalized amplitude. The base number (e.g., 2) controls the rate of intensity increase.
        float exponentialIntensity = Mathf.Pow(2, currentAmplitudeNormalized * 10) - 1; // The * 10 and - 1 adjust the curve's steepness and starting point

        // Map the exponential intensity to the alpha range, ensuring it doesn't exceed 1
        float v = Mathf.Lerp(minIntensity, maxIntensity, Mathf.Clamp01(exponentialIntensity / (Mathf.Pow(2, 10) - 1)));


        // Apply the color change to the particle system
        _propBlock.SetColor("_Color", Color.white * v);

        // Apply the updated material properties to the renderer
        mr.SetPropertyBlock(_propBlock);
    }
}
