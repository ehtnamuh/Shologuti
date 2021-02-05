using UnityEngine;

public class Guti : MonoBehaviour
{
    internal Address address;
    internal GutiType gutiType = GutiType.NoGuti;

    public void SetAddress(Address address, float scale = 2.0f)
    {
        this.address = address;
        var o = gameObject;
        o.transform.position= new Vector3(address.x, address.y, -1);
        o.transform.localScale = new Vector3(scale, scale, 1);
    }

    public void SetScale(float scale)
    {
        gameObject.transform.localScale = new Vector3(scale, scale, 1);
    }

    public void SetGutiType(GutiType gutiType)
    {
        this.gutiType = gutiType;
        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        switch (gutiType)
        {
            case GutiType.RedGuti:
                spriteRenderer.color = Color.red;
                gameObject.name = $"Red-x{address.x}-y{address.y}";
                break;
            case GutiType.GreenGuti:
                gameObject.name = $"Green-x{address.x}-y{address.y}";
                spriteRenderer.color = Color.green;
                break;
            case GutiType.NoGuti:
                Destroy(gameObject);
                break;
            case  GutiType.Highlight:
                spriteRenderer.color = Color.yellow;
                break;
            default:
                Destroy(gameObject);
                break;
        }
    }

    public void SetGutiColor(Color color)
    {
        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
    }

}
