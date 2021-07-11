using UnityEngine;

public class FigureEmitter : MonoBehaviour
{
    [SerializeField] private FigureReader _reader;

    private void OnEnable()
    {
        _reader.FigureReaded += EmitFigure;
    }

    private void OnDisable()
    {
        _reader.FigureReaded -= EmitFigure;
    }

    public void EmitFigure(Figure figure)
    {
        int count = 0;
        Enemy.ForeachEnemyInScreen(enemy =>
        {
            if (enemy.TakeDamage(figure))
            {
                count++;
            }
        });

        Score.Value += count;
    }
}
