using System.Collections;
using Tarodev;
using Unity.Mathematics;
using UnityEngine;

public class SmartMisille : MonoBehaviour
{

    [Header("REFERENCES")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Target _target;
    //[SerializeField] private GameObject _explosionPrefab;

    [SerializeField] private float searchRadius = 30f;
    [SerializeField] private string enemyTag = "enemy";
    private Transform _currentTarget;

    [Header("MOVEMENT")]
    [SerializeField] private float _speed = 15;
    [SerializeField] private float _rotateSpeed = 95;

    [Header("PREDICTION")]
    [SerializeField] private float _maxDistancePredict = 100;
    [SerializeField] private float _minDistancePredict = 5;
    [SerializeField] private float _maxTimePrediction = 5;
    private Vector3 _standardPrediction, _deviatedPrediction;

    [Header("DEVIATION")]
    [SerializeField] private float _deviationAmount = 50;
    [SerializeField] private float _deviationSpeed = 2;
    public float buffertime = 2f;

    private bool _canStart = false;  // Ýþlem yapmaya baþlamadan önce bekleme kontrolü

    public GameManager Gm;

    private void Start()
    {
        // Merhaba mesajýný konsola yazdýr
        Debug.Log("Merhaba");

        // 3 saniye bekle sonra iþlemlere baþla
        StartCoroutine(bombbuffer(buffertime));
    }

    private IEnumerator bombbuffer(float buffertime)
    {

        yield return new WaitForSeconds(buffertime);
        _canStart = true;
    }

    private void FixedUpdate()
    {
        if (!_canStart) return;

         _rb.linearVelocity = transform.forward * _speed + Gm.planeSpeed;
        
        // En yakýn hedefi bul
        FindClosestTarget();

        if (_currentTarget != null)
        {
            // Tahminli takip ve sapma
            float leadTimePercentage = Mathf.InverseLerp(_minDistancePredict, _maxDistancePredict, Vector3.Distance(transform.position, _currentTarget.position));

            PredictMovement(leadTimePercentage);
            AddDeviation(leadTimePercentage);
        }
        else
        {
            // Hedef yoksa rastgele yön
            _deviatedPrediction = transform.position + (transform.forward + UnityEngine.Random.insideUnitSphere).normalized * 10f;
        }

        RotateRocket();
    }

    private void PredictMovement(float leadTimePercentage)
    {
        var predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);

        if (_currentTarget.TryGetComponent<Rigidbody>(out Rigidbody targetRb))
        {
            _standardPrediction = targetRb.position + targetRb.linearVelocity * predictionTime;
        }
        else
        {
            _standardPrediction = _currentTarget.position;
        }
    }

    private void AddDeviation(float leadTimePercentage)
    {
        var deviation = new Vector3(Mathf.Cos(Time.time * _deviationSpeed), 0, 0);

        var predictionOffset = transform.TransformDirection(deviation) * _deviationAmount * leadTimePercentage;

        _deviatedPrediction = _standardPrediction + predictionOffset;
    }

    private void FindClosestTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float closestDistance = searchRadius;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestEnemy = enemy.transform;
            }
        }

        _currentTarget = closestEnemy;
    }

    private void RotateRocket()
    {
        var heading = _deviatedPrediction - transform.position;

        var rotation = Quaternion.LookRotation(heading);
        _rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, _rotateSpeed * Time.deltaTime));
    }

    private void OnCollisionEnter(Collision collision)
    {
        // if (_explosionPrefab) Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        if (collision.transform.TryGetComponent<IExplode>(out var ex)) ex.Explode();

        Destroy(gameObject);
    }

    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _standardPrediction);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_standardPrediction, _deviatedPrediction);
    }
}

