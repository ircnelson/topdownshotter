using UnityEngine;
using System.Collections;

public class Crosshairs : MonoBehaviour
{
    public LayerMask TargetMask;
    public SpriteRenderer Dot;
    public Color DotHightlightColor;

    private Color _originalDotColor;

	void Start () {

        Cursor.visible = false;

        _originalDotColor = Dot.color;
	}
	
	void Update ()
    {
        transform.Rotate(Vector3.forward * -40 * Time.deltaTime);	
	}

    public void DetectTargets(Ray ray)
    {
        if (Physics.Raycast(ray, 100, TargetMask))
        {
            Dot.color = DotHightlightColor;
        } else
        {
            Dot.color = _originalDotColor;
        }
    }
}
