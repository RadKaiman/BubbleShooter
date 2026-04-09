using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class BallView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private BallModel model;

    public void Initialize(BallModel ballModel)
    {
        model = ballModel;
        transform.position = new Vector3(model.Position.x, model.Position.y, 0);
        UpdateColor();
    }

    public void UpdateColor()
    {
        if (model != null)
            spriteRenderer.color = GetColorByID(model.ColorId);
    }

    public async UniTask PlayExplosionAsync()
    {
        await transform.DOScale(0f, 0.2f).SetEase(Ease.InBack).ToUniTask();
        transform.localScale = Vector3.one;
    }

    public void PushFrom(Vector2 source, float force)
    {
        Vector3 source3D = new Vector3(source.x, source.y, 0);
        Vector3 direction = (transform.position - source3D).normalized;
        Vector3 originalPos = transform.position;

        transform.DOMove(originalPos + direction * force, 0.05f).OnComplete(() =>
        {
            transform.DOMove(originalPos, 0.1f).SetEase(Ease.OutElastic);
        });
    }

    private Color GetColorByID(int id)
    {
        switch (id)
        {
            case 0: return Color.red;
            case 1: return Color.blue;
            case 2: return Color.green;
            default: return Color.white;
        }
    }
}
