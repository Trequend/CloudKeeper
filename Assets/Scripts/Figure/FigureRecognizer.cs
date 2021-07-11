using System;
using Unity.Barracuda;
using UnityEngine;

public class FigureRecognizer : IDisposable
{
    private readonly FigureDatabase _database;

    private readonly IWorker _worker;

    private readonly float[] _buffer = new float[32 * 32];

    private readonly Tensor _input = new Tensor(n: 1, c: 32 * 32);

    private float _threshold;

    public float Threshold
    {
        get => _threshold;
        set => _threshold = Mathf.Clamp(value, 0.01f, 0.99f);
    }

    public FigureRecognizer(FigureDatabase database, float threshold = 0.8f)
    {
        if (database == null)
        {
            throw new ArgumentNullException(nameof(database));
        }

        _database = database;
        NNModel neuralNetwork = database.LoadNeuralNetwork();
        if (neuralNetwork == null)
        {
            throw new NullReferenceException("Neural network for database");
        }

        Model model = ModelLoader.Load(neuralNetwork);
        _worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, model);
        Threshold = threshold;
    }

    public Figure Recognize(BitBuffer figureBuffer)
    {
        return Recognize(figureBuffer, out _);
    }

    public Figure Recognize(BitBuffer figureBuffer, out int index)
    {
        figureBuffer.CopyTo(_buffer);
        for (int i = 0; i < _buffer.Length; i++)
        {
            _input[i] = _buffer[i];
        }

        Tensor output = _worker.Execute(_input).PeekOutput();
        float max = float.NegativeInfinity;
        index = -1;
        for (int figureIndex = 0; figureIndex < output.channels; figureIndex++)
        {
            float value = output[figureIndex];
            if (value > max && value > _threshold)
            {
                index = figureIndex;
                max = value;
            }
        }

        if (index == -1)
        {
            return null;
        }
        else
        {
            Figure figure = _database.GetFigure(index);
            return figure.Name == "Unknown" ? null : figure;
        }
    }

    public void Dispose()
    {
        _input.Dispose();
        _worker.Dispose();
    }
}
