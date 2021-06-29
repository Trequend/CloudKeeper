using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FigureRotator : IEnumerable<List<Vector2Int>>
{
    private class Enumerator : IEnumerator<List<Vector2Int>>
    {
        private readonly List<Vector2> _points;

        private readonly List<float> _angles;

        private readonly Vector2 _center;

        private readonly List<Vector2Int> _rotatedPoints;

        public List<Vector2Int> Current => _rotatedPoints;

        object IEnumerator.Current => _rotatedPoints;

        private int _angleIndex = -1;

        public Enumerator(FigureRotator rotator)
        {
            _points = rotator.Points;
            _angles = rotator.Angles;
            _center = rotator.Center;
            _rotatedPoints = new List<Vector2Int>(_points.Count);
        }

        public void Dispose()
        {
            _rotatedPoints.Clear();
        }

        public bool MoveNext()
        {
            _angleIndex++;
            if (_angleIndex < _angles.Count)
            {
                Rotate();
                return true;
            }

            return false;
        }

        private void Rotate()
        {
            _rotatedPoints.Clear();
            float angle = _angles[_angleIndex] * Mathf.Deg2Rad;
            float cosa = Mathf.Cos(angle);
            float sina = Mathf.Sin(angle);
            for (int i = 0; i < _points.Count; i++)
            {
                /*
                    Translation(_center) * Rotation(cosa, sina) * point

                    / 1  0  _center.x \     / cosa  -sina  0 \     / point.x \
                   |  0  1  _center.y  | * |  sina   cosa  0  | * |  point.y  |
                    \ 0  0      1     /     \  0      0    1 /     \    1    /
                */

                Vector2 point = _points[i];
                _rotatedPoints.Add(new Vector2Int(
                    (int)(point.x * cosa - point.y * sina + _center.x),
                    (int)(point.x * sina + point.y * cosa + _center.y)
                ));
            }
        }

        public void Reset()
        {
            _angleIndex = -1;
        }
    }

    private List<Vector2> Points { get; }

    private List<float> Angles { get; }

    private Vector2 Center { get; }

    private FigureRotator(IEnumerable<Vector2Int> points, IEnumerable<float> angles)
    {
        Points = new List<Vector2>(
            points.Select(point => new Vector2(point.x, point.y))
        );

        Angles = new List<float>(angles);
        Center = Points.Aggregate((sum, point) => sum + point) / Points.Count;
    }

    IEnumerator<List<Vector2Int>> IEnumerable<List<Vector2Int>>.GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new Enumerator(this);
    }

    public static IEnumerable<List<Vector2Int>> Rotations(
        IEnumerable<Vector2Int> figurePoints,
        IEnumerable<float> angles
    ) {
        return new FigureRotator(figurePoints, angles);
    }
}
