using UnityEngine;
using System.Collections;

public class Shell : MonoBehaviour
{
    public float ForceMax;
    public float ForceMin;

    private Rigidbody _rigidbody;
    private float _lifetime = 4;
    private float _fadetime = 2;
    
    void Start ()
    {
        _rigidbody = GetComponent<Rigidbody>();

        float force = Random.Range(ForceMin, ForceMax);

        _rigidbody.AddForce(transform.right * force);
        _rigidbody.AddTorque(Random.insideUnitSphere * force);

        StartCoroutine(Fade());
	}
    
    IEnumerator Fade()
    {
        yield return new WaitForSeconds(_lifetime);

        float percent = 0;

        float fadeSpeed = 1 / _fadetime;

        Material material = GetComponent<Renderer>().material;

        Color initialColor = material.color;

        while (percent < 1)
        {
            percent += Time.deltaTime * fadeSpeed;
            material.color = Color.Lerp(initialColor, Color.clear, percent);

            yield return null;
        }

        Destroy(gameObject);
    }
}
